﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
                .AddJsonFile("local.appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("NEL:MESH:CLIENT:");

            IConfiguration configuration = configurationBuilder.Build();

            this.wireMockServer = WireMockServer.Start();

            var clientCert = configuration["MeshConfiguration:ClientCertificate"];
            var rootCert = configuration["MeshConfiguration:RootCertificate"];

            string[] intermediateCertificates =
                configuration.GetSection("MeshConfiguration:IntermediateCertificates").Get<string[]>();

            this.meshConfigurations = new MeshConfiguration
            {
                ClientCertificate = GetCertificate(clientCert),
                IntermediateCertificates = GetCertificates(intermediateCertificates),
                MailboxId = configuration["MeshConfiguration:MailboxId"],
                MexClientVersion = configuration["MeshConfiguration:MexClientVersion"],
                MexOSName = configuration["MeshConfiguration:MexOSName"],
                MexOSVersion = configuration["MeshConfiguration:MexOSVersion"],
                Password = configuration["MeshConfiguration:Password"],
                Key = configuration["MeshConfiguration:Key"],
                RootCertificate = GetCertificate(rootCert),
                Url = this.wireMockServer.Url
            };

            this.meshClient = new MeshClient(meshConfigurations: this.meshConfigurations);
        }

        private static X509Certificate2Collection GetCertificates(params string[] intermediateCertificates)
        {
            var certificates = new X509Certificate2Collection();

            foreach (string item in intermediateCertificates)
            {
                certificates.Add(GetCertificate(item));
            }

            return certificates;
        }

        private static X509Certificate2 GetCertificate(string value)
        {
            byte[] certBytes = Convert.FromBase64String(value);

            return new X509Certificate2(certBytes);
        }

        private string GenerateAuthorisationHeader()
        {
            string mailboxId = this.meshConfigurations.MailboxId;
            string password = this.meshConfigurations.Password;
            string nonce = Guid.NewGuid().ToString();
            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            string nonce_count = "0";
            string stringToHash = $"{mailboxId}:{nonce}:{nonce_count}:{password}:{timeStamp}";
            string key = "BackBone";
            string sharedKey = GenerateSha256(stringToHash, key);

            return $"NHSMESH {mailboxId}:{nonce}:{nonce_count}:{timeStamp}:{sharedKey}";
        }

        private string GenerateSha256(string value, string key)
        {
            var crypt = new HMACSHA256(Encoding.ASCII.GetBytes(key));
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(value));

            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }

            return hash;
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 1, wordMaxLength: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();
    }
}
