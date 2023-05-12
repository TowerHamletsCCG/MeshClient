﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NEL.MESH.Brokers.Mesh;
using NEL.MESH.Models.Foundations.Mesh;
using NEL.MESH.Models.Foundations.Mesh.ExternalModels;
using Newtonsoft.Json;

namespace NEL.MESH.Services.Foundations.Mesh
{
    internal partial class MeshService : IMeshService
    {
        private readonly IMeshBroker meshBroker;

        public MeshService(IMeshBroker meshBroker)
        {
            this.meshBroker = meshBroker;
        }

        public ValueTask<bool> HandshakeAsync(string authorizationToken) =>
            TryCatch(async () =>
            {
                ValidateOnHandshake(authorizationToken);
                HttpResponseMessage response = await this.meshBroker.HandshakeAsync(authorizationToken);
                ValidateResponse(response);

                return response.IsSuccessStatusCode;
            });

        public ValueTask<Message> SendMessageAsync(Message message, string authorizationToken) =>
            TryCatch(async () =>
            {
                ValidateMeshMessageOnSendMessage(message, authorizationToken);

                string chunkRange = GetKeyStringValue("Mex-Chunk-Range", message.Headers)
                    .Replace("{", string.Empty)
                        .Replace("}", string.Empty);

                if (string.IsNullOrEmpty(chunkRange))
                {
                    chunkRange = "1";
                }

                string chunkPart = (chunkRange.Split(':'))[0];

                int chunkNumber; 

                if ( ! int.TryParse(chunkPart, out chunkNumber))
                {
                    chunkNumber = 1;
                }

                HttpResponseMessage responseMessage;

                if (chunkNumber <= 1)
                {
                     responseMessage = await this.meshBroker.SendMessageAsync(
                        mailboxTo: GetKeyStringValue("Mex-To", message.Headers),
                        workflowId: GetKeyStringValue("Mex-WorkflowID", message.Headers),
                        localId: GetKeyStringValue("Mex-LocalID", message.Headers),
                        subject: GetKeyStringValue("Mex-Subject", message.Headers),
                        fileName: GetKeyStringValue("Mex-FileName", message.Headers),
                        contentChecksum: GetKeyStringValue("Mex-Content-Checksum", message.Headers),
                        contentEncrypted: GetKeyStringValue("Mex-Content-Encrypted", message.Headers),
                        encoding: GetKeyStringValue("Mex-Encoding", message.Headers),
                        chunkRange: GetKeyStringValue("Mex-Chunk-Range", message.Headers),
                        contentType: GetKeyStringValue("Content-Type", message.Headers),
                        authorizationToken,
                        stringContent: message.StringContent);
                }
                else
                {
                    ValidateMessageId(message.MessageId);
                    ValidateMexChunkRangeOnMultiPartMessage(message);

                    responseMessage = await this.meshBroker.SendMessageAsync(
                        mailboxTo: GetKeyStringValue("Mex-To", message.Headers),
                        workflowId: GetKeyStringValue("Mex-WorkflowID", message.Headers),
                        localId: GetKeyStringValue("Mex-LocalID", message.Headers),
                        subject: GetKeyStringValue("Mex-Subject", message.Headers),
                        fileName: GetKeyStringValue("Mex-FileName", message.Headers),
                        contentChecksum: GetKeyStringValue("Mex-Content-Checksum", message.Headers),
                        contentEncrypted: GetKeyStringValue("Mex-Content-Encrypted", message.Headers),
                        encoding: GetKeyStringValue("Mex-Encoding", message.Headers),
                        chunkRange: GetKeyStringValue("Mex-Chunk-Range", message.Headers),
                        contentType: GetKeyStringValue("Content-Type", message.Headers),
                        authorizationToken,
                        stringContent: message.StringContent,
                        messageId: message.MessageId,
                        chunkNumber: chunkNumber.ToString());
                }

                ValidateResponse(responseMessage);
                string responseMessageBody = responseMessage.Content.ReadAsStringAsync().Result;

                Message outputMessage = new Message
                {
                    MessageId = (JsonConvert.DeserializeObject<SendMessageResponse>(responseMessageBody)).MessageId,
                    StringContent = responseMessageBody,
                };

                GetHeaderValues(responseMessage, outputMessage);

                return outputMessage;
            });

