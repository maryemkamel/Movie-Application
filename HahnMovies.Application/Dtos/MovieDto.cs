namespace HahnMovies.Application.Dtos

{
    public class MovieDto
    {
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal VoteAverage { get; set; }
        public string PosterPath { get; set; }
    }
}
