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
    public partial class AddWindow : Window
    {
        public MediaFile? NewFile { get; private set; }
        public AddWindow()
        {
            InitializeComponent();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Video files|*.mp4;*.mkv;*.avi|All files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                FilePathText.Text = dialog.FileName;
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text) || string.IsNullOrWhiteSpace(FilePathText.Text))
            {
                MessageBox.Show("Fill every field to proceed.");
                return;
            }

            NewFile = new MediaFile
            {
                Title = TitleBox.Text,
                Author = AuthorBox.Text,
                Path = FilePathText.Text,
                Cover = string.IsNullOrWhiteSpace(selectedImagePath)
                        ? "pack://application:,,,/Icons/cover.png"
                        : selectedImagePath
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CoverPreview_Click(object sender, MouseButtonEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpeg;*.jpg;*.png;*.bmp;*.gif"
            };

            if (dlg.ShowDialog() == true)
            {
                CoverPreview.Source = new BitmapImage(new Uri(dlg.FileName));
                selectedImagePath = dlg.FileName;
            }
        }

        private string? selectedImagePath;
    }
}
