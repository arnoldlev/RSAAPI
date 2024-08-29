using Microsoft.EntityFrameworkCore;
using RSAAPI.Models;

namespace RSAAPI.Database
{
    public class UserConfiguration : IEntityTypeConfiguration<DbUser>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<DbUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.ApiToken).HasMaxLength(1025);
            builder.Property(x => x.SandBoxToken).HasMaxLength(1025);
            builder.Property(x => x.LicenseKey).HasMaxLength(1025);
            builder.Property(x => x.LastLogin);

            builder.Property<DateTime>("CreatedDate")
                .IsRequired()
                .HasDefaultValue(DateTime.Now)
                .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

            builder.Property<DateTime>("ModifiedDate")
                .IsRequired()
                .HasDefaultValue(DateTime.Now)
                .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
        }
    }
}
