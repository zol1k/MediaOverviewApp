using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace MediaOverviewApp.Model
{
    class ApplicationConfiguration : ObservableObject
    {
        #region Fields
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string settingsFilePath =
            AppDomain.CurrentDomain.BaseDirectory + "Settings\\Settings.xml";

        public static string rootApp = Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);



        #endregion

        #region Properties / Commands
        public static FileInfo GeneralFilmFolder { get => new FileInfo(XController.GeneralFilmFolderPath); }
        public static FileInfo GeneralSerialFolder { get => new FileInfo(XController.GeneralSerialFolderPath); }

        public static BitmapImage ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("..\\Images\\Deleteicon.png", UriKind.Relative));
            } 
        }

        #endregion

        public ApplicationConfiguration()
        {
            // Load XML Config file controller
            XController.LoadXDocument = XDocument.Load(settingsFilePath);
            
            XController.ValidateDriveLetterOfPathsOnInit();

            ValidateConfigFileOnInit();
        }

        #region Methods
        private void ValidateConfigFileOnInit()
        {
            List<string> notValidPaths = new List<string>();
            List<string> pathsToValidate = new List<string>();

            pathsToValidate.Add(XController.GeneralFilmFolderPath);
            pathsToValidate.Add(XController.GeneralSerialFolderPath);
            pathsToValidate = pathsToValidate.Concat(XController.GetGenrePathsFromConfigFile()).ToList();

            foreach (string path in pathsToValidate)
            {
                if (!ActionSet.FileOrDirectoryExists(path))
                {
                    notValidPaths.Add(path);
                    Log.Error("Application Configuration - Could not find destination of " + path + " path.");
                }
            }
            if (notValidPaths.Count > 0)
            {
                ShowMsgBoxWithNotValidPaths(notValidPaths);
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
        
        #endregion
    }
}
