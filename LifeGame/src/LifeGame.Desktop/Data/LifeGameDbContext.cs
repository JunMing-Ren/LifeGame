using Microsoft.EntityFrameworkCore;
using LifeGame.Core.Models;

namespace LifeGame.Core.Data;

public class LifeGameDbContext : DbContext
{
    public DbSet<GameRecord> GameRecords { get; set; }

    public LifeGameDbContext(DbContextOptions<LifeGameDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RecordId).IsRequired();
            entity.Property(e => e.GameName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.EndingType).HasMaxLength(50);
            entity.Property(e => e.EndingTitle).HasMaxLength(200);
            entity.HasIndex(e => e.CreatedTime);
            entity.HasIndex(e => e.EndingType);
            entity.HasIndex(e => e.IsCompleted);
        });
    }
}
