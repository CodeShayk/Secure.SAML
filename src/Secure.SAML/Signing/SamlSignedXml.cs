using System.Security.Cryptography.Xml;
using System.Xml;

namespace Secure.SAML.Signing
{
    /// <summary>
    ///     SamlSignedXml - Class is used to sign xml, basically the when the ID is retreived the correct ID is used.
    ///     without this, the id reference would not be valid.
    /// </summary>
    internal class SamlSignedXml : SignedXml
    {
        private readonly string _referenceAttributeId = "";

        public SamlSignedXml(XmlDocument document, string referenceAttributeId) : base(document) => _referenceAttributeId = referenceAttributeId;

        public override XmlElement GetIdElement(XmlDocument document, string idValue) => (XmlElement)document.SelectSingleNode(string.Format("//*[@{0}='{1}']", _referenceAttributeId, idValue));
    }
}