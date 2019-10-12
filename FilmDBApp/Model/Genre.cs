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
        }

        public int CompareTo(object obj)
        {
            Genre a = this;
            Genre b = (Genre)obj;
            return String.Compare(a.GenreName, b.GenreName);
        }
    
    }
}
