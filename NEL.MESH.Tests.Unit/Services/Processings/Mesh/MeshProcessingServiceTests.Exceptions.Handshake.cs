﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NEL.MESH.Models.Clients.Mesh.Exceptions;
using NEL.MESH.Models.Processings.Mesh;
using Xeptions;
using Xunit;

namespace NEL.MESH.Tests.Unit.Services.Processings.Mesh
{
    public partial class MeshProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnHandshakeIfDependencyValidationErrorOccurs(
            Xeption dependencyValidationException)
        {
            // given
            string authorizationToken = GetRandomString();

            var expectedMeshProcessingDependencyValidationException =
                new MeshProcessingDependencyValidationException(dependencyValidationException);

            this.meshServiceMock.Setup(service =>
                service.HandshakeAsync(authorizationToken))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<bool> HandshakeTask =
                this.meshProcessingService.HandshakeAsync(authorizationToken);

            MeshProcessingDependencyValidationException actualMeshProcessingDependencyValidationException =
                await Assert.ThrowsAsync<MeshProcessingDependencyValidationException>(HandshakeTask.AsTask);

            // then
            actualMeshProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedMeshProcessingDependencyValidationException);

            this.meshServiceMock.Verify(service =>
                service.HandshakeAsync(authorizationToken),
                    Times.Once);

            this.meshServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnHandshakeIfDependencyErrorOccurs(
            Xeption dependencyException)
        {
            // given
            string authorizationToken = GetRandomString();

            var expectedMeshProcessingDependencyException =
                new MeshProcessingDependencyException(dependencyException);

            this.meshServiceMock.Setup(service =>
                service.HandshakeAsync(authorizationToken))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<bool> HandshakeTask =
                this.meshProcessingService.HandshakeAsync(authorizationToken);

            MeshProcessingDependencyException actualMeshProcessingDependencyException =
                await Assert.ThrowsAsync<MeshProcessingDependencyException>(HandshakeTask.AsTask);

            // then
            actualMeshProcessingDependencyException.Should()
                .BeEquivalentTo(expectedMeshProcessingDependencyException);

            this.meshServiceMock.Verify(service =>
                service.HandshakeAsync(authorizationToken),
                    Times.Once);

            this.meshServiceMock.VerifyNoOtherCalls();
        }
    }
}
