﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace Backbone.Modules.Devices.Infrastructure.CompiledModels.Postgres
{
    internal partial class IdentityDeletionProcessAuditLogEntryEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType? baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Backbone.Modules.Devices.Domain.Entities.Identities.IdentityDeletionProcessAuditLogEntry",
                typeof(IdentityDeletionProcessAuditLogEntry),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(IdentityDeletionProcessAuditLogEntryId),
                propertyInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                maxLength: 20,
                unicode: false,
                valueConverter: new IdentityDeletionProcessAuditLogEntryIdEntityFrameworkValueConverter());
            id.AddAnnotation("Relational:IsFixedLength", true);

            var createdAt = runtimeEntityType.AddProperty(
                "CreatedAt",
                typeof(DateTime),
                propertyInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetProperty("CreatedAt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetField("<CreatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueConverter: new DateTimeValueConverter());

            var deviceIdHash = runtimeEntityType.AddProperty(
                "DeviceIdHash",
                typeof(byte[]),
                propertyInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetProperty("DeviceIdHash", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetField("<DeviceIdHash>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var identityAddressHash = runtimeEntityType.AddProperty(
                "IdentityAddressHash",
                typeof(byte[]),
                propertyInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetProperty("IdentityAddressHash", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetField("<IdentityAddressHash>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var identityDeletionProcessId = runtimeEntityType.AddProperty(
                "IdentityDeletionProcessId",
                typeof(IdentityDeletionProcessId),
                nullable: true,
                maxLength: 20,
                unicode: false,
                valueConverter: new IdentityDeletionProcessIdEntityFrameworkValueConverter());
            identityDeletionProcessId.AddAnnotation("Relational:IsFixedLength", true);

            var message = runtimeEntityType.AddProperty(
                "Message",
                typeof(string),
                propertyInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetProperty("Message", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetField("<Message>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var newStatus = runtimeEntityType.AddProperty(
                "NewStatus",
                typeof(DeletionProcessStatus),
                propertyInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetProperty("NewStatus", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetField("<NewStatus>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var oldStatus = runtimeEntityType.AddProperty(
                "OldStatus",
                typeof(DeletionProcessStatus?),
                propertyInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetProperty("OldStatus", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityDeletionProcessAuditLogEntry).GetField("<OldStatus>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { identityDeletionProcessId });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("IdentityDeletionProcessId")! },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id")! })!,
                principalEntityType);

            var auditLog = principalEntityType.AddNavigation("AuditLog",
                runtimeForeignKey,
                onDependent: false,
                typeof(IReadOnlyList<IdentityDeletionProcessAuditLogEntry>),
                propertyInfo: typeof(IdentityDeletionProcess).GetProperty("AuditLog", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityDeletionProcess).GetField("_auditLog", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "IdentityDeletionProcessAuditLog");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
