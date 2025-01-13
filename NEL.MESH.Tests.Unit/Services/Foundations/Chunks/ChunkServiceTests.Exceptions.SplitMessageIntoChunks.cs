// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using FluentAssertions;
using Moq;
using NEL.MESH.Models.Foundations.Chunking.Exceptions;
using NEL.MESH.Models.Foundations.Mesh;
using Xeptions;
using Xunit;

namespace NEL.MESH.Tests.Unit.Services.Foundations.Chunks
{
    public partial class ChunkServiceTests
    {
        [Fact]
        public void ShouldThrowServiceExceptionIfServiceErrorOccursOnSplitMessageIntoChunks()
        {
            // given
            Message someMessage = CreateRandomMessage();
            string someStringMessage = GetRandomString();
            Exception someException = new Exception(someStringMessage);

            var failedChunkServiceException = new FailedChunkServiceException(
                message: "Chunk service error occurred, contact support.",
                innerException: someException);

            var expectedChunkServiceException =
                new ChunkServiceException(
                    message: "Chunk service error occurred, contact support.",
                    innerException: failedChunkServiceException as Xeption);

            this.meshConfigurationBrokerMock.Setup(broker =>
                broker.MaxChunkSizeInBytes)
                    .Throws(someException);

            // when
            Action splitMessageIntoChunksAction = () =>
                this.chunkService.SplitMessageIntoChunks(message: someMessage);

            ChunkServiceException actualChunkServiceException =
               Assert.Throws<ChunkServiceException>(splitMessageIntoChunksAction);

            // then
            actualChunkServiceException.Should()
                .BeEquivalentTo(expectedChunkServiceException);

            this.meshConfigurationBrokerMock.Verify(broker =>
                broker.MaxChunkSizeInBytes,
                    Times.Once);

            this.meshConfigurationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
