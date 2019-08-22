using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using WpfApp1.Model;

namespace WpfApp1
{
    internal class HomeViewModel : ObservableObject, IPageViewModel
    {
        private DocumentList documentList;
        private ObservableCollection<Genre> _listOfGenres;
        public ObservableCollection<Genre> ListOfGenres
        {
            get { return _listOfGenres; }
        }
        public string Name
        {
            get
            {
                return "HomePage";
            }
        }
        public HomeViewModel()
        {
            documentList = new DocumentList();
            _listOfGenres = documentList.ListOfGenres;
        }
    }
}