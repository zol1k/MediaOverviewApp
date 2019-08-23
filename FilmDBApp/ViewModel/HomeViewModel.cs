using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using WpfApp1.Model;

namespace WpfApp1
{
    internal class HomeViewModel : ObservableObject, IPageViewModel
    {
        private Genre _selectedGenre;
        private Film _selectedFilm;
        private ApplicationDataList _applicationDataList;
        private ObservableCollection<Genre> _listOfGenres;
        public ObservableCollection<Genre> ListOfGenres
        {
            get { return _listOfGenres; }
        }
        public string Name
        {
            get
            {
                return "HomePage";
            }
        }

        
        public Genre SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (value != null)
                {
                    _selectedGenre = value;
                    OnPropertyChanged("SelectedGenre");
                }
            }
        }

        public Film SelectedFilm
        {
            get => _selectedFilm;
            set
            {
                if (value != null)
                {
                    _selectedFilm = value;
                    // value.RetrieveImdbInfo();
                    OnPropertyChanged("SelectedFilm");
                }
            }
        }




        public HomeViewModel()
        {
            _applicationDataList = new ApplicationDataList();
            _listOfGenres = _applicationDataList.ListOfGenres;
        }
    }
}