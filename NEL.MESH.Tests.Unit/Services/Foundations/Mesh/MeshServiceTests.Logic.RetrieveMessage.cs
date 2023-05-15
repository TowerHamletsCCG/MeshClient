﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NEL.MESH.Models.Foundations.Mesh;
using Xunit;

namespace NEL.MESH.Tests.Unit.Services.Foundations.Mesh
{
    public partial class MeshServiceTests
    {
        [Theory]
        [InlineData("text/plain")]
        [InlineData("text/html")]
        [InlineData("application/json")]
        [InlineData("text/xml")]
        [InlineData("application/xml")]
        public async Task ShouldRetrieveSinglePartMessageWithStringContentAsync(string contentType)
        {
            // given
            string authorizationToken = GetRandomString();
            Message randomMessage = CreateRandomSendMessage(contentType: contentType);
            Message inputMessage = randomMessage;

            Dictionary<string, List<string>> contentHeaders = new Dictionary<string, List<string>>
            {
                { "Content-Type", new List<string>{contentType} },
                { "Content-Length", new List<string>() },
                { "Last-Modified", new List<string>() },
            };

            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>
            {
                { "Content-Encoding", new List<string>() },
                { "Mex-FileName", new List<string>() },
                { "Mex-From", new List<string>() },
                { "Mex-To", new List<string>() },
                { "Mex-WorkflowID", new List<string>() },
                { "Mex-Chunk-Range", new List<string>{"{1:1}"} },
                { "Mex-LocalID", new List<string>() },
                { "Mex-Subject", new List<string>() },
                { "Mex-Content-Checksum", new List<string>() },
                { "Mex-Content-Encrypted", new List<string>() },
                { "Mex-ClientVersion", new List<string>() },
                { "Mex-OSVersion", new List<string>() },
                { "Mex-OSArchitecture", new List<string>() },
                { "Mex-JavaVersion", new List<string>() }
            };

            HttpResponseMessage responseMessage = CreateHttpResponseContentMessageForSendMessage(
                inputMessage,
                contentHeaders,
                headers,
                HttpStatusCode.OK);

            this.meshBrokerMock.Setup(broker =>
                broker.GetMessageAsync(inputMessage.MessageId, authorizationToken))
                    .ReturnsAsync(responseMessage);

            Message expectedMessage = GetMessageWithStringContentFromHttpResponseMessageForReceive(responseMessage, inputMessage.MessageId);

            // when
            var actualMessage = await this.meshService
                .RetrieveMessageAsync(inputMessage.MessageId, authorizationToken);

            // then
            actualMessage.Should().BeEquivalentTo(expectedMessage);
            actualMessage.FileContent.Should().BeNull();

            this.meshBrokerMock.Verify(broker =>
                broker.GetMessageAsync(inputMessage.MessageId, authorizationToken),
                        Times.Once);

            this.meshBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("application/octet-stream")]
        [InlineData("image/jpeg")]
        public async Task ShouldRetrieveSinglePartMessageWithFileContentAsync(string contentType)
        {
            // given
            string authorizationToken = GetRandomString();
            Message randomMessage = CreateRandomSendFileMessage(contentType: contentType);
            Message inputMessage = randomMessage;

            Dictionary<string, List<string>> contentHeaders = new Dictionary<string, List<string>>
            {
                { "Content-Type", new List<string>{contentType} },
                { "Content-Length", new List<string>() },
                { "Last-Modified", new List<string>() },
            };

            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>
            {
                { "Content-Encoding", new List<string>() },
                { "Mex-FileName", new List<string>() },
                { "Mex-From", new List<string>() },
                { "Mex-To", new List<string>() },
                { "Mex-WorkflowID", new List<string>() },
                { "Mex-Chunk-Range", new List<string>{"{1:1}"} },
                { "Mex-LocalID", new List<string>() },
                { "Mex-Subject", new List<string>() },
                { "Mex-Content-Checksum", new List<string>() },
                { "Mex-Content-Encrypted", new List<string>() },
                { "Mex-ClientVersion", new List<string>() },
                { "Mex-OSVersion", new List<string>() },
                { "Mex-OSArchitecture", new List<string>() },
                { "Mex-JavaVersion", new List<string>() }
            };

            HttpResponseMessage responseMessage = CreateHttpResponseContentMessageForSendMessage(
                inputMessage,
                contentHeaders,
                headers,
                HttpStatusCode.OK);

            this.meshBrokerMock.Setup(broker =>
                broker.GetMessageAsync(inputMessage.MessageId, authorizationToken))
                    .ReturnsAsync(responseMessage);

            Message expectedMessage = GetMessageWithFileContentFromHttpResponseMessageForReceive(responseMessage, inputMessage.MessageId);

            // when
            var actualMessage = await this.meshService
                .RetrieveMessageAsync(inputMessage.MessageId, authorizationToken);

            // then
            actualMessage.Should().BeEquivalentTo(expectedMessage);
            actualMessage.StringContent.Should().BeNull();

            this.meshBrokerMock.Verify(broker =>
                broker.GetMessageAsync(inputMessage.MessageId, authorizationToken),
                        Times.Once);

            this.meshBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("text/plain")]
        [InlineData("text/html")]
        [InlineData("application/json")]
        [InlineData("text/xml")]
        [InlineData("application/xml")]
        public async Task ShouldRetrieveMultiPartMessagesWithStringContentAsync(string contentType)
        {
            // given
            string authorizationToken = GetRandomString();
            Message randomMessage = CreateRandomSendMessage(contentType);
            int chunks = GetRandomNumber();
            Message inputMessage = randomMessage;

            if (chunks > randomMessage.StringContent.Length)
            {
                chunks = randomMessage.StringContent.Length;
            }

            Dictionary<string, List<string>> contentHeaders = new Dictionary<string, List<string>>
            {
                { "Content-Type", new List<string>() },
                { "Content-Length", new List<string>() },
                { "Last-Modified", new List<string>() },
            };

            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>
            {
                { "Content-Encoding", new List<string>() },
                { "Mex-FileName", new List<string>() },
                { "Mex-From", new List<string>() },
                { "Mex-To", new List<string>() },
                { "Mex-WorkflowID", new List<string>() },
                { "Mex-LocalID", new List<string>() },
                { "Mex-Subject", new List<string>() },
                { "Mex-Content-Checksum", new List<string>() },
                { "Mex-Content-Encrypted", new List<string>() },
                { "Mex-ClientVersion", new List<string>() },
                { "Mex-OSVersion", new List<string>() },
                { "Mex-OSArchitecture", new List<string>() },
                { "Mex-JavaVersion", new List<string>() }
            };

            List<HttpResponseMessage> responseMessages = 
                CreateHttpResponseContentMessagesWithStringContentForRetrieveMessage(
                    inputMessage,
                    contentHeaders,
                    headers,
                    chunks,
                    HttpStatusCode.PartialContent);

            Message expectedMessage = new Message();

            for (int i = 0; i < chunks; i++)
            {
                if (i == 0)
                {
                    this.meshBrokerMock.Setup(broker =>
                        broker.GetMessageAsync(inputMessage.MessageId, authorizationToken))
                            .ReturnsAsync(responseMessages[i]);

                    expectedMessage = GetMessageWithStringContentFromHttpResponseMessageForReceive(
                        responseMessages[i], inputMessage.MessageId);
                }
                else
                {
                    this.meshBrokerMock.Setup(broker =>
                        broker.GetMessageAsync(
                            inputMessage.MessageId, 
                            It.Is(SameStringAs((i + 1).ToString())), 
                            authorizationToken))
                                .ReturnsAsync(responseMessages[i]);
                }
            }

            expectedMessage.StringContent = responseMessages
                .Aggregate("", (current, message) => current + 
                    GetMessageWithStringContentFromHttpResponseMessageForReceive(message, inputMessage.MessageId).StringContent);

            // when
            var actualMessage = await this.meshService
                .RetrieveMessageAsync(inputMessage.MessageId, authorizationToken);

            // then
            actualMessage.Should().BeEquivalentTo(expectedMessage);
            actualMessage.StringContent.Should().BeEquivalentTo(expectedMessage.StringContent);

            for (int i = 0; i < chunks; i++)
            {
                if (i == 0)
                {
                    this.meshBrokerMock.Verify(broker =>
                        broker.GetMessageAsync(inputMessage.MessageId, authorizationToken),
                            Times.Once);
                }
                else
                {
                    this.meshBrokerMock.Verify(broker =>
                        broker.GetMessageAsync(inputMessage.MessageId, It.Is(SameStringAs((i + 1).ToString())), authorizationToken),
                            Times.Once);
                }
            }

            this.meshBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("application/octet-stream")]
        [InlineData("image/jpeg")]
        public async Task ShouldRetrieveMultiPartMessagesWithFileContentAsync(string contentType)
        {
            // given
            string authorizationToken = GetRandomString();
            Message randomMessage = CreateRandomSendFileMessage(contentType:contentType);
            int chunks = GetRandomNumber();
            Message inputMessage = randomMessage;

            if (chunks > randomMessage.FileContent.Length)
            {
                chunks = randomMessage.FileContent.Length;
            }

            Dictionary<string, List<string>> contentHeaders = new Dictionary<string, List<string>>
            {
                { "Content-Type", new List<string>{contentType} },
                { "Content-Length", new List<string>() },
                { "Last-Modified", new List<string>() },
            };

            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>
            {
                { "Content-Encoding", new List<string>() },
                { "Mex-FileName", new List<string>() },
                { "Mex-From", new List<string>() },
                { "Mex-To", new List<string>() },
                { "Mex-WorkflowID", new List<string>() },
                { "Mex-LocalID", new List<string>() },
                { "Mex-Subject", new List<string>() },
                { "Mex-Content-Checksum", new List<string>() },
                { "Mex-Content-Encrypted", new List<string>() },
                { "Mex-ClientVersion", new List<string>() },
                { "Mex-OSVersion", new List<string>() },
                { "Mex-OSArchitecture", new List<string>() },
                { "Mex-JavaVersion", new List<string>() }
            };

            List<HttpResponseMessage> responseMessages = 
                CreateHttpResponseContentMessagesWithFileContentForRetrieveMessage(
                    inputMessage,
                    contentHeaders,
                    headers,
                    chunks,
                    HttpStatusCode.PartialContent);

            Message expectedMessage = new Message();

            for (int i = 0; i < chunks; i++)
            {
                if (i == 0)
                {
                    this.meshBrokerMock.Setup(broker =>
                        broker.GetMessageAsync(inputMessage.MessageId, authorizationToken))
                            .ReturnsAsync(responseMessages[i]);

                    expectedMessage = GetMessageWithFileContentFromHttpResponseMessageForReceive(
                        responseMessages[i], inputMessage.MessageId);
                }
                else
                {
                    this.meshBrokerMock.Setup(broker =>
                        broker.GetMessageAsync(
                            inputMessage.MessageId, 
                            It.Is(SameStringAs((i + 1).ToString())), 
                            authorizationToken))
                                .ReturnsAsync(responseMessages[i]);
                }
            }

            expectedMessage.FileContent = responseMessages
                .SelectMany(message => 
                    GetMessageWithFileContentFromHttpResponseMessageForReceive(message, inputMessage.MessageId)
                        .FileContent).ToArray();

            // when
            var actualMessage = await this.meshService
                .RetrieveMessageAsync(inputMessage.MessageId, authorizationToken);

            // then
            actualMessage.Should().BeEquivalentTo(expectedMessage);
            actualMessage.StringContent.Should().BeEquivalentTo(expectedMessage.StringContent);

            for (int i = 0; i < chunks; i++)
            {
                if (i == 0)
                {
                    this.meshBrokerMock.Verify(broker =>
                        broker.GetMessageAsync(inputMessage.MessageId, authorizationToken),
                            Times.Once);
                }
                else
                {
                    this.meshBrokerMock.Verify(broker =>
                        broker.GetMessageAsync(inputMessage.MessageId, It.Is(SameStringAs((i + 1).ToString())), authorizationToken),
                            Times.Once);
                }
            }

            this.meshBrokerMock.VerifyNoOtherCalls();
        }
    }
}