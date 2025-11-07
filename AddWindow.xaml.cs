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
                MessageBox.Show("Enter title and author of the file");
                return;
            }

            NewFile = new MediaFile
            {
                Title = TitleBox.Text,
                Author = AuthorBox.Text,
                Path = FilePathText.Text,
                Cover = "pack://application:,,,/Icons/cover.png"
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
