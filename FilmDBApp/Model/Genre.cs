using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilmDBApp.Model;
using Microsoft.WindowsAPICodePack.Shell.Interop;

namespace FilmDBApp.Model
{
    public class Genre: ObservableObject, IComparable
    {
        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly FileInfo _genreFileInfo;
        private readonly bool _isRoot;

        #endregion

        #region Properties / Commands

        public string GenreName
        {
            get
            {
                return _isRoot ? "Root" : _genreFileInfo.Name;
            }
        }

        public string PathToGenreDirectory{ get => _genreFileInfo.FullName; }
        public CollectionOfFilms CollectionOfFilms { get; set; }
        public bool ToBeDeletedFromGenreCollection { get; set; }

        public ObservableCollection<Film> ListOfFilms
        {
            get => CollectionOfFilms.ListOfFilms;
        }

        #endregion

        public Genre(FileInfo fileInfo)
        {
            _genreFileInfo = fileInfo;
            CollectionOfFilms = new CollectionOfFilms();

            if (fileInfo.Directory == null) _isRoot = true;

            CollectFilms();
        }

        public int CompareTo(object obj)
        {
            Genre a = this;
            Genre b = (Genre)obj;
            return String.Compare(a.GenreName, b.GenreName);
        }

        /// <summary>
        /// Going throught paths of recieved genres, and fill its filmLists with CollectGenreFilms
        /// </summary>
        private void CollectFilms()
        {
            foreach (var file in Directory.GetFiles(PathToGenreDirectory))
            {
                FileInfo fileInfo = new FileInfo(file);

                //if current file is not hidden, add it into film db
                if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    CollectionOfFilms.AddNewFilm(new Film(fileInfo, false)
                    {
                        DirectoryGenre = GenreName
                    });
                }
            }

            foreach (var file in Directory.GetDirectories(PathToGenreDirectory))
            {
                FileInfo fileInfo = new FileInfo(file);

                //if (CollectionOfGenres.GenreNameList.Contains(fileInfo.Name))
                    //continue;
                //if current directory is not hidden, add it into film db
                if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    CollectionOfFilms.AddNewFilm(new Film(fileInfo, true)
                    {
                        DirectoryGenre = GenreName
                    });
                }
            }

        }
    }
}
