namespace R8.Lib.AspNetCore.Robots
{
    public class RobotsUrl
    {
        /// <summary>
        /// Matches the root and any lower level URL
        /// </summary>
        public const string All = "/";

        public static implicit operator string(RobotsUrl url)
        {
            return url.ToString();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
