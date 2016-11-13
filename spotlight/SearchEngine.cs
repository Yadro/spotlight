using System.Collections.Generic;
using System.IO;

namespace spotlight
{
    public class SearchEngine
    {
        private const string AppsPath = "C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs";
        private string[] cachedFileList;

        public SearchEngine()
        {
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

        public List<string> GetFileListDeep(string path)
        {
            return GetFileListDeep(path, "*", new List<string>());
        }

        public List<string> GetFileListDeep(string path, string filter)
        {
            return GetFileListDeep(path, filter, new List<string>());
        }

        private List<string> GetFileListDeep(string path, string filter, List<string> cache)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                cache = GetFileListDeep(dir, filter, cache);
            }
            string[] files = Directory.GetFiles(path, filter);
            cache.AddRange(files);
            return cache;
        }
    }
}