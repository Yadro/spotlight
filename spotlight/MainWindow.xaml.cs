using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace spotlight
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Icon extractAssociatedIcon = System.Drawing.Icon.ExtractAssociatedIcon(@"C:\WINDOWS\system32\notepad.exe");


            string[] files = Directory.GetFiles("C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs");
            List<ListItems> items = new List<ListItems>();
            foreach (string file in files)
            {
                items.Add(new ListItems()
                {
                    name = file,
                    icon = GetAssociatedIcon(file)
                });
            }
            listBox.ItemsSource = items;
        }

        public static ImageSource GetAssociatedIcon(string fileName)
        {
            return ToImageSource(
                System.Drawing.Icon.ExtractAssociatedIcon(fileName)
            );
        }

        public static ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        class ListItems
        {
            public string name { get; set; }
            public ImageSource icon { get; set; }
        }
    }
}
