using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MediaOverviewApp.Model;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MediaOverviewApp
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

        public ICommand AddNewGenreCommand { get; set; }
        public ICommand ChangeFilmsFolderFilePathCommand { get; set; }
        public ICommand ChangeSerialsFolderFilePathCommand { get; set; }
        public ICommand RemoveSelectedGenresCommand { get; set; }
        #endregion

        public SettingsViewModel(ApplicationModel model)
        {
            _model = model;
            _listOfGenres = model.ListOfGenres;
            AddNewGenreCommand = new RelayCommand(AddNewGenreButton_Click);
            ChangeFilmsFolderFilePathCommand = new RelayCommand(ChangeFilmsFolderButton_Click);
            ChangeSerialsFolderFilePathCommand = new RelayCommand(ChangeSerialsFolderButton_Click);
            RemoveSelectedGenresCommand = new RelayCommand(RemoveSelectedGenresButton_Click);
        }

        #region Methods 

        private void AddNewGenreButton_Click(object obj)
        {
            try
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog
                {
                    InitialDirectory = ApplicationConfiguration.GeneralFilmFolder != null ? ApplicationConfiguration.GeneralFilmFolder.FullName : ApplicationConfiguration.rootApp,
                    IsFolderPicker = true,
                    Multiselect = true
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {

                    if (dialog.FileNames.Count() != 0)
                    {
                        List<string> alreadyAdded = new List<string>();
                        foreach (string folderPath in dialog.FileNames.ToArray())
                        {
                            if (Model.CollectionOfGenres.IsInList(folderPath))
                            {
                                alreadyAdded.Add(folderPath);
                            }
                            else
                            {
                                Model.CollectionOfGenres.AddNewGenre(new Genre(new FileInfo(folderPath)));
                            }
                        }

                        if (alreadyAdded.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Bellow genre(s) are already added.");
                            sb.AppendLine();
                            sb.AppendLine();
                            sb.Append(String.Join(Environment.NewLine, alreadyAdded.ToArray()));
                            sb.AppendLine();
                            sb.AppendLine();
                            MessageBox.Show(sb.ToString(), "Genres already in list.");
                        }

                        XController.UpdateGenres(Model.CollectionOfGenres);
                        Model.GeneralFilmFolder.CollectFilms();
                    }
                }
                dialog.Dispose();
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
                InitialDirectory = ApplicationConfiguration.rootApp,
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && Directory.Exists(dialog.FileName))
            {
                Model.UpdateGeneralFilmFolder(new FileInfo(dialog.FileName));
                Model.GeneralFilmFolder.CollectFilms();
            }
            dialog.Dispose();
        }

        private void ChangeSerialsFolderButton_Click(object obj)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = ApplicationConfiguration.rootApp,
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && Directory.Exists(dialog.FileName))
            {
                Model.UpdateGeneralSerialFolder(new FileInfo(dialog.FileName));
            }
            dialog.Dispose();
        }

        private void RemoveSelectedGenresButton_Click(object obj)
        {
            List<Genre> toBeDeletedList = Model.CollectionOfGenres.GenresToBeDeleted;

            string stringOutputOfToBeDeletedGenres = String.Join("\n- ", toBeDeletedList.Select(o => o.Name).ToList());

            // If number of toBeDeleted items is more then 0, then proceed
            if (toBeDeletedList.Count != 0)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete following genres? \n- " + stringOutputOfToBeDeletedGenres, "Delete Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Model.CollectionOfGenres.RemoveGenreFromList(toBeDeletedList.ToList());
                    XController.UpdateGenres(Model.CollectionOfGenres);
                }
            }
        }
        #endregion

    }
}