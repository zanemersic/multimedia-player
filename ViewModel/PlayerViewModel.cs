using MultimedijskiPredvajalnik.Models;
using MultimedijskiPredvajalnik.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MultimedijskiPredvajalnik.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private MediaFile? selectedFile;
        private DispatcherTimer timer;
        private double sliderValue;
        private double sliderMaximum;
        private Uri? currentMediaSource;
        private bool isPlaying;
        private bool isLoopEnabled;
        private bool isShuffleEnabled;
        public ObservableCollection<MediaFile> Playlist { get; set; }


        public MediaFile? SelectedFile
        {
            get => selectedFile;
            set
            {
                if (selectedFile != value)
                {
                    selectedFile = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SliderValue
        {
            get => sliderValue;
            set 
            { 
                sliderValue = value; 
                OnPropertyChanged(); 
            }
        }

        public double SliderMaximum
        {
            get => sliderMaximum;
            set
            {  
                sliderMaximum = value; 
                OnPropertyChanged(); 
            }
        }

        public bool IsLoopEnabled
        {
            get => isLoopEnabled;
            set { isLoopEnabled = value; OnPropertyChanged(); }
        }

        public bool IsShuffleEnabled
        {
            get => isShuffleEnabled;
            set { isShuffleEnabled = value; OnPropertyChanged(); }
        }

        public ICommand TogglePlayPauseCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand LoopCommand { get; }
        public ICommand ShuffleCommand { get; }
        public ICommand ShowFileInfoCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand AddWindowCommand { get; }
        public ICommand EditWindowCommand { get; }
        public ICommand RemoveCommand { get; }  

        public PlayerViewModel()
        {
            Playlist = new ObservableCollection<MediaFile>()
            {
                new MediaFile { Title = "90210", Author = "Travis Scott", Path = "Resources/90210.mp4", Cover = "pack://application:,,,/Resources/90210.png" },
                new MediaFile { Title = "No Pole", Author = "Don Toliver", Path = "Resources/NoPole.mp4", Cover = "pack://application:,,,/Resources/nopole.png" },
                new MediaFile { Title = "Snowfall", Author = "Oneheart", Path = "Resources/snowfall.mp4", Cover = "pack://application:,,,/Resources/snowfall.png" }
            };

            ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
            TogglePlayPauseCommand = new RelayCommand(_ => TogglePlayPause());
            NextCommand = new RelayCommand(_ => PlayNext());
            PreviousCommand = new RelayCommand(_ => PlayPrevious());
            LoopCommand = new RelayCommand(_ => PlayLoop());
            ShuffleCommand = new RelayCommand(_ => PlayShuffle());
            ShowFileInfoCommand = new RelayCommand(ShowFileInfo);
            AddCommand = new RelayCommand(_ => AddMediaFile());
            AddWindowCommand = new RelayCommand(_ => OpenAddWindow());
            EditWindowCommand = new RelayCommand(_ => OpenEditWindow(), _ => SelectedFile != null);
            EditCommand = new RelayCommand(_ => EditSelectedFile(), _ => SelectedFile != null);
            RemoveCommand = new RelayCommand(_ => RemoveSelectedFile(), _ => SelectedFile != null);

            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
            timer.Tick += UpdateSlider;
        }
        public void PlayNext()
        {
            if (Playlist.Count == 0) return;
            int index = Playlist.IndexOf(SelectedFile);

            if (IsShuffleEnabled)
            {
                if (Playlist.Count == 1) return;

                var rnd = new Random();
                int newIndex;

                do
                {
                    newIndex = rnd.Next(Playlist.Count);
                } while (newIndex == index);

                index = newIndex;
            }
            else
            {
                index++;
                if (index >= Playlist.Count)
                {
                    if (IsLoopEnabled)
                        index = 0;
                    else
                        return;
                }
            }

            SelectedFile = Playlist[index];
            TogglePlayPause();
        }

        private void PlayPrevious()
        {
            if (Playlist.Count == 0) return;
            int index = Playlist.IndexOf(SelectedFile);

            if (IsShuffleEnabled)
            {
                if (Playlist.Count == 1) return;
                var rnd = new Random();
                int newIndex;

                do
                {
                    newIndex = rnd.Next(Playlist.Count);
                } while (newIndex == index);

                index = newIndex;
            }
            else
            {
                index--;
                if (index < 0)
                {
                    if (IsLoopEnabled)
                        index = Playlist.Count - 1;
                    else
                        return;
                }
            }

            SelectedFile = Playlist[index];
            TogglePlayPause();
        }


        private void PlayLoop()
        {
            IsLoopEnabled = !IsLoopEnabled;
        }

        private void PlayShuffle()
        {
            IsShuffleEnabled = !IsShuffleEnabled;
        }

        private void OpenAddWindow()
        {
            var window = new MultimedijskiPredvajalnik.AddWindow();
            if (window.ShowDialog() == true && window.NewFile != null)
            {
                Playlist.Add(window.NewFile);
            }
        }

        private void OpenEditWindow()
        {
            if (SelectedFile == null) return;

            var window = new MultimedijskiPredvajalnik.EditWindow(SelectedFile);
            window.Show();
        }

        private void EditSelectedFile()
        {
            if (SelectedFile == null) return;
            SelectedFile.Title = "Never gonna give you up";
            SelectedFile.Author = "Rick Astley";
            SelectedFile.Path = "Resources/rickroll.mp4";
            SelectedFile.Cover = "pack://application:,,,/Resources/rickcover.png";
            OnPropertyChanged();
        }

        private void RemoveSelectedFile()
        {
            if (SelectedFile != null)
            {
                Playlist.Remove(SelectedFile);
            }
        }

        private void AddMediaFile()
        {
            var newFile = new MediaFile
            {
                Title = "2003",
                Author = "TNT",
                Path = "Resources/2003.mp4",
                Cover = "pack://application:,,,/Resources/tnt.png"
            };

            Playlist.Add(newFile);
        }

        private void ShowFileInfo(object parameter)
        {
            if (parameter is MediaFile file)
            {
                MessageBox.Show($"Title: {file.Title}\nAuthor: {file.Author}", "Information");
            }
        }

        private void TogglePlayPause()
        {
            if (SelectedFile == null) return;

            string fullPath = System.IO.Path.GetFullPath(SelectedFile.Path);
            Uri newSource = new Uri(fullPath, UriKind.Absolute);

            if (CurrentMediaSource == null || CurrentMediaSource.ToString() != newSource.ToString())
            {
                foreach (var file in Playlist)
                    file.IsPlaying = false;

                SelectedFile.IsPlaying = true;

                CurrentMediaSource = newSource;
                IsPlaying = true;
                return;
            }

            IsPlaying = !IsPlaying;

            SelectedFile.IsPlaying = IsPlaying;
        }


        private TimeSpan currentPosition;
        public TimeSpan CurrentPosition
        {
            get => currentPosition;
            set
            {
                if (currentPosition != value)
                {
                    currentPosition = value;
                    SliderValue = value.TotalSeconds;
                    OnPropertyChanged();
                }
            }
        }


        private void UpdateSlider(object? sender, EventArgs e)
        {
            if (IsPlaying)
            {
                SliderValue = CurrentPosition.TotalSeconds;
            }
        }

        public Uri? CurrentMediaSource
        {
            get => currentMediaSource;
            set
            {
                if (currentMediaSource != value)
                {
                    currentMediaSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;
                    OnPropertyChanged();
                }
            }
        }

        private string currentTimeDisplay = "00:00";
        public string CurrentTimeDisplay
        {
            get => currentTimeDisplay;
            set
            {
                if (currentTimeDisplay != value)
                {
                    currentTimeDisplay = value;
                    OnPropertyChanged();
                }
            }
        }

        private string totalTimeDisplay = "00:00";
        public string TotalTimeDisplay
        {
            get => totalTimeDisplay;
            set
            {
                if (totalTimeDisplay != value)
                {
                    totalTimeDisplay = value;
                    OnPropertyChanged();
                }
            }
        }


    }
}
