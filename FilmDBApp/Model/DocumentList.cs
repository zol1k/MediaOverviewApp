using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class DocumentList
    {
        private GenreCollection _listOfGenres;
        private AppSettings documentSettings;
        public ObservableCollection<Genre> ListOfGenres
        {
            get => documentSettings.ListOfGenres;
        }

        public DocumentList()
        {
            _listOfGenres = new GenreCollection();

            documentSettings = new AppSettings();
        }
    }
}
