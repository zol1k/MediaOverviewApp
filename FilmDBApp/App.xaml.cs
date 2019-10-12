using FilmDBApp.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FilmDBApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplicationView app = new ApplicationView();
            ApplicationModel model = new ApplicationModel();
            ApplicationViewModel context = new ApplicationViewModel(model);
            app.DataContext = context;
            app.Show();
        }
    }
}
