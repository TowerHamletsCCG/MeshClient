// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NEL.MESH.Brokers.DateTimes;
using NEL.MESH.Brokers.Identifiers;
using NEL.MESH.Brokers.Mesh;

namespace NEL.MESH.Services.Foundations.Tokens
{
    internal partial class TokenService : ITokenService
    {
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly IMeshBroker meshBroker;

        public TokenService(
            IDateTimeBroker dateTimeBroker,
            IIdentifierBroker identifierBroker,
            IMeshBroker meshBroker)
        {
            this.dateTimeBroker = dateTimeBroker;
            this.identifierBroker = identifierBroker;
            this.meshBroker = meshBroker;
        }

        public ValueTask<string> GenerateTokenAsync() =>
            TryCatch(async () =>
            {
                ValidateGenerateTokenArgs(
                    this.meshBroker.MeshConfiguration.MailboxId,
                    this.meshBroker.MeshConfiguration.Password,
                    this.meshBroker.MeshConfiguration.SharedKey);

                string nonce = this.identifierBroker.GetIdentifier().ToString();
                string timeStamp = this.dateTimeBroker.GetCurrentDateTimeOffset().ToString("yyyyMMddHHmm");
                string nonce_count = "0";

                string stringToHash =
                    $"{this.meshBroker.MeshConfiguration.MailboxId}" +
                    $":{nonce}" +
                    $":{nonce_count}" +
                    $":{this.meshBroker.MeshConfiguration.Password}" +
                    $":{timeStamp}";

                string sharedKey = GenerateSha256(stringToHash, this.meshBroker.MeshConfiguration.SharedKey);

                string token = await ValueTask
                    .FromResult($"NHSMESH {this.meshBroker.MeshConfiguration.MailboxId}:{nonce}:{nonce_count}:{timeStamp}:{sharedKey}");

                Console.WriteLine($"Generated token: {token}");

                return token;
            });

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
    }
}
