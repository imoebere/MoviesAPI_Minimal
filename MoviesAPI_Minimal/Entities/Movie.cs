namespace MoviesAPI_Minimal.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public bool InTheater { get; set; }
        public string? Poster { get; set; }
    }
}
