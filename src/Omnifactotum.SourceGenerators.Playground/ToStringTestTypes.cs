using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Omnifactotum.Annotations;

//// ReSharper disable ArrangeAttributes
//// ReSharper disable RedundantNameQualifier

namespace Omnifactotum.SourceGenerators.Playground;

#pragma warning disable OFSG0001 // TODO: TEMP
#pragma warning disable OFSG0002 // TODO: TEMP
#pragma warning disable OFSG0003 // TODO: TEMP

[DebuggerDisplay("Class: {ToString(),nq}")]
[GeneratedToString]
internal sealed partial class ToStringTestClass
{
    public required string StringValue1 { get; init; }

    public int this[int index] => index;
}

[DebuggerDisplay("Class: {ToString(),nq}")]
[GeneratedToString]
internal sealed partial class ToStringTestGenericClass<T>
{
    public required string StringValue2 { get; init; }

    public required T GenericValue { get; init; }
}

[GeneratedToString, DebuggerDisplay("Record class: {ToString(),nq}")]
[SuppressMessage("ReSharper", "MissingXmlDoc")]
public sealed record ToStringTestRecordClass
{
    public required string StringValue3 { get; init; }
}

[DebuggerDisplay("Struct: {ToString(),nq}")]
[Omnifactotum.Annotations.GeneratedToString(IncludeTypeName = true)]
internal partial struct ToStringTestStruct
{
    public required string StringValue4 { get; init; }
}

[DebuggerDisplay("Record struct: {ToString(),nq}"), GeneratedToString]
internal record struct ToStringTestRecordStruct
{
    public required string StringValue5 { get; init; }
}

[GeneratedToString]
[SuppressMessage("ReSharper", "UnusedType.Local")]
[SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
internal static partial class ToStringStaticTestClass
{
    public static string? StringValue6 { get; set; }
}

[GeneratedToString]
internal partial class ToStringWithExistingToStringTestClass
{
    public static string? StringValue7 { get; set; }

    public override string ToString() => $"{nameof(ToStringWithExistingToStringTestClass)}: {StringValue7}";
}

[GeneratedToString]
[SuppressMessage("ReSharper", "UnusedType.Local")]
[SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
file partial class ToStringFileLocalTestClass
{
    public static string? StringValue8 { get; set; }
}