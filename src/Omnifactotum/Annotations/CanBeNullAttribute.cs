using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    /// Indicates that the value of the marked element could be <b>null</b> sometimes,
    /// so the check for <b>null</b> is necessary before its usage
    /// </summary>
    /// <example><code>
    /// [CanBeNull] public object Test() { return null; }
    /// public void UseTest() {
    ///   var p = Test();
    ///   var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
    /// }
    /// </code></example>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate
            | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class CanBeNullAttribute : Attribute
    {
    }
}