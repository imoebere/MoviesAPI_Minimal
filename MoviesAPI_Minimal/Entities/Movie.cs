﻿namespace MoviesAPI_Minimal.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public bool InTheaters { get; set; }
        public string? Poster { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<GenreMovie> GenresMovies { get; set; } = new List<GenreMovie>();
        public List<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();

    }
}
