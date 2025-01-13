// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using NEL.MESH.Clients;
using NEL.MESH.Models.Configurations;
using Tynamix.ObjectFiller;

namespace NEL.MESH.Tests.Integration
{
    public partial class MeshClientTests
    {
        private readonly MeshClient meshClient;
        private readonly MeshConfiguration meshConfigurations;

        public MeshClientTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = configurationBuilder.Build();
            var url = configuration["MeshConfiguration:Url"] ?? "NULL";
            var mailboxId = configuration["MeshConfiguration:MailboxId"] ?? "NULL";
            var mexClientVersion = configuration["MeshConfiguration:MexClientVersion"] ?? "NULL";
            var mexOSName = configuration["MeshConfiguration:MexOSName"] ?? "NULL";
            var mexOSVersion = configuration["MeshConfiguration:MexOSVersion"] ?? "NULL";
            var password = configuration["MeshConfiguration:Password"] ?? "NULL";
            var sharedKey = configuration["MeshConfiguration:SharedKey"] ?? "NULL";
            var maxChunkSizeInMegabytes = int.Parse(configuration["MeshConfiguration:MaxChunkSizeInMegabytes"]);
            var clientSigningCertificate = configuration["MeshConfiguration:ClientSigningCertificate"];
            var clientSigningCertificatePassword = configuration["MeshConfiguration:ClientSigningCertificatePassword"];

            List<string> tlsRootCertificates = configuration.GetSection("MeshConfiguration:TlsRootCertificates")
                .Get<List<string>>() ?? [];

            List<string> tlsIntermediateCertificates =
                configuration.GetSection("MeshConfiguration:TlsIntermediateCertificates")
                    .Get<List<string>>() ?? [];

            this.meshConfigurations = new MeshConfiguration
            {
                Url = url,
                MailboxId = mailboxId,
                MexClientVersion = mexClientVersion,
                MexOSName = mexOSName,
                MexOSVersion = mexOSVersion,
                Password = password,
                SharedKey = sharedKey,
                TlsRootCertificates = GetCertificates(tlsRootCertificates.ToArray(), "Root"),
                TlsIntermediateCertificates = GetCertificates(tlsIntermediateCertificates.ToArray(), "Intermediate"),

                ClientSigningCertificate =
                    GetPkcs12Certificate(clientSigningCertificate, clientSigningCertificatePassword, "Signing"),

                MaxChunkSizeInMegabytes = maxChunkSizeInMegabytes
            };

            Console.WriteLine($"MailboxId: '{meshConfigurations.MailboxId}'");
            Console.WriteLine($"Url: '{meshConfigurations.Url}'");

            Console.WriteLine(
                $"Password: '{meshConfigurations.Password.Substring(0, 2)}" +
                $"{new string('*', meshConfigurations.Password.Length - 4)}" +
                $"{meshConfigurations.Password.Substring(meshConfigurations.Password.Length - 2)}'");

            Console.WriteLine(
                $"SharedKey: '{meshConfigurations.SharedKey.Substring(0, 2)}" +
                $"{new string('*', meshConfigurations.SharedKey.Length - 4)}" +
                $"{meshConfigurations.SharedKey.Substring(meshConfigurations.SharedKey.Length - 2)}'");

            this.meshClient = new MeshClient(meshConfigurations: this.meshConfigurations);
        }

        private static X509Certificate2Collection GetCertificates(string[] certificates, string type = "")
        {
            var certificateCollection = new X509Certificate2Collection();

            foreach (string item in certificates)
            {
                certificateCollection.Add(GetPemOrDerCertificate(item, type));
            }

            return certificateCollection;
        }

        private static X509Certificate2 GetPemOrDerCertificate(string value, string type = "")
        {
            byte[] certBytes = Convert.FromBase64String(value);
            var certificate = X509CertificateLoader.LoadCertificate(certBytes);
            ConsoleWrite(value, type, certificate.Subject, certificate.Thumbprint);

            return certificate;
        }

        private static X509Certificate2 GetPkcs12Certificate(string value, string password = "", string type = "")
        {
            byte[] certBytes = Convert.FromBase64String(value);
            var certificate = X509CertificateLoader.LoadPkcs12(certBytes, password);
            ConsoleWrite(value, type, certificate.Subject, certificate.Thumbprint);

            return certificate;
        }

        private static void ConsoleWrite(string item, string type = "", string subject = "", string thumbprint = "")
        {
            if (item.Length > 30)
            {
                Console.WriteLine(
                    $"{type} Certificate: {item.Substring(0, 15)}...{item.Substring(item.Length - 15)}, " +
                    $"SUBJECT: {subject} " +
                    $"THUMBPRINT: {thumbprint}");
            }
            else
            {
                Console.WriteLine($"{type} Certificate: {item}");
            }
        }

        private static string GetRandomString(int wordMinLength = 2, int wordMaxLength = 100) =>
            new MnemonicString(
                wordCount: 1,
                wordMinLength: 1,
                wordMaxLength: wordMaxLength < wordMinLength ? wordMinLength : wordMaxLength).GetValue();
    }
}
