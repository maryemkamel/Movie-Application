
using HahnMovies.Application.Dtos;

namespace HahnMovies.Application.Interfaces
{
    public interface IMovieService
    {
        Task SavePopularMoviesAsync(int page, CancellationToken cancellationToken = default);
        Task<IEnumerable<MovieDto>> GetMoviesAsync(CancellationToken cancellationToken = default);
        Task FullSyncFromDailyExportAsync(CancellationToken cancellationToken = default);
        Task PartialSyncFromChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<MovieDto>> SearchMoviesAsync(string title, CancellationToken cancellationToken = default);
    }
}