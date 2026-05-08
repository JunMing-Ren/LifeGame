using Microsoft.EntityFrameworkCore;
using LifeGame.Core.Data;
using LifeGame.Core.Models;

namespace LifeGame.Desktop.Services;

public class ArchiveService
{
    private readonly Func<LifeGameDbContext> _contextFactory;

    public ArchiveService()
    {
        _contextFactory = () => new LifeGameDbContext();
    }

    public async Task<List<GameRecord>> GetAllRecordsAsync()
    {
        using var context = _contextFactory();
        return await context.GameRecords
            .OrderByDescending(r => r.CreatedTime)
            .ToListAsync();
    }

    public async Task<List<GameRecord>> GetCompletedRecordsAsync()
    {
        using var context = _contextFactory();
        return await context.GameRecords
            .Where(r => r.IsCompleted)
            .OrderByDescending(r => r.CreatedTime)
            .ToListAsync();
    }

    public async Task<GameRecord?> GetRecordByIdAsync(Guid recordId)
    {
        using var context = _contextFactory();
        return await context.GameRecords
            .FirstOrDefaultAsync(r => r.RecordId == recordId);
    }

    public async Task SaveRecordAsync(GameRecord record)
    {
        using var context = _contextFactory();
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
        using var context = _contextFactory();
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
        using var context = _contextFactory();
        return await context.GameRecords.CountAsync();
    }

    public async Task<Dictionary<string, int>> GetEndingStatsAsync()
    {
        using var context = _contextFactory();
        return await context.GameRecords
            .Where(r => r.IsCompleted)
            .GroupBy(r => r.EndingType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Type, x => x.Count);
    }

    public async Task ToggleFavoriteAsync(Guid recordId)
    {
        using var context = _contextFactory();
        var record = await context.GameRecords
            .FirstOrDefaultAsync(r => r.RecordId == recordId);

        if (record != null)
        {
            record.IsFavorite = !record.IsFavorite;
            await context.SaveChangesAsync();
        }
    }
}
