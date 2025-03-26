using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccessManager.EFCore.Configurations
{
    public class AlertConfigConfiguration : IEntityTypeConfiguration<AlertConfig>
    {
        public void Configure(EntityTypeBuilder<AlertConfig> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .IsRequired();

            builder.Property(a => a.Percentage)
                .IsRequired();

            builder.Property(a => a.DateAdded)
                .IsRequired();

            builder.Property(a => a.DateUpdated)
                .IsRequired();
        }
    }
}
