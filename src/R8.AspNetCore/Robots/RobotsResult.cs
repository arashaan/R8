using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace R8.AspNetCore.Robots
{
    public class RobotsResult : ActionResult
    {
        private readonly IEnumerable<IRobotsModel> _groups;

        /// <summary>
        /// More information: https://developers.google.com/search/reference/robots_txt
        /// </summary>
        public RobotsResult(IEnumerable<IRobotsModel> groups)
        {
            _groups = groups;
        }

        public RobotsResult()
        {
        }

        public string CreateSitemap(RobotsSitemap sitemap)
        {
            if (sitemap == null) throw new ArgumentNullException(nameof(sitemap));
            var result = new List<string>();

            if (!sitemap.Url.StartsWith("http"))
                throw new Exception($"{nameof(RobotsSitemap.Url)} must started with valid schema (http, https, ... )");

            return sitemap.Url;
        }

        public Dictionary<string, object> CreateGroup(RobotsGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(@group));
            var result = new Dictionary<string, object>();

            var userAgentValue = group.UserAgent == RobotsUserAgents.All
                ? "*"
                : group.UserAgent.ToString().ToLower().Replace("_", "-");
            result.Add("User-Agent", userAgentValue);

            if (group.Disallows?.Any() == true)
            {
                foreach (var disallow in group.Disallows)
                {
                    if (!disallow.StartsWith("/"))
                        throw new Exception($"{nameof(group.Disallows)} urls must started with /");

                    result.Add("Disallow", disallow.ToLower());
                }
            }
            else
            {
                result.Add("Disallow", "");
            }

            if (group.Allows?.Any() == true)
            {
                foreach (var allow in group.Allows)
                {
                    if (!allow.StartsWith("/"))
                        throw new Exception($"{nameof(group.Disallows)} urls must started with /");

                    result.Add("Allow", allow.ToLower());
                }
            }
            return result;
        }

        private ContentResult Prepare(ActionContext context)
        {
            var stringBuilder = new StringBuilder();
            if (_groups?.Any() == true)
            {
                foreach (var robotsModel in _groups)
                {
                    switch (robotsModel)
                    {
                        case RobotsGroup group:
                            var groupDic = CreateGroup(group);
                            if (groupDic.Any())
                                foreach (var (key, value) in groupDic)
                                    stringBuilder.AppendLine($"{key}: {value}");

                            stringBuilder.AppendLine("");
                            break;

                        case RobotsSitemap sitemap:
                            var sitemapString = CreateSitemap(sitemap);
                            stringBuilder.AppendLine($"Sitemap: {sitemapString}");
                            stringBuilder.AppendLine("");

                            break;

                        default:
                            break;
                    }
                }
            }
            var result = new ContentResult
            {
                Content = stringBuilder.ToString(),
                ContentType = "text/plain",
                StatusCode = StatusCodes.Status200OK
            };

            return result;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var result = Prepare(context);
            result.ExecuteResult(context);
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var result = Prepare(context);
            return result.ExecuteResultAsync(context);
        }
    }
}