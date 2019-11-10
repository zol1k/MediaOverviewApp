using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaOverviewApp.Model
{
    internal class GeneralSerialFolder:ObservableObject
    {

        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isRoot;

        #endregion

        #region Properties / Commands

        public string Name
        {
            get => FileInfo.Name;
        }

        public FileInfo FileInfo { get => ApplicationConfiguration.GeneralSerialFolder; }
        public string PathToDirectory { get => FileInfo.FullName; }
        public CollectionOfSerials CollectionOfSerials { get; set; }
        public ObservableCollection<Serial> ListOfSerials { get => CollectionOfSerials.ListOfSerials; }

        #endregion

        public GeneralSerialFolder()
        {
            CollectionOfSerials = new CollectionOfSerials();
        }

        #region Methods

   

        #endregion

    }
}
