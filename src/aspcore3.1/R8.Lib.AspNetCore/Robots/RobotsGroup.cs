using System.Collections.Generic;

namespace R8.Lib.AspNetCore.Robots
{
    public class RobotsGroup : IRobotsModel
    {
        public RobotsGroup(RobotsUserAgents userAgent)
        {
            UserAgent = userAgent;
        }

        public RobotsGroup()
        {
        }

        public RobotsUserAgents UserAgent { get; set; }
        public List<string> Disallows { get; set; }

        /// <summary>
        /// Leave it empty, if you all of the pages are allowed to crawl ( Same as * )
        /// </summary>
        public List<string> Allows { get; set; }
    }
}