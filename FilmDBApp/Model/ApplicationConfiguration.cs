using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace FilmDBApp.Model
{
    class ApplicationConfiguration : ObservableObject
    {
        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly XDocument XDoc;
        private readonly XElement XGenresNode;
        private static readonly string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "Settings\\Settings.xml";

        private FileInfo _generalFilmsFolder;
        private FileInfo _generalSerialsFolder;
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

        #region Methods
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

            List<string> configNotValidGenrePathsList = new List<string>();

            string genrePath;
            foreach (XElement el in XDoc.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
            {

                genrePath = el.Attribute("PathToGenreFolder").Value;
                if (ActionSet.FileOrDirectoryExists(genrePath))
                {
                    configGenrePathsList.Add(genrePath);
                }
                else
                {
                    configNotValidGenrePathsList.Add(genrePath);
                    Log.Error("Application Configuration - Could not find destination of " + genrePath + " path.");
                }
            }

            if (configNotValidGenrePathsList.Count > 0)
            {
                ShowMsgBoxWithNotValidPaths(configNotValidGenrePathsList);
            }

            return configGenrePathsList;
        }

        private void ShowMsgBoxWithNotValidPaths(List<string> configNotValidGenrePathsList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Bellow path to genre(s) Not Found.");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(String.Join(Environment.NewLine, configNotValidGenrePathsList.ToArray()));
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Please go to settings and choose your genre folder again !");
            MessageBox.Show(sb.ToString(), "Genres Not Found");
        }

        /// <summary>
        /// Change general folder for films.
        /// </summary>
        /// <param name="folder">fileInfo of general film folder</param>
        public void ChangeFilmsFolder(FileInfo folder)
        {
            GeneralFilmsFolder = folder;
            FilmFolderXmlUpdate();
        }

        /// <summary>
        /// Change general folder for serials.
        /// </summary>
        /// <param name="folder">fileInfo of general serial folder</param>
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

        /// <summary>
        /// Update Element("settings").Element("FilmsSettings").Attribute("PathToFolder");
        /// </summary>
        private void FilmFolderXmlUpdate()
        {
            XAttribute xmlFilmsFolderPath = XDoc.Root.Element("settings").Element("FilmsSettings")
                .Attribute("PathToFolder");

            xmlFilmsFolderPath.Value = (GeneralFilmsFolder == null) ? "" : GeneralFilmsFolder.FullName;
            SaveSettings();
        }

        /// <summary>
        /// Update Element("settings").Element("SerialsSettings").Attribute("PathToFolder");
        /// </summary>
        private void SerialsFolderXmlUpdate()
        {
            XAttribute xmlSerialsFolderPath = XDoc.Root.Element("settings").Element("SerialsSettings")
                .Attribute("PathToFolder");

            xmlSerialsFolderPath.Value = (GeneralSerialsFolder == null) ? "" : GeneralSerialsFolder.FullName;
            SaveSettings();
        }

        /// <summary>
        /// Save XML configuration file
        /// </summary>
        private void SaveSettings()
        {
            XDoc.Save(settingsFilePath);
        }
        #endregion
    }
}
