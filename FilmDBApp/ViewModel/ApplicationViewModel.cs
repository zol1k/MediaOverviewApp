using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace WpfApp1
{
    class ApplicationViewModel : ObservableObject
    {
        #region Fields

        private ICommand _changePageCommand;
        private List<IPageViewModel> _pageViewModels;
        private IPageViewModel _currentPageViewModel;

        #endregion

        public ApplicationViewModel()
        {
            PageViewModels.Add(new HomeViewModel());
            PageViewModels.Add(new SettingsViewModel());
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
