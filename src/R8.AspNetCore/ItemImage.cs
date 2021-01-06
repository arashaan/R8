namespace R8.AspNetCore
{
    public class ItemImage
    {
        public ItemImage(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            IsIcon = !path.StartsWith("/");
            Path = path;
        }

        public ItemImage()
        {
        }

        public string GetPath()
        {
            return IsIcon
                ? Path
                : $"~/img{Path}";
        }

        public void Deconstruct(out bool isIcon, out string path)
        {
            isIcon = IsIcon;
            path = GetPath();
        }

        public override string ToString()
        {
            return GetPath();
        }

        public bool IsIcon { get; set; }
        public string Path { get; set; }
    }
}