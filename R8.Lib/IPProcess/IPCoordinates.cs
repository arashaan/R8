namespace R8.Lib.IPProcess
{
    public class IPCoordinates
    {
        public IPCoordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; }

        public double Longitude { get; }

        public override string ToString()
        {
            return $"{Latitude}, {Longitude}";
        }
    }
}