using System;
using System.Net;

namespace R8.EntityFrameworkCore
{
    public class AuditOptions
    {
        public string UserAgent { get; set; }

        public IPAddress RemoteIpAddress { get; set; }

        public IPAddress LocalIpAddress { get; set; }
        public Guid? UserId { get; set; }
    }
}