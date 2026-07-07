using Conexa.Domain.Entities;
using Conexa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Conexa.Infrastructure.Persistence.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(MovieDetails.TitleMaxLength);

        builder.Property(m => m.Director)
            .HasMaxLength(MovieDetails.DirectorMaxLength);

        builder.Property(m => m.Producer)
            .HasMaxLength(MovieDetails.ProducerMaxLength);

        builder.Property(m => m.OpeningCrawl)
            .HasColumnType("text");

        builder.Property(m => m.ExternalUrl)
            .HasMaxLength(500);

        builder.HasIndex(m => m.ExternalUrl)
            .IsUnique();

        builder.ConfigureAuditFields();
    }
}
