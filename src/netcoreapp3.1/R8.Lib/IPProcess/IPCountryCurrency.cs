namespace R8.Lib.IPProcess
{
    public class IPCountryCurrency
    {
        public IPCountryCurrency(string name, string code, string symbol, int? rates, string plural)
        {
            Name = name;
            Code = code;
            Symbol = symbol;
            Rates = rates;
            Plural = plural;
        }

        public override string ToString()
        {
            return $"{Name} - {Symbol}";
        }

        public string Name { get; }

        public string Code { get; }

        public string Symbol { get; }

        public int? Rates { get; }

        public string Plural { get; }
    }
}