using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace spotlight
{
    public partial class MainWindow : Window
    {
        private List<FileInformation> FileInformations;

        public MainWindow()
        {
            InitializeComponent();

            var searchEngine = new SearchEngine();
            var files = searchEngine.GetFileListDeep("C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs");
            List<FileInformation> items = new List<FileInformation>();
            foreach (string file in files)
            {
                items.Add(new FileInformation(file));
            }
            FileInformations = items;
            listBox.ItemsSource = items;
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            FileInformation fileInformation = (FileInformation) ((Grid) sender).DataContext;
            Process.Start(fileInformation.FileLocation);
        }

        private void OnSearchInput(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox) sender).Text;
            listBox.ItemsSource = FileInformations.Where(file => (file.DisplayName.ToLower().StartsWith(text.ToLower())));
        }
    }
}