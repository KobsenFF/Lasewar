using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Laserwar.View;

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
            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            DownloadView downloadView = new DownloadView();
            frame.NavigationService.Navigate(downloadView);
            Download.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Blue));
        }

        private void DownloadView_Clicked(object sender, RoutedEventArgs e)
        {
            Download.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Blue));
            Sounds.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            Game.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            DownloadView downloadView = new DownloadView();
            frame.NavigationService.Navigate(downloadView);
        }

        private void SoundsView_Clicked(object sender, RoutedEventArgs e)
        {
            Download.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            Sounds.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Blue));
            Game.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            SoundsView soundsView = new SoundsView();
            frame.NavigationService.Navigate(soundsView);
        }

        private void GameView_Clicked(object sender, RoutedEventArgs e)
        {
            Download.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            Sounds.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            Game.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Blue));
            GamesView gamesView = new GamesView();
            frame.NavigationService.Navigate(gamesView);
        }
        
    }
}
