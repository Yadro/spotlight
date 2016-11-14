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
            object data = ((Grid) sender).DataContext;

            if (data is SearchItemTile)
            {
                SearchItemTile dataContext = (SearchItemTile) data;
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
        }

        private void OnGroupClick(object sender, MouseButtonEventArgs e)
        {
            Group group = (Group) ((TextBlock) sender).DataContext;
            // todo Add button Show All types (save group.Type)
            List<SearchItem> list = GroupToSearchItem(searchEngine.FilterData(mainInputBox.Text, group.Type, 0));
            listBox.ItemsSource = list;
        }

        private void OnSearchInput(object sender, TextChangedEventArgs e)
        {
            List<SearchItem> list = GroupToSearchItem(searchEngine.FilterData(mainInputBox.Text));
            listBox.ItemsSource = list;
        }

        private List<SearchItem> GroupToSearchItem(List<GroupSearchItems> items)
        {
            List<SearchItem> list = new List<SearchItem>();
            items.ForEach(group =>
            {
                if (group.Items.Count == 0)
                    return;

                list.Add(new Group(group.TypeName, group.Type));
                foreach (var file in group.Items)
                {
                    list.Add(new SearchItemSmallTitle(file));
                }
            });
            return list;
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