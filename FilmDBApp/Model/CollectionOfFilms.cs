﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Model
{
    public class CollectionOfFilms:ObservableObject
    {
        #region Fields

        private ObservableCollection<Film> _listOfFilms;

        #endregion


        #region Properties

        public ObservableCollection<Film> ListOfFilms
        {
            get => _listOfFilms;
        }


        #endregion
        public CollectionOfFilms()
        {
            _listOfFilms= new ObservableCollection<Film>();
        }


        #region Methods

        public void AddNewFilm(Film film)
        {
            _listOfFilms.Add(film);
        }

        public void RemoveGenreFromList(Film film)
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