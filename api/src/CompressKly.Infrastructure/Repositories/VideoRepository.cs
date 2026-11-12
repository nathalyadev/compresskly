using CompressKly.Application.Interfaces;
using CompressKly.Core.Entities;
using CompressKly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompressKly.Infrastructure.Repositories;

public sealed class VideoRepository : IVideoRepository
{
    private readonly CompressKlyDbContext _dbContext;

    public VideoRepository(CompressKlyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Video video, CancellationToken cancellationToken = default)
    {
        await _dbContext.Videos.AddAsync(video, cancellationToken);
    }

    public Task<Video?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Videos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task UpdateAsync(Video video, CancellationToken cancellationToken = default)
    {
        _dbContext.Videos.Update(video);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Video video, CancellationToken cancellationToken = default)
    {
        _dbContext.Videos.Remove(video);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
