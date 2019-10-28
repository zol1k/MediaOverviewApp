using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MediaOverviewApp.Model
{
    class ApplicationConfiguration : ObservableObject
    {
        #region Fields
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly XDocument XDoc = XDocument.Load(settingsFilePath);
        readonly XElement XGeneralFilmsElement;
        readonly XElement XGeneralSerialsElement;
        readonly XElement XGenresElement;
        static readonly string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "Settings\\Settings.xml";
        readonly string rootApp = Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);

        FileInfo _generalFilmFolder;
        FileInfo _generalSerialsFolder;
        #endregion

        #region Properties / Commands
        public FileInfo GeneralFilmFolder
        {
            get => _generalFilmFolder;
            set
            {
                _generalFilmFolder = value;
                OnPropertyChanged("GeneralFilmFolder");
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

            XGenresElement = XDoc.Root.Element("settings").Element("FilmsSettings").Element("Genres");
            XGeneralFilmsElement = XDoc.Root.Element("settings").Element("FilmsSettings");
            XGeneralSerialsElement = XDoc.Root.Element("settings").Element("SerialsSettings");
            
            ValidateDriveLetterOfPathsOnInit();
            ValidateConfigFileOnInit();
            GetFilmAndSerialFileInfoFromConfigFile();
        }

        #region Methods
        /// <summary>
        /// Get FILM / SERIAL folder path from configuration file 
        /// </summary>
        private void GetFilmAndSerialFileInfoFromConfigFile()
        {
            string filmsFolderPath = XGeneralFilmsElement.Attribute("PathToFolder").Value;

            string serialsFolderPath = XGeneralSerialsElement.Attribute("PathToFolder").Value;

                if (ActionSet.FileOrDirectoryExists(filmsFolderPath))
                {
                    GeneralFilmFolder = new FileInfo(filmsFolderPath);
                }

                if (ActionSet.FileOrDirectoryExists(serialsFolderPath))
                {
                    GeneralSerialsFolder = new FileInfo(serialsFolderPath);
                }
        }


        /// <summary>
        /// Parse config file and return List<string> with genre paths
        /// </summary>
        public static List<string> GetGenrePathsFromConfigFile()
        {
            List<string> configGenrePathsList = new List<string>();
            string genrePath;
            var XDoc1 = XDocument.Load(settingsFilePath);
            foreach (XElement el in XDoc1.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
            {
                genrePath = el.Attribute("PathToFolder").Value;
                if (ActionSet.FileOrDirectoryExists(genrePath))
                {
                    configGenrePathsList.Add(genrePath);
                }
            }
            return configGenrePathsList;
        }

        public void ValidateDriveLetterOfPathsOnInit()
        {
            ValidateXElementPath(XGeneralFilmsElement);
            ValidateXElementPath(XGeneralSerialsElement);

            foreach (XElement el in XGenresElement.Elements())
            {
                ValidateXElementPath(el);
            }

            SaveSettings();
        }

        private void ValidateXElementPath(XElement el)
        {
            string path;
            FileInfo fileInfo;

            XAttribute element = el.Attribute("PathToFolder");
            path = element.Value;

            if (!ActionSet.FileOrDirectoryExists(path))
            {
                fileInfo = new FileInfo(path);
                string rootPath = Path.GetPathRoot(new FileInfo(path).FullName);
                string pathWithChangedRoot = fileInfo.FullName.Replace(rootPath, rootApp);

                if (ActionSet.FileOrDirectoryExists(pathWithChangedRoot))
                {
                    element.Value = pathWithChangedRoot;
                }
            }
        }
        
        private void ValidateConfigFileOnInit()
        {
            List<string> configNotValidGenrePathsList = new List<string>();
            string genrePath;
            foreach (XElement el in XGenresElement.Elements())
            {
                genrePath = el.Attribute("PathToFolder").Value;
                if (! ActionSet.FileOrDirectoryExists(genrePath))
                {
                    configNotValidGenrePathsList.Add(genrePath);
                    Log.Error("Application Configuration - Could not find destination of " + genrePath + " path.");
                }
            }
            if (configNotValidGenrePathsList.Count > 0)
            {
                ShowMsgBoxWithNotValidPaths(configNotValidGenrePathsList);
            }
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
            GeneralFilmFolder = folder;
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
            XGenresElement.RemoveAll();
            foreach (var genre in collectionOfGenres.GenreList)
            {
                XGenresElement.Add(
                    new XElement("Genre",
                    new XAttribute("Name", genre.Name),
                    new XAttribute("PathToFolder", genre.PathToDirectory)
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

            xmlFilmsFolderPath.Value = (GeneralFilmFolder == null) ? "" : GeneralFilmFolder.FullName;
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
