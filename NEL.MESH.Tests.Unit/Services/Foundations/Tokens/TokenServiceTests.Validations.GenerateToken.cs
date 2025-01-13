// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NEL.MESH.Models.Configurations;
using NEL.MESH.Models.Foundations.Tokens.Exceptions;
using Xunit;

namespace NEL.MESH.Tests.Unit.Services.Foundations.Tokens
{
    public partial class TokenServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnGenerateTokenIfArgumentsAreInvalidAsync(string invalidText)
        {
            // given
            string mailboxId = invalidText;
            string password = invalidText;
            string key = invalidText;

            MeshConfiguration meshConfiguration = new MeshConfiguration
            {
                MailboxId = mailboxId,
                Password = password,
                SharedKey = key
            };

            this.meshBrokerMock.Setup(broker =>
                broker.MeshConfiguration)
                    .Returns(meshConfiguration);


            Guid identifier = Guid.NewGuid();
            DateTimeOffset randomDateTimeOffset = new DateTime(2022, 3, 24, 12, 00, 00);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifier())
                    .Returns(identifier);

            var InvalidTokenArgsException = new InvalidTokenArgsException(
                    message: "Invalid token argument validation errors occurred, " +
                        "please correct the errors and try again.");

            InvalidTokenArgsException.AddData(
                key: nameof(MeshConfiguration.MailboxId),
                values: "Text is required");

            InvalidTokenArgsException.AddData(
                key: nameof(MeshConfiguration.Password),
                values: "Text is required");

            InvalidTokenArgsException.AddData(
                key: nameof(MeshConfiguration.SharedKey),
                values: "Text is required");

            var expectedTokenValidationException =
                 new TokenValidationException(
                     message: "Token validation errors occurred, please try again.",
                     innerException: InvalidTokenArgsException);

            // when
            ValueTask<string> getTokenTask =
                this.tokenService.GenerateTokenAsync();

            TokenValidationException actualTokenValidationException =
                await Assert.ThrowsAsync<TokenValidationException>(() =>
                    getTokenTask.AsTask());

            // then
            actualTokenValidationException.Should()
                .BeEquivalentTo(expectedTokenValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifier(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
