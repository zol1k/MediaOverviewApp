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

        private readonly string _genreName;
        private readonly string _pathToGenreDirectory;
        private CollectionOfFilms _collectionOfFilms;



        #endregion

        #region Properties / Commands

        public string GenreName
        {
            get
            {
                


            return _genreName != "" ? _genreName : _pathToGenreDirectory;
            }
        }

        public string PathToGenreDirectory{ get => _pathToGenreDirectory;}
        public CollectionOfFilms CollectionOfFilms { get => _collectionOfFilms; set => _collectionOfFilms = value; }
        public bool ToBeDeletedFromGenreCollection { get; set; }

        public ObservableCollection<Film> ListOfFilms
        {
            get => _collectionOfFilms.ListOfFilms;
        }

        #endregion

        public Genre(FileInfo fileInfo)
        {
            _genreName = fileInfo.Name;
            _pathToGenreDirectory = fileInfo.FullName;
            ToBeDeletedFromGenreCollection = false;
            _collectionOfFilms = new CollectionOfFilms();
        }

        public int CompareTo(object obj)
        {
    
            Genre a = this;
            Genre b = (Genre)obj;
            return string.Compare(a.GenreName, b.GenreName);

        }
    
    }
}
