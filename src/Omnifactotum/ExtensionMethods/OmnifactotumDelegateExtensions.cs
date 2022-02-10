#nullable enable

using System.Linq;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System
{
    /// <summary>
    ///     Contains extension methods for the <see cref="Delegate"/> types.
    /// </summary>
    public static class OmnifactotumDelegateExtensions
    {
        /// <summary>
        ///     Gets the strongly typed invocation list of the specified delegate.
        /// </summary>
        /// <param name="delegate">
        ///     The delegate to get the strongly typed invocation list of.
        /// </param>
        /// <typeparam name="TDelegate">
        ///     The type of the delegate to get the invocations of.
        /// </typeparam>
        /// <returns>
        ///     The strongly typed invocation list of the specified delegate.
        /// </returns>
        /// <seealso cref="Delegate.GetInvocationList"/>
        /// <example>
        ///     <code>
        /// <![CDATA[
        /// public event EventHandler<EventArgs>? SomeAction;
        /// // ...
        /// protected void OnSomeAction()
        /// {
        ///     var invocations = SomeAction.GetTypedInvocations();
        ///     foreach (var invocation in invocations)
        ///     {
        ///         try
        ///         {
        ///             invocation(this, EventArgs.Empty);
        ///         }
        ///         catch (Exception)
        ///         {
        ///             //  Logging the exception
        ///         }
        ///     }
        /// }
        /// ]]>
        ///     </code>
        /// </example>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static TDelegate[] GetTypedInvocations<TDelegate>([CanBeNull] this TDelegate? @delegate)
            where TDelegate : Delegate
            => (@delegate?.GetInvocationList().Cast<TDelegate>().ToArray()).AvoidNull();
    }
}