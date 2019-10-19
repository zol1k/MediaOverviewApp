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
        private readonly ApplicationModel _model;
        private bool _fullListActive ;
        private string _filmNameEnToChangeTextBoxValue;
        private string _filmNameCzskToChangeTextBoxValue;
        private string _filmYearToChangeTextBoxValue;
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
            get { return _model.ListOfGenres; }
            set
            {
                CollectionOfGenres = value;
                OnPropertyChanged("CollectionOfGenres");
            }
        }

        public string Name
        {
            get => "Name";
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
        public ApplicationModel Model { get => _model; }

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


        public ICommand ExecuteFilmRenameButtonCommand { get; }
        public ICommand ExecuteFilmMoveButtonCommand { get; }
        public ICommand ExecuteFilmDeleteButtonCommand { get; }
        public ICommand ShowAllFilmsButtonCommand { get; }
        public ICommand ShowFilmFolderFilmsButtonCommand { get; }

        #endregion


        public HomeViewModel(ApplicationModel model)
        {
            _model = model;
            
            SelectedGenre = CollectionOfGenres.FirstOrDefault();
            _fullListActive = false;
            if (SelectedGenre != null)
            {
                SelectedFilm = SelectedGenre.ListOfFilms.FirstOrDefault();
                _selectedGenreFilmListView = CollectionViewSource.GetDefaultView(SelectedGenre.ListOfFilms);
                _selectedGenreFilmListView.Filter = x => Filter(x as Film);
            }


            ShowAllFilmsButtonCommand = new RelayCommand(ShowAllFilmsButton_Click);
            ShowFilmFolderFilmsButtonCommand = new RelayCommand(ShowFilmFolderFilmsButton_Click);
            ExecuteFilmDeleteButtonCommand = new RelayCommand(DeleteFilmFileButton_Click);
            ExecuteFilmMoveButtonCommand = new RelayCommand(MoveFilmFileButton_Click);
            ExecuteFilmRenameButtonCommand = new RelayCommand(RenameFilmFileNameButton_Click);
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
            Model.ChangeFilmGenre(SelectedFilm, SelectedGenre, NewGenreForSelectedFilm);

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

                if (_fullListActive)
                {
                    SelectedGenreFilmListView = CollectionViewSource.GetDefaultView(Model.CollectionOfAllFilms);
                    _fullListActive = true;
                }
            }

        }
        private void ShowAllFilmsButton_Click(object obj)
        {
            SelectedGenreFilmListView = CollectionViewSource.GetDefaultView(Model.CollectionOfAllFilms);
            _fullListActive = true;
        }
        private void ShowFilmFolderFilmsButton_Click(object obj)
        {
            SelectedGenreFilmListView = SelectedGenreFilmListView = CollectionViewSource.GetDefaultView(Model.GeneralFilmFolder.ListOfFilms);
            _fullListActive = false;
        }

        #endregion

    }
}