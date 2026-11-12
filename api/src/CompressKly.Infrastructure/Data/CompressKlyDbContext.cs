using CompressKly.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompressKly.Infrastructure.Data;

public sealed class CompressKlyDbContext : DbContext
{
    public CompressKlyDbContext(DbContextOptions<CompressKlyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Video> Videos => Set<Video>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompressKlyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
