﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Threading.Tasks;
using NEL.MESH.Models.Foundations.Mesh.Exceptions;
using NEL.MESH.Models.Foundations.Token.Exceptions;
using NEL.MESH.Models.Orchestrations.Mesh.Exceptions;
using Xeptions;

namespace NEL.MESH.Services.Orchestrations.Mesh
{
    internal partial class MeshOrchestrationService
    {
        private delegate ValueTask<bool> ReturningBooleanFunction();

        private async ValueTask<bool> TryCatch(ReturningBooleanFunction returningBooleanFunction)
        {
            try
            {
                return await returningBooleanFunction();
            }
            catch (TokenValidationException tokenValidationException)
            {
                throw CreateAndLogDependencyValidationException(tokenValidationException);
            }
            catch (TokenDependencyValidationException tokenDependencyValidationException)
            {
                throw CreateAndLogDependencyValidationException(tokenDependencyValidationException);
            }
            catch (MeshValidationException meshValidationException)
            {
                throw CreateAndLogDependencyValidationException(meshValidationException);
            }
            catch (MeshDependencyValidationException meshDependencyValidationException)
            {
                throw CreateAndLogDependencyValidationException(meshDependencyValidationException);
            }
        }

        private MeshOrchestrationDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var meshOrchestrationDependencyValidationException =
                new MeshOrchestrationDependencyValidationException(exception.InnerException as Xeption);

            return meshOrchestrationDependencyValidationException;
        }
    }
}