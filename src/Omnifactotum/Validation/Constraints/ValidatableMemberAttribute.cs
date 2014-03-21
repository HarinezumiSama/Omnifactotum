using System;
using System.Linq;

namespace Omnifactotum.Validation.Constraints
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidatableMemberAttribute : Attribute
    {
        // No members
    }
}