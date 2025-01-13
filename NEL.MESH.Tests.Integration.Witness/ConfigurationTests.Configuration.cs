// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace NEL.MESH.Tests.Integration.Witness
{
    public partial class ConfigurationTests
    {
        [WitnessTestsFact]
        public void ShouldGetConfigurationSettings()
        {
            // given
            var mailboxId = this.configuration["MeshConfiguration:MailboxId"];
            var password = this.configuration["MeshConfiguration:Password"];
            var sharedKey = this.configuration["MeshConfiguration:SharedKey"];
            var clientSigningCertificate = this.configuration["MeshConfiguration:ClientSigningCertificate"];

            var tlsRootCertificates = this.configuration.GetSection("MeshConfiguration:TlsRootCertificates")
                .Get<List<string>>();

            var tlsIntermediateCertificates =
                this.configuration.GetSection("MeshConfiguration:TlsIntermediateCertificates")
                    .Get<List<string>>();

            // then
            mailboxId.Should().NotBeNullOrEmpty();
            password.Should().NotBeNullOrEmpty();
            sharedKey.Should().NotBeNullOrEmpty();
            tlsRootCertificates.Count.Should().Be(1);
            tlsIntermediateCertificates.Count().Should().BeGreaterThan(0);
            clientSigningCertificate.Should().NotBeNullOrEmpty();
            tlsIntermediateCertificates.Count.Should().Be(1);
        }
    }
}
