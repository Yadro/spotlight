using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
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
            List<FileInformation> items = new List<FileInformation>();
            foreach (string file in files)
            {
                items.Add(new FileInformation(file));
            }
            listBox.ItemsSource = items;
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            FileInformation fileInformation = (FileInformation) ((Grid) sender).DataContext;
            Process.Start(fileInformation.FileLocation);
        }
    }
}
