using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace spotlight
{
    public enum EFileType
    {
        All,
        App,
        Document,
        Images,
        Music,
        Video,
        Folders,
        Other,
        Archive
    }

    public struct FileTypeName
    {
        public EFileType Type;
        public string TypeName; // todo remove
        public string[] Names;
        public Regex Regex;

        public FileTypeName(EFileType type, string typeName, string[] names, Regex regex)
        {
            Type = type;
            TypeName = typeName;
            Names = names;
            Regex = regex;
        }

        public bool Is(string type)
        {
            Regex regex = new Regex(type, RegexOptions.IgnoreCase);
            foreach (string name in Names)
                if (regex.Match(name).Success)
                    return true;
            return false;
        }

        public string GetName()
        {
            return Names[0];
        }

        public override string ToString()
        {
            return $"{Type}";
        }
    }

    public class FileTypesList : IEnumerable<FileTypeName>
    {
        public FileTypeName[] FileType =
        {
            #region FileTypes
            new FileTypeName()
            {
                Type = EFileType.App,
                TypeName = "Программы",
                Names = new[] {"Программы", "Приложения"},
                Regex = new Regex(@"\.(exe|lnk)$")
            },
            new FileTypeName()
            {
                Type = EFileType.Document,
                TypeName = "Документы",
                Names = new[] {"Документы"},
                Regex = new Regex(@"\.(txt|docx?|pdf|djvu)$")
            },
            new FileTypeName()
            {
                Type = EFileType.Images,
                TypeName = "Изображения",
                Names = new[] {"Изображения"},
                Regex = new Regex(@"\.(png|jpe?g|gif)$")
            },
            new FileTypeName()
            {
                Type = EFileType.Music,
                TypeName = "Музыка",
                Names = new[] {"Музыка"},
                Regex = new Regex(@"\.(mp\d|wav)$")
            },
            new FileTypeName()
            {
                Type = EFileType.Archive,
                TypeName = "Архивы",
                Names = new[] {"Архивы"},
                Regex = new Regex(@"\.(7z|zip|rar|tar)$")
            },
            new FileTypeName()
            {
                Type = EFileType.Video,
                TypeName = "Видео",
                Names = new[] {"Видео"},
                Regex = new Regex(@"\.(mp4|avi)$")
            },
            new FileTypeName()
            {
                Type = EFileType.Folders,
                TypeName = "Папки",
                Names = new[] {"Папки"},
                Regex = new Regex(@"\\$")
            },
            new FileTypeName()
            {
                Type = EFileType.Other,
                TypeName = "Другое",
                Names = new[] {"Другое"},
                Regex = null
            }
            #endregion
        };


        public FileTypeName this[int i]
        {
            get { return FileType[i]; }
            set { FileType[i] = value; }
        }

        public string GetTypeName(EFileType type)
        {
            foreach (FileTypeName fileType in FileType)
            {
                if (type.Equals(fileType.Type))
                    return fileType.TypeName;
            }
            return null;
        }

        public EFileType? GetTypeName(string type)
        {
            foreach (FileTypeName fileType in FileType)
            {
                if (fileType.GetName() == type)
                    return fileType.Type;
            }
            return null;
        }

        public IEnumerator<FileTypeName> GetEnumerator()
        {
            foreach (FileTypeName type in FileType)
                yield return type;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}