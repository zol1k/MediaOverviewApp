using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FilmDBApp.Helpers;

namespace FilmDBApp.Model
{
    public class CollectionOfGenres:ObservableObject
    {
        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ObservableCollection<Genre> _genreList;

        #endregion
        #region Properties

        public ObservableCollection<Genre> GenreList
        {
            get
            {
                return _genreList;
            }
        }

        public List<Genre> GenresToBeDeleted { get => GetGenresToBeDeleted(); }

        public List<string> GenreNamesList { get => ReturnListOfGenreNames(); }

        #endregion

        public CollectionOfGenres()
        {
            _genreList = new ObservableCollection<Genre>();
        }

        #region Methods
        /// <summary>
        /// Add genre to GenreList
        /// </summary>
        /// <param name="genre">will be added into GenreList</param>
        public void AddNewGenre(Genre genre)
        {
            bool _ifListCointainstGenre = _genreList.Any(p => p.Name == genre.Name);

            if (!_ifListCointainstGenre)
            {
                _genreList.Add(genre);
            }
            else
            {
                string msg = genre.Name + " is already in genre list!";
                Log.Debug(msg);
                MessageBox.Show(msg);
            }
        }

        /// <summary>
        /// Collect genre objects with ToBeDeletedFromGenreCollection set to false.
        /// </summary>
        /// <returns>List<Genre> with property ToBeDeletedFromGenreCollection set to false.</returns>
        internal List<Genre> GetGenresToBeDeleted()
        {
            return _genreList.Where(genre => genre.ToBeDeletedFromGenreCollection).ToList();
        }

        /// <summary>
        /// Remove genre from GenreList
        /// </summary>
        /// <param name="genre">will be removed from GenreList</param>
        public void RemoveGenreFromList(Genre genre)
        {
            _genreList.Remove(genre);
        }

        /// <summary>
        /// Remove list genres from GenreList
        /// </summary>
        /// <param name="listOfGenres">List of Genres that will be removed from genreList</param>
        public void RemoveGenreFromList(List<Genre> listOfGenres)
        {
            foreach (Genre genre in listOfGenres)
            {
                RemoveGenreFromList(genre);
            }
        }

        /// <summary>
        /// Clear current GenreList
        /// </summary>
        public void ClearAll()
        {
            _genreList.Clear();
        }

        /// <summary>
        /// Method to collect names of genres
        /// </summary>
        /// <returns>list<string> of genre names</returns>
        private List<string> ReturnListOfGenreNames()
        {
            List<string> list = GenreList.Select(o => o.Name).ToList();
            return list;
        }
        public static List<string>ReturnListOfGenreNamesFromConfigFile()
        {
            List<string> genreNames = new List<string>();

            foreach (var genrePath in ApplicationConfiguration.GetGenrePathsFromConfigFile())
            {
                genreNames.Add(Path.GetFileName(genrePath));
            }
            return genreNames;
        }

        #endregion
    }
}
