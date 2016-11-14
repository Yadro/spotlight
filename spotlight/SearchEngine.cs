using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using spotlight.ListItem;

namespace spotlight
{
    public enum EFileType
    {
        App,
        Document,
        Images,
        Music,
        Video
    }

    public struct SearchItemStruct
    {
        public EFileType Type;
        public SearchItem Item;
    }

    public struct GroupSearchItems
    {
        public EFileType Type;
        public string TypeName;
        public IEnumerable<FileInformation> Items;
    }

    public struct FileTypeStruct
    {
        public EFileType type;
        public string typeName;
        public Regex regex;
    }

    public class SearchEngine
    {
        public List<string> FileList { get; }
        public List<SearchItemStruct> SearchItems = new List<SearchItemStruct>();
        public List<GroupSearchItems> Groups = new List<GroupSearchItems>();

        private FileTypeStruct[] FileTypes =
        {
            new FileTypeStruct()
            {
                type = EFileType.App,
                typeName = "Приложения",
                regex = new Regex(@"\.(exe|lnk)$")
            },
            new FileTypeStruct()
            {
                type = EFileType.Document,
                typeName = "Документы",
                regex = new Regex(@"\.(txt|docx?|pdf|djvu)$")
            },
            new FileTypeStruct()
            {
                type = EFileType.Images,
                typeName = "Изображения",
                regex = new Regex(@"\.(png|jpe?g|gif)$")
            },
            new FileTypeStruct()
            {
                type = EFileType.Music,
                typeName = "Музыка",
                regex = new Regex(@"\.(mp\d|wav)$")
            },
            new FileTypeStruct()
            {
                type = EFileType.Video,
                typeName = "Видео",
                regex = new Regex(@"\.(mp4|avi)$")
            }
        };

        public SearchEngine()
        {
            string[] paths =
            {
                "C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs",
                @"E:\Downloads\",
                @"E:\Documents\",
                @"E:\MyProjects",
                @"E:\Dropbox\"
            };
            FileList = new List<string>();
            foreach (var path in paths)
            {
                FileList.AddRange(GetFileListDeep(path, "*", 2));
            }
            GenereateData();
        }

        private void GenereateData()
        {
            foreach (FileTypeStruct fileType in FileTypes)
            {
                IEnumerable<FileInformation> items =
                    FileList.Where(path => fileType.regex.IsMatch(path)).Select(path => new FileInformation(path));
                Groups.Add(new GroupSearchItems()
                {
                    Items = items,
                    Type = fileType.type,
                    TypeName = fileType.typeName
                });
            }
        }

        public List<GroupSearchItems> FilterData(string filter)
        {
            string[] filtres = filter.ToLower().Split(' ');
            List<GroupSearchItems> groupsFiltred = new List<GroupSearchItems>();
            Groups.ForEach(group =>
            {
                List<FileInformation> groupFiles = new List<FileInformation>();
                foreach (FileInformation file in group.Items)
                {
                    string[] fileName = file.DisplayName.ToLower().Split(' ');
                    foreach (var subFilter in filtres)
                    {
                        foreach (var subFileName in fileName)
                        {
                            if (subFileName.StartsWith(subFilter))
                            {
                                groupFiles.Add(file);
                                if (groupFiles.Count >= 3)
                                {
                                    goto NextGroup;
                                }
                                goto NextFile;
                            }
                        }
                    }
                    NextFile:;
                }
                NextGroup:
                groupsFiltred.Add(new GroupSearchItems()
                {
                    Items = groupFiles,
                    Type = group.Type,
                    TypeName = group.TypeName
                });
            });
            return groupsFiltred;
        }

        public List<string> GetFileList(string path)
        {
            return GetFileListDeep(path, "*", -1);
        }

        private struct SearchDeep
        {
            public int Deep;
            public int CurrentDeep;
        }

        public List<string> GetFileListDeep(string path, string filter, int deepSize)
        {
            SearchDeep deep = new SearchDeep
            {
                CurrentDeep = 0,
                Deep = deepSize
            };
            return GetFileListDeep(path, filter, deep, new List<string>());
        }

        private List<string> GetFileListDeep(string path, string filter, SearchDeep deep, List<string> cache)
        {
            if (deep.CurrentDeep < deep.Deep || deep.Deep == -1)
            {
                string[] dirs = Directory.GetDirectories(path);
                SearchDeep searchDeep = deep;
                searchDeep.CurrentDeep += 1;
                foreach (var dir in dirs)
                {
                    cache = GetFileListDeep(dir, filter, searchDeep, cache);
                }
            }
            string[] files = Directory.GetFiles(path, filter);
            cache.AddRange(files);
            return cache;
        }
    }
}