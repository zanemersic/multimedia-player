using MultimedijskiPredvajalnik.Controllers;
using MultimedijskiPredvajalnik.Models;
using MultimedijskiPredvajalnik.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Speech.Synthesis;

using MultimedijskiPredvajalnik.Core;
using System.Diagnostics;

namespace MultimedijskiPredvajalnik
{
    public partial class MainWindow : Window
    {

        private PlayerViewModel vm;
        private readonly SpeechController speech;
        private readonly SpeechSynthesizer voice = new();

        public MainWindow()
        {
            InitializeComponent();

            speech = new SpeechController();
            speech.SpeechRecognized += OnSpeechCommandRecognized;
            speech.Start();

            voice.Rate = 0;
            voice.Volume = 80;

            vm = new PlayerViewModel();
            DataContext = vm;

            vm.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(vm.IsPlaying))
                {
                    if (vm.IsPlaying)
                        MediaPlayer.Play();
                    else
                        MediaPlayer.Pause();
                }
                else if (args.PropertyName == nameof(vm.CurrentMediaSource))
                {
                    if (vm.CurrentMediaSource != null)
                    {
                        MediaPlayer.Source = vm.CurrentMediaSource;
                        MediaPlayer.Play();
                    }
                }
            };
        }

        private void OnSpeechCommandRecognized(VoiceCommand command)
        {
            Dispatcher.Invoke(() =>
            {
                if (DataContext is not PlayerViewModel vm)
                    return;

                if (vm.SelectedFile == null)
                {
                    HandleRunning(command, vm);
                } else
                {
                    HandleSelected(command, vm);
                }
            });
        }

        private void HandleRunning(VoiceCommand command, PlayerViewModel vm)
        {
            switch (command)
            {
                case VoiceCommand.Play:
                    vm.TogglePlayPauseCommand.Execute(null);
                    break;

                case VoiceCommand.Stop:
                    if (!vm.IsPlaying) break;
                    vm.TogglePlayPauseCommand.Execute(null);
                    break;

                case VoiceCommand.Next:
                    vm.NextCommand.Execute(null);
                    Speak("Playing next media");
                    break;

                case VoiceCommand.Previous:
                    vm.PreviousCommand.Execute(null);
                    Speak("Playing next media");
                    break;

                case VoiceCommand.Select:
                    if (vm.Playlist.Count > 0)
                    {
                        PlaylistView.SelectedIndex = 0;
                        Speak("Selected Media");
                    }
                    break;

                case VoiceCommand.Add:
                    vm.AddCommand.Execute(null);
                    Speak("Added new media");
                    break;


                case VoiceCommand.Exit:
                    vm.ExitCommand.Execute(null);
                    break;
            }
        }

        private void HandleSelected(VoiceCommand command, PlayerViewModel vm)
        {
            switch (command)
            {
                case VoiceCommand.Play:
                    vm.TogglePlayPauseCommand.Execute(null);
                    break;

                case VoiceCommand.Stop:
                    if (!vm.IsPlaying) break;
                    vm.TogglePlayPauseCommand.Execute(null);
                    break;

                case VoiceCommand.Next:
                    vm.NextCommand.Execute(null);
                    Speak("Playing next media");
                    break;

                case VoiceCommand.Previous:
                    vm.PreviousCommand.Execute(null);
                    Speak("Playing previous media");
                    break;

                case VoiceCommand.Remove:
                    vm.RemoveCommand.Execute(null);
                    Speak("Removed media");
                    break;

                case VoiceCommand.Add:
                    vm.AddCommand.Execute(null);
                    Speak("Added new media");
                    break;

                case VoiceCommand.Edit:
                    vm.EditCommand.Execute(null);
                    Speak("Edited selected media");
                    break;

                case VoiceCommand.Exit:
                    vm.ExitCommand.Execute(null);
                    break;
            }
        }

        private void Speak(string text)
        {
            voice.SpeakAsyncCancelAll();
            voice.SpeakAsync(text);
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            vm.CurrentTimeDisplay = "00:00";

            if (MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                vm.SliderMaximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                vm.TotalTimeDisplay = MediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
            else
            {
                vm.SliderMaximum = 0;
                vm.TotalTimeDisplay = "00:00";
            }

            MediaPlayer.Play();
            vm.IsPlaying = true;

            MediaPlayer.Volume = vm.Volume / 100.0;

            vm.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(vm.Volume))
                    MediaPlayer.Volume = vm.Volume / 100.0;
            };

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (s, _) =>
            {
                if (vm.IsPlaying && !isDragging)
                {
                    vm.CurrentPosition = MediaPlayer.Position;

                    vm.CurrentTimeDisplay = MediaPlayer.Position.ToString(@"mm\:ss");

                    if (MediaPlayer.NaturalDuration.HasTimeSpan)
                        vm.TotalTimeDisplay = MediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                }
            };
            timer.Start();

        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(MediaPlayer.Position.TotalSeconds - vm.SliderValue) > 1)
                MediaPlayer.Position = TimeSpan.FromSeconds(vm.SliderValue);
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (vm.IsLoopEnabled)
            {
                MediaPlayer.Position = TimeSpan.Zero;
                MediaPlayer.Play();
            }
            else
            {
                vm.PlayNext();
            }
        }

        private void Mute_Click(object sender, MouseButtonEventArgs e)
        {
            if (vm.Volume > 0)
                vm.Volume = 0;
            else
                vm.Volume = 100;
            MediaPlayer.Volume = vm.Volume / 100.0;
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Error while loading video: {e.ErrorException.Message}");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            vm.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(vm.IsPlaying))
                {
                    if (vm.IsPlaying)
                        MediaPlayer.Play();
                    else
                        MediaPlayer.Pause();
                }
            };
        }
        private void ListViewItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is PlayerViewModel vm)
                vm.TogglePlayPauseCommand.Execute(null);
        }

        private bool isDragging = false;

        private void ProgressSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDragging = true;
        }

        private void ProgressSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDragging = false;
            MediaPlayer.Position = TimeSpan.FromSeconds(vm.SliderValue);
        }

        private void ListViewItem_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.DataContext is MediaFile file)
            {
                var vm = DataContext as PlayerViewModel;
                vm?.ShowFileInfoCommand.Execute(file);
            }
        }



    }
}
