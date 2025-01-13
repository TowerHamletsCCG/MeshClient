// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using NEL.MESH.Clients;
using NEL.MESH.Models.Configurations;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace NEL.MESH.Tests.Acceptance
{
    public partial class MeshClientTests
    {
        private readonly MeshClient meshClient;
        private readonly WireMockServer wireMockServer;
        private readonly MeshConfiguration meshConfigurations;

        public MeshClientTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = configurationBuilder.Build();
            this.wireMockServer = WireMockServer.Start();
            var url = this.wireMockServer.Url;
            var mailboxId = configuration["MeshConfiguration:MailboxId"];
            var mexClientVersion = configuration["MeshConfiguration:MexClientVersion"];
            var mexOSName = configuration["MeshConfiguration:MexOSName"];
            var mexOSVersion = configuration["MeshConfiguration:MexOSVersion"];
            var password = configuration["MeshConfiguration:Password"];
            var sharedKey = configuration["MeshConfiguration:SharedKey"];
            var maxChunkSizeInMegabytes = int.Parse(configuration["MeshConfiguration:MaxChunkSizeInMegabytes"]);
            var clientSigningCertificate = configuration["MeshConfiguration:ClientSigningCertificate"];
            var clientSigningCertificatePassword = configuration["MeshConfiguration:ClientSigningCertificatePassword"];

            var tlsRootCertificates = configuration.GetSection("MeshConfiguration:TlsRootCertificates")
                .Get<List<string>>();

            List<string> tlsIntermediateCertificates =
                configuration.GetSection("MeshConfiguration:TlsIntermediateCertificates")
                    .Get<List<string>>();

            if (tlsIntermediateCertificates == null)
            {
                tlsIntermediateCertificates = new List<string>();
            }

            this.meshConfigurations = new MeshConfiguration
            {
                MailboxId = mailboxId,
                MexClientVersion = mexClientVersion,
                MexOSName = mexOSName,
                MexOSVersion = mexOSVersion,
                Password = password,
                SharedKey = sharedKey,
                TlsRootCertificates = GetCertificates(tlsRootCertificates.ToArray()),
                TlsIntermediateCertificates = GetCertificates(tlsIntermediateCertificates.ToArray()),

                ClientSigningCertificate =
                    GetPkcs12Certificate(clientSigningCertificate, clientSigningCertificatePassword),

                Url = url,
                MaxChunkSizeInMegabytes = maxChunkSizeInMegabytes
            };

            this.meshClient = new MeshClient(meshConfigurations: this.meshConfigurations);
        }

        private static X509Certificate2Collection GetCertificates(params string[] certificates)
        {
            var certificateCollection = new X509Certificate2Collection();

            foreach (string item in certificates)
            {
                certificateCollection.Add(GetPemOrDerCertificate(item));
            }

            return certificateCollection;
        }

        private static X509Certificate2 GetPemOrDerCertificate(string value)
        {
            byte[] certBytes = Convert.FromBase64String(value);
            var certificate = X509CertificateLoader.LoadCertificate(certBytes);

            return certificate;
        }

        private static X509Certificate2 GetPkcs12Certificate(string value, string password = "")
        {
            byte[] certBytes = Convert.FromBase64String(value);
            var certificate = X509CertificateLoader.LoadPkcs12(certBytes, password);

            return certificate;
        }

        private static string GetRandomString(
            int wordCount = 1,
            int wordMinLength = 1,
            int wordMaxLength = 10) =>
                new MnemonicString(
                    wordCount: wordCount,
                    wordMinLength: wordMinLength,
                    wordMaxLength: wordMaxLength == 0
                        ? GetRandomNumber()
                        : (wordMaxLength <= wordMinLength
                            ? wordMinLength + 1
                            : wordMaxLength)).GetValue();

        private static List<string> GetRandomStrings()
        {
            var randomStrings = new List<string>();
            for (int i = 0; i < GetRandomNumber(); i++)
            {
                string randomString = new MnemonicString(wordCount: 1, wordMinLength: 1, wordMaxLength: GetRandomNumber()).GetValue();
                randomStrings.Add(randomString);
            }
            return randomStrings;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetKeyStringValue(string key, Dictionary<string, List<string>> dictionary)
        {
            return dictionary.ContainsKey(key)
                ? dictionary[key]?.First()
                : string.Empty;
        }
    }
}