        public ValueTask<Message> SendFileAsync(Message message, string authorizationToken) =>
            TryCatch(async () =>
            {
                ValidateMeshMessageOnSendFile(message, authorizationToken);

                string chunkRange = GetKeyStringValue("Mex-Chunk-Range", message.Headers)
                    .Replace("{", string.Empty)
                        .Replace("}", string.Empty);

                if (string.IsNullOrEmpty(chunkRange))
                {
                    chunkRange = "1";
                }

                string chunkPart = (chunkRange.Split(':'))[0];
                int chunkNumber;

                if (!int.TryParse(chunkPart, out chunkNumber))
                {
                    chunkNumber = 1;
                }

                HttpResponseMessage responseFileMessage;

                if (chunkNumber <= 1)
                {
                    responseFileMessage = await this.meshBroker.SendFileAsync(
                        mailboxTo: GetKeyStringValue("Mex-To", message.Headers),
                        workflowId: GetKeyStringValue("Mex-WorkflowID", message.Headers),
                        localId: GetKeyStringValue("Mex-LocalID", message.Headers),
                        subject: GetKeyStringValue("Mex-Subject", message.Headers),
                        fileName: GetKeyStringValue("Mex-FileName", message.Headers),
                        contentChecksum: GetKeyStringValue("Mex-Content-Checksum", message.Headers),
                        contentEncrypted: GetKeyStringValue("Mex-Content-Encrypted", message.Headers),
                        encoding: GetKeyStringValue("Mex-Encoding", message.Headers),
                        chunkRange: GetKeyStringValue("Mex-Chunk-Range", message.Headers),
                        contentType: GetKeyStringValue("Content-Type", message.Headers),
                        authorizationToken,
                        fileContents: message.FileContent);
                }
                else
                {
                    ValidateMessageId(message.MessageId);
                    ValidateMexChunkRangeOnMultiPartFile(message);

                    responseFileMessage = await this.meshBroker.SendFileAsync(
                        mailboxTo: GetKeyStringValue("Mex-To", message.Headers),
                        workflowId: GetKeyStringValue("Mex-WorkflowID", message.Headers),
                        localId: GetKeyStringValue("Mex-LocalID", message.Headers),
                        subject: GetKeyStringValue("Mex-Subject", message.Headers),
                        fileName: GetKeyStringValue("Mex-FileName", message.Headers),
                        contentChecksum: GetKeyStringValue("Mex-Content-Checksum", message.Headers),
                        contentEncrypted: GetKeyStringValue("Mex-Content-Encrypted", message.Headers),
                        encoding: GetKeyStringValue("Mex-Encoding", message.Headers),
                        chunkRange: GetKeyStringValue("Mex-Chunk-Range", message.Headers),
                        contentType: GetKeyStringValue("Content-Type", message.Headers),
                        authorizationToken,
                        fileContents: message.FileContent,
                        messageId: message.MessageId,
                        chunkNumber: chunkNumber.ToString());
                }

                string responseMessageBody = responseFileMessage.Content.ReadAsStringAsync().Result;

                Message outputMessage = new Message
                {
                    MessageId = (JsonConvert.DeserializeObject<SendMessageResponse>(responseMessageBody)).MessageId,
                    StringContent = responseMessageBody,
                };

                GetHeaderValues(responseFileMessage, outputMessage);

                return outputMessage;
            });

        public ValueTask<Message> TrackMessageAsync(string messageId, string authorizationToken) =>
            TryCatch(async () =>
            {
                ValidateTrackMessageArguments(messageId, authorizationToken);

                HttpResponseMessage responseMessage =
                    await this.meshBroker.TrackMessageAsync(messageId, authorizationToken);

                ValidateResponse(responseMessage);
                string responseMessageBody = responseMessage.Content.ReadAsStringAsync().Result;

                Message outputMessage = new Message
                {
                    MessageId = messageId,
                    TrackingInfo = MapTrackMessageResponseToTrackingInfo(
                        JsonConvert.DeserializeObject<TrackMessageResponse>(responseMessageBody)),
                };

                GetHeaderValues(responseMessage, outputMessage);

                return outputMessage;
            });

        public ValueTask<List<string>> RetrieveMessagesAsync(string authorizationToken) =>
            TryCatch(async () =>
            {
                ValidateRetrieveMessagesArguments(authorizationToken);
                HttpResponseMessage responseMessage = await this.meshBroker.GetMessagesAsync(authorizationToken);
                ValidateResponse(responseMessage);
                string responseMessageBody = responseMessage.Content.ReadAsStringAsync().Result;

                GetMessagesResponse getMessagesResponse =
                    JsonConvert.DeserializeObject<GetMessagesResponse>(responseMessageBody);

                return getMessagesResponse.Messages;
            });

