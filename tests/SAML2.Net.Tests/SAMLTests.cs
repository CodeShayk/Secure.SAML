using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;
using VerifyTests;

namespace SAML2.Net.Tests
{
    [TestFixture]
    public class SAMLTests
    {
        [SetUp]
        public void Setup()
        {
            saml = new v2.SAML(Helper.GetCertificate);
            settings = new VerifySettings();
            settings.UseDirectory("Approvals");
        }

        private v2.SAML saml;
        private VerifySettings settings;

        [TestCase(SigningAlgorithm.SHA1)]
        [TestCase(SigningAlgorithm.SHA256)]
        [TestCase(SigningAlgorithm.SHA512)]
        public async Task TestCreateUsingGivenAlgorithm(SigningAlgorithm signingAlgorithm)
        {
            var response = saml.Create(Helper.GetParameters(signingAlgorithm));
            await Verifier.Verify(response.OuterXml, settings);
        }

        [TestCase(SigningAlgorithm.SHA1)]
        [TestCase(SigningAlgorithm.SHA256)]
        [TestCase(SigningAlgorithm.SHA512)]
        public async Task TestCreateEncodedUsingGivenAlgorithm(SigningAlgorithm signingAlgorithm)
        {
            var response = saml.CreateEncoded(Helper.GetParameters(signingAlgorithm));
            await Verifier.Verify(response, settings);
        }
    }
}