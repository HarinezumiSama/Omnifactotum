#if NETFRAMEWORK

using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Claims;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.ServiceModel
{
    /// <summary>
    ///     Contains extension methods for <see cref="OperationContext"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [PublicAPI]
    public static class OmnifactotumOperationContextExtensions
    {
        /// <summary>
        ///     Gets the client certificate for the specified operation context.
        /// </summary>
        /// <param name="operationContext">
        ///     The operation context to get the client certificate for.
        /// </param>
        /// <returns>
        ///     An <see cref="X509Certificate2"/> if the client has provided its certificate, or <see langword="null"/> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        public static X509Certificate2? GetClientCertificate([CanBeNull] this OperationContext? operationContext)
            => operationContext.GetAllClientCertificates().FirstOrDefault();

        [NotNull]
        private static X509Certificate2[] GetAllClientCertificates([CanBeNull] this OperationContext? operationContext)
            => operationContext?.ServiceSecurityContext?.AuthorizationContext?.ClaimSets?
                    .OfType<X509CertificateClaimSet>()
                    .Select(claimSet => claimSet.X509Certificate)
                    .Where(certificate => certificate is not null)
                    .ToArray()
                ?? Array.Empty<X509Certificate2>();
    }
}

#endif