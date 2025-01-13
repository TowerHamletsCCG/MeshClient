// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;

namespace NEL.MESH.Models.Configurations
{
    /// <summary>
    /// Represents the configuration settings required for Mesh communication.
    /// </summary>
    public class MeshConfiguration
    {
        /// <summary>
        /// Gets or sets the unique identifier for the mailbox.
        /// </summary>
        public string MailboxId { get; set; }

        /// <summary>
        /// Gets or sets the password for authenticating the mailbox.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the cryptographic key used for secure communication.
        /// The SharedKey is the MESH environment shared secret, provided by itoc.supportdesk@nhs.net 
        /// as part of onboarding to the PTL environment.
        /// </summary
        public string SharedKey { get; set; }

        /// <summary>
        /// Gets or sets the URL endpoint for the Mesh service.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the collection of root TLS certificates for secure communication.
        /// </summary>
        public X509Certificate2Collection TlsRootCertificates { get; set; }

        /// <summary>
        /// Gets or sets the collection of intermediate TLS certificates for secure communication.
        /// </summary>
        public X509Certificate2Collection TlsIntermediateCertificates { get; set; }

        /// <summary>
        /// Gets or sets the client signing certificate used for authentication and encryption.
        /// </summary>
        public X509Certificate2 ClientSigningCertificate { get; set; }

        /// <summary>
        /// Gets or sets the version of the Mesh client software being used.
        /// </summary>
        public string MexClientVersion { get; set; }

        /// <summary>
        /// Gets or sets the operating system name of the client system.
        /// </summary>
        public string MexOSName { get; set; }

        /// <summary>
        /// Gets or sets the operating system version of the client system.
        /// </summary>
        public string MexOSVersion { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of a message chunk in megabytes.
        /// </summary>
        public int MaxChunkSizeInMegabytes { get; set; }
    }
}
