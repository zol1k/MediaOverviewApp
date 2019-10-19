
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

        public GeneralFilmFolder GeneralFilmFolder { get; }
        public ApplicationConfiguration Config { get => _config; }

  

        #endregion

        public ApplicationModel()
        {
            _config = new ApplicationConfiguration();
            FillGenreCollectionByConfigurationFile();
            GeneralFilmFolder = new GeneralFilmFolder(Config.GeneralFilmFolder);
        }
        #region Methods
        /// <summary>
        /// Get genre paths from config
        /// </summary>
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
        
        /// <summary>
        /// Change genre of film.
        /// </summary>
        /// <param name="filmToMove">Film to move into new genre</param>
        /// <param name="filmOldGenre">Old film genre</param>
        /// <param name="filmNewGenre">New film genre</param>
        public void ChangeFilmGenre(Film filmToMove, Genre filmOldGenre, Genre filmNewGenre)
        {
            filmOldGenre.ListOfFilms.Remove(filmToMove);
            filmNewGenre.ListOfFilms.Add(filmToMove);
            string path = filmNewGenre.PathToDirectory;
            filmToMove.FilmFileInfo.MoveTo(path + Path.DirectorySeparatorChar + filmToMove.FilmFileInfo.Name);
        }

        /// <summary>
        /// Going throught ListOfGenres and merging films into one collection.
        /// </summary>
        /// <returns>ObservableCollection<Film> of films</returns>
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
