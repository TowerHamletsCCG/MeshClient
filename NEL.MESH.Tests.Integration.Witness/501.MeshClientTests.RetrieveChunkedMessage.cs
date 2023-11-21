﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NEL.MESH.Models.Foundations.Mesh;
using Xunit;

namespace NEL.MESH.Tests.Integration.Witness
{
    public partial class MeshClientTests
    {
        [Fact(DisplayName = "501 - Retrieving Chunked Messages")]
        [Trait("Category", "Witness")]
        public async Task ShouldRetrieveChunckedMessageAsync()
        {
            // given
            string mexTo = this.meshConfigurations.MailboxId;
            string mexWorkflowId = "WITNESS TEST - CHUNKING";
            int targetSizeInMegabytes = 2; //Over 1 will result in Chunking.
            string content = GetFileWithXBytes(targetSizeInMegabytes);
            string mexSubject = "WITNESS TEST -  ShouldRetrieveChunckedMessageAsync";
            string mexLocalId = Guid.NewGuid().ToString();
            string mexFileName = $"ShouldRetrieveChunckedMessageAsync.csv";
            string mexContentChecksum = Guid.NewGuid().ToString();
            string contentType = "text/plain";
            string contentEncoding = "";

            Message sendMessageResponse =
                await this.meshClient.Mailbox.SendMessageAsync(
                    mexTo,
                    mexWorkflowId,
                    content,
                    mexSubject,
                    mexLocalId,
                    mexFileName,
                    mexContentChecksum,
                    contentType,
                    contentEncoding);

            string messageId = sendMessageResponse.MessageId;

            // when
            Message retrievedMessage =
                await this.meshClient.Mailbox.RetrieveMessageAsync(messageId);

            // then
            await this.meshClient.Mailbox.AcknowledgeMessageAsync(retrievedMessage.MessageId);
        }
    }
}
