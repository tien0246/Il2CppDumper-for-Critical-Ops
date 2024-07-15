using System;
using System.Collections.Generic;
using System.Reflection;

namespace Il2CppDumper
{
    public class Il2CppGlobalMetadataHeader
    {
        public uint sanity;
        public int version;
        public uint stringLiteralOffset; // string data for managed code
        public int stringLiteralSize;
        public uint stringLiteralDataOffset;
        public int stringLiteralDataSize;
        public uint stringOffset; // string data for metadata
        public int stringSize;
        public uint eventsOffset; // Il2CppEventDefinition
        public int eventsSize;
        public uint propertiesOffset; // Il2CppPropertyDefinition
        public int propertiesSize;
        public uint methodsOffset; // Il2CppMethodDefinition
        public int methodsSize;
        public uint parameterDefaultValuesOffset; // Il2CppParameterDefaultValue
        public int parameterDefaultValuesSize;
        public uint fieldDefaultValuesOffset; // Il2CppFieldDefaultValue
        public int fieldDefaultValuesSize;
        public uint fieldAndParameterDefaultValueDataOffset; // uint8_t
        public int fieldAndParameterDefaultValueDataSize;
        public int fieldMarshaledSizesOffset; // Il2CppFieldMarshaledSize
        public int fieldMarshaledSizesSize;
        public uint parametersOffset; // Il2CppParameterDefinition
        public int parametersSize;
        public uint fieldsOffset; // Il2CppFieldDefinition
        public int fieldsSize;
        public uint genericParametersOffset; // Il2CppGenericParameter
        public int genericParametersSize;
        public uint genericParameterConstraintsOffset; // TypeIndex
        public int genericParameterConstraintsSize;
        public uint genericContainersOffset; // Il2CppGenericContainer
        public int genericContainersSize;
        public uint nestedTypesOffset; // TypeDefinitionIndex
        public int nestedTypesSize;
        public uint interfacesOffset; // TypeIndex
        public int interfacesSize;
        public uint vtableMethodsOffset; // EncodedMethodIndex
        public int vtableMethodsSize;
        public int interfaceOffsetsOffset; // Il2CppInterfaceOffsetPair
        public int interfaceOffsetsSize;
        public uint typeDefinitionsOffset; // Il2CppTypeDefinition
        public int typeDefinitionsSize;
        public uint imagesOffset; // Il2CppImageDefinition
        public int imagesSize;
        public uint assembliesOffset; // Il2CppAssemblyDefinition
        public int assembliesSize;
        public uint fieldRefsOffset; // Il2CppFieldRef
        public int fieldRefsSize;
        public int referencedAssembliesOffset; // int32_t
        public int referencedAssembliesSize;
        public uint attributeDataOffset;
        public int attributeDataSize;
        public uint attributeDataRangeOffset;
        public int attributeDataRangeSize;
        public int unresolvedVirtualCallParameterTypesOffset; // TypeIndex
        public int unresolvedVirtualCallParameterTypesSize;
        public int unresolvedVirtualCallParameterRangesOffset; // Il2CppRange
        public int unresolvedVirtualCallParameterRangesSize;
        public int windowsRuntimeTypeNamesOffset; // Il2CppWindowsRuntimeTypeNamePair
        public int windowsRuntimeTypeNamesSize;
        public int windowsRuntimeStringsOffset; // const char*
        public int windowsRuntimeStringsSize;
        public int exportedTypeDefinitionsOffset; // TypeDefinitionIndex
        public int exportedTypeDefinitionsSize;

