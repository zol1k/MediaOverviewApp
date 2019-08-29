using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FilmDBApp
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : Window
    {
        private string ImageFolderPath = AppDomain.CurrentDomain.BaseDirectory + "BackgroundImages\\" ;
        private static string ImagesFolderPath = AppDomain.CurrentDomain.BaseDirectory + "BackgroundImages\\1.jpg";

        public ApplicationView()
        {
            InitializeComponent();
            CollectImages();
            this.Background = new ImageBrush(new BitmapImage(new Uri(GetRandomImagePath())));

        }

        private List<string> CollectImages()
        {
            return Directory.GetFiles(ImageFolderPath, "*.jpg*", SearchOption.TopDirectoryOnly)
                .ToList();
        }

        private string GetRandomImagePath()
        {
            Random random = new Random();
            List<string> images = CollectImages();
            int index = random.Next(images.Count);
            return images[index];
        }
    }
}
