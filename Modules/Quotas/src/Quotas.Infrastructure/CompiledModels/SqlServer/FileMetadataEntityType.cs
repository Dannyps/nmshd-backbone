﻿// <auto-generated />
using System;
using System.Reflection;
using Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace Backbone.Modules.Quotas.Infrastructure.CompiledModels.SqlServer
{
    internal partial class FileMetadataEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType? baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata.FileMetadata",
                typeof(FileMetadata),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(string),
                propertyInfo: typeof(FileMetadata).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FileMetadata).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            id.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var cipherSize = runtimeEntityType.AddProperty(
                "CipherSize",
                typeof(long),
                propertyInfo: typeof(FileMetadata).GetProperty("CipherSize", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FileMetadata).GetField("<CipherSize>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            cipherSize.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var createdAt = runtimeEntityType.AddProperty(
                "CreatedAt",
                typeof(DateTime),
                propertyInfo: typeof(FileMetadata).GetProperty("CreatedAt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FileMetadata).GetField("<CreatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueConverter: new DateTimeValueConverter());
            createdAt.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var createdBy = runtimeEntityType.AddProperty(
                "CreatedBy",
                typeof(string),
                propertyInfo: typeof(FileMetadata).GetProperty("CreatedBy", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FileMetadata).GetField("<CreatedBy>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            createdBy.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", "Files");
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "FileMetadata");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
