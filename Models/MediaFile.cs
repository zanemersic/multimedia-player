using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MultimedijskiPredvajalnik.Models
{
    public class MediaFile : INotifyPropertyChanged
    {
        private string title;
        private string author;
        private string path;
        private string cover;
        private double duration;
        private bool isPlaying;

        public string Title
        {
            get => title;
            set
            {
                if (title != value) {
                    title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Author
        {
            get => author;
            set
            {
                if (author != value)
                {
                    author = value;
                    OnPropertyChanged(nameof(Author));
                }
            }
        }

        public string Path
        {
            get => path;
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        public string Cover
        {
            get => cover;
            set
            {
                if (cover != value)
                {
                    cover = value;
                    OnPropertyChanged(nameof(Cover));
                }
            }
        }

        public double Duration
        {
            get => duration;
            set
            {
                if (duration != value)
                {  
                    duration = value;
                    OnPropertyChanged(nameof(Duration));
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
                    OnPropertyChanged(nameof(IsPlaying));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
