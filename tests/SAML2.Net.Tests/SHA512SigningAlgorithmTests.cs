using System;
using System.Security.Cryptography.Xml;
using NUnit.Framework;
using SAML2.Net.Signing;

namespace SAML2.Net.Tests
{
    [TestFixture]
    public class SHA512SigningAlgorithmTests
    {
        [Test]
        public void TestSigningAlgorithmForCorrectSettings()
        {
            var encryptionMethod = new Sha512SigningAlgorithm();

            Assert.That(encryptionMethod.SignatureMethod, Is.EqualTo("http://www.w3.org/2001/04/xmldsig-more#rsa-sha512"));
            Assert.That(encryptionMethod.CanonicalizationMethod, Is.EqualTo("http://www.w3.org/2001/10/xml-exc-c14n#"));
            Assert.That(encryptionMethod.DigestMethod, Is.EqualTo("http://www.w3.org/2001/04/xmlenc#sha512"));

            var reference = new Reference();
            encryptionMethod.AddTransforms(reference);

            Assert.That(reference.TransformChain[0] is XmlDsigEnvelopedSignatureTransform, Is.True);
            Assert.That(reference.TransformChain[1] is XmlDsigExcC14NTransform, Is.True);

            var xmlDsigExcC14NTransform = (XmlDsigExcC14NTransform)reference.TransformChain[1];
            Assert.That(xmlDsigExcC14NTransform.InclusiveNamespacesPrefixList, Is.EqualTo("#default saml ds xs xsi"));
        }

        [Test]
        public void TestAddTransformsForNullArgumentToThrowException()
        {
            var encryptionMethod = new Sha512SigningAlgorithm();
            Assert.Throws<ArgumentException>(() => encryptionMethod.AddTransforms(null));
        }
    }
}