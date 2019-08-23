using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace WpfApp1.Model
{
    interface ICollectXMLData
    {
        #region Properties

        ObservableCollection<Genre> ListOfGenres { get; }
        FileInfo GeneralFilmsFolder { get; }
        FileInfo GeneralSerialsFolder { get; }

        #endregion

    } 
}