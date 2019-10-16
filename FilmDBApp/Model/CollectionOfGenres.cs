using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private readonly ObservableCollection<Genre> _genreList;
        private List<string> _genreNameList;

        #endregion


        #region Properties

        public ObservableCollection<Genre> GenreList
        {
            get
            {
                return _genreList;
            }
        }

        public IEnumerable<Genre> GenresToBeDeleted { get => GetGenresToBeDeleted(); }
        #endregion

        public CollectionOfGenres()
        {
            _genreNameList = new List<string>();
            _genreList = new ObservableCollection<Genre>();
        }


        #region Methods
        /// <summary>
        /// Add genre to GenreList
        /// </summary>
        /// <param name="genre">will be added into GenreList</param>
        public void AddNewGenre(Genre genre)
        {
            bool _ifListCointainstGenre = _genreList.Any(p => p.GenreName == genre.GenreName);

            if (!_ifListCointainstGenre)
            {
                _genreList.Add(genre);

            }
            else
            {
                MessageBox.Show(genre.GenreName + " is already in genre list!");
            }
        }

        internal IEnumerable<Genre> GetGenresToBeDeleted()
        {
            return _genreList.Where(genre => genre.ToBeDeletedFromGenreCollection);
        }

        /// <summary>
        /// Remove genre from GenreList
        /// </summary>
        /// <param name="genre">will be removed from GenreList</param>
        public void RemoveGenreFromList(Genre genre)
        {
            _genreList.Remove(genre);
        }

        public void RemoveGenreFromList(IEnumerable<Genre> listOfGenres)
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
        /// Loop throught of Genres, and those with attribute "ToBeDeleted" will be removed
        /// </summary>
        public void RemoveToBeDeletedFromGenreList()
        {
            for (int i = _genreList.Count - 1; i >= 0; i--)
            {
                if (_genreList[i].ToBeDeletedFromGenreCollection)
                { 
                    
                    _genreList.RemoveAt(i);
                }
            }
            MessageBox.Show(_genreList.ToString());
        }

        /// <summary>
        /// Method to collect names of genres
        /// </summary>
        /// <returns>list<string> of genre names</returns>
        private List<string> ReturnListOfGenreNames()
        {
            List<string> list = GenreList.Select(o => o.GenreName).ToList();
            return list;
        }

        internal void RemoveFromGenreList(IEnumerable<Genre> toBeDeletedList)
        {
            for (int i = _genreList.Count - 1; i >= 0; i--)
            {
                if (_genreList[i].ToBeDeletedFromGenreCollection)
                {

                    _genreList.RemoveAt(i);
                }
            }
            MessageBox.Show(_genreList.ToString());
        }

        #endregion
    }
}
