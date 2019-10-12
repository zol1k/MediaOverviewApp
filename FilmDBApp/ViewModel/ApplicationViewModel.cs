using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FilmDBApp.Model;


namespace FilmDBApp
{
    class ApplicationViewModel : ObservableObject
    {
        #region Fields

        private readonly ApplicationModel model;
        private ICommand _changePageCommand;
        private ICommand _exportToCSVCommand;
        private List<IPageViewModel> _pageViewModels;
        private IPageViewModel _currentPageViewModel;

        #endregion

        public ApplicationViewModel()
        {
            model = new ApplicationModel();
            PageViewModels.Add(new HomeViewModel(model));
            PageViewModels.Add(new SettingsViewModel(model));
            CurrentPageViewModel = PageViewModels[0];
        }

        #region Properties / Commands

        public List<IPageViewModel> PageViewModels {
            get
            {
                if(_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();
                return _pageViewModels;
            }

        }

        public IPageViewModel CurrentPageViewModel {
            get { return _currentPageViewModel; }
            private set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public ICommand ChangePageCommand {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                    p => ChangeViewModel((IPageViewModel)p),
                    p => p is IPageViewModel);
                }

                return _changePageCommand;

            }
        }
        public ICommand ExportToCSVCommand {
            get
            {
                if (_exportToCSVCommand == null)
                    _exportToCSVCommand = new RelayCommand(ExportToCSVButton_Click);
                return _exportToCSVCommand;

            }
        }



        #endregion

        #region Methods
        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
            {
                PageViewModels.Add(viewModel);
            }
            //tomuto zapisu trochu nerozumiemdobre d
            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }
        private void ExportToCSVButton_Click(object obj)
        {
            ExcelExport exportToCSV = new ExcelExport(model.CollectionOfGenres);
            exportToCSV.Run();
            exportToCSV.Dispose();
        }
        #endregion


    }
}
