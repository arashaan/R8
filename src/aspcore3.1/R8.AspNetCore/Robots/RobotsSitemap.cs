namespace R8.AspNetCore.Robots
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