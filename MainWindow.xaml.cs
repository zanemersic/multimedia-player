using MultimedijskiPredvajalnik.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace MultimedijskiPredvajalnik
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        public ObservableCollection<MediaFile> Playlist { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Playlist = new ObservableCollection<MediaFile>
            {
                new MediaFile { Title = "90210", Author = "Travis Scott", Path = "Resources/90210.mp4", Cover = "pack://application:,,,/Resources/90210.png"},
                new MediaFile { Title = "No Pole", Author = "Don Toliver", Path = "Resources/NoPole.mp4", Cover = "pack://application:,,,/Resources/nopole.png"},
                new MediaFile { Title = "Snowfall", Author = "Oneheart", Path = "Resources/snowfall.mp4", Cover = "pack://application:,,,/Resources/snowfall.png" }
            };

            DataContext = this;

            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            timer.Tick += UpdateSlider;

            Slider.Loaded += (s, e) =>
            {
                if (Slider.Template.FindName("PART_Track", Slider) is Track track &&
                    track.Thumb != null)
                {
                    track.Thumb.DragCompleted += ProgressSlider_DragCompleted;
                }
            };
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Playlist_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((ListView)sender).SelectedItem is MediaFile file)
            {
                foreach (var f in Playlist)
                    f.IsPlaying = false;

                file.IsPlaying = true;

                try
                {
                    string fullPath = System.IO.Path.GetFullPath(file.Path);
                    MediaPlayer.Source = new Uri(fullPath, UriKind.Absolute);
                    MediaPlayer.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Napaka pri predvajanju: {ex.Message}", "Napaka", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Playlist_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is MediaFile file)
            {
                MessageBox.Show($"Ime medija: {file.Title}", "Podrobnosti", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Play();
            Slider.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            timer.Start();
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Napaka pri nalaganju videa: {e.ErrorException.Message}", "Napaka", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void UpdateSlider(object sender, EventArgs e)
        {
            if (MediaPlayer.Source != null && MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                Slider.Value = MediaPlayer.Position.TotalSeconds;
            }
        }

        private void ProgressSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            MediaPlayer.Position = TimeSpan.FromSeconds(Slider.Value);
        }
    }
}
