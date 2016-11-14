using System.Collections.Generic;
using System.IO;

namespace spotlight
{
    public class SearchEngine
    {
        private const string AppsPath = "C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs";
        public List<string> FileList { get; }


        public SearchEngine()
        {
            string[] paths = {
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
        }

        List<string> GetFileList(string[] folders)
        {
            List<string> result = new List<string>();
            foreach (var folder in folders)
            {
               // Directory.GetDirectories()
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
            SearchDeep deep = new SearchDeep()
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