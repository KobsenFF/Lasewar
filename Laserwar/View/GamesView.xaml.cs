using Laserwar.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Laserwar.View
{
    /// <summary>
    /// Логика взаимодействия для GamesView.xaml
    /// </summary>
    public partial class GamesView : Page
    {
        List<Games> games = new List<Games>(3);

        public GamesView()
        {
            InitializeComponent();
        }

        private void gameGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            database.openConnection();

            string sqlCommand = "SELECT Name, Date, Games.Id, count(Players.Id)  AS Count " +
                                "FROM Games, Players, Teams " +
                                "WHERE Games.Id = Teams.GameId and Players.TeamId = Teams.Id " +
                                "GROUP BY Games.Id";

            SQLiteCommand cmd = new SQLiteCommand(sqlCommand, database.sqliteCon);
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string Name = String.Empty;
                string Id = String.Empty;
                string DateString = String.Empty;
                int playersCount;
                DateTime Date;
                while (r.Read())
                {
                    Name = r["Name"].ToString();
                    Id = r["Id"].ToString();
                    Date = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(r["Date"]));
                    DateString = Date.ToString("d");
                    playersCount = Convert.ToInt32(r["Count"]);



                    games.Add(new Games(Name, Id, DateString, playersCount));
                }
                r.Close();

                Console.WriteLine(games.Capacity);
                for (int i = 0; i < games.Count; i++)
                {
                    Console.WriteLine(games[i].Id + " " + games[i].Name + " " + games[i].Date + " " + games[i].playesrCount);
                }

            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            gamesGrid.ItemsSource = games;
        }


        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int index = gamesGrid.SelectedIndex;
            string game = games[index].Name;
            Console.WriteLine(game);
            this.NavigationService.Navigate(new GameView(game, index+1));
            Console.WriteLine("загружаю GameView");
        }


    }
}