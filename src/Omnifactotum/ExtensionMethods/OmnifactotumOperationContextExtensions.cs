using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Claims;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace

namespace System.ServiceModel
{
    /// <summary>
    ///     Contains extension methods for <see cref="OperationContext"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class OmnifactotumOperationContextExtensions
    {
        /// <summary>
        ///     Gets the client certificate for the specified operation context.
        /// </summary>
        /// <param name="operationContext">
        ///     The operation context to get the client certificate for.
        /// </param>
        /// <returns>
        ///     An <see cref="X509Certificate2"/> if the client has provided its certificate, or <c>null</c> otherwise.
        /// </returns>
        [CanBeNull]
        public static X509Certificate2 GetClientCertificate([CanBeNull] this OperationContext operationContext)
        {
            var certificates = GetAllClientCertificates(operationContext);
            return certificates.FirstOrDefault();
        }

        [NotNull]
        private static IEnumerable<X509Certificate2> GetAllClientCertificates(
            [CanBeNull] OperationContext operationContext)
        {
            var claimSets = operationContext?.ServiceSecurityContext?.AuthorizationContext?.ClaimSets;
            if (claimSets == null)
            {
                return Enumerable.Empty<X509Certificate2>();
            }

            return claimSets
                .OfType<X509CertificateClaimSet>()
                .Select(claimSet => claimSet.X509Certificate)
                .Where(certificate => certificate != null)
                .ToArray();
        }
    }
}