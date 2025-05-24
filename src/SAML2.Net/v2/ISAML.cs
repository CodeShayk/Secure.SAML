using System.Xml;

namespace SAML2.Net.v2
{
    public interface ISAML
    {
        XmlDocument Create(Parameters parameters);

        string CreateEncoded(Parameters parameters);
    }
}