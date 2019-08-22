using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class Genre: ObservableObject
    {
        #region Fields

        private string _genreName;
        private string _pathToGenreDirectory;


        public bool ToBeDeletedFromGenreCollection { get;set;}

        #endregion

        #region Properties / Commands
        public string GenreName{ get=> _genreName; }
        public string PathToGenreDirectory{ get => _pathToGenreDirectory;}

        #endregion

        public Genre(string Name)
        {
            _genreName = Name;

        }

        public Genre(FileInfo fileInfo)
        {
            _genreName = fileInfo.Name;
            _pathToGenreDirectory = fileInfo.FullName;
            ToBeDeletedFromGenreCollection = false;
        }


    }
}
