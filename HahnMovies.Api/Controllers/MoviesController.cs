using HahnMovies.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HahnMovies.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        /*[HttpGet("popular")]
        public async Task<IActionResult> GetPopularMovies(int page = 0, CancellationToken cancellationToken = default)
        {
            await _movieService.SavePopularMoviesAsync(page, cancellationToken);
            return Ok($"Popular movies from page {page} saved successfully!");
        }
        */

        [HttpGet]
        public async Task<IActionResult> GetMovies(CancellationToken cancellationToken)
        {
            var movies = await _movieService.GetMoviesAsync(cancellationToken);
            return Ok(movies);
        }
        
        [HttpPost("sync/popular")]
        public async Task<IActionResult> SyncPopularMovies(CancellationToken cancellationToken)
        {
            await _movieService.SavePopularMoviesAsync(1, cancellationToken);
            return Ok("Popular movies synced successfully!");
        }

        [HttpPost("sync/full")]
        public async Task<IActionResult> SyncFullMovies(CancellationToken cancellationToken)
        {
            await _movieService.FullSyncFromDailyExportAsync(cancellationToken);
            return Ok("Full movie sync completed successfully!");
        }

    }
}