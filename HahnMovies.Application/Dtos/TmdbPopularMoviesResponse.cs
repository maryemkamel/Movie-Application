using System.Text.Json.Serialization;

namespace HahnMovies.Application.Dtos
{
    public class TmdbPopularMoviesResponse
    {
        
        public int Page { get; set; }
        public List<TmdbMovieDto> Results { get; set; } = new();
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
    }
    public class TmdbMovieIdDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
    public class TmdbMovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        
        [JsonPropertyName("release_date")]
        public DateTime ReleaseDate { get; set; } // Keep this as a string initially for proper deserialization

        [JsonPropertyName("vote_average")]
        public decimal VoteAverage { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }
    }
}