﻿using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants;

namespace Infrastructure.DataAccessManager.EFCore.Configurations;

public class SalesOrderItemConfiguration : BaseEntityConfiguration<SalesOrderItem>
{
    public override void Configure(EntityTypeBuilder<SalesOrderItem> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.SalesOrderId).HasMaxLength(IdConsts.MaxLength).IsRequired(false);
        builder.Property(x => x.ProductId).HasMaxLength(IdConsts.MaxLength).IsRequired(false);
        builder.Property(x => x.Summary).HasMaxLength(DescriptionConsts.MaxLength).IsRequired(false);
        builder.Property(x => x.UnitPrice).IsRequired(false);
        builder.Property(x => x.Quantity).IsRequired(false);
        builder.Property(x => x.Total).IsRequired(false);

    }
}

