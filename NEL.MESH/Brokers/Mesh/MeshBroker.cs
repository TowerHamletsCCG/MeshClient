// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using NEL.MESH.Models.Configurations;

namespace NEL.MESH.Brokers.Mesh
{
    internal class MeshBroker : IMeshBroker
    {
        private readonly HttpClient httpClient;

        public MeshBroker(MeshConfiguration MeshConfiguration)
        {
            this.MeshConfiguration = MeshConfiguration;
            this.httpClient = SetupHttpClient();
        }

        public MeshConfiguration MeshConfiguration { get; private set; }

        public async ValueTask<HttpResponseMessage> HandshakeAsync(string authorizationToken)
        {
            string path = $"/messageexchange/{this.MeshConfiguration.MailboxId}";
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("authorization", authorizationToken);
            var response = await this.httpClient.SendAsync(request);

            return response;
        }

        public async ValueTask<HttpResponseMessage> SendMessageAsync(
            string authorizationToken,
            string mexFrom,
            string mexTo,
            string mexWorkflowId,
            string mexChunkRange,
            string mexSubject,
            string mexLocalId,
            string mexFileName,
            string mexContentChecksum,
            string contentType,
            string contentEncoding,
            string accept,
            byte[] fileContents)
        {
            var path = $"/messageexchange/{this.MeshConfiguration.MailboxId}/outbox";

            var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new ByteArrayContent(fileContents)
            };

            request.Headers.Add("authorization", authorizationToken);
            request.Headers.Add("mex-from", mexFrom);
            request.Headers.Add("mex-to", mexTo);
            request.Headers.Add("mex-workflowid", mexWorkflowId);
            request.Headers.Add("mex-chunk-range", mexChunkRange);
            request.Headers.Add("mex-subject", mexSubject);
            request.Headers.Add("mex-localid", mexLocalId);
            request.Headers.Add("mex-filename", mexFileName);
            request.Headers.Add("mex-content-checksum", mexContentChecksum);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            request.Content.Headers.Add("content-encoding", contentEncoding);
            request.Headers.Add("accept", accept);

            var response = await this.httpClient.SendAsync(request);

            return response;
        }

        public async ValueTask<HttpResponseMessage> SendMessageAsync(
            string authorizationToken,
            string mexFrom,
            string mexTo,
            string mexWorkflowId,
            string mexChunkRange,
            string mexSubject,
            string mexLocalId,
            string mexFileName,
            string mexContentChecksum,
            string contentType,
            string contentEncoding,
            string accept,
            byte[] fileContents,
            string messageId,
            string chunkNumber)
        {
            var path = $"/messageexchange/{this.MeshConfiguration.MailboxId}/outbox/{messageId}/{chunkNumber}";

            var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new ByteArrayContent(fileContents)
            };

            request.Headers.Add("authorization", authorizationToken);
            request.Headers.Add("mex-from", mexFrom);
            request.Headers.Add("mex-to", mexTo);
            request.Headers.Add("mex-workflowid", mexWorkflowId);
            request.Headers.Add("mex-chunk-range", mexChunkRange);
            request.Headers.Add("mex-subject", mexSubject);
            request.Headers.Add("mex-localid", mexLocalId);
            request.Headers.Add("mex-filename", mexFileName);
            request.Headers.Add("mex-content-checksum", mexContentChecksum);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            request.Content.Headers.Add("Content-Encoding", contentEncoding);
            request.Headers.Add("Accept", accept);

            var response = await this.httpClient.SendAsync(request);

            return response;
        }

        public async ValueTask<HttpResponseMessage> TrackMessageAsync(string messageId, string authorizationToken)
        {
            var path = $"/messageexchange/{this.MeshConfiguration.MailboxId}/outbox/tracking?messageID={messageId}";
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("authorization", authorizationToken);
            var response = await this.httpClient.SendAsync(request);

            return response;
        }

        public async ValueTask<HttpResponseMessage> GetMessagesAsync(string authorizationToken)
        {
            var path = $"/messageexchange/{this.MeshConfiguration.MailboxId}/inbox";
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("authorization", authorizationToken);
            var response = await this.httpClient.SendAsync(request);

            return response;
        }

        public async ValueTask<HttpResponseMessage> GetMessageAsync(string messageId, string authorizationToken)
        {
            var path = $"/messageexchange/{this.MeshConfiguration.MailboxId}/inbox/{messageId}";
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("authorization", authorizationToken);
            var response = await this.httpClient.SendAsync(request);

            return response;
        }

        public async ValueTask<HttpResponseMessage> GetMessageAsync(
            string messageId,
            string chunkNumber,
            string authorizationToken)
        {
            var path = $"/messageexchange/{this.MeshConfiguration.MailboxId}/inbox/{messageId}/{chunkNumber}";
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("authorization", authorizationToken);
            var response = await this.httpClient.SendAsync(request);

            return response;
        }

        public async ValueTask<HttpResponseMessage> AcknowledgeMessageAsync(string messageId, string authorizationToken)
        {
            var path = $"/messageexchange/{this.MeshConfiguration.MailboxId}/inbox/{messageId}/status/acknowledged";
            var request = new HttpRequestMessage(HttpMethod.Put, path);
            request.Headers.Add("authorization", authorizationToken);
            var response = await this.httpClient.SendAsync(request);

            return response;
        }

        private HttpClient SetupHttpClient()
        {
            HttpClientHandler handler = SetupHttpClientHandler();

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(this.MeshConfiguration.Url)
            };

            httpClient.DefaultRequestHeaders.Add(
                name: "mex-clientversion",
                value: this.MeshConfiguration.MexClientVersion);

            httpClient.DefaultRequestHeaders.Add(
                name: "mex-osname",
                value: this.MeshConfiguration.MexOSName);

            httpClient.DefaultRequestHeaders.Add(
                name: "mex-osversion",
                value: this.MeshConfiguration.MexOSVersion);

            return httpClient;
        }

        private HttpClientHandler SetupHttpClientHandler()
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                CheckCertificateRevocationList = false,
            };

            if (this.MeshConfiguration.ClientSigningCertificate != null)
            {
                handler.ClientCertificates.Add(this.MeshConfiguration.ClientSigningCertificate);
            }

            if (this.MeshConfiguration.TlsRootCertificates != null
                || this.MeshConfiguration.TlsRootCertificates.Count > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    if (chain != null)
                    {
                        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                        chain.ChainPolicy.CustomTrustStore.AddRange(this.MeshConfiguration.TlsRootCertificates);

                        if (this.MeshConfiguration.TlsIntermediateCertificates != null
                            || this.MeshConfiguration.TlsIntermediateCertificates.Count > 0)
                        {
                            chain.ChainPolicy.ExtraStore.AddRange(this.MeshConfiguration.TlsIntermediateCertificates);
                        }

                        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreWrongUsage;

                        if (cert != null && chain.Build(cert))
                        {
                            return true;
                        };
                    }

                    throw new Exception(chain.ChainStatus.FirstOrDefault().StatusInformation);
                };
            }

            return handler;
        }

        ~MeshBroker()
        {
            this.httpClient.Dispose();
        }
    }
}
