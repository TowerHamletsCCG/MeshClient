// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NEL.MESH.Models.Foundations.Mesh;
using NEL.MESH.Models.Foundations.Mesh.Exceptions;
using NEL.MESH.Models.Foundations.Mesh.ExternalModels;
using Newtonsoft.Json;
using Xeptions;

namespace NEL.MESH.Services.Foundations.Mesh
{
    internal partial class MeshService
    {
        private static void ValidateResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == false)
            {
                string body = response.Content.ReadAsStringAsync().Result;
                SendMessageErrorResponse error;

                try
                {
                    error = JsonConvert.DeserializeObject<SendMessageErrorResponse>(body);
                }
                catch (Exception exception)
                {
                    throw new HttpRequestException(
                            message: $"Unable to deserialize response: {body}",
                            inner: exception,
                            statusCode: response.StatusCode);
                }

                string message = $"{(int)response.StatusCode} - {response.ReasonPhrase}";

                var httpRequestException =
                    new HttpRequestException(
                        message: message,
                        inner: null,
                        statusCode: response.StatusCode);

                if (error != null)
                {
                    httpRequestException.Data.Add("MessageId", new List<string> { error.MessageId });
                    httpRequestException.Data.Add("ErrorEvent", new List<string> { error.ErrorEvent });
                    httpRequestException.Data.Add("ErrorCode", new List<string> { error.ErrorCode });
                    httpRequestException.Data.Add("ErrorDescription", new List<string> { error.ErrorDescription });
                }

                throw httpRequestException;
            }
        }

        private static void ValidateNullResponse(HttpResponseMessage response)
        {
            if (response is null)
            {
                throw new NullHttpResponseMessageException(message: "HTTP Response Message is null.");
            }
        }

        private static void ValidateReceivedResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == false)
            {
                string body = response.Content.ReadAsStringAsync().Result;
                SendMessageErrorResponse error = JsonConvert.DeserializeObject<SendMessageErrorResponse>(body);
                string message = $"{(int)response.StatusCode} - {response.ReasonPhrase}";

                var httpRequestException =
                   new HttpRequestException(
                       message: message,
                       inner: null,
                       statusCode: response.StatusCode);

                if (error != null)
                {
                    httpRequestException.Data.Add("MessageId", new List<string> { error.MessageId });
                    httpRequestException.Data.Add("ErrorEvent", new List<string> { error.ErrorEvent });
                    httpRequestException.Data.Add("ErrorCode", new List<string> { error.ErrorCode });
                    httpRequestException.Data.Add("ErrorDescription", new List<string> { error.ErrorDescription });
                }

                throw httpRequestException;
            }
        }

        private static void ValidateOnHandshake(string authorizationToken)
        {
            Validate<InvalidArgumentsMeshException>(
                message: "Invalid MESH argument validation errors occurred, " +
                    "please correct the errors and try again.",
                (Rule: IsInvalid(authorizationToken), Parameter: "Token"));
        }

        private static void ValidateMeshMessageOnSendMessage(Message message, string authorizationToken)
        {
            ValidateMessageIsNotNull(message);
            ValidateHeadersIsNotNull(message);
            Validate<InvalidMeshException>(
                message: "Invalid message, please correct errors and try again.",
                (Rule: IsInvalid(authorizationToken), Parameter: "Token"),
                (Rule: IsInvalid(message.Headers, "mex-from"), Parameter: "mex-from"),
                (Rule: IsInvalid(message.Headers, "mex-to"), Parameter: "mex-to"),
                (Rule: IsInvalid(message.Headers, "mex-workflowid"), Parameter: "mex-workflowid"),
                (Rule: IsInvalid(message.Headers, "mex-from", 100), Parameter: "mex-from"),
                (Rule: IsInvalid(message.Headers, "mex-to", 100), Parameter: "mex-to"),
                (Rule: IsInvalid(message.Headers, "mex-workflowid", 300), Parameter: "mex-workflowid"),
                (Rule: IsInvalid(message.Headers, "mex-chunk-range", 20), Parameter: "mex-chunk-range"),
                (Rule: IsInvalid(message.Headers, "mex-subject", 500), Parameter: "mex-subject"),
                (Rule: IsInvalid(message.Headers, "mex-localid", 300), Parameter: "mex-localid"),
                (Rule: IsInvalid(message.Headers, "mex-filename", 300), Parameter: "mex-filename"),
                (Rule: IsInvalid(message.Headers, "mex-content-checksum", 100), Parameter: "mex-content-checksum"),
                (Rule: IsInvalid(message.FileContent), Parameter: nameof(Message.FileContent)));
        }

        private static void ValidateMexChunkRangeOnMultiPartFile(Message message)
        {
            Validate<InvalidMeshException>(
                message: "Invalid message, please correct errors and try again.",
                (Rule: IsInvalid(message.Headers, "mex-chunk-range"), Parameter: "mex-chunk-range"));
        }

        public static void ValidateTrackMessageArguments(string messageId, string authorizationToken)
        {
            Validate<InvalidArgumentsMeshException>(
                message: "Invalid MESH argument validation errors occurred, " +
                    "please correct the errors and try again.",
                (Rule: IsInvalid(messageId), Parameter: nameof(Message.MessageId)),
                (Rule: IsInvalid(authorizationToken), Parameter: "Token"));
        }

        public static void ValidateRetrieveMessageArguments(string messageId, string authorizationToken)
        {
            Validate<InvalidArgumentsMeshException>(
                message: "Invalid MESH argument validation errors occurred, " +
                    "please correct the errors and try again.",
                (Rule: IsInvalid(messageId), Parameter: nameof(Message.MessageId)),
                (Rule: IsInvalid(authorizationToken), Parameter: "Token"));
        }

        public static void ValidateRetrieveMessagesArguments(string authorizationToken)
        {
            Validate<InvalidArgumentsMeshException>(
                message: "Invalid MESH argument validation errors occurred, " +
                    "please correct the errors and try again.",
                (Rule: IsInvalid(authorizationToken), Parameter: "Token"));
        }

        private static void ValidateMessageId(string messageId)
        {
            Validate<InvalidMeshException>(
                message: "Invalid message, please correct errors and try again.",
                (Rule: IsInvalid(messageId), Parameter: nameof(Message.MessageId)));
        }

        public static void ValidateAcknowledgeMessageArguments(string messageId, string authorizationToken)
        {
            Validate<InvalidArgumentsMeshException>(
                message: "Invalid MESH argument validation errors occurred, " +
                    "please correct the errors and try again.",
                (Rule: IsInvalid(messageId), Parameter: nameof(Message.MessageId)),
                (Rule: IsInvalid(authorizationToken), Parameter: "Token"));
        }

        private static string GetKey(Dictionary<string, List<string>> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key].FirstOrDefault();
            }

            return null;
        }

        private static void ValidateMessageIsNotNull(Message message)
        {
            if (message is null)
            {
                throw new NullMessageException(message: "Message is null.");
            }
        }

        private static void ValidateHeadersIsNotNull(Message message)
        {
            if (message.Headers is null)
            {
                throw new NullHeadersException(
                    message: "Message headers dictionary is null.");
            }
        }

        private static dynamic IsNotSame(
                 string first,
                 string second,
                 string secondName) => new
                 {
                     Condition = !String.IsNullOrWhiteSpace(first) && first != second,
                     Message = $"Requires a matching header value for key '{secondName}'"
                 };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(Dictionary<string, List<string>> dictionary, string key) => new
        {
            Condition = IsInvalidKey(dictionary, key),
            Message = "Header value is required"
        };

        private static dynamic IsInvalid(
            Dictionary<string, List<string>> dictionary,
            string key,
            int maxLength) => new
            {
                Condition = IsInvalidKeyLength(dictionary, key, maxLength),
                Message = $"Text length should not be greater than {maxLength}"
            };

        private static dynamic IsArgInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(byte[] data) => new
        {
            Condition = (data == null || data.Length == 0),
            Message = "Content is required"
        };

        private static bool IsInvalidKeyLength(Dictionary<string, List<string>> dictionary, string key, int maxLength)
        {
            if (dictionary == null)
            {
                return false;
            }

            bool keyExists = dictionary.ContainsKey(key);

            if (!keyExists)
            {
                return false;
            }

            string value = dictionary[key].FirstOrDefault();

            if (value == null || value.Length <= maxLength)
            {
                return false;
            }

            return true;
        }

        private static bool IsInvalidKey(Dictionary<string, List<string>> dictionary, string key)
        {
            if (dictionary == null)
            {
                return false;
            }

            bool keyExists = dictionary.ContainsKey(key);

            if (!keyExists)
            {
                return true;
            }

            string value = dictionary[key].FirstOrDefault();

            return String.IsNullOrWhiteSpace(value);
        }

        private static void Validate<T>(string message, params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            var invalidDataException = (T)Activator.CreateInstance(typeof(T), message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDataException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDataException.ThrowIfContainsErrors();
        }
    }
}
