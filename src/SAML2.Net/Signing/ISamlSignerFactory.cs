namespace SAML2.Net.Signing
{
    internal interface ISamlSignerFactory
    {
        ISamlSigner Create(SigningAlgorithm encryptionMethod);
    }
}