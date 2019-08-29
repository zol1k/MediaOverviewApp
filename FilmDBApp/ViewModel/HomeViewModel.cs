using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using FilmDBApp.Helpers;
using FilmDBApp.Model;

namespace FilmDBApp
{
    internal class HomeViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private Genre _selectedGenre;
        private Genre _newGenreForSelectedFilm;
        private Film _selectedFilm;
        private AppSettings _settings;
        private ObservableCollection<Genre> _collectionOfGenres;
        private bool _fullListActive ;

        private string _filmNameEnToChangeTextBoxValue;
        private string _filmNameCzskToChangeTextBoxValue;
        private string _filmYearToChangeTextBoxValue;

        private ICommand _executeFilmRenameButtonCommand;
        private ICommand _executeFilmMoveButtonCommand;
        private ICommand _executeFilmDeleteButtonCommand;
        private ICommand _updateGenreListButtonCommand;

        private string _searchString;




        #endregion

        #region Properties / Commands

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                OnPropertyChanged("SearchString");
                SelectedGenreFilmListView.Refresh();
            }
        }

        public ObservableCollection<Genre> CollectionOfGenres
        {
            get { return _collectionOfGenres; }
            set
            {
                _collectionOfGenres = value;
                OnPropertyChanged("CollectionOfGenres");
            }
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
                    SelectedFilm = SelectedGenre.ListOfFilms.FirstOrDefault();
                    SelectedGenreFilmListView = CollectionViewSource.GetDefaultView(_selectedGenre.ListOfFilms);
                    _fullListActive = false;
                }
            }
        }
        public Genre NewGenreForSelectedFilm
        {
            get => _newGenreForSelectedFilm;
            set
            {
                if (value != null)
                {
                    _newGenreForSelectedFilm = value;
                    OnPropertyChanged("NewGenreForSelectedFilm");
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
                    value.RetrieveImdbInfo();
                    OnPropertyChanged("SelectedFilm");
                }
            }
        }

        private ICollectionView _selectedGenreFilmListView;
        public ICollectionView SelectedGenreFilmListView
        {
            set
            {
                _selectedGenreFilmListView = value;
                _selectedGenreFilmListView.Filter = x => Filter(x as Film);
                OnPropertyChanged("SelectedGenreFilmListView");
            }
            get { return _selectedGenreFilmListView; }
        }


        public string FilmNameEnToChangeTextBoxValue
        {
            get { return _filmNameEnToChangeTextBoxValue; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                this._filmNameEnToChangeTextBoxValue = value;
                this.OnPropertyChanged("FilmNameEnToChangeTextBoxValue"); 
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


        public ICommand ExecuteFilmRenameButtonCommand
        {
            get
            {
                if (_executeFilmRenameButtonCommand == null)
                    _executeFilmRenameButtonCommand = new RelayCommand(RenameFilmFileNameButton_Click);
                return _executeFilmRenameButtonCommand;
            }
        }

        public ICommand ExecuteFilmMoveButtonCommand
        {
            get
            {
                if (_executeFilmMoveButtonCommand == null)
                    _executeFilmMoveButtonCommand = new RelayCommand(MoveFilmFileButton_Click);
                return _executeFilmMoveButtonCommand;
            }
        }

        public ICommand ExecuteFilmDeleteButtonCommand
        {
            get
            {
                if (_executeFilmDeleteButtonCommand == null)
                    _executeFilmDeleteButtonCommand = new RelayCommand(DeleteFilmFileButton_Click);
                return _executeFilmDeleteButtonCommand;
            }
        }
        public ICommand UpdateGenreListButtonCommand
        {
            get
            {
                if (_updateGenreListButtonCommand == null)
                    _updateGenreListButtonCommand = new RelayCommand(UpdateGenresAndFilmsButton_Click);
                return _updateGenreListButtonCommand;
            }
        }

        private ICommand _showAllFilmsButtonCommand;
        public ICommand ShowAllFilmsButtonCommand
        {
            get
            {
                if (_showAllFilmsButtonCommand == null)
                    _showAllFilmsButtonCommand = new RelayCommand(ShowAllFilmsButton_Click);
                return _showAllFilmsButtonCommand;
            }
        }

        #endregion


        public HomeViewModel()
        {
            _settings = new AppSettings();
            
            _collectionOfGenres = _settings.ListOfGenres;
            ActionSet.CollectGenreFilms(_collectionOfGenres);
            
            SelectedGenre = CollectionOfGenres.FirstOrDefault();
            _fullListActive = false;

            SelectedFilm = SelectedGenre.ListOfFilms.FirstOrDefault();

            _selectedGenreFilmListView = CollectionViewSource.GetDefaultView(SelectedGenre.ListOfFilms);
            _selectedGenreFilmListView.Filter = x => Filter(x as Film);
        }

        private bool Filter(Film film)
        {
            var searchstring = (SearchString ?? string.Empty).ToLower();

            return film != null &&
                   ((film.FileName ?? string.Empty).ToLower().Contains(searchstring) ||
                    (film.FilmNameEn ?? string.Empty).ToLower().Contains(searchstring) ||
                    (film.FilmNameCzsk ?? string.Empty).ToLower().Contains(searchstring));
        }

        #region Methods
        private void RenameFilmFileNameButton_Click(object obj)
        {
            SelectedFilm.ChangeFileName(FilmNameEnToChangeTextBoxValue,FilmNameCzskToChangeTextBoxValue,FilmYearToChangeTextBoxValue);
        }

        private void MoveFilmFileButton_Click(object obj)
        {
            SelectedFilm.ChangeFileName(FilmNameEnToChangeTextBoxValue,FilmNameCzskToChangeTextBoxValue,FilmYearToChangeTextBoxValue);
            ActionSet.ChangeFilmGenre(SelectedFilm, SelectedGenre, NewGenreForSelectedFilm);

        }
        private void DeleteFilmFileButton_Click(object film)
        {
            Film filmToDelete = (Film)film;
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete "+ filmToDelete.FileName.ToUpper() + " film?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            SelectedGenre.ListOfFilms.Remove(filmToDelete);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string pathToDelete = filmToDelete.FilmFileInfo.FullName;
                if (filmToDelete.IsDirectory)
                {
                    ActionSet.DeleteDirectory(pathToDelete);
                }
                else
                {
                    File.SetAttributes(pathToDelete, FileAttributes.Normal);
                    File.Delete(pathToDelete);
                }

                if (_fullListActive == true)
                {
                    SelectedGenreFilmListView = CollectionViewSource.GetDefaultView(CollectAllGenreFilms());
                    _fullListActive = true;
                }
            }

        }

        private void UpdateGenresAndFilmsButton_Click(object obj)
        {
            _settings.GetGenresFromConfigFile();
            CollectionOfGenres = _settings.ListOfGenres;
            ActionSet.CollectGenreFilms(CollectionOfGenres);
        }

        private void ShowAllFilmsButton_Click(object obj)
        {
            SelectedGenreFilmListView = CollectionViewSource.GetDefaultView(CollectAllGenreFilms());
            _fullListActive = true;
        }

        private ObservableCollection<Film> CollectAllGenreFilms()
        {
            ObservableCollection<Film> collectedFilms = new ObservableCollection<Film>();
            foreach (var genre in CollectionOfGenres)
            {
                foreach (var film in genre.ListOfFilms)
                {
                    collectedFilms.Add(film);
                }
            }

            collectedFilms.Sort();
            return collectedFilms;
        }

        #endregion

    }
}