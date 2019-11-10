using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MediaOverviewApp.Model
{
    static class XController
    {
        private static XDocument _xDocument;

        public static string GeneralFilmFolderPath
        {
            get => _xDocument.Root.Element("settings").Element("FilmsSettings").Attribute("PathToFolder").Value;
            set
            {
                _xDocument.Root.Element("settings").Element("FilmsSettings").Attribute("PathToFolder").Value = value;
            }
        }

        public static string GeneralSerialFolderPath
        {
            get => _xDocument.Root.Element("settings").Element("SerialsSettings").Attribute("PathToFolder").Value;
            set
            {
                _xDocument.Root.Element("settings").Element("SerialsSettings").Attribute("PathToFolder").Value = value;
            }
        }
    
        private static readonly string settingsFilePath =
            AppDomain.CurrentDomain.BaseDirectory + "Settings\\Settings.xml";
        public static XDocument LoadXDocument { set => _xDocument = value; }

        /// <summary>
        /// Parse config file and return List<string> with genre paths
        /// </summary>
        public static List<string> GetGenrePathsFromConfigFile()
        {
            List<string> configGenrePathsList = new List<string>();
            string genrePath;

            foreach (XElement el in _xDocument.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
            {
                genrePath = el.Attribute("PathToFolder").Value;
                if (ActionSet.FileOrDirectoryExists(genrePath))
                {
                    configGenrePathsList.Add(genrePath);
                }
            }
            return configGenrePathsList;
        }

        /// <summary>
        /// Change general folder for films.
        /// </summary>
        /// <param name="folder">fileInfo of general film folder</param>
        public static void ChangeFilmsFolder(FileInfo folder)
        {
            GeneralFilmFolderPath = folder.FullName;
            SaveSettings();
        }

        /// <summary>
        /// Change general folder for serials.
        /// </summary>
        /// <param name="folder">fileInfo of general serial folder</param>
        public static void ChangeSerialsFolder(FileInfo folder)
        {
            GeneralSerialFolderPath =  folder.FullName;
            SaveSettings();
        }

        /// <summary>
        /// Update Genre data in XML document. Remove all existing ones, and fill them with new data
        /// </summary>
        public static void UpdateGenres(CollectionOfGenres collectionOfGenres)
        {
            XElement genres = _xDocument.Root.Element("settings").Element("FilmsSettings").Element("Genres");
            genres.RemoveAll();
            foreach (var genre in collectionOfGenres.GenreList)
            {
                genres.Add(
                    new XElement("Genre",
                        new XAttribute("Name", genre.Name),
                        new XAttribute("PathToFolder", genre.PathToDirectory)
                    )
                );
            }
            SaveSettings();
        }

        /// <summary>
        /// Save XML configuration file
        /// </summary>
        private static void SaveSettings()
        {
            _xDocument.Save(settingsFilePath);
        }


        public static void ValidateDriveLetterOfPathsOnInit()
        {
            ValidateXElementPath(_xDocument.Root.Element("settings").Element("SerialsSettings"));
            ValidateXElementPath(_xDocument.Root.Element("settings").Element("FilmsSettings"));

            foreach (XElement el in _xDocument.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
            {
                ValidateXElementPath(el);
            }

            SaveSettings();
        }

        private static void ValidateXElementPath(XElement el)
        {
            string path;
            FileInfo fileInfo;

            XAttribute element = el.Attribute("PathToFolder");
            path = element.Value;

            if (!ActionSet.FileOrDirectoryExists(path))
            {
                fileInfo = new FileInfo(path);
                string rootPath = Path.GetPathRoot(new FileInfo(path).FullName);
                string pathWithChangedRoot = fileInfo.FullName.Replace(rootPath, ApplicationConfiguration.rootApp);

                if (ActionSet.FileOrDirectoryExists(pathWithChangedRoot))
                {
                    element.Value = pathWithChangedRoot;
                    SaveSettings();
                }
            }
        }

    }
}
