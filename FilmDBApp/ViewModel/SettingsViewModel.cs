using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using FilmDBApp.Model;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FilmDBApp
{
    class SettingsViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ObservableCollection<Genre> _listOfGenres;
        private readonly ApplicationModel _model;

        #endregion


        #region Properties / Commands

        public ObservableCollection<Genre> ListOfGenres
        {
            get { return _listOfGenres; }
        }


        public ApplicationModel Model { get => _model;}

        public string Name
        {
            get => "Settings Page";
        }
        public ICommand AddNewGenreCommand { get; set; }
        public ICommand ChangeFilmsFolderFilePathCommand { get; set; }
        public ICommand ChangeSerialsFolderFilePathCommand { get; set; }
        public ICommand RemoveSelectedGenresCommand { get; set; }
        public ICommand SaveSettingsCommand { get; set; }

        #endregion

        public SettingsViewModel(ApplicationModel model)
        {
            _model = model;
            _listOfGenres = model.ListOfGenres;
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
                CommonOpenFileDialog dialog = new CommonOpenFileDialog
                {
                    InitialDirectory = _model.Config.GeneralFilmsFolder != null ? _model.Config.GeneralFilmsFolder.FullName : "C:\\Users",
                    IsFolderPicker = true,
                    Multiselect = true
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (string filename in dialog.FileNames.ToArray())
                        Model.CollectionOfGenres.AddNewGenre(new Genre(new FileInfo(filename)));
                }
                dialog.Dispose();

                Model.Config.GenresXmlUpdate(Model.CollectionOfGenres);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        private void ChangeFilmsFolderButton_Click(object obj)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && Directory.Exists(dialog.FileName))
            {
                Model.Config.ChangeFilmsFolder(new FileInfo(dialog.FileName));
            }
            dialog.Dispose();

        }

        private void ChangeSerialsFolderButton_Click(object obj)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && Directory.Exists(dialog.FileName))
            {
                Model.Config.ChangeSerialsFolder(new FileInfo(dialog.FileName));
            }
            dialog.Dispose();
        }

        private void RemoveSelectedGenresButton_Click(object obj)
        {

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Model.CollectionOfGenres.RemoveToBeDeletedFromGenreList();
                Model.Config.GenresXmlUpdate(Model.CollectionOfGenres);
            }            
        }

        private void SaveSettingsButton_Click(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}