        public ValueTask<Message> RetrieveMessageAsync(string messageId, string authorizationToken) =>
            TryCatch(async () =>
            {
                ValidateRetrieveMessageArguments(messageId, authorizationToken);

                HttpResponseMessage initialResponse =
                    await this.meshBroker.GetMessageAsync(messageId, authorizationToken);

                ValidateResponse(initialResponse);

                //switch case: If any of the 5 content types match then string content else file content
                string contentType = initialResponse.Content.Headers
                    .FirstOrDefault(h => h.Key == "Content-Type")
                        .Value.FirstOrDefault();

                // Validate content type exists
                bool isStringContent = contentType switch
                {
                    var value when value.Contains("text/plain") => true,
                    var value when value.Contains("text/html") => true,
                    var value when value.Contains("application/json") => true,
                    var value when value.Contains("text/xml") => true,
                    var value when value.Contains("application/xml") => true,
                    _ => false
                };

                string stringBody = null;
                //byte[] fileBody = null;

                if (isStringContent)
                {
                    stringBody = initialResponse.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    //fileBody = initialResponse.Content.ReadAsByteArrayAsync().Result;
                };

                Message firstMessage = new Message
                {
                    MessageId = messageId,
                    StringContent = stringBody,
                    //FileContent = fileBody,
                };

                foreach (var header in initialResponse.Headers)
                {
                    firstMessage.Headers.Add(header.Key, header.Value.ToList());
                }

                foreach (var header in initialResponse.Content.Headers)
                {
                    firstMessage.Headers.Add(header.Key, header.Value.ToList());
                }

                if (initialResponse.StatusCode == HttpStatusCode.OK)
                {
                    return firstMessage;
                }

                var chunks = initialResponse.Content.Headers.FirstOrDefault(h => h.Key == "Mex-Chunk-Range").Value.FirstOrDefault();
                string chunkRange = chunks.Replace("{", string.Empty).Replace("}", string.Empty);
                string[] parts = chunkRange.Split(":");
                int totalChunks = int.Parse(parts[1]);

                for (int chunkId = 1; chunkId < totalChunks; chunkId++)
                {
                    HttpResponseMessage responseMessage =
                        await this.meshBroker.GetMessageAsync(messageId, (chunkId + 1).ToString(), authorizationToken);

                    ValidateResponse(responseMessage);

                    if (isStringContent)
                    {
                        string messageContent = responseMessage.Content.ReadAsStringAsync().Result;
                        firstMessage.StringContent += messageContent;
                    }
                    else
                    {
                        //byte[] fileContent = responseMessage.Content.ReadAsByteArrayAsync().Result;
                        //firstMessage.FileContent = firstMessage.FileContent.Concat(fileContent).ToArray();
                    };

                }

                return firstMessage;
            });


        public ValueTask<bool> AcknowledgeMessageAsync(string messageId, string authorizationToken) =>
            TryCatch(async () =>
            {
                ValidateAcknowledgeMessageArguments(messageId, authorizationToken);

                HttpResponseMessage response =
                    await this.meshBroker.AcknowledgeMessageAsync(messageId, authorizationToken);
                ValidateResponse(response);

                return response.IsSuccessStatusCode;
            });

        private static void GetHeaderValues(HttpResponseMessage responseMessage, Message outputMessage)
        {
            foreach (var header in responseMessage.Content.Headers)
            {
                outputMessage.Headers
                    .Add(header.Key, header.Value.ToList());
            }
        }

        private TrackingInfo MapTrackMessageResponseToTrackingInfo(TrackMessageResponse trackMessageResponse)
        {
            TrackingInfo trackingInfo = new TrackingInfo
            {
                AddressType = trackMessageResponse.AddressType,
                Checksum = trackMessageResponse.Checksum,
                ChunkCount = trackMessageResponse.ChunkCount,
                CompressFlag = trackMessageResponse.CompressFlag,
                DownloadTimestamp = trackMessageResponse.DownloadTimestamp,
                DtsId = trackMessageResponse.DtsId,
                EncryptedFlag = trackMessageResponse.EncryptedFlag,
                ExpiryTime = trackMessageResponse.ExpiryTime,
                FileName = trackMessageResponse.FileName,
                FileSize = trackMessageResponse.FileSize,
                IsCompressed = trackMessageResponse.IsCompressed,
                LocalId = trackMessageResponse.LocalId,
                MeshRecipientOdsCode = trackMessageResponse.MeshRecipientOdsCode,
                MessageId = trackMessageResponse.MessageId,
                MessageType = trackMessageResponse.MessageType,
                PartnerId = trackMessageResponse.PartnerId,
                Recipient = trackMessageResponse.Recipient,
                RecipientName = trackMessageResponse.RecipientName,
                RecipientOrgCode = trackMessageResponse.RecipientOrgCode,
                RecipientSmtp = trackMessageResponse.RecipientSmtp,
                Sender = trackMessageResponse.Sender,
                SenderName = trackMessageResponse.SenderName,
                SenderOdsCode = trackMessageResponse.SenderOdsCode,
                SenderOrgCode = trackMessageResponse.SenderOrgCode,
                SenderSmtp = trackMessageResponse.SenderSmtp,
                Status = trackMessageResponse.Status,
                StatusSuccess = trackMessageResponse.StatusSuccess,
                UploadTimestamp = trackMessageResponse.UploadTimestamp,
                Version = trackMessageResponse.Version,
                WorkflowId = trackMessageResponse.WorkflowId
            };

            return trackingInfo;
        }

        private static string GetKeyStringValue(string key, Dictionary<string, List<string>> dictionary)
        {
            return dictionary.ContainsKey(key)
                ? dictionary[key]?.First()
                : string.Empty;
        }
    }
}
