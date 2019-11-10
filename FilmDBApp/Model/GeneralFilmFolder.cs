using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaOverviewApp.Model;
using Microsoft.WindowsAPICodePack.Shell.Interop;

namespace MediaOverviewApp.Model
{
    internal class GeneralFilmFolder: ObservableObject, IComparable, IMediaCollection
    {
        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isRoot;

        #endregion

        #region Properties / Commands

        public string Name
        {
            get => FileInfo.Name;
        }

        public FileInfo FileInfo { get => ApplicationConfiguration.GeneralFilmFolder; }
        public string PathToDirectory{ get => FileInfo.FullName; }
        public CollectionOfFilms CollectionOfFilms { get; set; }
        public ObservableCollection<Film> ListOfFilms { get => CollectionOfFilms.ListOfFilms; }

        #endregion

        public GeneralFilmFolder()
        {
            CollectionOfFilms = new CollectionOfFilms();
            CollectFilms();
        }

        #region Methods
        public int CompareTo(object obj)
        {
            GeneralFilmFolder a = this;
            GeneralFilmFolder b = (GeneralFilmFolder)obj;
            return String.Compare(a.Name, b.Name);
        }

        /// <summary>
        /// Going throught paths of recieved genres, and fill its filmLists with CollectGenreFilms
        /// </summary>
        public void CollectFilms()
        {
            if (ActionSet.FileOrDirectoryExists(PathToDirectory))
            {
                CollectionOfFilms.ListOfFilms.Clear();

                foreach (var file in Directory.GetFiles(PathToDirectory))
                {
                    FileInfo fileInfo = new FileInfo(file);

                    //if current file is not hidden, add it into film db
                    if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        CollectionOfFilms.AddNewFilm(new Film(fileInfo, false)
                        );
                    }
                }
            

                List<string> listOfPathsToIgnore = XController.GetGenrePathsFromConfigFile();
                listOfPathsToIgnore.Add(XController.GeneralSerialFolderPath);

                foreach (var file in Directory.GetDirectories(PathToDirectory))
                {
                    FileInfo fileInfo = new FileInfo(file);

                    if (listOfPathsToIgnore.Contains(fileInfo.FullName))
                        continue;
                    //if current directory is not hidden, add it into film db
                    if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        CollectionOfFilms.AddNewFilm(new Film(fileInfo, true));
                    }
                }
            }
        }

        #endregion

    }

}
