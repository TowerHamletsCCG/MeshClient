// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using NEL.MESH.Models.Configurations;
using NEL.MESH.Models.Foundations.Tokens.Exceptions;
using Xeptions;

namespace NEL.MESH.Services.Foundations.Tokens
{
    internal partial class TokenService
    {
        public static void ValidateGenerateTokenArgs(string mailboxId, string password, string key)
        {
            Validate<InvalidTokenArgsException>(
                message: "Invalid token argument validation errors occurred, " +
                    "please correct the errors and try again.",
                (Rule: IsInvalid(mailboxId), Parameter: nameof(MeshConfiguration.MailboxId)),
                (Rule: IsInvalid(password), Parameter: nameof(MeshConfiguration.Password)),
                (Rule: IsInvalid(key), Parameter: nameof(MeshConfiguration.SharedKey)));
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static void Validate<T>(string message, params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            var invalidDataException = (T)Activator.CreateInstance(typeof(T), message);

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
