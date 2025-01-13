// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

namespace NEL.MESH.UI.Models
{
    public class MeshCertificates
    {
        public string Environment { get; set; } = string.Empty;
        public string ClientSigningCertificate { get; set; } = string.Empty;
        public List<string> TlsRootCertificates { get; set; } = new List<string>();
        public List<string> TlsIntermediateCertificates { get; set; } = new List<string>();
    }
}
