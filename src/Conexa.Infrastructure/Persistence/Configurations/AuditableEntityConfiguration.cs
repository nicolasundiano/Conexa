using Conexa.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Conexa.Infrastructure.Persistence.Configurations;

public static class AuditableEntityConfiguration
{
    private const int UserIdMaxLength = 100;

    public static void ConfigureAuditFields<T>(this EntityTypeBuilder<T> builder)
        where T : BaseAuditableEntity
    {
        builder.Property(e => e.CreatedBy).HasMaxLength(UserIdMaxLength);
        builder.Property(e => e.LastModifiedBy).HasMaxLength(UserIdMaxLength);
    }
}
