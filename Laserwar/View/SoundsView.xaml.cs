using Laserwar.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Laserwar.View
{
    /// <summary>
    /// Логика взаимодействия для SoundsView.xaml
    /// </summary>
    public partial class SoundsView : Page
    {
        string soundName = String.Empty;
        List<Sounds> sounds = new List<Sounds>(3);
        MediaPlayer sp = new MediaPlayer();

        public SoundsView()
        {
            InitializeComponent();
        }



        private void gridSounds_Loaded(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            database.openConnection();

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
                    Size = (Convert.ToInt32(Size) / 1024) + "Kb";
                    sounds.Add(new Sounds(Name, Url, Size));
                }
                r.Close();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            soundsGrid.ItemsSource = sounds;

            for (int i = 1; i <= soundsGrid.Items.Count; i++)
            {
                soundName = GlobalDefines.GetFrameworkElement<TextBlock>(GlobalDefines.GetCell(i - 1, 0, soundsGrid)).Text;
                if (File.Exists(soundName + ".wav"))
                {
                    InstallImageButton(@"Resourses/Images/downloaded_sound.png", GlobalDefines.GetFrameworkElement<Image>(GlobalDefines.GetCell(i - 1, 2, soundsGrid)));
                    GlobalDefines.GetFrameworkElement<Button>(GlobalDefines.GetCell(i - 1, 2, soundsGrid)).IsEnabled = false;
                    GlobalDefines.GetFrameworkElement<TextBlock>(GlobalDefines.GetCell(i - 1, 2, soundsGrid)).Visibility = Visibility.Visible;
                    InstallImageButton(@"Resourses/Images/play.png", GlobalDefines.GetFrameworkElement<Image>(GlobalDefines.GetCell(i - 1, 3, soundsGrid)));
                    GlobalDefines.GetFrameworkElement<Button>(GlobalDefines.GetCell(i - 1, 3, soundsGrid)).IsEnabled = true;
                }
            }
        }



        private void DownloadSound_Click(object sender, RoutedEventArgs e)
        {
            InstallImageButton(@"Resourses/Images/downloading_sound.png", GlobalDefines.GetFrameworkElement<Image>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)));

            string Url = String.Empty;
            string Name = String.Empty;
            int index = soundsGrid.SelectedIndex;
            Database database = new Database();
            database.openConnection();
            string sqlCommand = "SELECT Name, Url FROM sounds WHERE Id=" + (index + 1);
            SQLiteCommand cmd = new SQLiteCommand(sqlCommand, database.sqliteCon);
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Url = r["Url"].ToString();
                    Name = r["Name"].ToString();
                }
                r.Close();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }

            string fileName = Name;
            if (!File.Exists(fileName))
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(Url), fileName + ".wav");
            }
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //то что будет после скачивания файла
            GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)).Visibility = Visibility.Collapsed;
            GlobalDefines.GetFrameworkElement<TextBlock>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)).Visibility = Visibility.Visible;
            GlobalDefines.GetFrameworkElement<TextBlock>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)).HorizontalAlignment = HorizontalAlignment.Left;

            InstallImageButton(@"Resourses/Images/downloaded_sound.png", GlobalDefines.GetFrameworkElement<Image>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)));
            InstallImageButton(@"Resourses/Images/play.png", GlobalDefines.GetFrameworkElement<Image>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)));

            GlobalDefines.GetFrameworkElement<Button>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)).IsEnabled = false;
            GlobalDefines.GetFrameworkElement<Button>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)).IsEnabled = true;
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            GlobalDefines.GetFrameworkElement<Button>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)).IsEnabled = false;
            GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)).Visibility = Visibility.Visible;
            GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)).Maximum = (int)e.TotalBytesToReceive / 100;
            GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 2, soundsGrid)).Value = (int)e.BytesReceived / 100;
        }

        private void DownloadImage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void InstallImageButton(string imagePath, Image image)
        {
            string imgPath = System.IO.Path.GetFullPath(imagePath);
            Uri imgUri = new Uri(imgPath);
            BitmapImage bitmap = new BitmapImage(imgUri);
            image.Source = bitmap;
        }

        public int tempIndex = -1;
        public bool tempStatus = true;

        private void PlaySound_Click(object sender, RoutedEventArgs e)
        {
            if (tempIndex == soundsGrid.SelectedIndex && !tempStatus)
            {
                sp.Stop();
                tempStatus = !tempStatus;
                InstallImageButton(@"Resourses/Images/play.png", GlobalDefines.GetFrameworkElement<Image>(GlobalDefines.GetCell(tempIndex, 3, soundsGrid)));
                GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(tempIndex, 3, soundsGrid)).Visibility = Visibility.Collapsed;
                GlobalDefines.GetFrameworkElement<Label>(GlobalDefines.GetCell(tempIndex, 3, soundsGrid)).Visibility = Visibility.Collapsed;

                return;
            }

            if (sp.Source != null && tempIndex >= 0)
            {
                InstallImageButton(@"Resourses/Images/play.png", GlobalDefines.GetFrameworkElement<Image>(GlobalDefines.GetCell(tempIndex, 3, soundsGrid)));
                GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(tempIndex, 3, soundsGrid)).Visibility = Visibility.Hidden;
                GlobalDefines.GetFrameworkElement<Label>(GlobalDefines.GetCell(tempIndex, 3, soundsGrid)).Visibility = Visibility.Hidden;

                sp.Stop();
            }



            tempStatus = !tempStatus;
            tempIndex = soundsGrid.SelectedIndex;
            InstallImageButton(@"Resourses/Images/stop.png", GlobalDefines.GetFrameworkElement<Image>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)));



            string sound = GlobalDefines.GetFrameworkElement<TextBlock>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 0, soundsGrid)).Text;
            string soundPath = System.IO.Path.GetFullPath(sound + ".wav");
            Uri uri = new Uri(soundPath);
            sp.Open(uri);
            sp.Play();

            GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)).Visibility = Visibility.Visible;
            GlobalDefines.GetFrameworkElement<Label>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)).Visibility = Visibility.Visible;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {

            if ((sp.Source != null) && (sp.NaturalDuration.HasTimeSpan))
            {
                GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)).Minimum = 0;
                GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)).Maximum = sp.NaturalDuration.TimeSpan.TotalSeconds;
                GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)).Value = sp.Position.TotalSeconds;
                GlobalDefines.GetFrameworkElement<Label>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)).Content = String.Format("{0}", sp.Position.ToString(@"mm\:ss"));

                if (sp.Position.TotalSeconds >= sp.NaturalDuration.TimeSpan.TotalSeconds)
                {
                    InstallImageButton(@"Resourses/Images/play.png", GlobalDefines.GetFrameworkElement<Image>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)));
                    GlobalDefines.GetFrameworkElement<ProgressBar>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)).Visibility = Visibility.Hidden;
                    GlobalDefines.GetFrameworkElement<Label>(GlobalDefines.GetCell(soundsGrid.SelectedIndex, 3, soundsGrid)).Visibility = Visibility.Hidden;
                }
            }
        }
    }


    static public class GlobalDefines
    {
        public static T GetFrameworkElement<T>(FrameworkElement referenceElement) where T : FrameworkElement
        {
            FrameworkElement child = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(referenceElement); i++)
            {
                child = VisualTreeHelper.GetChild(referenceElement, i) as FrameworkElement;

                if (child != null)
                    if (child.GetType() == typeof(T))
                        break;
                    else
                    {
                        child = GetFrameworkElement<T>(child);
                        if (child != null && child.GetType() == typeof(T))
                            break;
                    }
            }

            return child as T;
        }

        public static FrameworkElement GetFrameworkElement(FrameworkElement referenceElement, Type NeedType)
        {
            FrameworkElement child = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(referenceElement); i++)
            {
                child = VisualTreeHelper.GetChild(referenceElement, i) as FrameworkElement;

                if (child != null)
                    if (child.GetType() == NeedType)
                        break;
                    else
                    {
                        child = GetFrameworkElement(child, NeedType);
                        if (child != null && child.GetType() == NeedType)
                            break;
                    }
            }

            return child;
        }

        public static DataGridRow GetRow(int index, DataGrid grid)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);

            if (row == null)
            {
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static DataGridCell GetCell(int row, int column, DataGrid grid)
        {
            DataGridRow rowContainer = GetRow(row, grid);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GlobalDefines.GetFrameworkElement<DataGridCellsPresenter>(rowContainer);

                if (presenter == null)
                {
                    grid.ScrollIntoView(rowContainer, grid.Columns[column]);
                    presenter = GlobalDefines.GetFrameworkElement<DataGridCellsPresenter>(rowContainer);

                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);

                return cell;
            }

            return null;
        }
    }

}
