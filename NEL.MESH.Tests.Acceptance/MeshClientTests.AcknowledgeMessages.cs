﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace NEL.MESH.Tests.Acceptance
{
    public partial class MeshClientTests
    {
        [Fact]
        public async Task ShouldAcknowledgeMessageAsync()
        {
            // given
            string messageId = GetRandomString();
            var path =
                $"/messageexchange/{this.meshConfigurations.MailboxId}/inbox/{messageId}/status/acknowledged";

            bool expectedAcknowledgeMessageResult = true;

            this.wireMockServer
                .Given(
                    Request.Create()
                        .WithPath(path)
                        .UsingPut()
                        .WithHeader("Mex-ClientVersion", this.meshConfigurations.MexClientVersion, MatchBehaviour.AcceptOnMatch)
                        .WithHeader("Mex-OSName", this.meshConfigurations.MexOSName, MatchBehaviour.AcceptOnMatch)
                        .WithHeader("Mex-OSVersion", this.meshConfigurations.MexOSVersion, MatchBehaviour.AcceptOnMatch)
                        .WithHeader("Authorization", "*", MatchBehaviour.AcceptOnMatch)
                        )
                .RespondWith(
                    Response.Create()
                        .WithSuccess());

            // when
            bool actualGetMessageResult =
                await this.meshClient.Mailbox.AcknowledgeMessageAsync(messageId);

            // then
            actualGetMessageResult.Should().Be(expectedAcknowledgeMessageResult);
        }
    }
}