        public void PrintAll()
        {
            Console.WriteLine($"sanity: {sanity}");
            Console.WriteLine($"version: {version}");
            Console.WriteLine($"stringLiteralOffset: {stringLiteralOffset}");
            Console.WriteLine($"stringLiteralSize: {stringLiteralSize}");
            Console.WriteLine($"stringLiteralDataOffset: {stringLiteralDataOffset}");
            Console.WriteLine($"stringLiteralDataSize: {stringLiteralDataSize}");
            Console.WriteLine($"stringOffset: {stringOffset}");
            Console.WriteLine($"stringSize: {stringSize}");
            Console.WriteLine($"eventsOffset: {eventsOffset}");
            Console.WriteLine($"eventsSize: {eventsSize}");
            Console.WriteLine($"propertiesOffset: {propertiesOffset}");
            Console.WriteLine($"propertiesSize: {propertiesSize}");
            Console.WriteLine($"methodsOffset: {methodsOffset}");
            Console.WriteLine($"methodsSize: {methodsSize}");
            Console.WriteLine($"parameterDefaultValuesOffset: {parameterDefaultValuesOffset}");
            Console.WriteLine($"parameterDefaultValuesSize: {parameterDefaultValuesSize}");
            Console.WriteLine($"fieldDefaultValuesOffset: {fieldDefaultValuesOffset}");
            Console.WriteLine($"fieldDefaultValuesSize: {fieldDefaultValuesSize}");
            Console.WriteLine($"fieldAndParameterDefaultValueDataOffset: {fieldAndParameterDefaultValueDataOffset}");
            Console.WriteLine($"fieldAndParameterDefaultValueDataSize: {fieldAndParameterDefaultValueDataSize}");
            Console.WriteLine($"fieldMarshaledSizesOffset: {fieldMarshaledSizesOffset}");
            Console.WriteLine($"fieldMarshaledSizesSize: {fieldMarshaledSizesSize}");
            Console.WriteLine($"parametersOffset: {parametersOffset}");
            Console.WriteLine($"parametersSize: {parametersSize}");
            Console.WriteLine($"fieldsOffset: {fieldsOffset}");
            Console.WriteLine($"fieldsSize: {fieldsSize}");
            Console.WriteLine($"genericParametersOffset: {genericParametersOffset}");
            Console.WriteLine($"genericParametersSize: {genericParametersSize}");
            Console.WriteLine($"genericParameterConstraintsOffset: {genericParameterConstraintsOffset}");
            Console.WriteLine($"genericParameterConstraintsSize: {genericParameterConstraintsSize}");
            Console.WriteLine($"genericContainersOffset: {genericContainersOffset}");
            Console.WriteLine($"genericContainersSize: {genericContainersSize}");
            Console.WriteLine($"nestedTypesOffset: {nestedTypesOffset}");
            Console.WriteLine($"nestedTypesSize: {nestedTypesSize}");
            Console.WriteLine($"interfacesOffset: {interfacesOffset}");
            Console.WriteLine($"interfacesSize: {interfacesSize}");
            Console.WriteLine($"vtableMethodsOffset: {vtableMethodsOffset}");
            Console.WriteLine($"vtableMethodsSize: {vtableMethodsSize}");
            Console.WriteLine($"interfaceOffsetsOffset: {interfaceOffsetsOffset}");
            Console.WriteLine($"interfaceOffsetsSize: {interfaceOffsetsSize}");
            Console.WriteLine($"typeDefinitionsOffset: {typeDefinitionsOffset}");
            Console.WriteLine($"typeDefinitionsSize: {typeDefinitionsSize}");
            Console.WriteLine($"imagesOffset: {imagesOffset}");
            Console.WriteLine($"imagesSize: {imagesSize}");
            Console.WriteLine($"assembliesOffset: {assembliesOffset}");
            Console.WriteLine($"assembliesSize: {assembliesSize}");
            Console.WriteLine($"fieldRefsOffset: {fieldRefsOffset}");
            Console.WriteLine($"fieldRefsSize: {fieldRefsSize}");
            Console.WriteLine($"referencedAssembliesOffset: {referencedAssembliesOffset}");
            Console.WriteLine($"referencedAssembliesSize: {referencedAssembliesSize}");
            Console.WriteLine($"attributeDataOffset: {attributeDataOffset}");
            Console.WriteLine($"attributeDataSize: {attributeDataSize}");
            Console.WriteLine($"attributeDataRangeOffset: {attributeDataRangeOffset}");
            Console.WriteLine($"attributeDataRangeSize: {attributeDataRangeSize}");
            Console.WriteLine($"unresolvedVirtualCallParameterTypesOffset: {unresolvedVirtualCallParameterTypesOffset}");
            Console.WriteLine($"unresolvedVirtualCallParameterTypesSize: {unresolvedVirtualCallParameterTypesSize}");
            Console.WriteLine($"unresolvedVirtualCallParameterRangesOffset: {unresolvedVirtualCallParameterRangesOffset}");
            Console.WriteLine($"unresolvedVirtualCallParameterRangesSize: {unresolvedVirtualCallParameterRangesSize}");
            Console.WriteLine($"windowsRuntimeTypeNamesOffset: {windowsRuntimeTypeNamesOffset}");
            Console.WriteLine($"windowsRuntimeTypeNamesSize: {windowsRuntimeTypeNamesSize}");
            Console.WriteLine($"windowsRuntimeStringsOffset: {windowsRuntimeStringsOffset}");
            Console.WriteLine($"windowsRuntimeStringsSize: {windowsRuntimeStringsSize}");
            Console.WriteLine($"exportedTypeDefinitionsOffset: {exportedTypeDefinitionsOffset}");
            Console.WriteLine($"exportedTypeDefinitionsSize: {exportedTypeDefinitionsSize}");
        }
    }

