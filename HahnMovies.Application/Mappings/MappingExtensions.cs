using HahnMovies.Application.Dtos;
using HahnMovies.Domain.Models;

namespace HahnMovies.Application.Mappings
{
    public static class MappingExtensions
    {
        public static Movies ToEntity(this TmdbMovieDto dto)
        {
            return new Movies(
                title: dto.Title,
                releaseDate: dto.ReleaseDate,
                voteAverage: dto.VoteAverage,
                posterPath: dto.PosterPath ?? string.Empty
            );
        }

        public static MovieDto ToDto(this Movies movie)
        {
            return new MovieDto
            {
                Title = movie.Title,
                ReleaseDate = movie.ReleaseDate,
                VoteAverage = movie.VoteAverage,
                PosterPath = movie.PosterPath
            };
        }
    }
}