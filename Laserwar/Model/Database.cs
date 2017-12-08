using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Data;

namespace Laserwar.Model
{
    class Database
    {
        public SQLiteConnection sqliteCon;
        public string databaseName = @".\test.db";

        public Database()
        {
            sqliteCon = new SQLiteConnection(@"Data Source=test.db");
            createDB();
            createTableSounds();
            createTableGames();
            createTableTeams();
            createTablePlayers();
        }

        public void createDB()
        {
            
            if (!File.Exists(databaseName))
            {
                SQLiteConnection.CreateFile(databaseName);
                Console.WriteLine(File.Exists(databaseName) ? "База данных создана" : "Возникла ошибка при создании базы данных");
            }
        }

        public void createTableSounds()
        {
            openConnection();

            

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS Sounds(" + string.Join(", ",
                    "Id INTEGER  PRIMARY KEY AUTOINCREMENT",
                    "Name TEXT NOT NULL",
                    "Url TEXT NOT NULL",
                    "Size INTEGER NOT NULL"
                    ) +
                    ");", sqliteCon).ExecuteNonQuery();
        }

        public void createTableGames()
        {
            openConnection();
            new SQLiteCommand("PRAGMA foreign_keys=on", sqliteCon).ExecuteNonQuery();

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS Games(" + string.Join(", ",
                    "Id INTEGER  PRIMARY KEY AUTOINCREMENT",
                    "Name TEXT NOT NULL ",
                    "Date TEXT NOT NULL"
                    ) +
                    ");", sqliteCon).ExecuteNonQuery();
        }

        public void AddGames(string name, string date)
        {
            string sqlCommand = "INSERT INTO " + "Games"
                + " (Name, Date) VALUES (@name, @date)";
            sqliteCon = new SQLiteConnection(@"Data Source=test.db");
            openConnection();
            SQLiteCommand mycommand = new SQLiteCommand(sqlCommand, sqliteCon);
            mycommand.Parameters.Add("@name", DbType.String).Value = name;
            mycommand.Parameters.Add("@date", DbType.String).Value = date;
            mycommand.ExecuteNonQuery();
            closeConnection();
        }

        public void createTableTeams()
        {
            openConnection();

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS Teams(" + string.Join(", ",
                    "Id INTEGER  PRIMARY KEY AUTOINCREMENT",
                    "GameId INTEGER NOT NULL",
                    "TeamName TEXT NOT NULL",
                    "FOREIGN KEY(GameId) REFERENCES Games(Id)"
                    ) +
                    ");", sqliteCon).ExecuteNonQuery();
        }

        public void AddTeams(int gameId, string teamName)
        {
            string sqlCommand = "INSERT INTO " + "Teams"
                + " (GameId, TeamName) VALUES (@gameId, @teamName)";
            sqliteCon = new SQLiteConnection(@"Data Source=test.db");
            
            openConnection();
            SQLiteCommand mycommand = new SQLiteCommand(sqlCommand, sqliteCon);
            mycommand.Parameters.Add("@gameId", DbType.Int32).Value = gameId;
            mycommand.Parameters.Add("@teamName", DbType.String).Value = teamName;
            mycommand.ExecuteNonQuery();
            closeConnection();
        }

        public void createTablePlayers()
        {
            openConnection();

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS Players(" + string.Join(", ",
                    "Id INTEGER  PRIMARY KEY AUTOINCREMENT",
                    "TeamId INTEGER NOT NULL",
                    "PlayerName TEXT NOT NULL",
                    "Rating INTEGER NOT NULL",
                    "Accuracy REAL NOT NULL",
                    "Shots INTEGER NOT NULL",
                    "FOREIGN KEY(TeamId) REFERENCES Teams(Id)"
                    ) +
                    ");", sqliteCon).ExecuteNonQuery();
        }
        
        public void AddPlayer(int teamId, string playerName, int rating, double accuracy, int shots)
        {
            openConnection();
            string sqlCommand = "INSERT INTO " + "Players"
                + " (TeamId, PlayerName, Rating, Accuracy, Shots) VALUES (@teamId, @playerName, @rating, @accuracy, @shots)";
            sqliteCon = new SQLiteConnection(@"Data Source=test.db");
            openConnection();
            SQLiteCommand mycommand = new SQLiteCommand(sqlCommand, sqliteCon);
            mycommand.Parameters.Add("@teamID", DbType.Int32).Value = teamId;
            mycommand.Parameters.Add("@playerName", DbType.String).Value = playerName;
            mycommand.Parameters.Add("@rating", DbType.Int32).Value = rating;
            mycommand.Parameters.Add("@accuracy", DbType.Double).Value = accuracy;
            mycommand.Parameters.Add("@shots", DbType.Int32).Value = shots;
            mycommand.ExecuteNonQuery();
            closeConnection();
        }

        public void clearTableSounds()
        {
            openConnection();

            new SQLiteCommand("DELETE FROM sounds", sqliteCon).ExecuteNonQuery();

            closeConnection();
        }

        public bool AddSound(string name, string url, string size)
        {
            try
            {
                string sqlCommand = "INSERT INTO " + "sounds"
                    + " (Name, Url, Size) VALUES (@name, @url, @size)";
                sqliteCon = new SQLiteConnection(@"Data Source=test.db");
                openConnection();
                SQLiteCommand mycommand = new SQLiteCommand(sqlCommand, sqliteCon);
                mycommand.Parameters.Add("@name", DbType.String).Value = name;
                mycommand.Parameters.Add("@url", DbType.String).Value = url;
                mycommand.Parameters.Add("@size", DbType.String).Value = size;
                mycommand.ExecuteNonQuery();
                closeConnection();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool UpdatePlayer(string name, string rating, string accurancy, string shots, int id)
        {
            try
            {
                string sqlCommand = "update Players set PlayerName = @name, Rating = @rating, Accuracy = @accurancy, Shots = @shots WHERE Players.Id = @id";
                sqliteCon = new SQLiteConnection(@"Data Source=test.db");
                openConnection();
                SQLiteCommand mycommand = new SQLiteCommand(sqlCommand, sqliteCon);
                mycommand.Parameters.Add("@name", DbType.String).Value = name;
                mycommand.Parameters.Add("@rating", DbType.Int32).Value = Convert.ToInt32(rating);
                mycommand.Parameters.Add("@accurancy", DbType.Double).Value = Convert.ToDouble(accurancy);
                mycommand.Parameters.Add("@shots", DbType.Int32).Value = Convert.ToInt32(shots);
                mycommand.Parameters.Add("@id", DbType.Int32).Value = id;
                mycommand.ExecuteNonQuery();
                closeConnection();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void openConnection()
        {
            if(sqliteCon.State != System.Data.ConnectionState.Open)
            {
                sqliteCon.Open();
            }
        }

        public void closeConnection()
        {
            if (sqliteCon.State != System.Data.ConnectionState.Closed)
            {
                sqliteCon.Close();
            }
        }
    }
}
