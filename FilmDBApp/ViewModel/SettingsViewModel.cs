using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FilmDBApp.Model;

namespace FilmDBApp
{
    class SettingsViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private ObservableCollection<Genre> _listOfGenres;
        private ICommand _addNewGenreCommand;
        private ICommand _removeSelectedGenresCommand;
        private ICommand _changeFilmsFolderFilePathCommand;
        private ICommand _changeSerialsFolderFilePathCommand;
        private ICommand _saveSettingsCommand;
        private ApplicationModel _settings;

        #endregion


        #region Properties / Commands

        public ObservableCollection<Genre> ListOfGenres
        {
            get { return _listOfGenres; }
        }

        public ApplicationModel Settings { get => _settings;}

        public string Name
        {
            get => "Settings Page";
        }
        public ICommand AddNewGenreCommand
        {
            get => _addNewGenreCommand;
            set => _addNewGenreCommand = value;
        }
        public ICommand ChangeFilmsFolderFilePathCommand
        {
            get => _changeFilmsFolderFilePathCommand;
            set => _changeFilmsFolderFilePathCommand = value;
        }

        public ICommand ChangeSerialsFolderFilePathCommand
        {
            get => _changeSerialsFolderFilePathCommand;
            set => _changeSerialsFolderFilePathCommand = value;
        }


        public ICommand RemoveSelectedGenresCommand
        {
            get => _removeSelectedGenresCommand;
            set => _removeSelectedGenresCommand = value;
        }

        public ICommand SaveSettingsCommand
        {
            get => _saveSettingsCommand;
            set => _saveSettingsCommand = value;
        }

        #endregion

        public SettingsViewModel(ApplicationModel settings)
        {
            _settings = settings;
            _listOfGenres = settings.ListOfGenres;
            AddNewGenreCommand = new RelayCommand(AddNewGenreButton_Click);
            ChangeFilmsFolderFilePathCommand = new RelayCommand(ChangeFilmsFolderButton_Click);
            ChangeSerialsFolderFilePathCommand = new RelayCommand(ChangeSerialsFolderButton_Click);
            RemoveSelectedGenresCommand = new RelayCommand(RemoveSelectedGenresButton_Click);
            SaveSettingsCommand = new RelayCommand(SaveSettingsButton_Click);
        }





        #region Methods 

        private void AddNewGenreButton_Click(object obj)
        {

            try
            {
                _settings.AddNewGenre();
                _listOfGenres = _settings.ListOfGenres;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ChangeFilmsFolderButton_Click(object obj)
        {
            Settings.AddPathToFilmsFolder();
        }

        private void ChangeSerialsFolderButton_Click(object obj)
        {
            _settings.AddPathToSerialsFolder();
        }

        private void RemoveSelectedGenresButton_Click(object obj)
        {
            _settings.CollectionOfGenres.RemoveToBeDeletedFromGenreList();
        }

        private void SaveSettingsButton_Click(object obj)
        {
            _settings.SaveSettings();
        }

        #endregion

    }
}