﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using spotlight.ListItem;

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

    public struct SearchItemStruct
    {
        public EFileType Type;
        public SearchItem Item;
    }

    public struct GroupSearchItems
    {
        public EFileType Type;
        public string TypeName;
        public List<FileInformation> Items;
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
                type = EFileType.Archive,
                typeName = "Архивы",
                regex = new Regex(@"\.(7z|zip|rar|tar)$")
            },
            new FileTypeStruct()
            {
                type = EFileType.Video,
                typeName = "Видео",
                regex = new Regex(@"\.(mp4|avi)$")
            },
            new FileTypeStruct()
            {
                type = EFileType.Folders,
                typeName = "Папки",
                regex = new Regex(@"\\$")
            },
            new FileTypeStruct()
            {
                type = EFileType.Other,
                typeName = "Другое",
                regex = null
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
            List<string> fileList = new List<string>(FileList);
            foreach (FileTypeStruct fileType in FileTypes)
            {
                List<string> other = new List<string>();
                List<FileInformation> items = new List<FileInformation>();
                foreach (string path in fileList)
                {
                    if (fileType.regex == null)
                        items.Add(new FileInformation(path));
                    else
                    {
                        if (fileType.regex.IsMatch(path))
                            items.Add(new FileInformation(path));
                        else
                            other.Add(path);
                    }
                }
                Groups.Add(new GroupSearchItems()
                {
                    Items = items,
                    Type = fileType.type,
                    TypeName = fileType.typeName
                });
                fileList = other;
            }
        }

        public List<GroupSearchItems> FilterData(string filter)
        {
            return FilterData(filter, EFileType.All, 3);
        }

        public List<GroupSearchItems> FilterData(string filter, EFileType type, int resultCount)
        {
            string[] filtres = filter.ToLower().Split(' ');
            List<GroupSearchItems> result = new List<GroupSearchItems>();
            foreach (GroupSearchItems group in Groups)
            {
                if (!EFileType.All.Equals(type))
                    if (group.Type != type)
                        continue;

                List<FileInformation> groupFiles = new List<FileInformation>();
                foreach (FileInformation file in group.Items)
                {
                    string[] fileName = file.DisplayName.ToLower().Split(' ');
                    if (Search(fileName, filtres))
                    {
                        /*string name = fileName.Aggregate((bas, substr) => $"{bas} {substr}"); todo Select find pattern*/
                        if (resultCount > 0)
                        {
                            if (groupFiles.Count < resultCount)
                                groupFiles.Add(file);
                            else 
                                break;
                        }
                            
                    }
                }
                result.Add(new GroupSearchItems()
                {
                    Items = groupFiles,
                    Type = group.Type,
                    TypeName = group.TypeName
                });
            }
            return result;
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

        private bool Search(string[] source, string[] search)
        {
            foreach (var str in source)
                foreach (var subSearch in search)
                    if (str.StartsWith(subSearch))
                        return true;
            return false;
        }
    }
}