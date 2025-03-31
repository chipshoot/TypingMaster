using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Client.Services
{
    public class ReportWebService(HttpClient httpClient, IApiConfiguration apiConfig) : IReportWebService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IApiConfiguration _apiConfig = apiConfig;
        private const string BaseUrl = "api/report";

        public async Task<IEnumerable<string>> GetKeyLabels(PracticeLog history)
        {
            // Since this is client-side logic that doesn't need server interaction,
            // we'll process it directly for better performance
            if (history?.KeyStats == null || !history.KeyStats.Any())
                return Array.Empty<string>();

            // Extract key labels from the key stats
            var keys = history.KeyStats.Values.Select(x => x.Key.ToString()).ToList();
            return keys;
        }

        public async Task<Dictionary<string, IEnumerable<double>>> GetKeyStats(PracticeLog history, bool includeLastSession)
        {
            // Local processing for better performance
            if (history?.KeyStats == null || !history.KeyStats.Any())
                return new Dictionary<string, IEnumerable<double>>();

            // Calculate type speed (press duration per correct key)
            var typeSpeed = history.KeyStats.Values.Select(k =>
                k.CorrectCount == 0 ? 0 : k.PressDuration / k.CorrectCount
            ).ToList();

            // Calculate latency (delay before pressing key)
            var latency = history.KeyStats.Values.Select(k =>
                k.CorrectCount == 0 ? 0 : k.Latency / k.CorrectCount
            ).ToList();

            // Calculate accuracy percentages
            var accuracies = history.KeyStats.Values.Select(k =>
                k.TypingCount == 0 ? 0 : (double)k.CorrectCount / k.TypingCount * 100
            ).ToList();

            // Return combined stats
            return new Dictionary<string, IEnumerable<double>>
            {
                { "typeSpeed", typeSpeed },
                { "latency", latency },
                { "accuracy", accuracies }
            };
        }

        public async Task<IEnumerable<ProgressRecord>> GetProgressRecords(PracticeLog history, CourseDto course, TrainingType type)
        {
            if (history?.PracticeStats == null || course == null)
                return Array.Empty<ProgressRecord>();

            var dataList = new List<ProgressRecord>();

            foreach (var item in history.PracticeStats)
            {
                // Only process items matching the requested type
                if (item.Type != type)
                    continue;

                var record = new ProgressRecord
                {
                    Type = type.ToString(),
                    Name = course.Name,
                    Date = item.StartTime?.ToString() ?? DateTime.Now.ToString(),
                    GoodWpmKeys = CalculateGoodWpmKeys(item.KeyEvents),
                    OverallAccuracy = item.Accuracy,
                    OverallSpeed = item.Wpm,
                    BreakdownLetter = CalculateBreakdownLetter(item.KeyEvents, item.Wpm),
                    BreakdownNumber = CalculateBreakdownNumber(item.KeyEvents, item.Wpm),
                    BreakdownSymbol = CalculateBreakdownSymbol(item.KeyEvents, item.Wpm)
                };

                dataList.Add(record);
            }

            return dataList;
        }

        #region Helper Methods
        private static int CalculateGoodWpmKeys(Queue<KeyEvent> keyEvents)
        {
            var count = 0;
            if (keyEvents.Count < 2)
                return count;

            var keyGroups = keyEvents.GroupBy(k => k.Key).ToList();

            foreach (var keyGroup in keyGroups)
            {
                var keyEventList = keyGroup.ToList();
                var totalTime = (keyEventList.Last().KeyUpTime - keyEventList.First().KeyDownTime).TotalMinutes;

                if (totalTime <= 0)
                    continue;

                var totalLetters = keyEventList.Count;
                var wpm = (totalLetters / 5.0) / totalTime;

                if (wpm > 20)
                    count++;
            }

            return count;
        }

        private static int CalculateBreakdownLetter(Queue<KeyEvent> keyEvents, int overallSpeed)
        {
            if (keyEvents.Count == 0)
                return 0;

            // Filter for letter keys only
            var letterEvents = keyEvents.Where(k => char.IsLetter(k.Key)).ToList();
            return letterEvents.Count == 0 ? 0 : CalculateKeySpeed(letterEvents);
        }

        private static int CalculateBreakdownNumber(Queue<KeyEvent> keyEvents, int overallSpeed)
        {
            if (keyEvents.Count == 0)
                return 0;

            // Filter for number keys only
            var numberEvents = keyEvents.Where(k => char.IsDigit(k.Key)).ToList();
            return numberEvents.Count == 0 ? 0 : CalculateKeySpeed(numberEvents);
        }

        private static int CalculateBreakdownSymbol(Queue<KeyEvent> keyEvents, int overallSpeed)
        {
            if (keyEvents.Count == 0)
                return 0;

            // Filter for symbol keys
            var symbolEvents = keyEvents.Where(k => !char.IsLetterOrDigit(k.Key) && !char.IsWhiteSpace(k.Key)).ToList();
            return symbolEvents.Count == 0 ? 0 : CalculateKeySpeed(symbolEvents);
        }

        private static int CalculateKeySpeed(List<KeyEvent> keyEvents)
        {
            if (keyEvents.Count == 0)
                return 0;

            // Calculate time span between first and last key event
            var totalTime = (keyEvents.Last().KeyUpTime - keyEvents.First().KeyDownTime).TotalMinutes;

            if (totalTime <= 0)
                return 0;

            // Calculate WPM (5 characters = 1 word)
            var totalLetters = keyEvents.Count;
            var wpm = (totalLetters / 5.0) / totalTime;
            return (int)wpm;
        }
        #endregion
    }
}
