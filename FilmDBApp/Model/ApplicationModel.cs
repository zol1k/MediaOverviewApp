
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using FilmDBApp.Helpers;
using Microsoft.WindowsAPICodePack.Dialogs;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace FilmDBApp.Model
{
    class ApplicationModel : ObservableObject
    {
        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ApplicationConfiguration _config;


        #endregion

        #region Properties / Commands

        public CollectionOfGenres CollectionOfGenres { get; set; }

        public ObservableCollection<Genre> ListOfGenres
        {
            get => CollectionOfGenres.GenreList;
        }

        public ObservableCollection<Film> CollectionOfAllFilms { get => MergeGenreFilmCollections(); }
        public ApplicationConfiguration Config { get => _config; }

        #endregion

        public ApplicationModel()
        {
            _config = new ApplicationConfiguration();
            FillGenreCollectionByConfigurationFile();
        }
        #region Methods
        public void FillGenreCollectionByConfigurationFile()
        {
            CollectionOfGenres = new CollectionOfGenres();
            List<string> genrePaths = _config.GenrePaths;
            CollectionOfGenres.ClearAll();

            foreach (string path in genrePaths)
            {
                CollectionOfGenres.AddNewGenre(new Genre(new FileInfo(path)));
            }
        }
        
        public void ChangeFilmGenre(Film filmToMove, Genre filmOldGenre, Genre filmNewGenre)
        {
            filmOldGenre.ListOfFilms.Remove(filmToMove);
            filmNewGenre.ListOfFilms.Add(filmToMove);
            string path = filmNewGenre.PathToGenreDirectory;
            filmToMove.FilmFileInfo.MoveTo(path + Path.DirectorySeparatorChar + filmToMove.FilmFileInfo.Name);
        }

        private ObservableCollection<Film> MergeGenreFilmCollections()
        {
            ObservableCollection<Film> collectedFilms = new ObservableCollection<Film>();
            foreach (var genre in ListOfGenres)
            {
                foreach (var film in genre.ListOfFilms)
                {
                    collectedFilms.Add(film);
                }
            }

            collectedFilms.Sort();
            return collectedFilms;
        }
        #endregion
    }
}
