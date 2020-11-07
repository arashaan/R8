namespace R8.Lib.IPProcess
{
    public class IPCountryTimeZone
    {
        public IPCountryTimeZone(string timeZone, string name, int? dstOffset, int? gmtOffset, string gmt)
        {
            TimeZone = timeZone;
            Name = name;
            DstOffset = dstOffset;
            GmtOffset = gmtOffset;
            Gmt = gmt;
        }

        public string TimeZone { get; }

        public string Name { get; }

        public int? DstOffset { get; }

        public int? GmtOffset { get; }

        public string Gmt { get; }
    }
}