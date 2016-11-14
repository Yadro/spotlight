using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using MahApps.Metro.Controls;
using spotlight.ListItem;

namespace spotlight
{
    public partial class MainWindow : MetroWindow
    {
        private SearchEngine searchEngine = new SearchEngine();
        private List<SearchItem> FileInformations;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            SearchItemTile dataContext = (SearchItemTile) ((Grid)sender).DataContext;
            FileInformation fileInformation = dataContext.file;
            try
            {
                Process.Start(fileInformation.FileLocation);
            }
            catch (Win32Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void OnSearchInput(object sender, TextChangedEventArgs e)
        {
            List<SearchItem> list = new List<SearchItem>();
            List<GroupSearchItems> resultGroups = searchEngine.FilterData(((TextBox)sender).Text);
            resultGroups.ForEach(group =>
            {
                if (group.Items.Count == 0)
                    return;
                
                list.Add(new Groups(group.TypeName));
                foreach (var file in group.Items)
                {
                    list.Add(new SearchItemSmallTitle(file));
                }
            });

            listBox.ItemsSource = list;
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