using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace spotlight.ListItem
{
    public class FileInformation : SearchListItem
    {
        public string FileLocation { get; }
        public string DisplayName { get; }
        public string Extension { get; }
        public ImageSource Icon { get; }

        public FileInformation(string fileLocation)
        {
            FileLocation = fileLocation;
            DisplayName = Path.GetFileNameWithoutExtension(FileLocation);
            Extension = Path.GetExtension(FileLocation);
            Icon = GetAssociatedIcon(FileLocation);
        }

        private static ImageSource GetAssociatedIcon(string fileName)
        {
            return ToImageSource(
                System.Drawing.Icon.ExtractAssociatedIcon(fileName)
            );
        }

        private static ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
    }
}