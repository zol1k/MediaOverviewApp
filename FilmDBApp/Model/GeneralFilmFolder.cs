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
    internal class GeneralFilmFolder: ObservableObject, IComparable, IFilmCollection
    {
        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private FileInfo _fileInfo;
        private readonly bool _isRoot;

        #endregion

        #region Properties / Commands

        public string Name
        {
            get => _fileInfo.Name;
        }

        public string PathToDirectory{ get => _fileInfo.FullName; }
        public CollectionOfFilms CollectionOfFilms { get; set; }
        public ObservableCollection<Film> ListOfFilms
        {
            get
            {
                return CollectionOfFilms.ListOfFilms;
            } 
        }

        #endregion

        public GeneralFilmFolder(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
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
        private void CollectFilms()
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


            List<string> listOfGenreNames = CollectionOfGenres.ReturnListOfGenreNamesFromConfigFile();

            foreach (var file in Directory.GetDirectories(PathToDirectory))
            {
                FileInfo fileInfo = new FileInfo(file);

                if (listOfGenreNames.Contains(fileInfo.Name))
                    continue;
                //if current directory is not hidden, add it into film db
                if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    CollectionOfFilms.AddNewFilm(new Film(fileInfo, true));
                }
            }
        }

        public void UpdateFileInfo(FileInfo generalFilmFolder)
        {
            _fileInfo = generalFilmFolder;
            CollectFilms();
        }

        #endregion

        public void ChangeDestinationFolder(FileInfo fileInfo, ApplicationConfiguration configuration)
        {
            _fileInfo = fileInfo;
            configuration.ChangeFilmsFolder(fileInfo);
            CollectFilms();
        }
    }

}
