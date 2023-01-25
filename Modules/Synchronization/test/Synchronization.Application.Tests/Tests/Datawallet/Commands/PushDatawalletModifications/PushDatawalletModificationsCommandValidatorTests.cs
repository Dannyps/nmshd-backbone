﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;
using Synchronization.Application.Datawallets.DTOs;
using Xunit;

namespace Synchronization.Application.Tests.Tests.Datawallet.Commands.PushDatawalletModifications
{
    public class PushDatawalletModificationsCommandValidatorTests
    {
        [Fact]
        public void Happy_path()
        {
            var validator = new PushDatawalletModificationsCommandValidator();

            var command = new PushDatawalletModificationsCommand(
                new[] { new PushDatawalletModificationItem { Collection = "x", DatawalletVersion = 1, EncryptedPayload = new byte[0], ObjectIdentifier = "x", PayloadCategory = "x", Type = DatawalletModificationDTO.DatawalletModificationType.Create } },
                1);
            var validationResult = validator.TestValidate(command);

            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Fails_when_not_passing_a_SupportedDatawalletVersion()
        {
            var validator = new PushDatawalletModificationsCommandValidator();

            var command = new PushDatawalletModificationsCommand(
                new[] { new PushDatawalletModificationItem { Collection = "x", DatawalletVersion = 1, EncryptedPayload = new byte[0], ObjectIdentifier = "x", PayloadCategory = "x", Type = DatawalletModificationDTO.DatawalletModificationType.Create } },
                0);
            var validationResult = validator.TestValidate(command);

            validationResult.ShouldHaveValidationErrorFor(x => x.SupportedDatawalletVersion);
        }
    }
}
