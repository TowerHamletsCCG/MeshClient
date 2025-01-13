// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using Microsoft.Extensions.Configuration;

namespace NEL.MESH.Tests.Integration.Witness
{
    public partial class ConfigurationTests
    {
        private readonly IConfiguration configuration;

        public ConfigurationTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            this.configuration = configurationBuilder.Build();
        }
    }
}
