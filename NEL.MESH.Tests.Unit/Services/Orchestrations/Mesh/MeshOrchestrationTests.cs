// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Moq;
using NEL.MESH.Brokers.Mesh;
using NEL.MESH.Models.Foundations.Mesh;
using NEL.MESH.Models.Foundations.Mesh.Exceptions;
using NEL.MESH.Models.Foundations.Tokens.Exceptions;
using NEL.MESH.Services.Foundations.Chunks;
using NEL.MESH.Services.Foundations.Mesh;
using NEL.MESH.Services.Foundations.Tokens;
using NEL.MESH.Services.Orchestrations.Mesh;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace NEL.MESH.Tests.Unit.Services.Orchestrations.Mesh
{
    public partial class MeshOrchestrationTests
    {
        private readonly Mock<ITokenService> tokenServiceMock;
        private readonly Mock<IMeshService> meshServiceMock;
        private readonly Mock<IChunkService> chunkServiceMock;
        private readonly IMeshOrchestrationService meshOrchestrationService;
        private readonly Mock<IMeshConfigurationBroker> meshConfigurationBrokerMock;

        public MeshOrchestrationTests()
        {
            this.tokenServiceMock = new Mock<ITokenService>();
            this.meshServiceMock = new Mock<IMeshService>();
            this.chunkServiceMock = new Mock<IChunkService>();
            this.meshConfigurationBrokerMock = new Mock<IMeshConfigurationBroker>();

            this.meshOrchestrationService = new MeshOrchestrationService(
                tokenService: this.tokenServiceMock.Object,
                meshService: this.meshServiceMock.Object,
                chunkService: this.chunkServiceMock.Object,
                meshConfigurationBroker: this.meshConfigurationBrokerMock.Object);
        }

        private static string GetRandomString(int wordCount = 0)
        {
            return new MnemonicString(
                wordCount: wordCount == 0 ? GetRandomNumber() : wordCount,
                wordMinLength: 1,
                wordMaxLength: GetRandomNumber()).GetValue();
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static List<string> GetRandomMessages()
        {
            return Enumerable.Range(1, GetRandomNumber())
                .Select(item => GetRandomString())
                .ToList();
        }

        public static TheoryData<List<Message>> InvalidMessageList()
        {
            return new TheoryData<List<Message>>
            {
                null,
                new List<Message>(),
            };
        }

        public static TheoryData<Xeption> MeshDependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new TokenValidationException(
                    message: "Token validation errors occurred, please try again.",
                    innerException),

                new TokenDependencyValidationException(
                    message: "Token dependency error occurred, contact support.",
                    innerException),

                new MeshValidationException(
                    message: "Message validation errors occurred, please try again.",
                    innerException),

                new MeshDependencyValidationException(
                    message: "Mesh dependency error occurred, contact support.",
                    innerException),
            };
        }

        public static TheoryData<Xeption> MeshDependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new TokenDependencyException(
                    message: "Token dependency error occurred, contact support.",
                    innerException),

                new TokenServiceException(
                    message: "Token service error occurred, contact support.",
                    innerException),

                new MeshDependencyException(
                    message: "Mesh dependency error occurred, contact support.",
                    innerException),

                new MeshServiceException(
                    message: "Mesh service error occurred, contact support.",
                    innerException),
            };
        }

        private static List<Message> CreateRandomChunkedSendMessages(int messageChunkCount)
        {
            List<Message> messages = new List<Message>();

            for (int i = 0; i < messageChunkCount; i++)
            {
                var message = CreateRandomSendMessage();
                message.Headers["mex-chunk-range"] = new List<string> { $"{{{i + 1}:{messageChunkCount}}}" };
                messages.Add(message);
            }

            return messages;
        }

        private static Message CreateRandomSendMessage()
        {
            var message = CreateMessageFiller().Create();
            message.Headers.Add("mex-from", new List<string> { GetRandomString() });
            message.Headers.Add("mex-to", new List<string> { GetRandomString() });
            message.Headers.Add("mex-workflowid", new List<string> { GetRandomString() });
            message.Headers.Add("mex-chunk-range", new List<string> { "1:1" });
            message.Headers.Add("mex-subject", new List<string> { GetRandomString() });
            message.Headers.Add("mex-localid", new List<string> { GetRandomString() });
            message.Headers.Add("mex-filename", new List<string> { GetRandomString() });
            message.Headers.Add("mex-content-checksum", new List<string> { GetRandomString() });
            message.Headers.Add("content-type", new List<string> { "text/plain" });
            message.Headers.Add("content-encoding", new List<string> { GetRandomString() });
            message.Headers.Add("Accept", new List<string> { "application/json" });
            message.MessageId = null;

            return message;
        }

        private static Filler<Message> CreateMessageFiller()
        {
            var filler = new Filler<Message>();
            filler.Setup()
                .OnProperty(message => message.Headers).Use(new Dictionary<string, List<string>>());

            return filler;
        }
    }
}
