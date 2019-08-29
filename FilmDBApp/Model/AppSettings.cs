
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FilmDBApp.Model
{
    class AppSettings : ObservableObject
    {
        #region Fields

        private static XDocument XDoc;
        private static XElement XGenresNode;
        private static string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "Settings\\Settings.xml";
        private CollectionOfGenres _collectionOfGenres;
        private FileInfo _generalFilmsFolder;
        private FileInfo _generalSerialsFolder;

        public static string ImdbURL = @"http://www.omdbapi.com/?t=";
        public static string ImdbURLApi = "&apikey=9757f013";
        public static string ImdbURLYear = "&y=";


        #endregion

        #region Properties / Commands

        public CollectionOfGenres CollectionOfGenres { get => _collectionOfGenres; set =>_collectionOfGenres = value; }

        public ObservableCollection<Genre> ListOfGenres
        {
            get => CollectionOfGenres.GenreList;
        }

        public FileInfo GeneralFilmsFolder
        {
            get => _generalFilmsFolder;
            set
            {
                _generalFilmsFolder = value;
                OnPropertyChanged("GeneralFilmsFolder");
            }
        }

        public FileInfo GeneralSerialsFolder
        {
            get => _generalSerialsFolder;
            set
            {
                _generalSerialsFolder = value;
                OnPropertyChanged("GeneralSerialsFolder");
            }
        }

        #endregion

        public AppSettings()
        {
            XDoc = new XDocument();
            //DefineStructureOfDocument();
            XDoc = XDocument.Load(settingsFilePath);
            XGenresNode = XDoc.Root.Element("settings").Element("FilmsSettings").Element("Genres");
            _collectionOfGenres = new CollectionOfGenres();

            // Retrieve folder path for 
            GetFilmAndSerialFileInfoFromConfigFile();
            
            // Parse config file and fill genre list
            GetGenresFromConfigFile();
        }




        public void GetFilmAndSerialFileInfoFromConfigFile()
        {
            string filmsFolderPath = XDoc.Root.Element("settings").Element("FilmsSettings")
                .Attribute("PathToFolder").Value;
            string serialsFolderPath = XDoc.Root.Element("settings").Element("SerialsSettings")
                .Attribute("PathToFolder").Value;

            if (Directory.Exists(filmsFolderPath))
            {
                GeneralFilmsFolder = new FileInfo(filmsFolderPath);
            }

            if (Directory.Exists(serialsFolderPath))
            {
                GeneralSerialsFolder = new FileInfo(serialsFolderPath);
            }
        }

        public void GetGenresFromConfigFile()
        {
            CollectionOfGenres.ClearAll();
            string genrePath = "";
            foreach (XElement el in XDoc.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
            {
                genrePath = el.Attribute("PathToGenreFolder").Value;
                CollectionOfGenres.AddNewGenre(new Genre(new FileInfo(genrePath)));
            }
        }

        public void AddNewGenre()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = _generalFilmsFolder != null ? _generalFilmsFolder.FullName : "C:\\Users",
                IsFolderPicker = true,
                Multiselect = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                foreach (string filename in dialog.FileNames.ToArray())
                    CollectionOfGenres.AddNewGenre(new Genre(new FileInfo(filename)));
            }


        }

        public void AddPathToFilmsFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                if (Directory.Exists(dialog.FileName))
                    GeneralFilmsFolder = new FileInfo(dialog.FileName);
            }
        }
        public void AddPathToSerialsFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                if (Directory.Exists(dialog.FileName))
                    GeneralSerialsFolder = new FileInfo(dialog.FileName);
            }
        }


        /*
         * Update Genre data in XML document. Remove all existing ones, and fill them with new data
         */
        private void UpdateGenresInXmlDocument()
        {

            XGenresNode.RemoveAll();

            //Loop through selected folders and save their <FolderName>,<FolderPath>
            foreach (var genre in CollectionOfGenres.GenreList)
            {
                XGenresNode.Add(
                    new XElement("Genre",
                    new XAttribute("GenreName", genre.GenreName),
                    new XAttribute("PathToGenreFolder", genre.PathToGenreDirectory)
                    )
                );
            }
           
        }

        /*
         * Update values of main node folder paths
         */
        private void UpdateGeneralFolderPathsInXmlDocument()
        {
            XAttribute xmlFilmsFolderPath = XDoc.Root.Element("settings").Element("FilmsSettings")
                .Attribute("PathToFolder");
            XAttribute xmlSerialsFolderPath = XDoc.Root.Element("settings").Element("SerialsSettings")
                .Attribute("PathToFolder");

            // If GeneralFilmsFolder is Null => ""
            xmlFilmsFolderPath.Value = (GeneralFilmsFolder == null) ? "" : GeneralFilmsFolder.FullName;
           
            // If GeneralSerialsFolder is Null => ""
            xmlSerialsFolderPath.Value = (GeneralSerialsFolder == null) ? "" : GeneralSerialsFolder.FullName;
        }

        /*
         * Save genre, folder paths informations
         */

        public void SaveSettings()
        {
            UpdateGeneralFolderPathsInXmlDocument();
            UpdateGenresInXmlDocument();
            XDoc.Save(settingsFilePath);
        }

        private bool IsInGenreCollection(Genre genre)
        {

            return true;
        }
    }
}
