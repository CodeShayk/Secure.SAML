using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SAML2.Net;

namespace SAML2.Net.Tests
{
    public static class Helper
    {
        /// <summary>
        ///     Get certificate
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate()
        {
            var signedStream = typeof(Helper)
                .Assembly.GetManifestResourceStream("SAML2.Net.Tests.SelfSignedKey.pfx");
            var signingCertRawData = new byte[signedStream.Length];
            signedStream.Read(signingCertRawData, 0, (int)signedStream.Length);
            return new X509Certificate2(signingCertRawData, "password", X509KeyStorageFlags.Exportable);
        }

        public static Parameters GetParameters(SigningAlgorithm algorithm) => new(
            "http://ninjacorp.com",
            "https://xyz.target-link.co.uk:443/saml/api",
            new[] { "xyz.target-link.co.uk" },
            "NIN0123456",
            NameIdFormat.Unspecified,
            new Dictionary<string, string> { { "Custom_key", "value" } },
            SignType.Response,
            10,
            algorithm,
            Guid.Parse("95AD6A84-95C1-4B39-AE5E-FE1E700C406C"),
            Guid.Parse("B3CA912A-4A6B-4F31-9FD8-FC5E55837656"),
            DateTime.Parse("2018-02-27T09:36:44.0665619Z")
        );
    }
}