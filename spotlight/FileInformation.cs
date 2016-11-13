using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace spotlight
{
    public class FileInformation
    {
        public FileInformation(string fileLocation)
        {
            FileLocation = fileLocation;
        }

        public string FileLocation { get; }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName))
                    displayName = Path.GetFileNameWithoutExtension(FileLocation);
                return displayName;
            }
        }

        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(extension))
                    extension = Path.GetExtension(FileLocation);
                return extension;
            }
        }

        public ImageSource Icon
        {
            get { return icon ?? (icon = GetAssociatedIcon(FileLocation)); }
        }

        private string displayName;
        private string extension;
        private ImageSource icon;
        private string RealFileLocation;
        

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