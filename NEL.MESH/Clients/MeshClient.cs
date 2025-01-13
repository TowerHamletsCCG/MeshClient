// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NEL.MESH.Brokers.DateTimes;
using NEL.MESH.Brokers.Identifiers;
using NEL.MESH.Brokers.Loggings;
using NEL.MESH.Brokers.Mesh;
using NEL.MESH.Clients.Mailboxes;
using NEL.MESH.Models.Configurations;
using NEL.MESH.Services.Foundations.Chunks;
using NEL.MESH.Services.Foundations.Mesh;
using NEL.MESH.Services.Foundations.Tokens;
using NEL.MESH.Services.Orchestrations.Mesh;

namespace NEL.MESH.Clients
{
    /// <summary>
    /// Represents a client for interacting with the Mesh service.
    /// </summary>
    public class MeshClient : IMeshClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeshClient"/> class with the specified configuration.
        /// </summary>
        /// <param name="meshConfigurations">The Mesh configuration settings.</param>
        public MeshClient(MeshConfiguration meshConfigurations)
        {
            IHost host = RegisterServices(meshConfigurations, null);
            Mailbox = InitializeClient(host);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshClient"/> class with the specified configuration 
        /// and optional logger factory.
        /// </summary>
        /// <param name="meshConfigurations">The Mesh configuration settings.</param>
        /// <param name="loggerFactory">An optional logger factory for logging operations.</param>
        public MeshClient(MeshConfiguration meshConfigurations, ILoggerFactory loggerFactory = null)
        {
            IHost host = RegisterServices(meshConfigurations, loggerFactory);
            Mailbox = InitializeClient(host);
        }

        /// <summary>
        /// Gets the <see cref="IMailboxClient"/> instance for interacting with a MESH mailbox.
        /// </summary>
        public IMailboxClient Mailbox { get; private set; }

        private static IMailboxClient InitializeClient(IHost host) =>
            host.Services.GetRequiredService<IMailboxClient>();

        private static IHost RegisterServices(MeshConfiguration meshConfigurations, ILoggerFactory loggerFactory)
        {
            if (loggerFactory is null)
            {
                loggerFactory = LoggerFactory.Create(builder => { });
            }

            ILogger<LoggingBroker> loggingBroker = loggerFactory.CreateLogger<LoggingBroker>();

            IHostBuilder builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices(configuration =>
            {
                configuration.AddSingleton(options => meshConfigurations);
                configuration.AddTransient(_ => loggingBroker);
                configuration.AddTransient<ILoggingBroker, LoggingBroker>();
                configuration.AddTransient<IMeshConfigurationBroker, MeshConfigurationBroker>();
                configuration.AddTransient<IMeshBroker, MeshBroker>();
                configuration.AddTransient<IDateTimeBroker, DateTimeBroker>();
                configuration.AddTransient<IIdentifierBroker, IdentifierBroker>();
                configuration.AddTransient<IChunkService, ChunkService>();
                configuration.AddTransient<IMeshService, MeshService>();
                configuration.AddTransient<ITokenService, TokenService>();
                configuration.AddTransient<IMeshOrchestrationService, MeshOrchestrationService>();
                configuration.AddTransient<IMailboxClient, MailboxClient>();
            });

            IHost host = builder.Build();

            return host;
        }
    }
}
