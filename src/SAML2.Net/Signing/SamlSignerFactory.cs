using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SAML2.Net.Signing
{
    internal class SamlSignerFactory : ISamlSignerFactory
    {
        private readonly Func<X509Certificate2> certificateFactory;
        private readonly IDictionary<SigningAlgorithm, ISigningAlgorithm> encrytionMethods;

        public SamlSignerFactory(Func<X509Certificate2> certificateFactory)
        {
            this.certificateFactory = certificateFactory;
            encrytionMethods = GetEncryptionMethods();
        }

        public ISamlSigner Create(SigningAlgorithm encryptionMethod)
        {
            var certificate = certificateFactory();
            return new SamlSigner(encrytionMethods[encryptionMethod], certificate);
        }

        private IDictionary<SigningAlgorithm, ISigningAlgorithm> GetEncryptionMethods() =>
            new Dictionary<SigningAlgorithm, ISigningAlgorithm>
            {
                { SigningAlgorithm.SHA1, new Sha1SigningAlgorithm() }, { SigningAlgorithm.SHA256, new Sha256SigningAlgorithm() }, { SigningAlgorithm.SHA512, new Sha512SigningAlgorithm() }
            };
    }
}