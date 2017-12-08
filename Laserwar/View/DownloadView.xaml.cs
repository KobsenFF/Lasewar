using Laserwar.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Laserwar.View
{
    /// <summary>
    /// Логика взаимодействия для DownloadView.xaml
    /// </summary>
    public partial class DownloadView : Page
    {
        public WebClient c = new WebClient();
        public string data;
        public string adress;
        int teamid = 1;
        Database database = new Database();
        List<Games> games = new List<Games>();

        public DownloadView()
        {
            InitializeComponent();
        }

        private void ApplyEffect()
        {
            GrayBorder.Dispatcher.BeginInvoke(new Action(delegate { GrayBorder.Visibility = Visibility.Visible; }));
            Spinner.Dispatcher.BeginInvoke(new Action(delegate { Spinner.Visibility = Visibility.Visible; }));
            
        }

        private void ClearEffect()
        {
            GrayBorder.Dispatcher.BeginInvoke(new Action(delegate { GrayBorder.Visibility = Visibility.Hidden; }));
            Spinner.Dispatcher.BeginInvoke(new Action(delegate { Spinner.Visibility = Visibility.Hidden; }));
        }

        private void downloadFile_Click(object sender, RoutedEventArgs e)
        {
            GrayBorder.Visibility = Visibility.Visible;
            adress = AdressField.Text;
            
            Download();
           
            GrayBorder.Visibility = Visibility.Collapsed;
        }

        private void Download()
        {
            try
            {
                data = c.DownloadString(adress);
                JObject o = JObject.Parse(data);

                if (o != null)
                {
                    SuccesLabel.Visibility = Visibility.Visible;
                    JArray array = (JArray)o["sounds"];
                    if (array.Count > 0)
                    {
                        database.clearTableSounds();
                    }

                    for (int i = 0; i < array.Count; i++)
                    {
                        var name = (string)array[i]["name"];
                        var url = (string)array[i]["url"];
                        var size = (string)array[i]["size"];
                        database.AddSound(name, url, size);
                    }

                    JArray arrayGames = (JArray)o["games"];

                    for (int i = 0; i < arrayGames.Count; i++)
                    {
                        Uri Url = new Uri(arrayGames[i]["url"].ToString());
                        data = c.DownloadString(Url);
                        data = Win1251ToUTF8(data);
                        parseXmlGames(data);
                        parseXmlTeam(data, i + 1);
                    }
                }
            }

            catch (Exception ex)
            {
                ErrorWindow errorWindow = new ErrorWindow();
                errorWindow.ErrorLabel.Content = ex.Message;
                errorWindow.ShowDialog();
                ClearEffect();
            }
        }

        private string Win1251ToUTF8(string source)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(data);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);

            data = win1251.GetString(win1251Bytes);

            return data;
        }

        private void parseXmlGames(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            foreach (XmlNode n in doc.SelectNodes("/game"))
            {
                var name = n.Attributes["name"].Value;
                var date = n.Attributes["date"].Value;

                database.AddGames(name, date);
            }
        }

        private void parseXmlTeam(string xml, int i)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            foreach (XmlNode n in doc.SelectNodes("/game/team"))
            {
                var game = n.ParentNode.Attributes["name"].Value;
                var name = n.Attributes["name"].Value;

                database.AddTeams(i, name);

                foreach (XmlNode x in n.ChildNodes)
                {
                    var team = x.ParentNode.Attributes["name"].Value;
                    var playername = x.Attributes["name"].Value;
                    var rating = x.Attributes["rating"].Value;
                    var accuracy = x.Attributes["accuracy"].Value;
                    var shots = x.Attributes["shots"].Value;
                    Console.WriteLine(team + " " + teamid + " " + playername + " " + rating + " " + accuracy + " " + shots);
                    database.AddPlayer(teamid, playername, Convert.ToInt32(rating), Convert.ToDouble(accuracy, System.Globalization.CultureInfo.InvariantCulture), Convert.ToInt32(shots));
                }
                teamid++;
            }
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}