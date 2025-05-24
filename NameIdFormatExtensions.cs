using System;
using SAML2.Net;

namespace Secure.SAML.v2
{
    public static class NameIdFormatExtensions
    {
        public static string ToSamlString(this NameIdFormat nameIdFormat)
        {
            return nameIdFormat switch
            {
                NameIdFormat.EmailAddress => "urn:oasis:names:tc:SAML:2.0:nameid-format:emailAddress",
                NameIdFormat.Transient => "urn:oasis:names:tc:SAML:2.0:nameid-format:transient",
                NameIdFormat.Persistent => "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent",
                NameIdFormat.Unspecified => "urn:oasis:names:tc:SAML:2.0:nameid-format:unspecified",
                _ => throw new ArgumentOutOfRangeException(nameof(nameIdFormat), nameIdFormat, null)
            };
        }
    }
}