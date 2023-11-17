﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace Backbone.Modules.Devices.Infrastructure.CompiledModels.Postgres
{
    public partial class DevicesDbContextModel
    {
        partial void Initialize()
        {
            var pnsRegistration = PnsRegistrationEntityType.Create(this);
            var tier = TierEntityType.Create(this);
            var challenge = ChallengeEntityType.Create(this);
            var applicationUser = ApplicationUserEntityType.Create(this);
            var device = DeviceEntityType.Create(this);
            var identity = IdentityEntityType.Create(this);
            var identityDeletionProcess = IdentityDeletionProcessEntityType.Create(this);
            var identityDeletionProcessAuditLogEntry = IdentityDeletionProcessAuditLogEntryEntityType.Create(this);
            var customOpenIddictEntityFrameworkCoreApplication = CustomOpenIddictEntityFrameworkCoreApplicationEntityType.Create(this);
            var customOpenIddictEntityFrameworkCoreAuthorization = CustomOpenIddictEntityFrameworkCoreAuthorizationEntityType.Create(this);
            var customOpenIddictEntityFrameworkCoreScope = CustomOpenIddictEntityFrameworkCoreScopeEntityType.Create(this);
            var customOpenIddictEntityFrameworkCoreToken = CustomOpenIddictEntityFrameworkCoreTokenEntityType.Create(this);
            var identityRole = IdentityRoleEntityType.Create(this);
            var identityRoleClaim = IdentityRoleClaimEntityType.Create(this);
            var identityUserClaim = IdentityUserClaimEntityType.Create(this);
            var identityUserLogin = IdentityUserLoginEntityType.Create(this);
            var identityUserRole = IdentityUserRoleEntityType.Create(this);
            var identityUserToken = IdentityUserTokenEntityType.Create(this);

            ApplicationUserEntityType.CreateForeignKey1(applicationUser, device);
            DeviceEntityType.CreateForeignKey1(device, identity);
            IdentityDeletionProcessEntityType.CreateForeignKey1(identityDeletionProcess, identity);
            IdentityDeletionProcessAuditLogEntryEntityType.CreateForeignKey1(identityDeletionProcessAuditLogEntry, identityDeletionProcess);
            CustomOpenIddictEntityFrameworkCoreApplicationEntityType.CreateForeignKey1(customOpenIddictEntityFrameworkCoreApplication, tier);
            CustomOpenIddictEntityFrameworkCoreAuthorizationEntityType.CreateForeignKey1(customOpenIddictEntityFrameworkCoreAuthorization, customOpenIddictEntityFrameworkCoreApplication);
            CustomOpenIddictEntityFrameworkCoreTokenEntityType.CreateForeignKey1(customOpenIddictEntityFrameworkCoreToken, customOpenIddictEntityFrameworkCoreApplication);
            CustomOpenIddictEntityFrameworkCoreTokenEntityType.CreateForeignKey2(customOpenIddictEntityFrameworkCoreToken, customOpenIddictEntityFrameworkCoreAuthorization);
            IdentityRoleClaimEntityType.CreateForeignKey1(identityRoleClaim, identityRole);
            IdentityUserClaimEntityType.CreateForeignKey1(identityUserClaim, applicationUser);
            IdentityUserLoginEntityType.CreateForeignKey1(identityUserLogin, applicationUser);
            IdentityUserRoleEntityType.CreateForeignKey1(identityUserRole, identityRole);
            IdentityUserRoleEntityType.CreateForeignKey2(identityUserRole, applicationUser);
            IdentityUserTokenEntityType.CreateForeignKey1(identityUserToken, applicationUser);

            PnsRegistrationEntityType.CreateAnnotations(pnsRegistration);
            TierEntityType.CreateAnnotations(tier);
            ChallengeEntityType.CreateAnnotations(challenge);
            ApplicationUserEntityType.CreateAnnotations(applicationUser);
            DeviceEntityType.CreateAnnotations(device);
            IdentityEntityType.CreateAnnotations(identity);
            IdentityDeletionProcessEntityType.CreateAnnotations(identityDeletionProcess);
            IdentityDeletionProcessAuditLogEntryEntityType.CreateAnnotations(identityDeletionProcessAuditLogEntry);
            CustomOpenIddictEntityFrameworkCoreApplicationEntityType.CreateAnnotations(customOpenIddictEntityFrameworkCoreApplication);
            CustomOpenIddictEntityFrameworkCoreAuthorizationEntityType.CreateAnnotations(customOpenIddictEntityFrameworkCoreAuthorization);
            CustomOpenIddictEntityFrameworkCoreScopeEntityType.CreateAnnotations(customOpenIddictEntityFrameworkCoreScope);
            CustomOpenIddictEntityFrameworkCoreTokenEntityType.CreateAnnotations(customOpenIddictEntityFrameworkCoreToken);
            IdentityRoleEntityType.CreateAnnotations(identityRole);
            IdentityRoleClaimEntityType.CreateAnnotations(identityRoleClaim);
            IdentityUserClaimEntityType.CreateAnnotations(identityUserClaim);
            IdentityUserLoginEntityType.CreateAnnotations(identityUserLogin);
            IdentityUserRoleEntityType.CreateAnnotations(identityUserRole);
            IdentityUserTokenEntityType.CreateAnnotations(identityUserToken);

            AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            AddAnnotation("ProductVersion", "7.0.13");
            AddAnnotation("Relational:MaxIdentifierLength", 63);
        }
    }
}
