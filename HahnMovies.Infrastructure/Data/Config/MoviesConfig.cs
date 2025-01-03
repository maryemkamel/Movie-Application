using HahnMovies.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HahnMovies.Infrastructure.Data.Config;

public class MoviesConfig : IEntityTypeConfiguration<Movies>
{
    public void Configure(EntityTypeBuilder<Movies> builder)
    {
        // Configure table name (optional)
        builder.ToTable("Movies");

        // Configure properties

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.ReleaseDate)
            .IsRequired();

        builder.Property(m => m.VoteAverage)
            .HasPrecision(5, 2); // Example: Precision for decimal values

        builder.Property(m => m.PosterPath)
            .HasMaxLength(500)
            .IsRequired();

        // Add any additional configurations here
    }
}