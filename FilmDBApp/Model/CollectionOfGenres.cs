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

        #endregion


        #region Properties

        public ObservableCollection<Genre> GenreList
        {
            get
            {
                _genreList.Sort();
                return _genreList;
            }

        }

        #endregion

        public CollectionOfGenres()
        {
            _genreList = new ObservableCollection<Genre>();
        }


        #region Methods
        public void AddNewGenre(Genre genre)
        {
            bool _ifListCointainstGenre = _genreList.Any(p => p.GenreName == genre.GenreName);

            if (! _ifListCointainstGenre)
                _genreList.Add(genre);
            else
            {
                MessageBox.Show(genre.GenreName + " is already in genre list!");
            }
        }

        public void RemoveGenreFromList(Genre genre)
        {
            _genreList.Remove(genre);
        }

        public void ClearAll()
        {
            _genreList.Clear();
        }

        public void RemoveToBeDeletedFromGenreList()
        {
            for (int i = _genreList.Count - 1; i >= 0; i--)
            {
                if (_genreList[i].ToBeDeletedFromGenreCollection)
                { 
                    MessageBox.Show(_genreList[i].GenreName);
                    _genreList.RemoveAt(i);
                }
            }
        }


        #endregion
    }
}
