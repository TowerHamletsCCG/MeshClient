// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using KellermanSoftware.CompareNetObjects;
using Moq;
using NEL.MESH.Brokers.Mesh;
using NEL.MESH.Models.Foundations.Mesh;
using NEL.MESH.Models.Foundations.Mesh.ExternalModels;
using NEL.MESH.Services.Foundations.Mesh;
using Newtonsoft.Json;
using Tynamix.ObjectFiller;
using Xunit;

namespace NEL.MESH.Tests.Unit.Services.Foundations.Mesh
{
    public partial class MeshServiceTests
    {
        private readonly Mock<IMeshBroker> meshBrokerMock;
        private readonly IMeshService meshService;
        private readonly ICompareLogic compareLogic;

        public MeshServiceTests()
        {
            this.meshBrokerMock = new Mock<IMeshBroker>();
            this.meshService = new MeshService(meshBroker: this.meshBrokerMock.Object);
            this.compareLogic = new CompareLogic();
        }

        public static TheoryData<HttpResponseMessage> DependencyValidationResponseMessages()
        {
            var invalidValue =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "400"))
                {
                    ReasonPhrase = "Invalid Value"
                };

            var missingValue =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "400"))
                {
                    ReasonPhrase = "Missing Value"
                };

            var accessDenied =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "401"))
                {
                    ReasonPhrase = "Access Denied"
                };

            var unauthorised =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "401"))
                {
                    ReasonPhrase = "Unauthorised"
                };

            var forbidden =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "403"))
                {
                    ReasonPhrase = "Forbidden"
                };

            var access_Denied =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "403"))
                {
                    ReasonPhrase = "Access Denied"
                };

            var resourceNotFound =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "404"))
                {
                    ReasonPhrase = "Unauthorised Value"
                };

            var tooManyRequests =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "429"))
                {
                    ReasonPhrase = "Too Many Requests"
                };

            return new TheoryData<HttpResponseMessage>
            {
                invalidValue,
                missingValue,
                accessDenied,
                unauthorised,
                forbidden,
                access_Denied,
                resourceNotFound,
                tooManyRequests
            };
        }

        public static TheoryData<HttpResponseMessage> DependencyResponseMessages()
        {
            var internalServerError =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "500"))
                {
                    ReasonPhrase = "Internal Server Error"
                };

            var missingValue =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "501"))
                {
                    ReasonPhrase = "Not Implemented"
                };

            var accessDenied =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "502"))
                {
                    ReasonPhrase = "Bad Gateway"
                };

            var unauthorised =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "503"))
                {
                    ReasonPhrase = "Service Unavailable"
                };

            var forbidden =
                new HttpResponseMessage((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), "504"))
                {
                    ReasonPhrase = "gateway Timeout"
                };

            return new TheoryData<HttpResponseMessage>
            {
                internalServerError,
                missingValue,
                accessDenied,
                unauthorised,
                forbidden
            };
        }

        private static string GetRandomString(int wordMinLength = 2, int wordMaxLength = 100) =>
            new MnemonicString(
                wordCount: 1,
                wordMinLength,
                wordMaxLength: wordMaxLength < wordMinLength ? wordMinLength : wordMaxLength)
                    .GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomChunkNumber(int min, int max) =>
            new IntRange(min, max).GetValue();

        private static string GetKeyStringValue(string key, Dictionary<string, List<string>> dictionary)
        {
            return dictionary.ContainsKey(key)
                ? dictionary[key]?.First()
                : string.Empty;
        }

        public static List<string> GetStringContentParts(string stringContent, int chunkParts)
        {
            if (string.IsNullOrEmpty(stringContent) || chunkParts <= 0)
            {
                return new List<string>();
            }

            int length = stringContent.Length;
            int chunkSize = length / chunkParts;
            int remainder = length % chunkParts;
            List<string> partsList = new List<string>();

            int currentStartIndex = 0;
            for (int i = 0; i < chunkParts; i++)
            {
                int currentChunkSize = i < remainder ? chunkSize + 1 : chunkSize;
                string chunk = stringContent.Substring(currentStartIndex, currentChunkSize);
                partsList.Add(chunk);
                currentStartIndex += currentChunkSize;
            }

            return partsList;
        }

        public static List<byte[]> GetByteContentParts(byte[] content, int chunkParts)
        {
            if (content == null || content.Length == 0 || chunkParts <= 0)
            {
                return new List<byte[]>();
            }

            int length = content.Length;
            int chunkSize = length / chunkParts;
            int remainder = length % chunkParts;
            List<byte[]> partsList = new List<byte[]>();

            int currentStartIndex = 0;
            for (int i = 0; i < chunkParts; i++)
            {
                int currentChunkSize = i < remainder ? chunkSize + 1 : chunkSize;
                byte[] chunk = new byte[currentChunkSize];
                Array.Copy(content, currentStartIndex, chunk, 0, currentChunkSize);
                partsList.Add(chunk);
                currentStartIndex += currentChunkSize;
            }

            return partsList;
        }

        private static List<HttpResponseMessage> CreateHttpResponseContentMessagesForRetrieveMessage(
            Message message,
            Dictionary<string, List<string>> contentHeaders,
            Dictionary<string, List<string>> headers = null,
            int chunks = 1,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            List<HttpResponseMessage> messages = new List<HttpResponseMessage>();
            List<byte[]> parts = GetByteContentParts(message.FileContent, chunks);

            for (int i = 0; i < parts.Count; i++)
            {
                Message chunkMessage = new Message
                {
                    MessageId = message.MessageId,
                    Headers = message.Headers,
                    FileContent = parts[i],
                };

                HttpResponseMessage httpResponseMessage =
                    CreateHttpResponseContentMessageForRetrieveMessage(
                        chunkMessage,
                        contentHeaders,
                        headers,
                        statusCode);

                string chunkRangeValue = $"{i + 1}:{chunks}";

                if (httpResponseMessage.Headers.Contains("mex-chunk-range"))
                {
                    httpResponseMessage.Headers.Remove("mex-chunk-range");
                }

                httpResponseMessage.Headers.Add("mex-chunk-range", chunkRangeValue);
                messages.Add(httpResponseMessage);
            }

            return messages;
        }

        private static HttpResponseMessage CreateHttpResponseContentMessageForRetrieveMessage(
            Message message,
            Dictionary<string, List<string>> contentHeaders,
            Dictionary<string, List<string>> headers = null,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new ByteArrayContent(message.FileContent),
            };

            foreach (var item in contentHeaders)
            {
                responseMessage.Content.Headers.Add(item.Key.ToLower(), item.Value);
            }

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    responseMessage.Content.Headers.Add(item.Key.ToLower(), item.Value);
                }
            }

            return responseMessage;
        }

        private static HttpResponseMessage CreateHttpResponseContentMessageForSendMessage(
                    Message message,
                    Dictionary<string, List<string>> headers = null,
                    HttpStatusCode statusCode = HttpStatusCode.OK)
        {

            string contentType = message.Headers.ContainsKey("content-type")
                ? message.Headers["content-type"].FirstOrDefault()
                : "text/plain";

            if (string.IsNullOrEmpty(contentType))
            {
                contentType = "text/plain";
            }

            HttpResponseMessage responseMessage = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content =
                    new StringContent("{\"messageID\": \"" + message.MessageId + "\"}", Encoding.UTF8, contentType)
            };

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    if (item.Key.ToLower() != "content-type" && item.Key.ToLower() != "content-encoding")
                    {
                        responseMessage.Headers.Add(item.Key.ToLower(), item.Value);
                    }

                    if (item.Key.ToLower() == "content-encoding")
                    {
                        responseMessage.Content.Headers.Add(item.Key.ToLower(), item.Value);
                    }
                }
            }

            return responseMessage;
        }

        private static HttpResponseMessage CreateTrackingHttpResponseMessage(
            TrackMessageResponse trackMessageResponse)
        {
            string contentType = "application/json";
            string jsonContent = JsonConvert.SerializeObject(trackMessageResponse);

            HttpResponseMessage responseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent, Encoding.UTF8, contentType)
            };

            return responseMessage;
        }

        private static Message GetMessageFromHttpResponseMessageForReceive(
            HttpResponseMessage responseMessage,
            string messageId)
        {
            byte[] responseMessageBody = responseMessage.Content.ReadAsByteArrayAsync().Result;

            Dictionary<string, List<string>> headers = GetHeaders(
                responseMessage.Headers,
                responseMessage.Content.Headers);

            Message message = new Message
            {
                MessageId = messageId,
                FileContent = responseMessageBody,
            };

            foreach (var item in headers)
            {
                message.Headers.Add(item.Key.ToLower(), item.Value);
            }

            return message;
        }

        private static Message GetMessageFromHttpResponseMessage(
            HttpResponseMessage responseMessage, Message inputMessage)
        {
            string responseMessageBody = responseMessage.Content.ReadAsStringAsync().Result;

            Dictionary<string, List<string>> headers = GetHeaders(
                responseMessage.Headers,
                responseMessage.Content.Headers);

            Message message = new Message
            {
                MessageId = (JsonConvert.DeserializeObject<SendMessageResponse>(responseMessageBody)).MessageId,
                FileContent = inputMessage.FileContent,
                Headers = headers
            };

            return message;
        }

        private static Message GetMessageFromTrackingHttpResponseMessage(
            string messageId,
            HttpResponseMessage responseMessage)
        {
            string responseMessageBody = responseMessage.Content.ReadAsStringAsync().Result;

            Dictionary<string, List<string>> headers = GetHeaders(
                responseMessage.Headers,
                responseMessage.Content.Headers);

            Message message = new Message
            {
                MessageId = messageId,
                TrackingInfo = JsonConvert.DeserializeObject<TrackingInfo>(responseMessageBody),
                Headers = headers
            };

            return message;
        }

        private dynamic CreateRandomTrackingProperties()
        {
            return new
            {
                AddressType = GetRandomString(),
                Checksum = GetRandomString(),
                ChunkCount = GetRandomNumber(),
                CompressFlag = GetRandomString(),
                DownloadTimestamp = GetRandomString(),
                DtsId = GetRandomString(),
                EncryptedFlag = GetRandomString(),
                ExpiryTime = GetRandomString(),
                FileName = GetRandomString(),
                FileSize = GetRandomNumber(),
                IsCompressed = GetRandomString(),
                LocalId = GetRandomString(),
                MeshRecipientOdsCode = GetRandomString(),
                MessageId = GetRandomString(),
                MessageType = GetRandomString(),
                PartnerId = GetRandomString(),
                Recipient = GetRandomString(),
                RecipientName = GetRandomString(),
                RecipientOrgCode = GetRandomString(),
                RecipientSmtp = GetRandomString(),
                Sender = GetRandomString(),
                SenderName = GetRandomString(),
                SenderOdsCode = GetRandomString(),
                SenderOrgCode = GetRandomString(),
                SenderSmtp = GetRandomString(),
                Status = GetRandomString(),
                StatusSuccess = GetRandomString(),
                UploadTimestamp = GetRandomString(),
                Version = GetRandomString(),
                WorkflowId = GetRandomString()
            };
        }

        private TrackMessageResponse MapDynamicObjectToTrackMessageResponse(dynamic randomTrackingProperties)
        {
            return new TrackMessageResponse
            {
                AddressType = randomTrackingProperties.AddressType,
                Checksum = randomTrackingProperties.Checksum,
                ChunkCount = randomTrackingProperties.ChunkCount,
                CompressFlag = randomTrackingProperties.CompressFlag,
                DownloadTimestamp = randomTrackingProperties.DownloadTimestamp,
                DtsId = randomTrackingProperties.DtsId,
                EncryptedFlag = randomTrackingProperties.EncryptedFlag,
                ExpiryTime = randomTrackingProperties.ExpiryTime,
                FileName = randomTrackingProperties.FileName,
                FileSize = randomTrackingProperties.FileSize,
                IsCompressed = randomTrackingProperties.IsCompressed,
                LocalId = randomTrackingProperties.LocalId,
                MeshRecipientOdsCode = randomTrackingProperties.MeshRecipientOdsCode,
                MessageId = randomTrackingProperties.MessageId,
                MessageType = randomTrackingProperties.MessageType,
                PartnerId = randomTrackingProperties.PartnerId,
                Recipient = randomTrackingProperties.Recipient,
                RecipientName = randomTrackingProperties.RecipientName,
                RecipientOrgCode = randomTrackingProperties.RecipientOrgCode,
                RecipientSmtp = randomTrackingProperties.RecipientSmtp,
                Sender = randomTrackingProperties.Sender,
                SenderName = randomTrackingProperties.SenderName,
                SenderOdsCode = randomTrackingProperties.SenderOdsCode,
                SenderOrgCode = randomTrackingProperties.SenderOrgCode,
                SenderSmtp = randomTrackingProperties.SenderSmtp,
                Status = randomTrackingProperties.Status,
                StatusSuccess = randomTrackingProperties.StatusSuccess,
                UploadTimestamp = randomTrackingProperties.UploadTimestamp,
                Version = randomTrackingProperties.Version,
                WorkflowId = randomTrackingProperties.WorkflowId
            };
        }

        private TrackingInfo MapDynamicObjectToTrackingInfo(dynamic randomTrackingProperties)
        {
            return new TrackingInfo
            {
                AddressType = randomTrackingProperties.AddressType,
                Checksum = randomTrackingProperties.Checksum,
                ChunkCount = randomTrackingProperties.ChunkCount,
                CompressFlag = randomTrackingProperties.CompressFlag,
                DownloadTimestamp = randomTrackingProperties.DownloadTimestamp,
                DtsId = randomTrackingProperties.DtsId,
                EncryptedFlag = randomTrackingProperties.EncryptedFlag,
                ExpiryTime = randomTrackingProperties.ExpiryTime,
                FileName = randomTrackingProperties.FileName,
                FileSize = randomTrackingProperties.FileSize,
                IsCompressed = randomTrackingProperties.IsCompressed,
                LocalId = randomTrackingProperties.LocalId,
                MeshRecipientOdsCode = randomTrackingProperties.MeshRecipientOdsCode,
                MessageId = randomTrackingProperties.MessageId,
                MessageType = randomTrackingProperties.MessageType,
                PartnerId = randomTrackingProperties.PartnerId,
                Recipient = randomTrackingProperties.Recipient,
                RecipientName = randomTrackingProperties.RecipientName,
                RecipientOrgCode = randomTrackingProperties.RecipientOrgCode,
                RecipientSmtp = randomTrackingProperties.RecipientSmtp,
                Sender = randomTrackingProperties.Sender,
                SenderName = randomTrackingProperties.SenderName,
                SenderOdsCode = randomTrackingProperties.SenderOdsCode,
                SenderOrgCode = randomTrackingProperties.SenderOrgCode,
                SenderSmtp = randomTrackingProperties.SenderSmtp,
                Status = randomTrackingProperties.Status,
                StatusSuccess = randomTrackingProperties.StatusSuccess,
                UploadTimestamp = randomTrackingProperties.UploadTimestamp,
                Version = randomTrackingProperties.Version,
                WorkflowId = randomTrackingProperties.WorkflowId
            };
        }

        private static Dictionary<string, List<string>> GetHeaders(
            HttpResponseHeaders headers,
            HttpContentHeaders contentHeaders)
        {
            var dictionary = new Dictionary<string, List<string>>();

            foreach (var header in headers)
            {
                dictionary.Add(header.Key.ToLower(), header.Value.ToList());
            }

            foreach (var contentHeader in contentHeaders)
            {
                dictionary.Add(contentHeader.Key.ToLower(), contentHeader.Value.ToList());
            }

            return dictionary;
        }

        private static Message CreateRandomSendMessage(
            string chunkSize = "{1:1}",
            string contentType = "text/plain")
        {
            var message = CreateMessageFiller().Create();
            message.Headers.Add("mex-from",
                new List<string> { GetRandomString(wordMinLength: 2, wordMaxLength: 100) });

            message.Headers.Add("mex-to",
                new List<string> { GetRandomString(wordMinLength: 2, wordMaxLength: 100) });

            message.Headers.Add("mex-workflowid",
                new List<string> { GetRandomString(wordMinLength: 2, wordMaxLength: 300) });

            message.Headers.Add("mex-chunk-range", new List<string> { chunkSize });

            message.Headers.Add("mex-subject",
                new List<string> { GetRandomString(wordMinLength: 2, wordMaxLength: 500) });

            message.Headers.Add("mex-localid",
                new List<string> { GetRandomString(wordMinLength: 2, wordMaxLength: 300) });

            message.Headers.Add("mex-filename",
                new List<string> { GetRandomString(wordMinLength: 2, wordMaxLength: 300) });

            message.Headers.Add("mex-content-checksum",
                new List<string> { GetRandomString(wordMinLength: 2, wordMaxLength: 100) });

            message.Headers.Add("content-type", new List<string> { contentType });
            message.Headers.Add("content-encoding", new List<string> { GetRandomString() });

            return message;
        }

        private static Message CreateRandomInvalidSendMessage(
            string chunkSize = "{1:1}",
            string contentType = "text/plain")
        {
            var message = CreateMessageFiller().Create();
            message.Headers.Add("mex-from",
                new List<string> { GetRandomString(wordMinLength: 101, wordMaxLength: 101) });

            message.Headers.Add("mex-to",
                new List<string> { GetRandomString(wordMinLength: 101, wordMaxLength: 101) });

            message.Headers.Add("mex-workflowid",
                new List<string> { GetRandomString(wordMinLength: 301, wordMaxLength: 301) });

            message.Headers.Add("mex-chunk-range", new List<string> { "100000000000:100000000000" });

            message.Headers.Add("mex-subject",
                new List<string> { GetRandomString(wordMinLength: 501, wordMaxLength: 501) });

            message.Headers.Add("mex-localid",
                new List<string> { GetRandomString(wordMinLength: 301, wordMaxLength: 301) });

            message.Headers.Add("mex-filename",
                new List<string> { GetRandomString(wordMinLength: 301, wordMaxLength: 301) });

            message.Headers.Add("mex-content-checksum",
                new List<string> { GetRandomString(wordMinLength: 101, wordMaxLength: 101) });

            message.Headers.Add("content-type", new List<string> { contentType });
            message.Headers.Add("content-encoding", new List<string> { GetRandomString() });

            return message;
        }

        private Expression<Func<string, bool>> SameStringAs(
            string expectedString)
        {
            return actualString =>
            this.compareLogic.Compare(expectedString, actualString)
            .AreEqual;
        }

        private static Message CreateRandomMessage() =>
            CreateMessageFiller().Create();

        private static Filler<Message> CreateMessageFiller()
        {
            var filler = new Filler<Message>();
            filler.Setup()
                .OnProperty(message => message.Headers).Use(new Dictionary<string, List<string>>());

            return filler;
        }
    }
}
