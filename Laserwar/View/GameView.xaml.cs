using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using Laserwar.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using iTextSharp.text.pdf;
using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.html;
using System.Windows.Documents;
using System.IO.Packaging;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Drawing.Printing;

namespace Laserwar.View
{
    /// <summary>
    /// Логика взаимодействия для GameView.xaml
    /// </summary>
    public partial class GameView : Page
    {
        List<Players> players = new List<Players>(7);
        int currentGameId = 1;
        ListCollectionView collectionView;

        public GameView(string gameName, int gameId)
        {
            InitializeComponent();
            gameLabel.Content = gameName;
            currentGameId = gameId;
        }

        private void gameGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            database.openConnection();

            string sqlCommand = "SELECT Players.Id AS PlayerId, Players.PlayerName, Players.Rating, Players.Accuracy, Players.Shots, Teams.TeamName, Players.TeamId AS TeamId, Games.Id from Teams, Players, Games " +
                                "WHERE Games.Id = Teams.GameId and Games.Id = @currentGameId and Players.TeamId = Teams.Id " +
                                "GROUP BY Players.Id";
            SQLiteConnection sqliteCon = new SQLiteConnection(@"Data Source=test.db");

            SQLiteCommand cmd = new SQLiteCommand(sqlCommand, database.sqliteCon);
            SQLiteParameter param = new SQLiteParameter("@currentGameId", DbType.Int32);
            param.Value = currentGameId;
            cmd.Parameters.Add(param);
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                int PlayerId;
                int TeamId;
                string PlayerName = String.Empty;
                string Rating = String.Empty;
                string Accuracy = String.Empty;
                string Shots = String.Empty;
                string TeamName = String.Empty;
                string GameId = String.Empty;
                
                while (r.Read())
                {
                    PlayerId = Convert.ToInt32(r["PlayerId"]);
                    PlayerName = r["PlayerName"].ToString();
                    Rating = r["Rating"].ToString();
                    Accuracy = r["Accuracy"].ToString();
                    Shots = r["Shots"].ToString();
                    TeamName = r["TeamName"].ToString();
                    GameId = r["Id"].ToString();
                    TeamId = Convert.ToInt32(r["TeamId"]);

                    //Console.WriteLine(PlayerName + " " + Rating + " " + Accuracy + " " + Shots + " " + TeamName + " " + GameId);
                    players.Add(new Players(TeamId, PlayerId, PlayerName, GameId, TeamName, Convert.ToInt32(Rating), Accuracy, Convert.ToInt32(Shots)));
                }
                r.Close();
                Console.WriteLine(players.Count);
                
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            collectionView = new ListCollectionView(players);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("TeamName"));

            gameGrid.ItemsSource = collectionView;
        }

        private void Back_Clicked(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GamesView());
        }

        private void gameGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var SelectPlayer = players[gameGrid.SelectedIndex];
            EditPlayerWindow window = new EditPlayerWindow();
            window.PlayerName.Text = SelectPlayer.PlayerName;
            window.Rating.Text = SelectPlayer.Rating.ToString();
            window.Accurancy.Text = SelectPlayer.Accurancy;
            window.Shots.Text = SelectPlayer.Shots.ToString();
            window.id = SelectPlayer.Id;
            window.gameid = Convert.ToInt32(SelectPlayer.GameId);
            window.gamename = gameLabel.Content.ToString();
            border.Visibility = Visibility.Visible;
            window.ShowDialog();
            border.Visibility = Visibility.Collapsed;
            this.NavigationService.Navigate(new GameView(gameLabel.Content.ToString(), currentGameId));
        }

        public static RenderTargetBitmap GetImage(UIElement view)
        {
            Size size = new Size(view.RenderSize.Width, view.RenderSize.Height);
            if (size.IsEmpty)
                return null;

            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 100, 100, PixelFormats.Pbgra32);

            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(view), null, new Rect(0, 0, (int)size.Width, (int)size.Height));
                context.Close();
            }

            result.Render(drawingvisual);
            return result;
        }

        public static void createPdfFromImage(string imageFile, string pdfFile)
        {
            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER.Rotate(), 0, 0, 0, 0);
                PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();

                FileStream fs = new FileStream(imageFile, FileMode.Open);
                var image = iTextSharp.text.Image.GetInstance(fs);
                image.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                document.Add(image);
                document.Close();

                //open pdf file
                Process.Start("explorer.exe", pdfFile);
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
       where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }

            return null;
        }

        public static void SaveAsPng(RenderTargetBitmap src, string targetFile)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            using (var stm = System.IO.File.Create(targetFile))
            {
                encoder.Save(stm);
            }
        }

        private void pdf_Click(object sender, RoutedEventArgs e)
        {
                        string sPDFFileName = System.IO.Path.GetTempPath() + "PDFFile.pdf";
                        string sImagePath = System.IO.Path.GetTempPath() + "window.png";

                        SaveAsPng(GetImage(this), sImagePath);
                        createPdfFromImage(sImagePath, sPDFFileName);
                        
        }
        
    }
    
}