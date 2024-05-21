public class HomeViewModel
{
    public List<Film> Films { get; set; }
    public List<Review> Reviews { get; set; }
    public List<Genre> Genres{ get; set; }
    public List<Director> Directors{ get; set;}
    public List<Actor> Actors{ get; set; }
    public List<FilmActor> FilmActors{ get; set; }
    public string SortOrder { get; set; }
    public string SearchString { get; set; }
    public string SelectedGenre { get; set; }
}