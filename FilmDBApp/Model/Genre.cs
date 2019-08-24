using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            CollectGenreFilms();
        }


        private void CollectGenreFilms()
        {
            try
            {

                foreach (var file in Directory.GetFiles(PathToGenreDirectory))
                {
                    FileInfo fileInfo = new FileInfo(file);

                    //if current file is not hidden, add it into film db
                    if ( !fileInfo.Attributes.HasFlag(FileAttributes.Hidden) )
                        CollectionOfFilms.AddNewFilm(new Film(fileInfo, false));
                }

                foreach (var file in Directory.GetDirectories(PathToGenreDirectory))
                {
                    FileInfo fileInfo = new FileInfo(file);

                    //if current directory is not hidden, add it into film db
                    if ( !fileInfo.Attributes.HasFlag(FileAttributes.Hidden) )
                        CollectionOfFilms.AddNewFilm(new Film(fileInfo, true));
                }

                //genre.Films = genre.Films.OrderBy(o => o.FileName).ToList();
            }
            catch (Exception ex)
            {
                
            }
        }


    }
}
