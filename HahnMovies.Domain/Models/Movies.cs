namespace HahnMovies.Domain.Models;

public class Movies
{
 
 public int Id { get; set; }
 public string Title { get; set; }
 
 public DateTime ReleaseDate { get; set; }
 
 public decimal VoteAverage { get; set; }
 
 public string PosterPath { get; set; }
 
 public Movies(string title, DateTime releaseDate, decimal voteAverage, string posterPath)
 {
  Title = title;
  ReleaseDate = releaseDate;
  VoteAverage = voteAverage;
  PosterPath = posterPath;
 }
    
}