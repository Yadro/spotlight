using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using spotlight.ListItem;

namespace spotlight
{
    public partial class MainWindow : MetroWindow
    {
        private List<SearchListItem> FileInformations;

        public MainWindow()
        {
            InitializeComponent();

            var searchEngine = new SearchEngine();
            var files = searchEngine.GetFileListDeep("C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs");
            List<SearchListItem> items = new List<SearchListItem>();
            items.Add(new Groups("Лучшее соответсвтие"));
            int i = 0;
            foreach (string file in files)
            {
                if (i % 3 == 0 || i % 4 == 0)
                    items.Add(new FileInformationSmall(file));
                else
                    items.Add(new FileInformation(file));
                i++;
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
                string[] strings;
                if (file is FileInformation)
                {
                    strings = ((FileInformation) file).DisplayName.ToLower().Split(' ');
                } else if (file is Groups)
                {
                    Groups group = (Groups) file;
                    strings = group.Name.ToLower().Split(' ');
                }
                else
                {
                    strings = new string[0];
                }
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