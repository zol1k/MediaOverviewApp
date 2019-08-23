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

namespace WpfApp1.Model
{
    class AppSettings : ObservableObject
    {
        #region Fields

        private static XDocument XDoc;
        private static XElement XGenresNode;
        private static string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "Settings\\Settings.xml";
        private FileInfo _generalFilmsFolder;
        private FileInfo _generalSerialsFolder;
        private string _generalFilmsFolderPath;

        #endregion

        #region Properties / Commands

        public GenreCollection GenreCollection { get; set; }

        public ObservableCollection<Genre> ListOfGenres
        {
            get => GenreCollection.GenreList;
        }

        public FileInfo GeneralFilmsFolder
        {
            get => _generalFilmsFolder;
            set
            {
                _generalFilmsFolder = value;
                GeneralFilmsFolderPath = _generalFilmsFolder.FullName;
            }
        }
        public string GeneralFilmsFolderPath {
            get => _generalFilmsFolderPath;
            set {
                _generalFilmsFolderPath = value;
                OnPropertyChanged("GeneralFilmsFolderPath");
            }
        }

        public FileInfo GeneralSerialsFolder
        {
            get => _generalSerialsFolder;
            set
            {
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
            GenreCollection = new GenreCollection();

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
                _generalSerialsFolder = new FileInfo(serialsFolderPath);
            }
        }

        private void GetGenresFromConfigFile()
        {
            GenreCollection.ClearAll();
            string genrePath = "";
            foreach (XElement el in XDoc.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
            {
                genrePath = el.Attribute("PathToGenreFolder").Value;
                GenreCollection.AddNewGenre(new Genre(new FileInfo(genrePath)));
            }
        }

        public void AddNewGenre()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = _generalFilmsFolder != null ? _generalFilmsFolder.FullName : "C:\\Users";
            dialog.IsFolderPicker = true;
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                foreach (string filename in dialog.FileNames.ToArray())
                    GenreCollection.AddNewGenre(new Genre(new FileInfo(filename)));
            }

            WriteGenresIntoXMLDocument();
        }

        public void AddPathToFilmsFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                if (Directory.Exists(dialog.FileName))
                    GeneralFilmsFolder = new FileInfo(dialog.FileName);
            }
        }
        public void AddPathToSerialsFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                if (File.Exists(dialog.FileName))
                    _generalSerialsFolder = new FileInfo(dialog.FileName);
            }
        }

        public void WriteGenresIntoXMLDocument()
        {
            XGenresNode.RemoveAll();

            foreach (var genre in GenreCollection.GenreList)
            {
                XGenresNode.Add(new XElement("Genre",
                    new XAttribute("GenreName", genre.GenreName),
                    new XAttribute("PathToGenreFolder", genre.PathToGenreDirectory)));
            }
            XDoc.Save(settingsFilePath);
        }



    }
}
