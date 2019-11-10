using System.Collections.ObjectModel;

namespace MediaOverviewApp.Model
{
    interface IMediaCollection
    {
        string PathToDirectory { get; }
        string Name { get; }
        ObservableCollection<Film> ListOfFilms{get;}
    }
}
