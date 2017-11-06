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
using Laserwar.Model;
using System.Data.SQLite;

namespace Laserwar.View
{
    /// <summary>
    /// Логика взаимодействия для SoundsView.xaml
    /// </summary>
    public partial class SoundsView : UserControl
    {
        public SoundsView()
        {
            InitializeComponent();
        }

        private void gridSounds_Loaded(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            database.openConnection();
            List<Sounds> sounds = new List<Sounds>(3);

            string sqlCommand = "SELECT Name, Url, Size FROM sounds";
            SQLiteCommand cmd = new SQLiteCommand(sqlCommand, database.sqliteCon);
            
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string Name = String.Empty;
                string Url = String.Empty;
                string Size;
                while (r.Read())
                {
                    Name = r["Name"].ToString();
                    Url = r["Url"].ToString();
                    Size = r["Size"].ToString();
                    Size = (Convert.ToInt32(Size)/1024) + "Kb";
                    sounds.Add(new Sounds(Name, Url, Size));
                }
                r.Close();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            soundsGrid.ItemsSource = sounds;
        }
    }
}
