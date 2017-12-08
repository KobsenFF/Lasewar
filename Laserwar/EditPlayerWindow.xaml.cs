using Laserwar.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Laserwar
{
    /// <summary>
    /// Логика взаимодействия для EditPlayerWindow.xaml
    /// </summary>
    public partial class EditPlayerWindow : Window
    {
        public string playerName;
        public string rating;
        public string accurancy;
        public string shots;
        public int id;
        public int gameid;
        public string gamename;

        public EditPlayerWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string accurancy = Accurancy.Text;
            try
            {
                accurancy = accurancy.Remove(accurancy.IndexOf('%'));
            }
            catch
            {
            }
            Database database = new Database();
            database.UpdatePlayer(PlayerName.Text, Rating.Text, accurancy, Shots.Text, id);
            this.Close();
        }
    }
}
