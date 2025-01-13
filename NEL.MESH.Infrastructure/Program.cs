// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using NEL.MESH.Infrastructure.Services;

namespace NEL.MESH.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scriptGenerationService = new ScriptGenerationService();

            scriptGenerationService.GenerateBuildScript(
                branchName: "main",
                projectName: "ISL.ReIdentification.Core",
                dotNetVersion: "9.0.100");

            scriptGenerationService.GeneratePrLintScript("main");
        }
    }
}