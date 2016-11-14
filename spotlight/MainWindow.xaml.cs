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
            } else if (data is Group)
            {
                Group group = (Group) data;
                listBox.ItemsSource = searchEngine.FilterData(mainInputBox.Text, group.Type, 3);
            }
        }

        private void OnGroupClick(object sender, MouseButtonEventArgs e)
        {
            Group groupG = (Group) ((TextBlock) sender).DataContext;
            List<GroupSearchItems> resultGroups = searchEngine.FilterData(mainInputBox.Text, groupG.Type, 3);
            List<SearchItem> list = new List<SearchItem>();
            resultGroups.ForEach(group =>
            {
                if (group.Items.Count == 0)
                    return;
                
                list.Add(new Group(group.TypeName, group.Type));
                foreach (var file in group.Items)
                {
                    list.Add(new SearchItemSmallTitle(file));
                }
            });
            listBox.ItemsSource = list;
        }

        private void OnSearchInput(object sender, TextChangedEventArgs e)
        {
            List<SearchItem> list = new List<SearchItem>();
            List<GroupSearchItems> resultGroups = searchEngine.FilterData(mainInputBox.Text);
            resultGroups.ForEach(group =>
            {
                if (group.Items.Count == 0)
                    return;
                
                list.Add(new Group(group.TypeName, group.Type));
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