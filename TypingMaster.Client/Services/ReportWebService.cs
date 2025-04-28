using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Client.Services
{
    public class ReportWebService(HttpClient httpClient, IApiConfiguration apiConfig, Serilog.ILogger logger) : IReportWebService
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

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

        public async Task<PagedResult<ProgressRecord>> GetProgressRecords(PracticeLog history, CourseDto course, TrainingType type, int page = 1, int pageSize = 10, bool sortByNewest = true)
        {
            if (history == null || course == null)
                return new PagedResult<ProgressRecord>();

            try
            {
                var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.PracticeLogService}/{history.Id}");
                var fullUrl = $"{url}/drill-stats?page={page}&pageSize={pageSize}&sortByNewest={sortByNewest}&type={type}";
                var response = await httpClient.GetAsync($"{fullUrl}");

                if (!response.IsSuccessStatusCode)
                {
                    logger.Error("Failed to get progress records. Status code: {StatusCode}", response.StatusCode);
                    return new PagedResult<ProgressRecord>();
                }

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                {
                    logger.Error("Empty response content received");
                    return new PagedResult<ProgressRecord>();
                }

                try
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiDrillStatsResponse>(_jsonOptions);
                    if (apiResponse == null || !apiResponse.Success || apiResponse.Items == null)
                    {
                        logger.Error("Invalid API response: {Content}", content);
                        return new PagedResult<ProgressRecord>();
                    }

                    // Ensure all numeric fields are properly converted
                    var items = new List<ProgressRecord>();

                    foreach (var item in apiResponse.Items)
                    {
                        try
                        {
                            var record = new ProgressRecord
                            {
                                Type = item.Type.ToString(),
                                Name = course.Name,
                                Date = item.StartTime?.ToString() ?? DateTime.Now.ToString(),
                                GoodWpmKeys = CalculateGoodWpmKeys(item.KeyEvents),
                                OverallAccuracy = item.Accuracy,
                                OverallSpeed = item.Wpm,
                                BreakdownLetter = CalculateBreakdownLetter(item.KeyEvents, item.Wpm),
                                BreakdownNumber = CalculateBreakdownNumber(item.KeyEvents, item.Wpm),
                                BreakdownSymbol = CalculateBreakdownSymbol(item.KeyEvents, item.Wpm)
                            };
                            items.Add(record);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Error converting numeric values for record");
                        }
                    }

                    var records = new PagedResult<ProgressRecord>
                    {
                        Items = items,
                        Page = apiResponse.Page,
                        TotalPages = apiResponse.TotalPages,
                        TotalCount = apiResponse.TotalCount,
                        PageSize = apiResponse.PageSize,
                    };

                    return records;
                }
                catch (JsonException ex)
                {
                    logger.Error(ex, "Error deserializing response content: {Content}", content);
                    return new PagedResult<ProgressRecord>();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error getting progress records");
                return new PagedResult<ProgressRecord>();
            }
        }

        public class ApiDrillStatsResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public IEnumerable<DrillStats>? Items { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
            public int TotalCount { get; set; }
            public bool HasNextPage { get; set; }
            public bool HasPreviousPage { get; set; }
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

        #endregion Helper Methods
    }
}