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
        private bool ResultsFocus = false;
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

        protected override void OnClosed(EventArgs e)
        {
            SearchEngine.SearchRangeManager.DoSerializeData();
            base.OnClosed(e);
        }

        private void SearchBox_Input(object sender, TextChangedEventArgs e)
        {
            SearchString = SearchBox.Text;
            List<SearchItem> list = GroupToSearchItem(SearchEngine.FilterRangeData(SearchBox.Text, null, 3));
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

        #region ResultItem
        private void ResultItem_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                SearchItemTile dataContext = ((SearchItemTile)((Grid)sender).DataContext);
                ResultItemClick(dataContext);
            }
        }
        private void ResultItem_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            SearchItemTile dataContext = ((SearchItemTile)((Grid)sender).DataContext);
            ResultItemClick(dataContext);
        }

        private void ResultItemClick(SearchItemTile dataContext)
        {
            FileInformation fileInformation = dataContext.file;
            SearchEngine.AddQuery(SearchString, fileInformation);
            try
            {
                Process.Start(fileInformation.FileLocation);
            }
            catch (Win32Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        #endregion

        private void Group_OnClick(object sender, MouseButtonEventArgs e)
        {
            string filter = SearchEngine.GetSearchIgnoreFilter(SearchBox.Text);
            Group group = (Group)((TextBlock)sender).DataContext;

            EFileType type = group.Type;
            if (type == EFileType.All)
            {
                // что вернет, если запрос ": SearchString"
                SearchBox.Text = SearchString = filter;
            }
            else
            {
                string typeName = SearchEngine.FileTypesList.GetTypeName(type);
                SearchBox.Text = SearchString = $"{typeName}: {filter}";
            }

            List<SearchItem> list = GroupToSearchItem(SearchEngine.FilterRangeData(filter, group.Type, 0));

            // todo Add button Show All types (save group.Type)
            list.Add(new Group("Показать все результаты", EFileType.All));
            listBox.ItemsSource = list;
        }

        private void SearchBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
                listBox.Focus();
        }

        private void Window_OnKeyUp(object sender, KeyEventArgs e)
        {
            bool isText = (e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                          (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                          || e.Key == Key.OemQuestion || e.Key == Key.OemQuotes || e.Key == Key.OemPlus
                          || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.OemMinus
                          || e.Key == Key.DeadCharProcessed || e.Key == Key.Oem1 || e.Key == Key.Oem7
                          || e.Key == Key.OemPeriod || e.Key == Key.OemComma || e.Key == Key.OemMinus
                          || e.Key == Key.Add || e.Key == Key.Divide || e.Key == Key.Multiply || e.Key == Key.Subtract
                          || e.Key == Key.Oem102 || e.Key == Key.Decimal;
            if (isText)
                SearchBox.Focus();
        }

        
    }
}