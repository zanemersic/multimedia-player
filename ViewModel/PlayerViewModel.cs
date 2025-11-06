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

        public ICommand TogglePlayPauseCommand { get; }
        public ICommand InfoCommand { get; }
        public ICommand ExitCommand { get; }

        public PlayerViewModel()
        {
            Playlist = new ObservableCollection<MediaFile>()
            {
                new MediaFile { Title = "90210", Author = "Travis Scott", Path = "Resources/90210.mp4", Cover = "pack://application:,,,/Resources/90210.png" },
                new MediaFile { Title = "No Pole", Author = "Don Toliver", Path = "Resources/NoPole.mp4", Cover = "pack://application:,,,/Resources/nopole.png" },
                new MediaFile { Title = "Snowfall", Author = "Oneheart", Path = "Resources/snowfall.mp4", Cover = "pack://application:,,,/Resources/snowfall.png" }
            };

            InfoCommand = new RelayCommand(_ => ShowFileInfo(), _ => SelectedFile != null);
            ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
            TogglePlayPauseCommand = new RelayCommand(_ => TogglePlayPause());


            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
            timer.Tick += UpdateSlider;
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


        private void ShowFileInfo()
        {
            if (SelectedFile != null)
                MessageBox.Show($"Ime medija: {SelectedFile.Title}", "Podrobnosti");
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
