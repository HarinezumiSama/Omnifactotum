using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation;

internal sealed partial class ObjectValidatorTests
{
    public sealed class SimpleContainer<T>
    {
        [MemberConstraint(typeof(NotNullConstraint))]
        public T? ContainedValue
        {
            [UsedImplicitly]
            get;
            set;
        }
    }

    private sealed class MapContainer
    {
        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(MapContainerPropertiesPairConstraint))]
        [MemberItemConstraint(typeof(CustomNotAbcStringMapKeyConstraint))]
        [MemberItemConstraint(
            typeof(KeyValuePairConstraint<string, SimpleContainer<int?>?, NotAbcStringConstraint, NotNullConstraint<SimpleContainer<int?>>>))]
        public IEnumerable<KeyValuePair<string, SimpleContainer<int?>>>? Properties
        {
            [UsedImplicitly]
            get;
            set;
        }
    }

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
        //// ReSharper disable once UnusedParameter.Local
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

    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Local")]
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

#if NET7_0_OR_GREATER
        [MemberConstraint<NotNullConstraint>]
        [MemberConstraint<NotNullConstraint<SimpleData>>]
        public SimpleData? GenericMemberConstraintAttributeData
        {
            [UsedImplicitly]
            internal get;
            set;
        }
#endif

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public AnotherSimpleData[]? AnotherSimpleDataArray
        {
            [UsedImplicitly]
            get;
            set;
        }

#if NET7_0_OR_GREATER
        [MemberConstraint<NotNullConstraint>]
        [MemberItemConstraint<NotNullConstraint>]
        public AnotherSimpleData[]? GenericMemberItemConstraintAnotherSimpleDataArray
        {
            [UsedImplicitly]
            get;
            set;
        }
#endif

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