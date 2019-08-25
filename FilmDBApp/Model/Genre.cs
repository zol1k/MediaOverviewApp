using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilmDBApp.Model;
using Microsoft.WindowsAPICodePack.Shell.Interop;

namespace WpfApp1.Model
{
    public class Genre: ObservableObject
    {
        #region Fields

        private string _genreName;
        private string _pathToGenreDirectory;
        private CollectionOfFilms _collectionOfFilms;



        #endregion

        #region Properties / Commands

        public string GenreName{ get=> _genreName; }
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


        


    }
}
