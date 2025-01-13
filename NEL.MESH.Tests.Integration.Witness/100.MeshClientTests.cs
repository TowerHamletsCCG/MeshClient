// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Configuration;
using NEL.MESH.Clients;
using NEL.MESH.Models.Configurations;
using NEL.MESH.Models.Foundations.Mesh;
using Tynamix.ObjectFiller;
using Xunit.Abstractions;


namespace NEL.MESH.Tests.Integration.Witness
{
    public partial class MeshClientTests
    {
        private readonly MeshClient meshClient;
        private readonly MeshConfiguration meshConfigurations;
        private readonly ITestOutputHelper output;

        public MeshClientTests(ITestOutputHelper output)
        {
            this.output = output;


            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = configurationBuilder.Build();
            var mailboxId = configuration["MeshConfiguration:MailboxId"];
            var mexClientVersion = configuration["MeshConfiguration:MexClientVersion"];
            var mexOSName = configuration["MeshConfiguration:MexOSName"];
            var mexOSVersion = configuration["MeshConfiguration:MexOSVersion"];
            var password = configuration["MeshConfiguration:Password"];
            var sharedKey = configuration["MeshConfiguration:SharedKey"];
            var url = configuration["MeshConfiguration:Url"];
            var maxChunkSizeInMegabytes = int.Parse(configuration["MeshConfiguration:MaxChunkSizeInMegabytes"]);
            var clientSigningCertificate = configuration["MeshConfiguration:ClientSigningCertificate"];

            var tlsRootCertificates = configuration.GetSection("MeshConfiguration:TlsRootCertificates")
                .Get<List<string>>();

            var tlsIntermediateCertificates =
                configuration.GetSection("MeshConfiguration:TlsIntermediateCertificates")
                    .Get<List<string>>();

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
                ClientSigningCertificate = GetCertificate(clientSigningCertificate),
                Url = url,
                MaxChunkSizeInMegabytes = maxChunkSizeInMegabytes
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

        static string GetMD5Checksum(byte[] data)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(data);

                // Convert the byte array to a hexadecimal string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private static X509Certificate2 GetCertificate(string value)
        {
            byte[] certBytes = Convert.FromBase64String(value);

            return new X509Certificate2(certBytes);
        }

        private static string GetRandomString(int wordMinLength = 10, int wordMaxLength = 10) =>
            new MnemonicString(
                wordCount: 1,
                wordMinLength: 1,
                wordMaxLength: wordMaxLength < wordMinLength ? wordMinLength : wordMaxLength).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetFileWithXBytes(int targetSizeInMegabytes)
        {
            int bytesPerMegabyte = 1024 * 1024;
            int targetSizeInBytes = targetSizeInMegabytes * bytesPerMegabyte;
            string smallString = "9694116538, 9694116414,"; // Test Patients
            int repeatCount = targetSizeInBytes / Encoding.UTF8.GetByteCount(smallString);

            return string.Concat(Enumerable.Repeat(smallString, repeatCount));
        }

        private static Message CreateRandomSendMessage(
            string mexFrom,
            string mexTo,
            string mexWorkflowId,
            string mexLocalId,
            string mexSubject,
            string mexFileName,
            string mexContentChecksum,
            string mexContentEncrypted,
            string mexEncoding,
            string mexChunkRange,
            string contentType,
            string content)
        {
            var message = CreateMessageFiller(content).Create();
            message.Headers.Add("Mex-From", new List<string> { mexFrom });
            message.Headers.Add("Mex-To", new List<string> { mexTo });
            message.Headers.Add("Mex-WorkflowID", new List<string> { mexWorkflowId });
            message.Headers.Add("Mex-LocalID", new List<string> { mexLocalId });
            message.Headers.Add("Mex-Subject", new List<string> { mexSubject });
            message.Headers.Add("Mex-FileName", new List<string> { mexFileName });
            message.Headers.Add("Mex-Content-Checksum", new List<string> { mexContentChecksum });
            message.Headers.Add("Mex-Content-Encrypted", new List<string> { mexContentEncrypted });
            message.Headers.Add("Mex-Encoding", new List<string> { mexEncoding });
            message.Headers.Add("Mex-Chunk-Range", new List<string> { mexChunkRange });
            message.Headers.Add("Content-Type", new List<string> { contentType });

            return message;
        }

        private static Filler<Message> CreateMessageFiller(string content)
        {
            byte[] fileContent = Encoding.UTF8.GetBytes(content);
            var filler = new Filler<Message>();

            filler.Setup()
                .OnProperty(message => message.FileContent).Use(fileContent)
                .OnProperty(message => message.Headers).Use(new Dictionary<string, List<string>>());

            return filler;
        }
    }
}
