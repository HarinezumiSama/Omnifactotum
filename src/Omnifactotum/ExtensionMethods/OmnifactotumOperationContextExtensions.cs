using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System.ServiceModel
{
    /// <summary>
    ///     Contains extension methods for <see cref="OperationContext"/> class.
    /// </summary>
    public static class OmnifactotumOperationContextExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the client certificate for the specified operation context.
        /// </summary>
        /// <param name="operationContext">
        ///     The operation context to get the client certificate for.
        /// </param>
        /// <returns>
        ///     An <see cref="X509Certificate2"/> if the client has provided its certificate, or <b>null</b> otherwise.
        /// </returns>
        public static X509Certificate2 GetClientCertificate(this OperationContext operationContext)
        {
            var certificates = GetAllClientCertificates(operationContext);
            return certificates.FirstOrDefault();
        }

        #endregion

        #region Private Methods

        private static IEnumerable<X509Certificate2> GetAllClientCertificates(OperationContext operationContext)
        {
            if (operationContext == null || operationContext.ServiceSecurityContext == null
                || operationContext.ServiceSecurityContext.AuthorizationContext == null)
            {
                return Enumerable.Empty<X509Certificate2>();
            }

            var claimSets = operationContext.ServiceSecurityContext.AuthorizationContext.ClaimSets;
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

        #endregion
    }
}