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
        private FileInfo _genreFileInfo;
        private CollectionOfFilms _collectionOfFilms;
        private bool _isRoot;

        #endregion

        #region Properties / Commands

        public string GenreName
        {
            get
            {
                return _isRoot == false ? _genreFileInfo.Name : "Root";
            }
        }

        public string PathToGenreDirectory{ get => _genreFileInfo.FullName; }
        public CollectionOfFilms CollectionOfFilms { get => _collectionOfFilms; set => _collectionOfFilms = value; }
        public bool ToBeDeletedFromGenreCollection { get; set; }

        public ObservableCollection<Film> ListOfFilms
        {
            get => _collectionOfFilms.ListOfFilms;
        }

        #endregion

        public Genre(FileInfo fileInfo)
        {
            _genreFileInfo = fileInfo;
            _collectionOfFilms = new CollectionOfFilms();

            if (fileInfo.Directory == null) _isRoot = true;
        }

        public int CompareTo(object obj)
        {
            Genre a = this;
            Genre b = (Genre)obj;
            return String.Compare(a.GenreName, b.GenreName);
        }
    
    }
}
