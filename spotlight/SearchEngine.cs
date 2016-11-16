using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using spotlight.ListItem;

namespace spotlight
{
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

        public override string ToString()
        {
            return $"Type = {Type}, Count = {Items.Count}";
        }
    }

    public class SearchEngine
    {
        public List<string> FileList { get; }
        public List<SearchItemStruct> SearchItems = new List<SearchItemStruct>();
        public List<GroupSearchItems> Groups = new List<GroupSearchItems>();
        public static FileTypesList FileTypesList = new FileTypesList();

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
            
            foreach (FileTypeName fileType in FileTypesList)
            {
                List<string> other = new List<string>();
                List<FileInformation> items = new List<FileInformation>();
                foreach (string path in fileList)
                {
                    if (fileType.Regex == null)
                        items.Add(new FileInformation(path));
                    else
                    {
                        if (fileType.Regex.IsMatch(path))
                            items.Add(new FileInformation(path));
                        else
                            other.Add(path);
                    }
                }
                Groups.Add(new GroupSearchItems()
                {
                    Items = items,
                    Type = fileType.Type,
                    TypeName = fileType.TypeName
                });
                fileList = other;
            }
        }

        public List<GroupSearchItems> FilterData(string filter)
        {
            return FilterData(filter, EFileType.All, 3);
        }

        private class RangeSearchResult
        {
            private struct FileRangeStruct
            {
                public readonly List<FileInformation> Files;
                public readonly int Range;

                public override string ToString()
                {
                    return $"Range = {Range}, Count = {Files.Count}";
                }

                public FileRangeStruct(List<FileInformation> files, int range)
                {
                    Files = files;
                    Range = range;
                }
            }

            private List<FileRangeStruct> data = new List<FileRangeStruct>();

            public void Add(int range, FileInformation file)
            {
                FileRangeStruct rangedFiles = data.Find(item => item.Range == range);
                if (rangedFiles.Files == null)
                {
                    data.Add(new FileRangeStruct(new List<FileInformation>() {file}, range));
                    data.Sort((a, b) => a.Range.CompareTo(b.Range));
                }
                else
                    rangedFiles.Files.Add(file);
            }

            public List<FileInformation> Get(int range)
            {
                FileRangeStruct rangedFiles = data.Find(item => item.Range == range);
                return rangedFiles.Files ?? new List<FileInformation>();
            }

            public List<FileInformation> GetNum(int size)
            {
                if (size == 0)
                {
                    List<FileInformation> all = new List<FileInformation>();
                    data.ForEach(range => all.AddRange(range.Files));
                    return all;
                }
                int left = size;
                List<FileInformation> result = new List<FileInformation>();
                foreach (FileRangeStruct range in data)
                {
                    List<FileInformation> files = range.Files;
                    result.AddRange(files.GetRange(0, Math.Min(files.Count, left)));
                    left -= result.Count;
                    if (left < 0)
                        break;
                }
                return result;
            }

            public int Total()
            {
                return data.Aggregate(0, (last, item) => last + item.Files.Count);
            }

            public int TotalFirstRange()
            {
                return data[0].Files.Count;
            }
        }

        public List<GroupSearchItems> FilterRangeData(string filterQuery, EFileType typeQuery, int resultCount)
        {
            string filter;
            EFileType type;
            if (typeQuery != EFileType.All)
            {
                filter = GetSearchIgnoreFilter(filterQuery);
                type = typeQuery;
            }
            else
            {
                SearchInputStruct searchInput = ParseSearchInput(filterQuery);
                filter = searchInput.Search;
                type = searchInput.Type;
                resultCount = 10;
            }

            var result = new List<GroupSearchItems>();
            foreach (GroupSearchItems group in Groups)
            {
                if (EFileType.All != type && group.Type != type)
                    continue;

                var rangeSearchResult = new RangeSearchResult();
                foreach (FileInformation file in group.Items)
                {
                    int range = Search(file.DisplayName, filter);
                    if (range > 0)
                    {
                        rangeSearchResult.Add(range, file);
                        if (resultCount > 0 && rangeSearchResult.TotalFirstRange() >= resultCount)
                            break;
                    }
                }
                result.Add(new GroupSearchItems()
                {
                    Items = rangeSearchResult.GetNum(resultCount),
                    Type = group.Type,
                    TypeName = group.TypeName
                });
            }
            return result;
        }

        public List<GroupSearchItems> FilterData(string filter, EFileType type, int resultCount)
        {
            List<GroupSearchItems> result = new List<GroupSearchItems>();
            foreach (GroupSearchItems group in Groups)
            {
                if (!EFileType.All.Equals(type) && group.Type != type)
                    continue;

                List<FileInformation> groupFiles = new List<FileInformation>();
                foreach (FileInformation file in group.Items)
                {
                    if (Search(file.DisplayName, filter) != 0)
                    {
                        /*string name = fileName.Aggregate((bas, substr) => $"{bas} {substr}"); todo Select find pattern*/
                        if (resultCount > 0)
                        {
                            if (groupFiles.Count < resultCount)
                                groupFiles.Add(file);
                            else
                                break;
                        }
                        else
                            groupFiles.Add(file);
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

        public struct SearchInputStruct
        {
            public EFileType Type;
            public string Search;

            public SearchInputStruct(EFileType type, string search)
            {
                Type = type;
                Search = search;
            }
        }

        public static string GetSearchIgnoreFilter(string search)
        {
            var match = new Regex(@"(\w*?):\s?([\w\W]+)").Match(search);
            return match.Success ? match.Groups[2].Value : search;
        }

        private SearchInputStruct ParseSearchInput(string search)
        {
            Regex regex = new Regex(@"(\w+):\s?([\w\W]+)");
            Match match = regex.Match(search);
            if (match.Success)
            {
                EFileType? fileType = FileTypesList.GetTypeName(match.Groups[1].Value);
                if (fileType != null)
                    return new SearchInputStruct((EFileType)fileType, match.Groups[2].Value);
                
            }
            return new SearchInputStruct(EFileType.All, search);
        }

        private int Search(string source, string search)
        {
            Regex regex1 = new Regex($"^{search}", RegexOptions.IgnoreCase);
            Match match1 = regex1.Match(source);
            if (match1.Success)
                return 1;

            Regex regex = new Regex($"{search}", RegexOptions.IgnoreCase);
            Match match = regex.Match(source);
            if (match.Success == false)
                return 0;

            int beginMatch = match.Index;
            Regex spaces = new Regex(@"\w+(\W)");
            MatchCollection matchCollection = spaces.Matches(source.Substring(0, beginMatch));
            return matchCollection.Count + 1;
        }
    }
}