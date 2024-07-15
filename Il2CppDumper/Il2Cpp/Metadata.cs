using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Il2CppDumper
{
    public sealed class Metadata : BinaryStream
    {
        public Il2CppGlobalMetadataHeader header;
        public Il2CppImageDefinition[] imageDefs;
        public Il2CppAssemblyDefinition[] assemblyDefs;
        public Il2CppTypeDefinition[] typeDefs;
        public Il2CppMethodDefinition[] methodDefs;
        public Il2CppParameterDefinition[] parameterDefs;
        public Il2CppFieldDefinition[] fieldDefs;
        private readonly Dictionary<int, Il2CppFieldDefaultValue> fieldDefaultValuesDic;
        private readonly Dictionary<int, Il2CppParameterDefaultValue> parameterDefaultValuesDic;
        public Il2CppPropertyDefinition[] propertyDefs;
        public Il2CppCustomAttributeTypeRange[] attributeTypeRanges;
        public Il2CppCustomAttributeDataRange[] attributeDataRanges;
        private readonly Dictionary<Il2CppImageDefinition, Dictionary<uint, int>> attributeTypeRangesDic;
        public Il2CppStringLiteral[] stringLiterals;
        private readonly Il2CppMetadataUsageList[] metadataUsageLists;
        private readonly Il2CppMetadataUsagePair[] metadataUsagePairs;
        public int[] attributeTypes;
        public int[] interfaceIndices;
        public Dictionary<Il2CppMetadataUsage, SortedDictionary<uint, uint>> metadataUsageDic;
        public long metadataUsagesCount;
        public int[] nestedTypeIndices;
        public Il2CppEventDefinition[] eventDefs;
        public Il2CppGenericContainer[] genericContainers;
        public Il2CppFieldRef[] fieldRefs;
        public Il2CppGenericParameter[] genericParameters;
        public int[] constraintIndices;
        public uint[] vtableMethods;
        public Il2CppRGCTXDefinition[] rgctxEntries;

        private readonly Dictionary<uint, string> stringCache = new();

        public Metadata(Stream stream) : base(stream)
        {
            var sanity = ReadUInt32();
            if (sanity != 0xFAB11BAF)
            {
                throw new InvalidDataException("ERROR: Metadata file supplied is not valid metadata file.");
            }
            var version = ReadInt32();
            if (version < 0) version = Math.Abs(version) - 1;
            if (version < 0 || version > 1000)
            {
                throw new InvalidDataException("ERROR: Metadata file supplied is not valid metadata file.");
            }
            if (version < 16 || version > 29)
            {
                throw new NotSupportedException($"ERROR: Metadata file supplied is not a supported version[{version}].");
            }
            Version = version;
            // header = ReadClass<Il2CppGlobalMetadataHeader>(0);
            header = restructMetadata();

            imageDefs = ReadMetadataClassArray<Il2CppImageDefinition>(header.imagesOffset, header.imagesSize);
            if (Version == 24.2 && header.assembliesSize / 68 < imageDefs.Length)
            {
                Version = 24.4;
            }
            var v241Plus = false;
            if (Version == 24.1 && header.assembliesSize / 64 == imageDefs.Length)
            {
                v241Plus = true;
            }
            if (v241Plus)
            {
                Version = 24.4;
            }
            assemblyDefs = ReadMetadataClassArray<Il2CppAssemblyDefinition>(header.assembliesOffset, header.assembliesSize);

            if (v241Plus)
            {
                Version = 24.1;
            }
            typeDefs = ReadMetadataClassArray<Il2CppTypeDefinition>(header.typeDefinitionsOffset, header.typeDefinitionsSize);
            methodDefs = ReadMetadataClassArray<Il2CppMethodDefinition>(header.methodsOffset, header.methodsSize);
            parameterDefs = ReadMetadataClassArray<Il2CppParameterDefinition>(header.parametersOffset, header.parametersSize);
            fieldDefs = ReadMetadataClassArray<Il2CppFieldDefinition>(header.fieldsOffset, header.fieldsSize);
            var fieldDefaultValues = ReadMetadataClassArray<Il2CppFieldDefaultValue>(header.fieldDefaultValuesOffset, header.fieldDefaultValuesSize);
            var parameterDefaultValues = ReadMetadataClassArray<Il2CppParameterDefaultValue>(header.parameterDefaultValuesOffset, header.parameterDefaultValuesSize);
            fieldDefaultValuesDic = fieldDefaultValues.ToDictionary(x => x.fieldIndex);
            parameterDefaultValuesDic = parameterDefaultValues.ToDictionary(x => x.parameterIndex);
            propertyDefs = ReadMetadataClassArray<Il2CppPropertyDefinition>(header.propertiesOffset, header.propertiesSize);
            interfaceIndices = ReadClassArray<int>(header.interfacesOffset, header.interfacesSize / 4);
            nestedTypeIndices = ReadClassArray<int>(header.nestedTypesOffset, header.nestedTypesSize / 4);
            eventDefs = ReadMetadataClassArray<Il2CppEventDefinition>(header.eventsOffset, header.eventsSize);
            genericContainers = ReadMetadataClassArray<Il2CppGenericContainer>(header.genericContainersOffset, header.genericContainersSize);
            genericParameters = ReadMetadataClassArray<Il2CppGenericParameter>(header.genericParametersOffset, header.genericParametersSize);
            constraintIndices = ReadClassArray<int>(header.genericParameterConstraintsOffset, header.genericParameterConstraintsSize / 4);
            vtableMethods = ReadClassArray<uint>(header.vtableMethodsOffset, header.vtableMethodsSize / 4);
            stringLiterals = ReadMetadataClassArray<Il2CppStringLiteral>(header.stringLiteralOffset, header.stringLiteralSize);
            if (Version > 16)
            {
                fieldRefs = ReadMetadataClassArray<Il2CppFieldRef>(header.fieldRefsOffset, header.fieldRefsSize);
            }
            if (Version >= 29)
            {
                attributeDataRanges = ReadMetadataClassArray<Il2CppCustomAttributeDataRange>(header.attributeDataRangeOffset, header.attributeDataRangeSize);
            }

            if (Version > 24)
            {
                attributeTypeRangesDic = new Dictionary<Il2CppImageDefinition, Dictionary<uint, int>>();
                foreach (var imageDef in imageDefs)
                {
                    var dic = new Dictionary<uint, int>();
                    attributeTypeRangesDic[imageDef] = dic;
                    var end = imageDef.customAttributeStart + imageDef.customAttributeCount;
                    for (int i = imageDef.customAttributeStart; i < end; i++)
                    {
                        if (Version >= 29)
                        {
                            dic.Add(attributeDataRanges[i].token, i);
                        }
                        else
                        {
                            dic.Add(attributeTypeRanges[i].token, i);
                        }
                    }
                }
            }

        }

        private Il2CppGlobalMetadataHeader restructMetadata()
        {
            Il2CppGlobalMetadataHeader header = new();
            int[] data = ReadClassArray<int>(0, SizeOf(typeof(Il2CppGlobalMetadataHeader)) / 4);

            header.sanity = (uint)data[0];
            header.version = data[1];
            header.stringLiteralOffset = (uint)data[2];
            header.stringLiteralSize = data[3];

            List<Tuple<uint, int>> list = new();
            Dictionary<int, int> seenValues = new Dictionary<int, int>();

            for (int i = 4; i < data.Length; i += 2)
            {
                for (int j = 5; j < data.Length; j += 2)
                {
                    if (data[i] == 0 || data[j] == 0)
                        continue;

                    if (seenValues.ContainsKey(data[i]))
                        continue;

                    for (int k = 4; k < data.Length; k += 2)
                    {
                        if (Math.Abs(data[i] + data[j] - data[k]) <= 8 && data[i] < data[k])
                        {
                            seenValues.Add(data[i], data[j]);
                            list.Add(new Tuple<uint, int>((uint)data[i], data[j]));
                            break;
                        }
                    }
                }
            }

            for (int i = 5; i < data.Length; i += 2) {
                if (data[i] == 0) continue;
                if (seenValues.ContainsKey(data[i])) continue;
                list.Add(new Tuple<uint, int>((uint)data[data.Length - 2], data[i]));
                break;
            }

            // for (int i = 0; i < list.Count; i++)
            // {
            //     Console.WriteLine($"0x{list[i].Item1:X} - 0x{list[i].Item2:X}");
            // }

            int checkDone = 0;
            bool check = false;

            float range = 0.4f;

            // uint stringLiteralDataSize_MIN = 0x90000;
            // uint stringSize_MIN = 0x1B0000;
            uint eventsSize_MIN = 0x2A00;
            uint propertiesSize_MIN = 0x57000;
            uint methodsSize_MIN = 0x2E0000;
            uint parameterDefaultValuesSize_MIN = 0x4000;
            uint fieldDefaultValuesSize_MIN = 0x28000;
            // uint fieldAndParameterDefaultValueDataSize_MIN = 0x20000;
            uint parametersSize_MIN = 0x111000;

            uint fieldsSize_MIN = 0xA0000;
            uint genericParametersSize_MIN = 0xB000;
            uint genericParameterConstraintsSize_MIN = 0xA00;
            uint genericContainersSize_MIN = 0x7000;
            uint nestedTypesSize_MIN = 0x4800;
            uint interfacesSize_MIN = 0x6800;
            uint vtableMethodsSize_MIN = 0x8D000;
            uint typeDefinitionsSize_MIN = 0x13C000;
            uint imagesSize_MIN = 0xE00;
            uint assembliesSize_MIN = 0x1700;
            uint fieldRefsSize_MIN = 0x1200;
            uint attributeDataSize_MIN = 0x8C000;
            uint attributeDataRangeSize_MIN = 0x3B000;


            // stringLiteralDataOffset
            if (header.stringLiteralDataOffset == 0)
            {
                byte[] checkString = new byte[] {
                    0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48,
                    0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50,
                    0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58,
                    0x59, 0x5A, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66,
                    0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E,
                    0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76,
                    0x77, 0x78, 0x79, 0x7A
                }; // ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz

                for (int i = 0; i < list.Count; i++)
                {
                    // if (list[i].Item2 < stringLiteralDataSize_MIN || list[i].Item2 > stringLiteralDataSize_MIN * (1 + range)) continue;
                    for (int j = 0; j < 0x200; j++)
                    {
                        Position = (ulong)(list[i].Item1 + j);
                        byte[] subBytes = ReadBytes(0x34);
                        if (subBytes.SequenceEqual(checkString))
                        {
                            header.stringLiteralDataOffset = list[i].Item1;
                            header.stringLiteralDataSize = list[i].Item2;
                            list.RemoveAt(i);
                            checkDone++;
                            check = true;
                            break;
                        }
                    }
                    if (check) break;
                }
                check = false;
            }

            // stringOffset
            if (header.stringOffset == 0)
            {
                // for (int i = 0; i < list.Count; i++)
                // {
                //     // if (list[i].Item2 < stringSize_MIN || list[i].Item2 > stringSize_MIN * (1 + range)) continue;
                //     for (int j = 0; j < 0x200; j++)
                //     {
                //         string str = ReadStringToNull((ulong)(list[i].Item1 + j));
                //         if (str == "Assembly-CSharp")
                //         {
                //             header.stringOffset = list[i].Item1;
                //             header.stringSize = list[i].Item2;
                //             list.RemoveAt(i);
                //             checkDone++;
                //             check = true;
                //             break;
                //         }
                //     }
                //     if (check) break;
                // }
                // check = false;
                int firstMax = int.MinValue;
                int secondMax = int.MinValue;

                foreach (var tuple in list)
                {
                    if (tuple.Item2 > firstMax)
                    {
                        secondMax = firstMax;
                        firstMax = tuple.Item2;
                    }
                    else if (tuple.Item2 > secondMax && tuple.Item2 != firstMax)
                    {
                        secondMax = tuple.Item2;
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 == secondMax)
                    {
                        header.stringOffset = list[i].Item1;
                        header.stringSize = list[i].Item2;
                        list.RemoveAt(i);
                        checkDone++;
                        break;
                    }
                }
            }

            // typeDefinitionsOffset
            if (header.typeDefinitionsOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < typeDefinitionsSize_MIN || list[i].Item2 > typeDefinitionsSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppTypeDefinition)) != 0) continue;
                    header.typeDefinitionsOffset = list[i].Item1;
                    header.typeDefinitionsSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppAssemblyDefinition
            if (header.assembliesOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < assembliesSize_MIN || list[i].Item2 > assembliesSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppAssemblyDefinition)) != 0) continue;
                    header.assembliesOffset = list[i].Item1;
                    header.assembliesSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppImageDefinition
            if (header.imagesOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < imagesSize_MIN || list[i].Item2 > imagesSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppImageDefinition)) != 0) continue;
                    header.imagesOffset = list[i].Item1;
                    header.imagesSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppMethodDefinition
            if (header.methodsOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < methodsSize_MIN || list[i].Item2 > methodsSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppMethodDefinition)) != 0) continue;
                    header.methodsOffset = list[i].Item1;
                    header.methodsSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppEventDefinition
            if (header.eventsOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < eventsSize_MIN || list[i].Item2 > eventsSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppEventDefinition)) != 0) continue;
                    header.eventsOffset = list[i].Item1;
                    header.eventsSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppPropertyDefinition
            if (header.propertiesOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < propertiesSize_MIN || list[i].Item2 > propertiesSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppPropertyDefinition)) != 0) continue;
                    header.propertiesOffset = list[i].Item1;
                    header.propertiesSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppGenericContainer
            if (header.genericContainersOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < genericContainersSize_MIN || list[i].Item2 > genericContainersSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppGenericContainer)) != 0) continue;
                    header.genericContainersOffset = list[i].Item1;
                    header.genericContainersSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppGenericParameter
            if (header.genericParametersOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < genericParametersSize_MIN || list[i].Item2 > genericParametersSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppGenericParameter)) != 0) continue;
                    header.genericParametersOffset = list[i].Item1;
                    header.genericParametersSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppParameterDefinition
            if (header.parametersOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < parametersSize_MIN || list[i].Item2 > parametersSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppParameterDefinition)) != 0) continue;
                    header.parametersOffset = list[i].Item1;
                    header.parametersSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppFieldDefinition
            if (header.fieldsOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < fieldsSize_MIN || list[i].Item2 > fieldsSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppFieldDefinition)) != 0) continue;
                    header.fieldsOffset = list[i].Item1;
                    header.fieldsSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppFieldDefaultValue
            if (header.fieldDefaultValuesOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < fieldDefaultValuesSize_MIN || list[i].Item2 > fieldDefaultValuesSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppFieldDefaultValue)) != 0) continue;
                    header.fieldDefaultValuesOffset = list[i].Item1;
                    header.fieldDefaultValuesSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppParameterDefaultValue
            if (header.parameterDefaultValuesOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < parameterDefaultValuesSize_MIN || list[i].Item2 > parameterDefaultValuesSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppParameterDefaultValue)) != 0) continue;
                    header.parameterDefaultValuesOffset = list[i].Item1;
                    header.parameterDefaultValuesSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppFieldRef
            if (header.fieldRefsOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < fieldRefsSize_MIN || list[i].Item2 > fieldRefsSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppFieldRef)) != 0) continue;
                    header.fieldRefsOffset = list[i].Item1;
                    header.fieldRefsSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // Il2CppCustomAttributeDataRange
            if (header.attributeDataRangeOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < attributeDataRangeSize_MIN || list[i].Item2 > attributeDataRangeSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % SizeOf(typeof(Il2CppCustomAttributeDataRange)) != 0) continue;

                    Il2CppCustomAttributeDataRange entry = ReadClass<Il2CppCustomAttributeDataRange>(list[i].Item1);
                    if (entry.startOffset != 0) continue;

                    header.attributeDataRangeOffset = list[i].Item1;
                    header.attributeDataRangeSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // fieldAndParameterDefaultValueDataOffset
            if (header.fieldAndParameterDefaultValueDataOffset == 0)
            {
                byte[] checkString = new byte[] {
                    0x22, 0x55, 0x49, 0x5F, 0x4C, 0x6F, 0x63, 0x61,
                    0x6C, 0x69, 0x7A, 0x65, 0x64, 0x46, 0x6F, 0x6E, 
                    0x74, 0x73
                }; // "UI_LocalizedFonts
                
                for (int i = 0; i < list.Count; i++)
                {
                    // if (list[i].Item2 < fieldAndParameterDefaultValueDataSize_MIN || list[i].Item2 > fieldAndParameterDefaultValueDataSize_MIN * (1 + range)) continue;
                    for (int j = 0; j < list[i].Item2 - 0x12; j++)
                    {
                        Position = (ulong)(list[i].Item1 + j);
                        byte[] subBytes = ReadBytes(0x12);
                        if (subBytes.SequenceEqual(checkString))
                        {
                            header.fieldAndParameterDefaultValueDataOffset = list[i].Item1;
                            header.fieldAndParameterDefaultValueDataSize = list[i].Item2;
                            list.RemoveAt(i);
                            checkDone++;
                            check = true;
                            break;
                        }
                    }
                    if (check) break;
                }
                check = false;
            }

            // genericParameterConstraintsOffset
            if (header.genericParameterConstraintsOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < genericParameterConstraintsSize_MIN || list[i].Item2 > genericParameterConstraintsSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % 4 != 0) continue;
                    header.genericParameterConstraintsOffset = list[i].Item1;
                    header.genericParameterConstraintsSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // nestedTypesOffset
            if (header.nestedTypesOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < nestedTypesSize_MIN || list[i].Item2 > nestedTypesSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % 4 != 0) continue;
                    header.nestedTypesOffset = list[i].Item1;
                    header.nestedTypesSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // interfacesOffset
            if (header.interfacesOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < interfacesSize_MIN || list[i].Item2 > interfacesSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % 4 != 0) continue;
                    header.interfacesOffset = list[i].Item1;
                    header.interfacesSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // attributeDataOffset
            if (header.attributeDataOffset == 0)
            {
                byte[] checkString = new byte[] {
                    0x4E, 0x65, 0x77, 0x20, 0x41, 0x70, 0x70, 0x20,
                    0x53, 0x65, 0x74, 0x74
                }; // "New App Sett

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < attributeDataSize_MIN || list[i].Item2 > attributeDataSize_MIN * (1 + range)) continue;
                    for (int j = 0; j < list[i].Item2 - 0x0C; j++)
                    {
                        Position = (ulong)(list[i].Item1 + j);
                        byte[] subBytes = ReadBytes(0x0C);
                        if (subBytes.SequenceEqual(checkString))
                        {
                            header.attributeDataOffset = list[i].Item1;
                            header.attributeDataSize = list[i].Item2;
                            list.RemoveAt(i);
                            checkDone++;
                            check = true;
                            break;
                        }
                    }
                    if (check) break;
                }
                check = false;
            }

            // vtableMethodsOffset
            if (header.vtableMethodsOffset == 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Item2 < vtableMethodsSize_MIN || list[i].Item2 > vtableMethodsSize_MIN * (1 + range)) continue;
                    if (list[i].Item2 % 4 != 0) continue;
                    header.vtableMethodsOffset = list[i].Item1;
                    header.vtableMethodsSize = list[i].Item2;
                    list.RemoveAt(i);
                    checkDone++;
                    break;
                }
            }

            // header.PrintAll();

            if (checkDone != 22)
            {
                throw new InvalidDataException("ERROR: Metadata file supplied is not valid metadata file.");
            }

            return header;
        }

        private T[] ReadMetadataClassArray<T>(uint addr, int count) where T : new()
        {
            return ReadClassArray<T>(addr, count / SizeOf(typeof(T)));
        }

        public bool GetFieldDefaultValueFromIndex(int index, out Il2CppFieldDefaultValue value)
        {
            return fieldDefaultValuesDic.TryGetValue(index, out value);
        }

        public bool GetParameterDefaultValueFromIndex(int index, out Il2CppParameterDefaultValue value)
        {
            return parameterDefaultValuesDic.TryGetValue(index, out value);
        }

        public uint GetDefaultValueFromIndex(int index)
        {
            return (uint)(header.fieldAndParameterDefaultValueDataOffset + index);
        }

        public string GetStringFromIndex(uint index)
        {
            if (!stringCache.TryGetValue(index, out var result))
            {
                result = ReadStringToNull(header.stringOffset + index);
                stringCache.Add(index, result);
            }
            return result;
        }

        public int GetCustomAttributeIndex(Il2CppImageDefinition imageDef, int customAttributeIndex, uint token)
        {
            if (Version > 24)
            {
                if (attributeTypeRangesDic[imageDef].TryGetValue(token, out var index))
                {
                    return index;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return customAttributeIndex;
            }
        }

        public string GetStringLiteralFromIndex(uint index)
        {
            var stringLiteral = stringLiterals[index];
            Position = (uint)(header.stringLiteralDataOffset + stringLiteral.dataIndex);
            return Encoding.UTF8.GetString(ReadBytes((int)stringLiteral.length));
        }

        private void ProcessingMetadataUsage()
        {
            metadataUsageDic = new Dictionary<Il2CppMetadataUsage, SortedDictionary<uint, uint>>();
            for (uint i = 1; i <= 6; i++)
            {
                metadataUsageDic[(Il2CppMetadataUsage)i] = new SortedDictionary<uint, uint>();
            }
            foreach (var metadataUsageList in metadataUsageLists)
            {
                for (int i = 0; i < metadataUsageList.count; i++)
                {
                    var offset = metadataUsageList.start + i;
                    if (offset >= metadataUsagePairs.Length - 1)
                    {
                        continue;
                    }
                    var metadataUsagePair = metadataUsagePairs[offset];
                    var usage = GetEncodedIndexType(metadataUsagePair.encodedSourceIndex);
                    var decodedIndex = GetDecodedMethodIndex(metadataUsagePair.encodedSourceIndex);
                    metadataUsageDic[(Il2CppMetadataUsage)usage][metadataUsagePair.destinationIndex] = decodedIndex;
                }
            }
            //metadataUsagesCount = metadataUsagePairs.Max(x => x.destinationIndex) + 1;
            metadataUsagesCount = metadataUsageDic.Max(x => x.Value.Select(y => y.Key).DefaultIfEmpty().Max()) + 1;
        }

        public static uint GetEncodedIndexType(uint index)
        {
            return (index & 0xE0000000) >> 29;
        }

        public uint GetDecodedMethodIndex(uint index)
        {
            if (Version >= 27)
            {
                return (index & 0x1FFFFFFEU) >> 1;
            }
            return index & 0x1FFFFFFFU;
        }

        public int SizeOf(Type type)
        {
            var size = 0;
            foreach (var i in type.GetFields())
            {
                var attr = (VersionAttribute)Attribute.GetCustomAttribute(i, typeof(VersionAttribute));
                if (attr != null)
                {
                    if (Version < attr.Min || Version > attr.Max)
                        continue;
                }
                var fieldType = i.FieldType;
                if (fieldType.IsPrimitive)
                {
                    size += GetPrimitiveTypeSize(fieldType.Name);
                }
                else if (fieldType.IsEnum)
                {
                    var e = fieldType.GetField("value__").FieldType;
                    size += GetPrimitiveTypeSize(e.Name);
                }
                else if (fieldType.IsArray)
                {
                    var arrayLengthAttribute = i.GetCustomAttribute<ArrayLengthAttribute>();
                    size += arrayLengthAttribute.Length;
                }
                else
                {
                    size += SizeOf(fieldType);
                }
            }
            return size;

            static int GetPrimitiveTypeSize(string name)
            {
                return name switch
                {
                    "Int32" or "UInt32" => 4,
                    "Int16" or "UInt16" => 2,
                    _ => 0,
                };
            }
        }
    }
}
