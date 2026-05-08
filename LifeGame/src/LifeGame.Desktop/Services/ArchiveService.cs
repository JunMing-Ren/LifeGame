using Microsoft.EntityFrameworkCore;
using LifeGame.Core.Data;
using LifeGame.Core.Models;

namespace LifeGame.Desktop.Services;

public class ArchiveService
{
    private readonly IDbContextFactory<LifeGameDbContext> _contextFactory;

    public ArchiveService(IDbContextFactory<LifeGameDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<GameRecord>> GetAllRecordsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GameRecords
            .OrderByDescending(r => r.CreatedTime)
            .ToListAsync();
    }

    public async Task<List<GameRecord>> GetCompletedRecordsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GameRecords
            .Where(r => r.IsCompleted)
            .OrderByDescending(r => r.CreatedTime)
            .ToListAsync();
    }

    public async Task<GameRecord?> GetRecordByIdAsync(Guid recordId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GameRecords
            .FirstOrDefaultAsync(r => r.RecordId == recordId);
    }

    public async Task SaveRecordAsync(GameRecord record)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.GameRecords
            .FirstOrDefaultAsync(r => r.RecordId == record.RecordId);

        if (existing != null)
        {
            context.Entry(existing).CurrentValues.SetValues(record);
        }
        else
        {
            context.GameRecords.Add(record);
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteRecordAsync(Guid recordId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var record = await context.GameRecords
            .FirstOrDefaultAsync(r => r.RecordId == recordId);

        if (record != null)
        {
            context.GameRecords.Remove(record);
            await context.SaveChangesAsync();
        }
    }

    public async Task<int> GetRecordCountAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GameRecords.CountAsync();
    }

    public async Task<Dictionary<string, int>> GetEndingStatsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GameRecords
            .Where(r => r.IsCompleted)
            .GroupBy(r => r.EndingType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Type, x => x.Count);
    }

    public async Task ToggleFavoriteAsync(Guid recordId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var record = await context.GameRecords
            .FirstOrDefaultAsync(r => r.RecordId == recordId);

        if (record != null)
        {
            record.IsFavorite = !record.IsFavorite;
            await context.SaveChangesAsync();
        }
    }
}
