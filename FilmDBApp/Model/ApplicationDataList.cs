using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class ApplicationDataList :  ObservableObject
    {
        private AppSettings settings;
        public ObservableCollection<Genre> ListOfGenres
        {
            get => settings.ListOfGenres;
        }
        public ApplicationDataList()
        {
            settings = new AppSettings();
        }


























    }
}
