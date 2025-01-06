using System.IO.Compression;
using System.Text.Json;
using HahnMovies.Application.Dtos;
using HahnMovies.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HahnMovies.Infrastructure.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _dailyExportUrl;

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["TMDB:ApiKey"];
            _baseUrl = configuration["TMDB:BaseUrl"];
            _dailyExportUrl = configuration["TMDB:DailyExportUrl"];
        }

        public async Task<TmdbPopularMoviesResponse?> GetPopularMoviesAsync(int page = 2, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/movie/popular?page={page}&api_key={_apiKey}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<TmdbPopularMoviesResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<int>> GetMovieIdsFromDailyExportAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(_dailyExportUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var decompressedStream = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new StreamReader(decompressedStream);

            var movieIds = new List<int>();
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var movieDto = JsonSerializer.Deserialize<TmdbMovieIdDto>(line);
                if (movieDto != null)
                {
                    movieIds.Add(movieDto.Id);
                }
            }

            return movieIds;
        }

        public async Task<TmdbMovieDto?> GetMovieDetailsAsync(int movieId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/movie/{movieId}?api_key={_apiKey}", cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<TmdbMovieDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task<TmdbMovieChangesResponse?> GetMovieChangesAsync(int page = 1, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/movie/changes?page={page}&api_key={_apiKey}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<TmdbMovieChangesResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

    }
}
