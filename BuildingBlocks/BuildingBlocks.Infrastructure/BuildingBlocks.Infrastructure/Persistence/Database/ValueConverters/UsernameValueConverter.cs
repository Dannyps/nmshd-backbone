﻿using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;

public class UsernameValueConverter : ValueConverter<Username, string>
{
    public UsernameValueConverter() : this(null)
    {
    }

    public UsernameValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.StringValue,
            value => Username.Parse(value),
            mappingHints
        )
    {
    }
}

public class NullableUsernameValueConverter : ValueConverter<Username?, string?>
{
    public NullableUsernameValueConverter() : this(null)
    {
    }

    public NullableUsernameValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => value == null ? null : Username.Parse(value),
            mappingHints
        )
    {
    }
}