    public class Il2CppAssemblyDefinition
    {
        public int imageIndex;
        [Version(Min = 24.1)]
        public uint token;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Version(Min = 20)]
        public int referencedAssemblyStart;
        [Version(Min = 20)]
        public int referencedAssemblyCount;
        public Il2CppAssemblyNameDefinition aname;
    }

    public class Il2CppAssemblyNameDefinition
    {
        public uint nameIndex;
        public uint cultureIndex;
        [Version(Max = 24.3)]
        public int hashValueIndex;
        public uint publicKeyIndex;
        public uint hash_alg;
        public int hash_len;
        public uint flags;
        public int major;
        public int minor;
        public int build;
        public int revision;
        [ArrayLength(Length = 8)]
        public byte[] public_key_token;
    }

    public class Il2CppImageDefinition
    {
        public uint nameIndex;
        public int assemblyIndex;

        public int typeStart;
        public uint typeCount;

        [Version(Min = 24)]
        public int exportedTypeStart;
        [Version(Min = 24)]
        public uint exportedTypeCount;

        public int entryPointIndex;
        [Version(Min = 19)]
        public uint token;

        [Version(Min = 24.1)]
        public int customAttributeStart;
        [Version(Min = 24.1)]
        public uint customAttributeCount;
    }

    public class Il2CppTypeDefinition
    {
        public uint nameIndex;
        public uint namespaceIndex;
        [Version(Max = 24)]
        public int customAttributeIndex;
        public int byvalTypeIndex;
        [Version(Max = 24.5)]
        public int byrefTypeIndex;

        public int declaringTypeIndex;
        public int parentIndex;
        public int elementTypeIndex; // we can probably remove this one. Only used for enums

        [Version(Max = 24.1)]
        public int rgctxStartIndex;
        [Version(Max = 24.1)]
        public int rgctxCount;

        public int genericContainerIndex;

        [Version(Max = 22)]
        public int delegateWrapperFromManagedToNativeIndex;
        [Version(Max = 22)]
        public int marshalingFunctionsIndex;
        [Version(Min = 21, Max = 22)]
        public int ccwFunctionIndex;
        [Version(Min = 21, Max = 22)]
        public int guidIndex;

        public uint flags;

        public int fieldStart;
        public int methodStart;
        public int eventStart;
        public int propertyStart;
        public int nestedTypesStart;
        public int interfacesStart;
        public int vtableStart;
        public int interfaceOffsetsStart;

        public ushort method_count;
        public ushort property_count;
        public ushort field_count;
        public ushort event_count;
        public ushort nested_type_count;
        public ushort vtable_count;
        public ushort interfaces_count;
        public ushort interface_offsets_count;

        // bitfield to portably encode boolean values as single bits
        // 01 - valuetype;
        // 02 - enumtype;
        // 03 - has_finalize;
        // 04 - has_cctor;
        // 05 - is_blittable;
        // 06 - is_import_or_windows_runtime;
        // 07-10 - One of nine possible PackingSize values (0, 1, 2, 4, 8, 16, 32, 64, or 128)
        // 11 - PackingSize is default
        // 12 - ClassSize is default
        // 13-16 - One of nine possible PackingSize values (0, 1, 2, 4, 8, 16, 32, 64, or 128) - the specified packing size (even for explicit layouts)
        public uint bitfield;
        [Version(Min = 19)]
        public uint token;

        public bool IsValueType => (bitfield & 0x1) == 1;
        public bool IsEnum => ((bitfield >> 1) & 0x1) == 1;
    }

    public class Il2CppMethodDefinition
    {
        public uint nameIndex;
        public int declaringType;
        public int returnType;
        public int parameterStart;
        [Version(Max = 24)]
        public int customAttributeIndex;
        public int genericContainerIndex;
        [Version(Max = 24.1)]
        public int methodIndex;
        [Version(Max = 24.1)]
        public int invokerIndex;
        [Version(Max = 24.1)]
        public int delegateWrapperIndex;
        [Version(Max = 24.1)]
        public int rgctxStartIndex;
        [Version(Max = 24.1)]
        public int rgctxCount;
        public uint token;
        public ushort flags;
        public ushort iflags;
        public ushort slot;
        public ushort parameterCount;
    }

    public class Il2CppParameterDefinition
    {
        public uint nameIndex;
        public uint token;
        [Version(Max = 24)]
        public int customAttributeIndex;
        public int typeIndex;
    }

    public class Il2CppFieldDefinition
    {
        public uint nameIndex;
        public int typeIndex;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Version(Min = 19)]
        public uint token;
    }

    public class Il2CppFieldDefaultValue
    {
        public int fieldIndex;
        public int typeIndex;
        public int dataIndex;
    }

    public class Il2CppPropertyDefinition
    {
        public uint nameIndex;
        public int get;
        public int set;
        public uint attrs;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Version(Min = 19)]
        public uint token;
    }

    public class Il2CppCustomAttributeTypeRange
    {
        [Version(Min = 24.1)]
        public uint token;
        public int start;
        public int count;
    }

    public class Il2CppMetadataUsageList
    {
        public uint start;
        public uint count;
    }

    public class Il2CppMetadataUsagePair
    {
        public uint destinationIndex;
        public uint encodedSourceIndex;
    }

    public class Il2CppStringLiteral
    {
        public uint length;
        public int dataIndex;
    }

    public class Il2CppParameterDefaultValue
    {
        public int parameterIndex;
        public int typeIndex;
        public int dataIndex;
    }

    public class Il2CppEventDefinition
    {
        public uint nameIndex;
        public int typeIndex;
        public int add;
        public int remove;
        public int raise;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Version(Min = 19)]
        public uint token;
    }

    public class Il2CppGenericContainer
    {
        /* index of the generic type definition or the generic method definition corresponding to this container */
        public int ownerIndex; // either index into Il2CppClass metadata array or Il2CppMethodDefinition array
        public int type_argc;
        /* If true, we're a generic method, otherwise a generic type definition. */
        public int is_method;
        /* Our type parameters. */
        public int genericParameterStart;
    }

    public class Il2CppFieldRef
    {
        public int typeIndex;
        public int fieldIndex; // local offset into type fields
    }

    public class Il2CppGenericParameter
    {
        public int ownerIndex;  /* Type or method this parameter was defined in. */
        public uint nameIndex;
        public short constraintsStart;
        public short constraintsCount;
        public ushort num;
        public ushort flags;
    }

    public enum Il2CppRGCTXDataType
    {
        IL2CPP_RGCTX_DATA_INVALID,
        IL2CPP_RGCTX_DATA_TYPE,
        IL2CPP_RGCTX_DATA_CLASS,
        IL2CPP_RGCTX_DATA_METHOD,
        IL2CPP_RGCTX_DATA_ARRAY,
        IL2CPP_RGCTX_DATA_CONSTRAINED,
    }

    public class Il2CppRGCTXDefinitionData
    {
        public int rgctxDataDummy;
        public int methodIndex => rgctxDataDummy;
        public int typeIndex => rgctxDataDummy;
    }

    public class Il2CppRGCTXDefinition
    {
        public Il2CppRGCTXDataType type => type_post29 == 0 ? (Il2CppRGCTXDataType)type_pre29 : (Il2CppRGCTXDataType)type_post29;
        [Version(Max = 27.1)]
        public int type_pre29;
        [Version(Min = 29)]
        public ulong type_post29;
        [Version(Max = 27.1)]
        public Il2CppRGCTXDefinitionData data;
        [Version(Min = 27.2)]
        public ulong _data;
    }

    public enum Il2CppMetadataUsage
    {
        kIl2CppMetadataUsageInvalid,
        kIl2CppMetadataUsageTypeInfo,
        kIl2CppMetadataUsageIl2CppType,
        kIl2CppMetadataUsageMethodDef,
        kIl2CppMetadataUsageFieldInfo,
        kIl2CppMetadataUsageStringLiteral,
        kIl2CppMetadataUsageMethodRef,
    };

    public class Il2CppCustomAttributeDataRange
    {
        public uint token;
        public uint startOffset;
    }
}