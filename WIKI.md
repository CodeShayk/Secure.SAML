# Secure.SAML - Comprehensive Wiki Documentation

## Table of Contents
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Core Components](#core-components)
4. [Installation & Setup](#installation--setup)
5. [API Reference](#api-reference)
6. [Usage Examples](#usage-examples)
7. [Configuration Options](#configuration-options)
8. [Security Features](#security-features)
9. [Testing](#testing)
10. [Troubleshooting](#troubleshooting)
11. [Contributing](#contributing)

## Overview

**Secure.SAML** is a robust .NET utility library designed to generate signed SAML 2.0 Response messages. It provides a clean, type-safe API for creating SAML assertions with support for multiple cryptographic signing algorithms and flexible configuration options.

### Key Features
- **Multiple Signing Algorithms**: Supports SHA1, SHA256, and SHA512
- **Flexible Signing**: Sign either the entire Response or individual Assertion
- **Multiple Output Formats**: XML Document or Base64 encoded string
- **Cross-Platform**: Supports .NET Framework 4.6.2+, .NET Standard 2.0+, and .NET 9.0
- **Type Safety**: Strongly-typed parameters and validation
- **Extensible**: Factory pattern for custom signing algorithms

### Supported Platforms
- .NET Framework 4.6.2+
- .NET Standard 2.0+
- .NET Standard 2.1+
- .NET 9.0
- .NET 10.0

## Architecture

The Secure.SAML library follows a clean, layered architecture with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Public API Layer                        │
├─────────────────────────────────────────────────────────────┤
│                    Core SAML Logic                         │
├─────────────────────────────────────────────────────────────┤
│                    Signing Infrastructure                  │
├─────────────────────────────────────────────────────────────┤
│                    XML Schema Support                      │
└─────────────────────────────────────────────────────────────┘
```

### Design Patterns Used
- **Factory Pattern**: For creating signing algorithms and signers
- **Strategy Pattern**: For different signing algorithms
- **Builder Pattern**: For constructing SAML parameters
- **Interface Segregation**: Clean separation of concerns

## Core Components

### 1. Main SAML Class (`SAML.cs`)
The primary entry point for creating SAML responses.

**Key Responsibilities:**
- Orchestrates SAML response creation
- Manages signing process
- Handles XML document generation
- Coordinates between parameters and signing infrastructure

**Key Methods:**
- `Create(Parameters parameters)`: Returns XMLDocument with signed SAML
- `CreateEncoded(Parameters parameters)`: Returns Base64 encoded string

### 2. Parameters Class (`Parameters.cs`)
Configuration container for SAML generation.

**Core Properties:**
```csharp
public class Parameters
{
    public string Issuer { get; set; }                    // Issuer name/domain
    public string Recipient { get; set; }                 // Consumer service URL
    public string[] AudienceRestrictions { get; set; }    // Audience restrictions
    public string NamedId { get; set; }                   // User identity/subject
    public NameIdFormat NameIdFormat { get; set; }        // Name ID format
    public Dictionary<string, string> Attributes { get; set; } // Custom attributes
    public SignType SignatureType { get; set; }           // Response or Assertion signing
    public int NotOnOrAfterInMinutes { get; }             // Validity period
    public SigningAlgorithm SigningAlgorithm { get; set; } // Cryptographic algorithm
    public DateTime Timestamp { get; }                     // Message timestamp
}
```

**Default Values:**
- `NotOnOrAfterInMinutes`: 10 minutes
- `SignatureType`: Response signing
- `NameIdFormat`: Unspecified
- `SigningAlgorithm`: SHA512
- `Timestamp`: Current UTC time
- `SamlId` & `AssertionId`: Auto-generated GUIDs

### 3. Signing Infrastructure

#### SamlSignerFactory
Creates appropriate signer instances based on the requested algorithm.

**Supported Algorithms:**
- **SHA1**: `SigningAlgorithm.SHA1`
- **SHA256**: `SigningAlgorithm.SHA256` 
- **SHA512**: `SigningAlgorithm.SHA512` (Default)

#### SamlSigner
Handles the actual XML signing process using the selected algorithm.

**Signing Process:**
1. Loads X.509 certificate
2. Applies canonicalization (Exclusive Canonical XML)
3. Creates digital signature
4. Embeds signature in XML document
5. Adds certificate information

#### Signing Algorithms
Each algorithm implements `ISigningAlgorithm` interface:

```csharp
public interface ISigningAlgorithm
{
    string CanonicalizationMethod { get; }
    string SignatureMethod { get; }
    string DigestMethod { get; }
    void AddTransforms(Reference reference);
}
```

### 4. SAML Schema Support (`v2/` folder)
Contains generated classes from SAML 2.0 XSD schemas:
- `saml-schema-assertion-2.0.xsd`
- `saml-schema-protocol-2.0.xsd`
- `xmldsig-core-schema.xsd`
- `xenc-core-schema.xsd`

## Installation & Setup

### NuGet Package
```bash
Install-Package Secure.SAML
```

### Basic Setup
```csharp
using Secure.SAML;
using Secure.SAML.v2;

// Create SAML instance with certificate factory
var saml = new SAML(() => GetSigningCertificate());
```

### Dependency Injection Setup
```csharp
// Register in your DI container
services.AddSingleton<ISAML>(provider => 
    new SAML(() => GetSigningCertificate()));

// Or with custom certificate factory
services.AddSingleton<ISAML>(provider => 
    new SAML(() => CertificateService.GetSigningCertificate()));
```

## API Reference

### ISAML Interface
```csharp
public interface ISAML
{
    /// <summary>
    /// Creates a signed SAML Response as XmlDocument
    /// </summary>
    XmlDocument Create(Parameters parameters);
    
    /// <summary>
    /// Creates a signed SAML Response as Base64 encoded string
    /// </summary>
    string CreateEncoded(Parameters parameters);
}
```

### Parameters Builder Pattern
```csharp
var parameters = new Parameters
{
    Issuer = "https://your-domain.com",
    Recipient = "https://sp-domain.com/saml/acs",
    AudienceRestrictions = new[] { "sp-domain.com" },
    NamedId = "user123@domain.com",
    NameIdFormat = NameIdFormat.EmailAddress,
    Attributes = new Dictionary<string, string>
    {
        { "FirstName", "John" },
        { "LastName", "Doe" },
        { "Email", "john.doe@domain.com" }
    },
    SignatureType = SignType.Response,
    NotOnOrAfterInMinutes = 15,
    SigningAlgorithm = SigningAlgorithm.SHA256
};
```

### Enums

#### SignType
```csharp
public enum SignType
{
    Response,    // Sign the entire SAML Response
    Assertion    // Sign only the SAML Assertion
}
```

#### NameIdFormat
```csharp
public enum NameIdFormat
{
    None,                    // No format specified
    Unspecified,            // urn:oasis:names:tc:SAML:2.0:nameid-format:unspecified
    EmailAddress,           // urn:oasis:names:tc:SAML:2.0:nameid-format:emailAddress
    X509SubjectName,        // urn:oasis:names:tc:SAML:2.0:nameid-format:X509SubjectName
    WindowsDomainQualifiedName, // urn:oasis:names:tc:SAML:2.0:nameid-format:WindowsDomainQualifiedName
    Kerberos,               // urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos
    Entity,                 // urn:oasis:names:tc:SAML:2.0:nameid-format:entity
    Persistent,             // urn:oasis:names:tc:SAML:2.0:nameid-format:persistent
    Transient               // urn:oasis:names:tc:SAML:2.0:nameid-format:transient
}
```

#### SigningAlgorithm
```csharp
public enum SigningAlgorithm
{
    SHA1,       // SHA-1 with RSA (legacy support)
    SHA256,     // SHA-256 with RSA (recommended)
    SHA512      // SHA-512 with RSA (maximum security)
}
```

## Usage Examples

### Basic SAML Response Creation
```csharp
var saml = new SAML(() => GetSigningCertificate());

var parameters = new Parameters
{
    Issuer = "https://idp.company.com",
    Recipient = "https://app.company.com/saml/acs",
    AudienceRestrictions = new[] { "app.company.com" },
    NamedId = "john.doe@company.com",
    NameIdFormat = NameIdFormat.EmailAddress,
    Attributes = new Dictionary<string, string>
    {
        { "Department", "Engineering" },
        { "Role", "Developer" }
    }
};

// Generate XML document
var xmlDocument = saml.Create(parameters);

// Generate Base64 encoded string
var base64String = saml.CreateEncoded(parameters);
```

### Advanced Configuration
```csharp
var parameters = new Parameters
{
    Issuer = "https://idp.company.com",
    Recipient = "https://app.company.com/saml/acs",
    AudienceRestrictions = new[] { "app.company.com", "legacy-app.company.com" },
    NamedId = "john.doe@company.com",
    NameIdFormat = NameIdFormat.Persistent,
    Attributes = new Dictionary<string, string>
    {
        { "EmployeeId", "EMP001" },
        { "Groups", "Developers,Admins" },
        { "LastLogin", DateTime.UtcNow.ToString("O") }
    },
    SignatureType = SignType.Assertion,  // Sign only the assertion
    NotOnOrAfterInMinutes = 30,         // 30 minute validity
    SigningAlgorithm = SigningAlgorithm.SHA256,
    SamlId = Guid.Parse("95AD6A84-95C1-4B39-AE5E-FE1E700C406C"),
    AssertionId = Guid.Parse("B3CA912A-4A6B-4F31-9FD8-FC5E55837656"),
    Timestamp = DateTime.Parse("2024-01-15T10:00:00Z")
};
```

### Custom Certificate Factory
```csharp
public class CertificateService
{
    public static X509Certificate2 GetSigningCertificate()
    {
        // Load from certificate store
        using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        
        var cert = store.Certificates.Find(
            X509FindType.FindBySubjectName,
            "SAML-Signing-Certificate",
            false)[0];
            
        return cert;
    }
}

var saml = new SAML(CertificateService.GetSigningCertificate);
```

### Dependency Injection with Custom Factory
```csharp
public class SamlService
{
    private readonly ISAML _saml;
    
    public SamlService(ISAML saml)
    {
        _saml = saml;
    }
    
    public string CreateSamlResponse(string userId, Dictionary<string, string> attributes)
    {
        var parameters = new Parameters
        {
            Issuer = Configuration["Saml:Issuer"],
            Recipient = Configuration["Saml:Recipient"],
            AudienceRestrictions = Configuration.GetSection("Saml:AudienceRestrictions").Get<string[]>(),
            NamedId = userId,
            Attributes = attributes,
            SigningAlgorithm = SigningAlgorithm.SHA256
        };
        
        return _saml.CreateEncoded(parameters);
    }
}

// Registration
services.AddScoped<SamlService>();
services.AddSingleton<ISAML>(provider => 
    new SAML(() => CertificateService.GetSigningCertificate()));
```

## Configuration Options

### SAML Response Configuration
- **Issuer**: Identity Provider identifier
- **Recipient**: Service Provider assertion consumer service URL
- **Audience Restrictions**: List of allowed audience URLs
- **Name ID**: User identifier (email, username, etc.)
- **Name ID Format**: SAML name identifier format
- **Custom Attributes**: Key-value pairs for user data
- **Signature Type**: Response or Assertion signing
- **Validity Period**: NotOnOrAfter time in minutes
- **Signing Algorithm**: Cryptographic algorithm selection
- **Timestamps**: Custom issue and validity times
- **IDs**: Custom SAML and Assertion identifiers

### Security Configuration
- **Certificate Management**: X.509 certificate loading and validation
- **Algorithm Selection**: Choose appropriate cryptographic strength
- **Signature Placement**: Control what gets signed
- **Validity Windows**: Configure time-based restrictions

## Security Features

### Cryptographic Signing
- **RSA Digital Signatures**: Industry-standard asymmetric cryptography
- **Multiple Hash Algorithms**: SHA1, SHA256, SHA512 support
- **XML Digital Signature**: Compliant with XML-DSig standard
- **Certificate Validation**: X.509 certificate integration

### SAML Security
- **Audience Restrictions**: Prevent assertion forwarding attacks
- **Time-based Validity**: Configurable expiration windows
- **Signature Verification**: Built-in signature validation methods
- **Canonicalization**: Exclusive Canonical XML for consistent signing

### Best Practices
- Use SHA256 or SHA512 for new implementations
- Implement proper certificate lifecycle management
- Validate all input parameters
- Use appropriate validity windows
- Monitor and log SAML operations

## Testing

The library includes comprehensive test coverage using NUnit and approval testing.

### Running Tests
```bash
# Navigate to test directory
cd tests/Secure.SAML.Tests

# Run tests
dotnet test
```

### Test Structure
- **Unit Tests**: Individual component testing
- **Integration Tests**: End-to-end SAML generation
- **Approval Tests**: XML output validation
- **Algorithm Tests**: All signing algorithm combinations

### Test Coverage
- SHA1, SHA256, SHA512 signing algorithms
- Response vs. Assertion signing
- XML document and Base64 output formats
- Parameter validation
- Error handling scenarios

## Troubleshooting

### Common Issues

#### Certificate Errors
```csharp
// Ensure certificate has private key access
var cert = new X509Certificate2("path/to/cert.pfx", "password");
if (!cert.HasPrivateKey)
{
    throw new InvalidOperationException("Certificate must have private key");
}
```

#### Parameter Validation Errors
```csharp
// All required parameters must be provided
var parameters = new Parameters
{
    Issuer = "https://your-domain.com",      // Required
    Recipient = "https://sp-domain.com/acs", // Required
    NamedId = "user@domain.com"              // Required
};

// Validate before use
parameters.Validate();
```

#### XML Generation Issues
```csharp
// Check for valid XML output
try
{
    var xmlDoc = saml.Create(parameters);
    var xmlString = xmlDoc.OuterXml;
    
    // Validate XML can be parsed
    var validationDoc = new XmlDocument();
    validationDoc.LoadXml(xmlString);
}
catch (XmlException ex)
{
    // Handle XML parsing errors
    Console.WriteLine($"XML Error: {ex.Message}");
}
```

### Debugging Tips
1. **Enable XML Validation**: Validate generated XML against SAML schemas
2. **Check Certificate**: Ensure certificate is valid and accessible
3. **Verify Parameters**: Validate all required parameters are set
4. **Monitor Output**: Use approval tests to catch regressions
5. **Log Operations**: Add logging for troubleshooting

## Contributing

### Development Setup
1. Clone the repository
2. Install .NET 9.0 SDK
3. Restore NuGet packages
4. Run tests to verify setup

### Code Style
- Follow existing naming conventions
- Add XML documentation for public APIs
- Include unit tests for new features
- Use approval tests for XML output validation

### Testing Guidelines
- Write tests for all new functionality
- Ensure existing tests pass
- Use approval testing for XML output
- Test all signing algorithm combinations

### Pull Request Process
1. Create feature branch
2. Implement changes with tests
3. Ensure all tests pass
4. Submit pull request with description
5. Address review feedback

## Additional Resources

### SAML 2.0 Specifications
- [SAML 2.0 Core](http://docs.oasis-open.org/security/saml/Post2.0/sstc-saml-core-2.0-os.pdf)
- [SAML 2.0 Bindings](http://docs.oasis-open.org/security/saml/Post2.0/sstc-saml-bindings-2.0-os.pdf)
- [SAML 2.0 Profiles](http://docs.oasis-open.org/security/saml/Post2.0/sstc-saml-profiles-2.0-os.pdf)

### XML Digital Signature
- [XML-Signature Syntax and Processing](https://www.w3.org/TR/xmldsig-core/)

### Security Best Practices
- [OWASP SAML Security Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/SAML_Security_Cheat_Sheet.html)
- [NIST Digital Signature Guidelines](https://csrc.nist.gov/publications/detail/sp/800-102/final)

---

**Secure.SAML** is maintained by CodeShayk and is licensed under the MIT License. For support, please create an issue on the GitHub repository. 
