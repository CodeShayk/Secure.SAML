using System;
using System.Security.Cryptography.Xml;
using NUnit.Framework;
using Secure.SAML.Signing;

namespace Secure.SAML.Tests
{
    [TestFixture]
    public class SHA256SigningAlgorithmTests
    {
        [Test]
        public void TestSigningAlgorithmForCorrectSettings()
        {
            var encryptionMethod = new Sha256SigningAlgorithm();

            Assert.That(encryptionMethod.SignatureMethod, Is.EqualTo("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"));
            Assert.That(encryptionMethod.CanonicalizationMethod, Is.EqualTo("http://www.w3.org/2001/10/xml-exc-c14n#"));
            Assert.That(encryptionMethod.DigestMethod, Is.EqualTo("http://www.w3.org/2001/04/xmlenc#sha256"));

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
            var encryptionMethod = new Sha256SigningAlgorithm();
            Assert.Throws<ArgumentException>(() => encryptionMethod.AddTransforms(null));
        }
    }
}