using System;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies that the content of annotated member is validated.
    ///     Used for members that do not have constraints applied to them.
    /// </summary>
    //// ReSharper disable RedundantAttributeUsageProperty - Just making sure
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    //// ReSharper restore RedundantAttributeUsageProperty
    public sealed class ValidatableMemberAttribute : BaseValidatableMemberAttribute
    {
        // No members
    }
}