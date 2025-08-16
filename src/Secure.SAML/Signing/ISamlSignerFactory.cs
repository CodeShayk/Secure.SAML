using Secure.SAML;

namespace Secure.SAML.Signing
{
    internal interface ISamlSignerFactory
    {
        ISamlSigner Create(SigningAlgorithm encryptionMethod);
    }
}