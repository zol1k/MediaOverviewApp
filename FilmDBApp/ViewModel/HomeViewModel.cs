using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FilmDBApp.Model;
using WpfApp1.Model;

namespace WpfApp1
{
    internal class HomeViewModel : ObservableObject, IPageViewModel
    {
        private Genre _selectedGenre;
        private Genre _genreToMoveSelectedFilm;
        private Film _selectedFilm;
        private ApplicationDataList _applicationDataList;
        private ObservableCollection<Genre> _listOfGenres;
        public ObservableCollection<Genre> ListOfGenres
        {
            get { return _listOfGenres; }
        }

        public string Name
        {
            get { return "HomePage"; }
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
        public Genre GenreToMoveSelectedFilm
        {
            get => _genreToMoveSelectedFilm;
            set
            {
                if (value != null)
                {
                    _genreToMoveSelectedFilm = value;
                    OnPropertyChanged("GenreToMoveSelectedFilm");
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
                    FilmNameEnToChangeTextBoxValue = value.FilmNameEn;
                    FilmNameCzskToChangeTextBoxValue = value.FilmNameCzsk;
                    FilmYearToChangeTextBoxValue = value.FilmYear;
                    // value.RetrieveImdbInfo();
                    OnPropertyChanged("SelectedFilm");
                }
            }
        }

        private string _filmNameToChangeTextBoxValue;
        private string _filmNameEnToChangeTextBoxValue;
        private string _filmNameCzskToChangeTextBoxValue;
        private string _filmYearToChangeTextBoxValue;

        public string FilmNameEnToChangeTextBoxValue
        {
            get { return _filmNameEnToChangeTextBoxValue; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                this._filmNameEnToChangeTextBoxValue = value;
                this.OnPropertyChanged(
                    "FilmNameEnToChangeTextBoxValue"); // Method to raise the PropertyChanged event in your BaseViewModel class...
            }
        }

        public string FilmNameCzskToChangeTextBoxValue
        {
            get { return _filmNameCzskToChangeTextBoxValue; }
            set
            {
                this._filmNameCzskToChangeTextBoxValue = value;
                this.OnPropertyChanged("FilmNameCzskToChangeTextBoxValue");
            }
        }

        public string FilmYearToChangeTextBoxValue
        {
            get { return _filmYearToChangeTextBoxValue; }
            set
            {
                this._filmYearToChangeTextBoxValue = value;
                this.OnPropertyChanged("FilmYearToChangeTextBoxValue");
            }
        }

        private ICommand _executeFilmRenameButtonCommand;
        public ICommand ExecuteFilmRenameButtonCommand
        {
            get
            {
                if (_executeFilmRenameButtonCommand == null)
                    _executeFilmRenameButtonCommand = new RelayCommand(RenameFilmFileNameButton_Click);
                return _executeFilmRenameButtonCommand;
            }
        }

        private ICommand _executeFilmMoveButtonCommand;
        public ICommand ExecuteFilmMoveButtonCommand
        {
            get
            {
                if (_executeFilmMoveButtonCommand == null)
                    _executeFilmMoveButtonCommand = new RelayCommand(MoveFilmFileButton_Click);
                return _executeFilmMoveButtonCommand;
            }
        }




        public HomeViewModel()
        {
            _applicationDataList = new ApplicationDataList();
            _listOfGenres = _applicationDataList.ListOfGenres;
        }

        private void RenameFilmFileNameButton_Click(object obj)
        {
            SelectedFilm.ChangeFileName(FilmNameEnToChangeTextBoxValue,FilmNameCzskToChangeTextBoxValue,FilmYearToChangeTextBoxValue);
        }
        private void MoveFilmFileButton_Click(object obj)
        {
            SelectedFilm.ChangeFileName(FilmNameEnToChangeTextBoxValue,FilmNameCzskToChangeTextBoxValue,FilmYearToChangeTextBoxValue);
            ActionSet.ChangeFilmGenre(SelectedFilm, SelectedGenre, GenreToMoveSelectedFilm);

        }
    }
}