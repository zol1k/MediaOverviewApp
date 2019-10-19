using System.Collections.ObjectModel;

namespace FilmDBApp.Model
{
    interface IFilmCollection
    {
        string PathToDirectory { get; }
        string Name { get; }
        ObservableCollection<Film> ListOfFilms{get;}
    }
}