namespace R8.Lib.AspNetCore.Robots
{
    public class RobotsSitemap : IRobotsModel
    {
        public RobotsSitemap()
        {
        }

        public RobotsSitemap(string url)
        {
            Url = url;
        }

        public string Url { get; set; }

        public static implicit operator string(RobotsSitemap sitemap)
        {
            return sitemap.Url;
        }

        public override string ToString()
        {
            return Url;
        }
    }
}