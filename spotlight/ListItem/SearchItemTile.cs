namespace spotlight.ListItem
{
    public class SearchItemTile : SearchItem
    {
        public FileInformation file { get; }

        public SearchItemTile(FileInformation file)
        {
            this.file = file;
        }
    }
}