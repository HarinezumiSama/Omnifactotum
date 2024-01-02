using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation;

internal sealed partial class ObjectValidatorTests
{
    private sealed class SimpleData
    {
        [SuppressMessage(
            "StyleCop.CSharp.MaintainabilityRules",
            "SA1401:FieldsMustBePrivate",
            Justification = "OK in the unit test.")]
        [MemberConstraint(typeof(UtcDateConstraint))]
        [MemberConstraint(typeof(UtcDateTypedConstraint))]
        public DateTime StartDate;

        [UsedImplicitly]
        [MemberConstraint(typeof(NeverCalledConstraint))]
        public int WriteOnlyValue
        {
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                // Nothing to do
            }
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        public string? Value
        {
            [UsedImplicitly]
            private get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        public int? NullableValue
        {
            [UsedImplicitly]
            private get;
            set;
        }

        [MemberConstraint(typeof(NeverCalledConstraint))]
        [MemberItemConstraint(typeof(NeverCalledConstraint))]
        [UsedImplicitly]
        public string this[int index] => throw new NotSupportedException();
    }

    private abstract class BaseAnotherSimpleData
    {
        //// No members
    }

    private sealed class AnotherSimpleData : BaseAnotherSimpleData
    {
        [MemberConstraint(typeof(NotNullConstraint))]
        public string? Value
        {
            [UsedImplicitly]
            get;
            set;
        }
    }

    private sealed class ComplexData
    {
        [UsedImplicitly]
        public string? Value { get; set; }

        [MemberConstraint(typeof(NotNullConstraint))]
        public SimpleData? Data
        {
            [UsedImplicitly]
            internal get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public AnotherSimpleData[]? AnotherSimpleDataArray
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public Collection<AnotherSimpleData>? AnotherSimpleDataCollection
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public CustomEnumerable? AnotherSimpleDataCustomEnumerable
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public CustomGenericEnumerable<AnotherSimpleData>? AnotherSimpleDataCustomGenericEnumerable
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public object? AnotherSimpleDataCustomGenericEnumerableObject
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public CustomGenericList<AnotherSimpleData>? AnotherSimpleDataCustomGenericList
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public CustomList? AnotherSimpleDataCustomList
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public CustomReadOnlyList<AnotherSimpleData>? AnotherSimpleDataCustomReadOnlyList
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public ImmutableList<AnotherSimpleData>? AnotherSimpleDataImmutableList
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public BaseAnotherSimpleData[]? MultipleDataItems
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullOrEmptyCollectionConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public ImmutableArray<BaseAnotherSimpleData> ImmutableMultipleDataItems
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberItemConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint<string>))]
        public ImmutableArray<string> ImmutableStrings
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint<string>))]
        public ImmutableArray<string>? NullableImmutableStrings
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint<string>))]
        public ImmutableArray<string>? AnotherNullableImmutableStrings
        {
            [UsedImplicitly]
            get;
            set;
        }

        [ValidatableMember]
        public BaseAnotherSimpleData? SingleBaseData
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberConstraint(typeof(NotNullOrEmptyStringConstraint))]
        public string? NonEmptyValue
        {
            [UsedImplicitly]
            private get;
            set;
        }
    }
}