using System.IO.Compression;
using System.Text.Json;
using HahnMovies.Application.Dtos;
using HahnMovies.Application.Interfaces;
using HahnMovies.Application.Mappings;
using EFCore.BulkExtensions;
using HahnMovies.Domain.Models;
using HahnMovies.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HahnMovies.Infrastructure.Services;

public class MovieService : IMovieService
{
    private readonly ITmdbService _tmdbService;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IMoviesRepository _moviesRepository;

    public MovieService(ITmdbService tmdbService, AppDbContext dbContext, IConfiguration configuration, IMoviesRepository moviesRepository)
    {
        _tmdbService = tmdbService;
        _dbContext = dbContext;
        _configuration = configuration;
        _moviesRepository = moviesRepository;
    }

    public async Task SavePopularMoviesAsync(int page, CancellationToken cancellationToken = default)
    {
        var response = await _tmdbService.GetPopularMoviesAsync(page, cancellationToken);

        if (response?.Results == null || !response.Results.Any())
            return;

        // Map DTOs to entity models
        var movies = response.Results.Select(movieDto => movieDto.ToEntity()).ToList();

        // Check for duplicates before bulk inserting
        var existingMovieIds = await _dbContext.Movies
            .Where(m => movies.Select(dto => dto.Id).Contains(m.Id))
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        var newMovies = movies.Where(m => !existingMovieIds.Contains(m.Id)).ToList();

        if (newMovies.Any())
        {
            await _dbContext.BulkInsertOrUpdateAsync(newMovies, cancellationToken: cancellationToken);
        }
    }

    public async Task<IEnumerable<MovieDto>> GetMoviesAsync(CancellationToken cancellationToken = default)
    {
        var movies = await _dbContext.Movies.ToListAsync(cancellationToken);
        return movies.Select(m => m.ToDto());
    }

    public async Task FullSyncFromDailyExportAsync(CancellationToken cancellationToken = default)
    {
        var dailyExportUrl = _configuration["TMDB:DailyExportUrl"];

        var date = DateTime.UtcNow.AddDays(-1).ToString("MM_dd_yyyy");
       // var response = await _httpClient.GetAsync($"http://files.tmdb.org/p/exports/movie_ids_{date}.json.gz");
        using var response = await new HttpClient().GetAsync($"http://files.tmdb.org/p/exports/movie_ids_{date}.json.gz", HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to download export file. Status code: {response.StatusCode}");
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);
        using var reader = new StreamReader(decompressedStream);

        var moviesToInsert = new List<Movies>();
        string? line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            var movieIdObject = JsonSerializer.Deserialize<Dictionary<string, object>>(line);

            if (movieIdObject != null && movieIdObject.TryGetValue("id", out var movieIdValue) && int.TryParse(movieIdValue?.ToString(), out var movieId))
            {
                var movieDetails = await _tmdbService.GetMovieDetailsAsync(movieId, cancellationToken);

                if (movieDetails != null)
                {
                    var movieEntity = movieDetails.ToEntity();

                    if (!await _dbContext.Movies.AnyAsync(m => m.Id == movieEntity.Id, cancellationToken))
                    {
                        moviesToInsert.Add(movieEntity);
                    }

                    if (moviesToInsert.Count >= 100)
                    {
                        await _dbContext.BulkInsertAsync(moviesToInsert, cancellationToken: cancellationToken);
                        moviesToInsert.Clear();
                    }
                }
            }
        }

        if (moviesToInsert.Any())
        {
            await _dbContext.BulkInsertAsync(moviesToInsert, cancellationToken: cancellationToken);
        }
    }
    public async Task PartialSyncFromChangesAsync(CancellationToken cancellationToken = default)
    {
        int page = 1;
        bool hasMorePages;

        do
        {
            // Fetch movie changes from the TMDB API
            var changesResponse = await _tmdbService.GetMovieChangesAsync(page, cancellationToken);

            if (changesResponse?.Results == null || !changesResponse.Results.Any())
                break;

            var updatedMovies = new List<Movies>();

            foreach (var change in changesResponse.Results)
            {
                // Fetch the movie details using its ID
                var movieDetails = await _tmdbService.GetMovieDetailsAsync(change.Id, cancellationToken);

                if (movieDetails != null)
                {
                    var movieEntity = movieDetails.ToEntity();
                    updatedMovies.Add(movieEntity);
                }
            }

            // Bulk upsert updated movies into the database
            if (updatedMovies.Any())
            {
                await _dbContext.BulkInsertOrUpdateAsync(updatedMovies, cancellationToken: cancellationToken);
            }

            page++;
            hasMorePages = changesResponse.TotalPages >= page;

        } while (hasMorePages);
    }
    public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string title, CancellationToken cancellationToken = default)
    {
        var movies = await _moviesRepository.SearchMoviesByTitleAsync(title, cancellationToken);
        return movies.Select(m => m.ToDto());
    }
}
