﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Laserwar.Model;
using System.Data.SQLite;

namespace Laserwar.View
{
    /// <summary>
    /// Логика взаимодействия для DownloadView.xaml
    /// </summary>
    public partial class DownloadView : UserControl
    {

        public string data;

        public DownloadView()
        {
            InitializeComponent();
        }

        private void downloadFile_Click(object sender, RoutedEventArgs e)
        {
            string adress = AdressField.Text;
            WebClient c = new WebClient();
            try
            {
                data = c.DownloadString(adress);
                JObject o = JObject.Parse(data);
                if (o != null)
                {
                    SuccesLabel.Visibility = Visibility.Visible;
                    
                    Database database = new Database();

                    JArray array = (JArray)o["sounds"];
                    if(array.Count > 0)
                    {
                        database.clearTableSounds();
                    }
                    Console.WriteLine(array.Count);
                   
                    for (int i=0;i<array.Count;i++)
                    {
                        var name = (string)array[i]["name"];
                        var url = (string)array[i]["url"];
                        var size = (string)array[i]["size"];
                        database.AddSound(name, url, size);
                    }

                    JArray arrayGames = (JArray)o["games"];
                    for (int i = 0; i < array.Count; i++)
                    {
                        var url = (string)array[i]["url"];
                        data = c.DownloadString(url);
                        JObject g = JObject.Parse(data);
                        if (g != null)
                        {
                            var NameGame = g["name"].ToString();
                            var DateGame = g["date"].ToString();
                        }
                    }




                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
        }
    }
}
