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

        private AppSettings _appSettings;
        private ICommand _changePageCommand;
        private List<IPageViewModel> _pageViewModels;
        private IPageViewModel _currentPageViewModel;

        #endregion

        public ApplicationViewModel()
        {
            _appSettings = new AppSettings();
            PageViewModels.Add(new HomeViewModel(_appSettings));
            PageViewModels.Add(new SettingsViewModel(_appSettings));
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

        #endregion

        #region Methods
        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
            {
                PageViewModels.Add(viewModel);
            }
            //tomuto zapisu trochu nerozumiem
            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        #endregion


    }
}
