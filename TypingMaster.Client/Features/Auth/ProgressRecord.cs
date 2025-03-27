namespace TypingMaster.Business.Models
{
    public class ProgressRecord
    {
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Date { get; set; } = null!;
        public int GoodWpmKeys { get; set; }
        public double OverallAccuracy { get; set; }
        public double OverallSpeed { get; set; }
        public int BreakdownLetter { get; set; }
        public int BreakdownNumber { get; set; }
        public int BreakdownSymbol { get; set; }
    }
}