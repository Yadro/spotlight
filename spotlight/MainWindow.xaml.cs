using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using spotlight.ListItem;

namespace spotlight
{
    public partial class MainWindow : MetroWindow
    {
        private string SearchString { get; set; }
        private SearchEngine SearchEngine = new SearchEngine();

        public MainWindow()
        {
            InitializeComponent();
            SearchString = "";
            DataContext = this;
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
            string filter = SearchEngine.GetSearchIgnoreFilter(mainInputBox.Text);
            Group group = (Group) ((TextBlock) sender).DataContext;

            EFileType type = group.Type;
            if (type == EFileType.All)
            {
                // что вернет, если запрос ": SearchString"
                mainInputBox.Text = SearchString = filter;
            }
            else
            {
                string typeName = SearchEngine.FileTypesList.GetTypeName(type);
                mainInputBox.Text = SearchString = $"{typeName}: {filter}";
            }
            
            List<SearchItem> list = GroupToSearchItem(SearchEngine.FilterRangeData(filter, group.Type, 0));

            // todo Add button Show All types (save group.Type)
            list.Add(new Group("Показать все результаты", EFileType.All));
            listBox.ItemsSource = list;
        }

        private void OnSearchInput(object sender, TextChangedEventArgs e)
        {
            SearchString = mainInputBox.Text;
            List<SearchItem> list = GroupToSearchItem(SearchEngine.FilterRangeData(mainInputBox.Text, EFileType.All, 3));
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