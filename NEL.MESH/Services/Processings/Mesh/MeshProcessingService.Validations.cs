﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using NEL.MESH.Models.Processings.Mesh;
using Xeptions;

namespace NEL.MESH.Services.Processings.Mesh
{
    internal partial class MeshProcessingService : IMeshProcessingService
    {
        private static void ValidateOnHandshake(string authorizationToken)
        {
            Validate<InvalidArgumentsMeshProcessingException>(
                (Rule: IsInvalid(authorizationToken), Parameter: "Token"));
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static void Validate<T>(params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            var invalidDataException = (T)Activator.CreateInstance(typeof(T));

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDataException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDataException.ThrowIfContainsErrors();
        }
    }
}
