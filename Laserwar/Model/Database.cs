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

        public bool Add(string name, string url, string size)
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
