using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Laserwar.ViewModel;
using System.Data.SQLite;
using System.IO;

namespace Laserwar
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new DownloadViewModel();
            Download.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Blue));
            
        }
       
        private void DownloadView_Clicked(object sender, RoutedEventArgs e)
        {
            Download.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Blue));
            Sounds.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            Game.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            DataContext = new DownloadViewModel();
        }

        private void SoundsView_Clicked(object sender, RoutedEventArgs e)
        {
            Download.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            Sounds.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Blue));
            Game.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            DataContext = new SoundsViewModel();
        }

        private void GameView_Clicked(object sender, RoutedEventArgs e)
        {
            Download.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            Sounds.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            Game.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Blue));
            DataContext = new GameViewModel();
        }
    }
}
