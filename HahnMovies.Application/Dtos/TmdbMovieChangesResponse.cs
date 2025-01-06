using System.Text.Json.Serialization;

namespace HahnMovies.Application.Dtos;

public class TmdbMovieChangesResponse
{
    public List<TmdbMovieChangeDto> Results { get; set; } = new();
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
}

public class TmdbMovieChangeDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}