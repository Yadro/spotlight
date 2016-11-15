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
        
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            //Hide();
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            SearchItemTile dataContext = ((SearchItemTile) ((Grid) sender).DataContext);
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

        private void OnGroupClick(object sender, MouseButtonEventArgs e)
        {
            Group group = (Group) ((TextBlock) sender).DataContext;
            List<SearchItem> list = GroupToSearchItem(searchEngine.FilterRangeData(mainInputBox.Text, group.Type, 0));

            // todo Add button Show All types (save group.Type)
            list.Add(new Group("Показать все результаты", EFileType.All));
            listBox.ItemsSource = list;
        }

        private void OnSearchInput(object sender, TextChangedEventArgs e)
        {
            List<SearchItem> list = GroupToSearchItem(searchEngine.FilterRangeData(mainInputBox.Text, EFileType.All, 3));
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