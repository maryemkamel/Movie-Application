using HahnMovies.Application.Common.Interfaces;
using HahnMovies.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HahnMovies.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext, IUnitOfWork
{
    private readonly IMediator _mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Movies> Movies { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        throw new InvalidOperationException("Use SaveChangesAsync method instead of SaveChanges");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}