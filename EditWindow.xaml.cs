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
        public EditWindow(MediaFile fileToEdit)
        {
            InitializeComponent();
            file = fileToEdit;

            TitleBox.Text = file.Title;
            AuthorBox.Text = file.Author;
            FilePathText.Text = file.Path;
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
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
            file.Title = TitleBox.Text;
            file.Author = AuthorBox.Text;
            if (!string.IsNullOrWhiteSpace(FilePathText.Text))
            {
                file.Path = FilePathText.Text;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close(); 
        }
    }
}
