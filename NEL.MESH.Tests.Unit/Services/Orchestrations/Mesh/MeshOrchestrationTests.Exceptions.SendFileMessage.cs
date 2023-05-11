﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NEL.MESH.Models.Foundations.Mesh;
using NEL.MESH.Models.Orchestrations.Mesh.Exceptions;
using Xeptions;
using Xunit;

namespace NEL.MESH.Tests.Unit.Services.Orchestrations.Mesh
{
    public partial class MeshOrchestrationTests
    {
        [Theory]
        [MemberData(nameof(MeshDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnSendFileIfDependencyValidationErrorOccursAsync(
            Xeption dependancyValidationException)
        {
            // given
            Message someMessage = CreateRandomSendFileMessage();
            var expectedMeshOrchestrationDependencyValidationException =
            new MeshOrchestrationDependencyValidationException(
                dependancyValidationException.InnerException as Xeption);

            this.chunkServiceMock.Setup(service =>
                service.SplitFileMessageIntoChunks(It.IsAny<Message>()))
                    .Throws(dependancyValidationException);

            // when
            ValueTask<Message> sendMessageTask = this.meshOrchestrationService.SendFileAsync(someMessage);

            MeshOrchestrationDependencyValidationException actualMeshOrchestrationDependencyValidationException =
                await Assert.ThrowsAsync<MeshOrchestrationDependencyValidationException>(sendMessageTask.AsTask);

            // then
            actualMeshOrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedMeshOrchestrationDependencyValidationException);

            this.chunkServiceMock.Verify(service =>
                service.SplitFileMessageIntoChunks(It.IsAny<Message>()),
                    Times.Once);

            this.chunkServiceMock.VerifyNoOtherCalls();
            this.meshServiceMock.VerifyNoOtherCalls();
            this.tokenServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MeshDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSendFileIfDependencyErrorOccursAsync(
            Xeption dependancyException)
        {
            // given
            Message someMessage = CreateRandomSendFileMessage();
            var expectedMeshOrchestrationDependencyException =
            new MeshOrchestrationDependencyException(
                dependancyException.InnerException as Xeption);

            this.chunkServiceMock.Setup(service =>
                service.SplitFileMessageIntoChunks(It.IsAny<Message>()))
                    .Throws(dependancyException);

            // when
            ValueTask<Message> sendMessageTask = this.meshOrchestrationService.SendFileAsync(someMessage);

            MeshOrchestrationDependencyException actualMeshOrchestrationDependencyException =
                await Assert.ThrowsAsync<MeshOrchestrationDependencyException>(sendMessageTask.AsTask);

            // then
            actualMeshOrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedMeshOrchestrationDependencyException);

            this.chunkServiceMock.Verify(service =>
                service.SplitFileMessageIntoChunks(It.IsAny<Message>()),
                    Times.Once);

            this.chunkServiceMock.VerifyNoOtherCalls();
            this.meshServiceMock.VerifyNoOtherCalls();
            this.tokenServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSendFileIfServiceErrorOccursAsync()
        {
            // given
            Message someMessage = CreateRandomSendFileMessage();
            string someErrorMessage = GetRandomString();
            var serviceException = new Exception(someErrorMessage);

            var failedMeshOrchestrationServiceException =
                new FailedMeshOrchestrationServiceException(serviceException);

            var expectedMeshOrchestrationServiceException =
            new MeshOrchestrationServiceException(failedMeshOrchestrationServiceException);

            this.chunkServiceMock.Setup(service =>
                service.SplitFileMessageIntoChunks(It.IsAny<Message>()))
                    .Throws(serviceException);

            // when
            ValueTask<Message> sendMessageTask = this.meshOrchestrationService.SendFileAsync(someMessage);

            MeshOrchestrationServiceException actualMeshOrchestrationServiceException =
                await Assert.ThrowsAsync<MeshOrchestrationServiceException>(sendMessageTask.AsTask);

            // then
            actualMeshOrchestrationServiceException.Should()
                .BeEquivalentTo(expectedMeshOrchestrationServiceException);

            this.chunkServiceMock.Verify(service =>
                service.SplitFileMessageIntoChunks(It.IsAny<Message>()),
                    Times.Once);

            this.chunkServiceMock.VerifyNoOtherCalls();
            this.meshServiceMock.VerifyNoOtherCalls();
            this.tokenServiceMock.VerifyNoOtherCalls();
        }
    }
}