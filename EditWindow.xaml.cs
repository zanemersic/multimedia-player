using Microsoft.Win32;
using MultimedijskiPredvajalnik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultimedijskiPredvajalnik
{
    public partial class EditWindow : Window
    {
        private MediaFile file;
        private string? selectedVideoPath;
        private string? selectedImagePath;

        public EditWindow(MediaFile fileToEdit)
        {
            InitializeComponent();
            file = fileToEdit;
            TitleBox.Text = file.Title;
            AuthorBox.Text = file.Author;
            FilePathText.Text = file.Path;

            try
            {
                if (!string.IsNullOrWhiteSpace(file.Cover))
                    CoverPreview.Source = new BitmapImage(new Uri(file.Cover, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                CoverPreview.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/default.png"));
            }
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Video Files|*.mp4;*.mkv;*.avi|All Files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                selectedVideoPath = dlg.FileName;
                FilePathText.Text = dlg.FileName;
            }
        }

        private void CoverPreview_Click(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image Files|*.jpeg;*.jpg;*.png;*.bmp;*.gif"
            };

            if (dlg.ShowDialog() == true)
            {
                selectedImagePath = dlg.FileName;
                CoverPreview.Source = new BitmapImage(new Uri(selectedImagePath));
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            file.Title = TitleBox.Text;
            file.Author = AuthorBox.Text;

            if (!string.IsNullOrWhiteSpace(selectedVideoPath))
                file.Path = selectedVideoPath;

            if (!string.IsNullOrWhiteSpace(selectedImagePath))
                file.Cover = selectedImagePath;

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
