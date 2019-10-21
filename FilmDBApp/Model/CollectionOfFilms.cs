using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MediaOverviewApp.Helpers;

namespace MediaOverviewApp.Model
{
    public class CollectionOfFilms:ObservableObject
    {
        #region Fields
        private readonly ObservableCollection<Film> _listOfFilms;
        #endregion

        #region Properties
        public ObservableCollection<Film> ListOfFilms
        {
            get
            {
                _listOfFilms.Sort();
                return _listOfFilms;
            }
        }

        #endregion
        public CollectionOfFilms()
        {
            _listOfFilms= new ObservableCollection<Film>();
        }


        #region Methods

        /// <summary>
        /// Add obj from parameter into CollectionOfFilms
        /// </summary>
        /// <param name="film">Film to add into collection</param>
        public void AddNewFilm(Film film)
        {
            _listOfFilms.Add(film);
        }
        public void RemoveFilmFromList(Film film)
        {
            _listOfFilms.Remove(film);
        }
        public void ClearAll()
        {
            _listOfFilms.Clear();
        }



        #endregion
    }
}
