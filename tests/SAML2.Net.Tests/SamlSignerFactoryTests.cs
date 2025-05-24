using System;
using NUnit.Framework;
using SAML2.Net;
using SAML2.Net.Signing;

namespace SAML2.Net.Tests
{
    [TestFixture]
    public class SamlSignerFactoryTests
    {
        [TestCase(SigningAlgorithm.SHA1, typeof(Sha1SigningAlgorithm))]
        [TestCase(SigningAlgorithm.SHA256, typeof(Sha256SigningAlgorithm))]
        [TestCase(SigningAlgorithm.SHA512, typeof(Sha512SigningAlgorithm))]
        public void TestFactoryForReturningCorrectSignerType(SigningAlgorithm encryptionMethod, Type type)
        {
            var signerFactory = new SamlSignerFactory(Helper.GetCertificate);
            var signer = (SamlSigner)signerFactory.Create(encryptionMethod);
            Assert.That(type.IsAssignableFrom(signer.SigningAlgorithm.GetType()));
        }
    }
}