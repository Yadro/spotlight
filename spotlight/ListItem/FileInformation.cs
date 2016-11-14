using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace spotlight.ListItem
{
    public class FileInformation : SearchItem
    {
        public FileInformation(string fileLocation)
        {
            FileLocation = fileLocation;
        }

        public string FileLocation { get; }

        private string displayName;
        public string DisplayName
        {
            get { return displayName ?? (displayName = Path.GetFileNameWithoutExtension(FileLocation)); }
        }

        private string extension;
        public string Extension
        {
            get { return extension ?? (extension = Path.GetExtension(FileLocation)); }
        }

        private ImageSource icon;
        public ImageSource Icon
        {
            get { return icon ?? (icon = GetAssociatedIcon(FileLocation)); }
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