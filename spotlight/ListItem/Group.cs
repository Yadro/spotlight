namespace spotlight.ListItem
{
    class Group: SearchItem
    {
        public string Name { get; }
        public EFileType Type { get; }

        public Group(string name, EFileType type)
        {
            Name = name;
            Type = type;
        }
    }
}
