using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaOverviewApp.Model
{
    class CollectionOfSerials
    {
        #region Fields
        private readonly ObservableCollection<Serial> _listOfSerials;
        #endregion

        #region Properties
        public ObservableCollection<Serial> ListOfSerials
        {
            get
            {
                return _listOfSerials;
            }
        }

        #endregion
        public CollectionOfSerials()
        {
            _listOfSerials = new ObservableCollection<Serial>();
        }


        #region Methods

        /// <summary>
        /// Add obj from parameter into CollectionOfFilms
        /// </summary>
        /// <param name="film">Film to add into collection</param>
        public void AddNewSerial(Serial serial)
        {
            _listOfSerials.Add(serial);
        }
        public void RemoveSerialFromList(Serial serial)
        {
            _listOfSerials.Remove(serial);
        }
        public void ClearAll()
        {
            _listOfSerials.Clear();
        }



        #endregion
    }
}
