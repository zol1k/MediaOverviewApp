using System.Collections.ObjectModel;

namespace MediaOverviewApp.Model
{
    interface IFilmCollection
    {
        string PathToDirectory { get; }
        string Name { get; }
        ObservableCollection<Film> ListOfFilms{get;}
    }
}