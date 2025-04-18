using System.Text.RegularExpressions;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;

namespace TypingMaster.Business;

public class TypingMaterialGenerator : ITypingMaterialGenerator
{
    private readonly HttpClient _httpClient;
    private readonly Random _random;
    private readonly Dictionary<SkillLevel, string[]> _levelSpecificWords;
    private readonly string[] _commonWords;

    public TypingMaterialGenerator()
    {
        _httpClient = new HttpClient();
        _random = new Random();
        _commonWords = new[]
        {
            "the", "be", "to", "of", "and", "a", "in", "that", "have", "I", "it", "for", "not", "on", "with", "he",
            "as", "you", "do", "at", "this", "but", "his", "by", "from", "they", "we", "say", "her", "she", "or",
            "an", "will", "my", "one", "all", "would", "there", "their", "what", "so", "up", "out", "if", "about",
            "who", "get", "which", "go", "me", "when", "make", "can", "like", "time", "no", "just", "him", "know",
            "take", "people", "into", "year", "your", "good", "some", "could", "them", "see", "other", "than",
            "then", "now", "look", "only", "come", "its", "over", "think", "also", "back", "after", "use", "two",
            "how", "our", "work", "first", "well", "way", "even", "new", "want", "because", "any", "these", "give",
            "day", "most", "us"
        };

        _levelSpecificWords = new Dictionary<SkillLevel, string[]>
        {
            {
                SkillLevel.Beginner, new[]
                {
                    "cat", "dog", "run", "jump", "walk", "eat", "sleep", "play", "read", "write",
                    "book", "pen", "desk", "chair", "table", "door", "window", "wall", "floor", "roof"
                }
            },
            {
                SkillLevel.Novice, new[]
                {
                    "computer", "keyboard", "monitor", "screen", "mouse", "printer", "scanner", "network",
                    "internet", "website", "email", "message", "document", "folder", "file", "program",
                    "software", "hardware", "system", "data"
                }
            },
            {
                SkillLevel.Intermediate, new[]
                {
                    "algorithm", "database", "function", "variable", "parameter", "interface", "class",
                    "method", "object", "property", "event", "handler", "service", "client", "server",
                    "protocol", "framework", "library", "module", "component"
                }
            },
            {
                SkillLevel.Advanced, new[]
                {
                    "encryption", "authentication", "authorization", "validation", "optimization",
                    "performance", "reliability", "scalability", "maintainability", "compatibility",
                    "integration", "deployment", "configuration", "implementation", "architecture",
                    "infrastructure", "environment", "repository", "dependency", "framework"
                }
            },
            {
                SkillLevel.Expert, new[]
                {
                    "microservices", "containerization", "orchestration", "virtualization", "cloud",
                    "distributed", "concurrent", "asynchronous", "synchronous", "persistence",
                    "transaction", "replication", "fragmentation", "normalization", "denormalization",
                    "polymorphism", "inheritance", "encapsulation", "abstraction", "refactoring"
                }
            }
        };
    }

    public Task<string> GenerateTestMaterial(SkillLevel level, int wordCount = 100)
    {
        var words = new List<string>();
        var levelWords = _levelSpecificWords[level];
        var commonWordRatio = level switch
        {
            SkillLevel.Beginner => 0.8,
            SkillLevel.Novice => 0.7,
            SkillLevel.Intermediate => 0.6,
            SkillLevel.Advanced => 0.5,
            SkillLevel.Expert => 0.4,
            _ => 0.6
        };

        for (int i = 0; i < wordCount; i++)
        {
            if (_random.NextDouble() < commonWordRatio)
            {
                words.Add(_commonWords[_random.Next(_commonWords.Length)]);
            }
            else
            {
                words.Add(levelWords[_random.Next(levelWords.Length)]);
            }
        }

        return Task.FromResult(string.Join(" ", words));
    }

    public async Task<string> GenerateTestMaterialFromArticle(string url)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(url);
            var text = ExtractTextFromHtml(html);
            return CleanAndFormatText(text);
        }
        catch (Exception)
        {
            // Fallback to generated content if article fetch fails
            return await GenerateTestMaterial(SkillLevel.Intermediate);
        }
    }

    public Task<string> GenerateTestMaterialFromKeywords(string[] keywords, int wordCount = 100)
    {
        var words = new List<string>();
        var keywordRatio = 0.3; // 30% of words will be keywords or related words

        for (int i = 0; i < wordCount; i++)
        {
            if (_random.NextDouble() < keywordRatio)
            {
                var keyword = keywords[_random.Next(keywords.Length)];
                words.Add(keyword);
            }
            else
            {
                words.Add(_commonWords[_random.Next(_commonWords.Length)]);
            }
        }

        return Task.FromResult(string.Join(" ", words));
    }

    public Task<string> GenerateKeyPracticeText(char[] keys, int wordCount = 50, int minWordLength = 2, int maxWordLength = 4)
    {
        if (keys == null || keys.Length == 0)
            throw new ArgumentException("Keys array cannot be empty", nameof(keys));

        var words = new List<string>();
        var vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        var consonants = keys.Where(k => !vowels.Contains(k)).ToArray();

        for (int i = 0; i < wordCount; i++)
        {
            // Generate word length
            var wordLength = _random.Next(minWordLength, maxWordLength + 1);
            var word = new char[wordLength];

            // Ensure at least one key from the practice set is used
            var mustUseKey = keys[_random.Next(keys.Length)];
            var mustUsePosition = _random.Next(wordLength);
            word[mustUsePosition] = mustUseKey;

            // Fill remaining positions
            for (int j = 0; j < wordLength; j++)
            {
                if (j != mustUsePosition)
                {
                    // 70% chance to use a practice key, 30% chance to use a vowel
                    if (_random.NextDouble() < 0.7)
                    {
                        word[j] = keys[_random.Next(keys.Length)];
                    }
                    else if (consonants.Length > 0)
                    {
                        word[j] = consonants[_random.Next(consonants.Length)];
                    }
                    else
                    {
                        word[j] = keys[_random.Next(keys.Length)];
                    }
                }
            }

            words.Add(new string(word));
        }

        // Add some repetition exercises
        var repetitionCount = Math.Min(5, wordCount / 10);
        for (int i = 0; i < repetitionCount; i++)
        {
            var key = keys[_random.Next(keys.Length)];
            var repetition = new string(key, _random.Next(3, 6));
            words.Add(repetition);
        }

        // Shuffle the words
        words = words.OrderBy(x => _random.Next()).ToList();

        return Task.FromResult(string.Join(" ", words));
    }

    private string ExtractTextFromHtml(string html)
    {
        // Remove script and style elements
        html = Regex.Replace(html, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>", "", RegexOptions.IgnoreCase);

        // Get text content
        var text = Regex.Replace(html, "<[^>]*>", " ");

        // Decode HTML entities
        text = System.Web.HttpUtility.HtmlDecode(text);

        return text;
    }

    private string CleanAndFormatText(string text)
    {
        // Remove extra whitespace
        text = Regex.Replace(text, @"\s+", " ");

        // Remove special characters except basic punctuation
        text = Regex.Replace(text, @"[^\w\s.,!?-]", "");

        // Ensure proper spacing around punctuation
        text = Regex.Replace(text, @"\s+([.,!?])", "$1");

        return text.Trim();
    }
}