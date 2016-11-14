using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace spotlight
{
    public partial class MainWindow : MetroWindow
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
            string text = ((TextBox) sender).Text.ToLower();
            listBox.ItemsSource = FileInformations.Where(file =>
            {
                string[] strings = file.DisplayName.ToLower().Split(' ');
                foreach (string s in strings)
                {
                    if (s.StartsWith(text))
                        return true;
                }
                return false;
            });
        }

        private void MainInputBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                listBox.Focus();
            }
        }
    }
}