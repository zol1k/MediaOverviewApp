using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FilmDBApp.Model;
using WpfApp1.Model;

namespace WpfApp1
{
    internal class HomeViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private Genre _selectedGenre;
        private Genre _newGenreForSelectedFilm;
        private Film _selectedFilm;
        private AppSettings _settings;
        private CollectionOfGenres _collectionOfGenres;

        private string _filmNameEnToChangeTextBoxValue;
        private string _filmNameCzskToChangeTextBoxValue;
        private string _filmYearToChangeTextBoxValue;

        private ICommand _executeFilmRenameButtonCommand;
        private ICommand _executeFilmMoveButtonCommand;
        private ICommand _executeFilmDeleteButtonCommand;

        #endregion

        #region Properties / Commands

        public ObservableCollection<Genre> CollectionOfGenres
        {
            get { return _collectionOfGenres.GenreList; }
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

        #endregion


        public HomeViewModel()
        {
            _settings = new AppSettings();

            _collectionOfGenres = _settings.CollectionOfGenres;
            _collectionOfGenres.AddNewGenre(new Genre(new FileInfo(_settings.GeneralFilmsFolder.FullName)));

            ActionSet.CollectGenreFilms(_collectionOfGenres);
            

            SelectedGenre = CollectionOfGenres.FirstOrDefault();
            SelectedFilm = SelectedGenre.ListOfFilms.FirstOrDefault();

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
        private void DeleteFilmFileButton_Click(object obj)
        {

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete "+ SelectedFilm.FileName.ToUpper() + " film?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            SelectedGenre.ListOfFilms.Remove(SelectedFilm);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                File.SetAttributes(SelectedFilm.FilmFileInfo.FullName, FileAttributes.Normal);
                File.Delete(SelectedFilm.FilmFileInfo.FullName);
            }

        }
        #endregion

    }
}