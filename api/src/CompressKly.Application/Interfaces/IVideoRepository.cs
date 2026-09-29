using CompressKly.Domain.Entities;

namespace CompressKly.Application.Interfaces;

public interface IVideoRepository
{
    Task AddAsync(Video video, CancellationToken cancellationToken = default);

    Task<Video?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task UpdateAsync(Video video, CancellationToken cancellationToken = default);

    Task DeleteAsync(Video video, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
