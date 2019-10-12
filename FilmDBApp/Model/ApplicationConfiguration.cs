using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FilmDBApp.Model
{
    class ApplicationConfiguration : ObservableObject
    {
        #region Fields

        private readonly XDocument XDoc;
        private readonly XElement XGenresNode;
        private static readonly string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "Settings\\Settings.xml";

        private FileInfo _generalFilmsFolder;
        private FileInfo _generalSerialsFolder;

        public static string ImdbURL { get => @"http://www.omdbapi.com/?t="; }
        public static string ImdbURLApi { get => "&apikey=9757f013"; }
        public static string ImdbURLYear { get => "&y="; }

        #endregion


        #region Properties / Commands
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

        public List<string> GenrePaths { get => GetGenrePathsFromConfigFile(); }
        #endregion
        public ApplicationConfiguration()
        {
            XDoc = XDocument.Load(settingsFilePath);
            XGenresNode = XDoc.Root.Element("settings").Element("FilmsSettings").Element("Genres");
            GetFilmAndSerialFileInfoFromConfigFile();
        }

        /// <summary>
        /// Get FILM / SERIAL folder path from configuration file 
        /// </summary>
        private void GetFilmAndSerialFileInfoFromConfigFile()
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


        /// <summary>
        /// Parse config file and return List<string> with genre paths
        /// </summary>
        public List<string> GetGenrePathsFromConfigFile()
        {
            List<string> configGenrePathsList = new List<string>();
            string genrePath;
            foreach (XElement el in XDoc.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
            {
                genrePath = el.Attribute("PathToGenreFolder").Value;
                configGenrePathsList.Add(genrePath);
            }
            return configGenrePathsList;
        }

        public void ChangeFilmsFolder(FileInfo folder)
        {
            GeneralFilmsFolder = folder;
            FilmFolderXmlUpdate();
        }

        public void ChangeSerialsFolder(FileInfo folder)
        {
            GeneralSerialsFolder = folder;
            SerialsFolderXmlUpdate();
        }


        /// <summary>
        /// Update Genre data in XML document. Remove all existing ones, and fill them with new data
        /// </summary>
        public void GenresXmlUpdate(CollectionOfGenres collectionOfGenres)
        {
            XGenresNode.RemoveAll();
            foreach (var genre in collectionOfGenres.GenreList)
            {
                XGenresNode.Add(
                    new XElement("Genre",
                    new XAttribute("GenreName", genre.GenreName),
                    new XAttribute("PathToGenreFolder", genre.PathToGenreDirectory)
                    )
                );
            }
            SaveSettings();
        }


        /*
         * Update values of main node folder paths
         */
        private void FilmFolderXmlUpdate()
        {
            XAttribute xmlFilmsFolderPath = XDoc.Root.Element("settings").Element("FilmsSettings")
                .Attribute("PathToFolder");
            // If GeneralFilmsFolder is Null => ""
            xmlFilmsFolderPath.Value = (GeneralFilmsFolder == null) ? "" : GeneralFilmsFolder.FullName;
            SaveSettings();
        }

        private void SerialsFolderXmlUpdate()
        {
            XAttribute xmlSerialsFolderPath = XDoc.Root.Element("settings").Element("SerialsSettings")
                .Attribute("PathToFolder");
            // If GeneralSerialsFolder is Null => ""
            xmlSerialsFolderPath.Value = (GeneralSerialsFolder == null) ? "" : GeneralSerialsFolder.FullName;
            SaveSettings();
        }

        /*
         * Save genre, folder paths informations
         */

        private void SaveSettings()
        {
            XDoc.Save(settingsFilePath);
        }

    }
}
