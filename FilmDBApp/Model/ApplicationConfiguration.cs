using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace MediaOverviewApp.Model
{
    class ApplicationConfiguration : ObservableObject
    {
        #region Fields

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private XDocument Xdoc;
        private static readonly string settingsFilePath =
            AppDomain.CurrentDomain.BaseDirectory + "Settings\\Settings.xml";

        private string rootApp = Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);

        #endregion

        #region Properties / Commands

        public static XDocument LoadXDoc { get => XDocument.Load(settingsFilePath); }
        public static FileInfo GeneralFilmFolder { get => new FileInfo(LoadXDoc.Root.Element("settings").Element("FilmsSettings").Attribute("PathToFolder").Value); }
        public static FileInfo GeneralSerialsFolder { get => new FileInfo(LoadXDoc.Root.Element("settings").Element("SerialsSettings").Attribute("PathToFolder").Value); }

        #endregion
        public ApplicationConfiguration()
        {
            Xdoc = XDocument.Load(settingsFilePath);

            ValidateDriveLetterOfPathsOnInit();

            ValidateConfigFileOnInit();
        }

        #region Methods
        /// <summary>
        /// Parse config file and return List<string> with genre paths
        /// </summary>
        public static List<string> GetGenrePathsFromConfigFile()
        {
            List<string> configGenrePathsList = new List<string>();
            string genrePath;
            //var XDoc1 = XDocument.Load(settingsFilePath);
            foreach (XElement el in LoadXDoc.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
            {
                genrePath = el.Attribute("PathToFolder").Value;
                if (ActionSet.FileOrDirectoryExists(genrePath))
                {
                    configGenrePathsList.Add(genrePath);
                }
            }
            return configGenrePathsList;
        }

        private void ValidateDriveLetterOfPathsOnInit()
        {
            ValidateXElementPath(Xdoc.Root.Element("settings").Element("SerialsSettings"));
            ValidateXElementPath(Xdoc.Root.Element("settings").Element("FilmsSettings"));

            foreach (XElement el in Xdoc.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
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
                    SaveSettings();
                }
            }
        }


        private void ValidateConfigFileOnInit()
        {
            List<string> configNotValidGenrePathsList = new List<string>();
            string genrePath;
            foreach (XElement el in Xdoc.Root.Element("settings").Element("FilmsSettings").Element("Genres").Elements())
            {
                genrePath = el.Attribute("PathToFolder").Value;
                if (!ActionSet.FileOrDirectoryExists(genrePath))
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
            XAttribute xmlFilmssFolderPath = Xdoc.Root.Element("settings").Element("FilmsSettings").Attribute("PathToFolder");
            xmlFilmssFolderPath.Value = (GeneralFilmFolder == null) ? "" : folder.FullName;
            SaveSettings();
        }

        /// <summary>
        /// Change general folder for serials.
        /// </summary>
        /// <param name="folder">fileInfo of general serial folder</param>
        public void ChangeSerialsFolder(FileInfo folder)
        {
            XAttribute xmlSerialsFolderPath = Xdoc.Root.Element("settings").Element("SerialsSettings").Attribute("PathToFolder");
            xmlSerialsFolderPath.Value = (GeneralSerialsFolder == null) ? "" : folder.FullName;
            SaveSettings();
            OnPropertyChanged("GeneralSerialsFolder");
        }

        /// <summary>
        /// Update Genre data in XML document. Remove all existing ones, and fill them with new data
        /// </summary>
        public void GenresXmlUpdate(CollectionOfGenres collectionOfGenres)
        {
            XElement genres = Xdoc.Root.Element("settings").Element("FilmsSettings").Element("Genres");
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
        private void SaveSettings()
        {
            Xdoc.Save(settingsFilePath);
        }
        #endregion
    }
}
