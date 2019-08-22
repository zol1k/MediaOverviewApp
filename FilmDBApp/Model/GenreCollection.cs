using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Model
{
    class GenreCollection:ObservableObject
    {
        #region Fields

        private ObservableCollection<Genre> _genreList;

        #endregion


        #region Properties

        public ObservableCollection<Genre> GenreList
        {
            get => _genreList;
        }


        #endregion
        public GenreCollection()
        {
            _genreList = new ObservableCollection<Genre>();
        }

        #region Methods
        public void AddNewGenre(Genre genre)
        {
            _genreList.Add(genre);
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
                if (_genreList[i].ToBeDeletedFromGenreCollection == true)
                { 
                    MessageBox.Show(_genreList[i].GenreName);
                    _genreList.RemoveAt(i);
                }
            }
        }


        #endregion
    }
}
