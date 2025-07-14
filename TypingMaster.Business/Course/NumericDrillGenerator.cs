using System.Text;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Course;

public class NumericDrillGenerator : INumericDrillGenerator
{
    private readonly Random _random = new();

    private static readonly List<string> Stage2Templates =
    [
        "yyyy-MM-dd", "MM/dd/yyyy", "dd.MM.yy",
        "HH:mm:ss", "hh:mm tt", "H:m",
        "$#,###.00",
        "##.##%", "###%",
        "#/#", "##/##",
        "##.##N, ##.##E",
        "p=3.14159", "e=2.71828", "f=1.61803",
        "(###) ###-####", "+# ###-####-####",
        "####-####-####-####", "*** **** **** ####"
    ];

    private static readonly List<string> Stage3Templates =
    [
        "int x = {0};", "const double PI = {0};", "var id = \"{0}\";",
        "userID: {0}", "errorCode: {0}", "version: {0}",
        "Yield: {0}%", "Price: ${0}", "Volume: {0} shares",
        "Balance: {0}", "Interest: {0}%", "Tax: {0}",
        "ZIP: {0}", "Room: {0}", "Phone: {0}",
        "Code: {0}", "ID: {0}", "Serial: {0}",
        "Speed of light: {0} m/s", "Gravity: {0} m/s²",
        "Avogadro: {0}", "Planck: {0}",
        "Meeting: {0}", "Order#: {0}", "Account: {0}",
        "Reference#: {0}", "Transaction#: {0}", "Confirmation: {0}"
    ];

    public int MaxCharacters { get; set; } = TypingMasterConstants.DefaultTypingWindowWidth;

    public string GenerateDrill(NumericPracticePhases phases, int count)
    {
        switch (phases)
        {
            case NumericPracticePhases.NotSet:
                return GenerateDefaultNumberSequence(count);

            case NumericPracticePhases.SingleKeyFocus:

                // Simple repeated characters, e.g., "11111"
                return GenerateSimpleRepetition(count);

            case NumericPracticePhases.SequenceCombos:

                return GenerateDigitSequence(count);

            case NumericPracticePhases.HorizontalCombination:

                // Simple patterns with keys, e.g., "1029"
                return GenerateHorizontalPatterns(count);

            case NumericPracticePhases.PracticalPatterns:

                // Real number patterns, e.g., dates, times, formatted numbers
                var drills2 = GenerateStage2Drills(1, 2);
                return drills2.Count > 0 ? drills2[0] : string.Empty;

            case NumericPracticePhases.FullIntegration:

                // Full key and number mix application
                var drills3 = GenerateStage3Drills(1, 2);
                return drills3.Count > 0 ? drills3[0] : string.Empty;

            case NumericPracticePhases.DomainSpecialization:

                // Domain specific numbers, e.g., phone, zip, scientific
                var domainTemplates = new List<string>
                {
                    "phone: {0}", "zip: {0}", "isbn: {0}", "ein: {0}", "score: {0}", "temp: {0}C"
                };

                string template = domainTemplates[_random.Next(domainTemplates.Count)];
                string value = GenerateNumericPattern(3);
                return string.Format(template, value);

            case NumericPracticePhases.CompetitiveSpeed:

                // Competitive speed: random fast pattern
                return GenerateDigitSequence(3);

            default:
                return string.Empty;
        }
    }

    private string GenerateDefaultNumberSequence(int count)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < count; i++)
        {
            sb.Append("1234567890 ");
        }

        var result = sb.ToString().Trim();
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    private string GenerateSimpleRepetition(int repeatCount)
    {
        var sb = new StringBuilder();
        List<char> leftHandDigits = ['1', '2', '3', '4', '5'];
        List<char> rightHandDigits = ['6', '7', '8', '9', '0'];

        while (sb.Length < MaxCharacters)
        {
            GenerateSingleKeyPattern(sb, leftHandDigits, repeatCount);
            GenerateSingleKeyPattern(sb, rightHandDigits, repeatCount);
            if (sb.Length >= MaxCharacters)
            {
                break;
            }
        }

        var result = sb.ToString().Trim();
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    /// <summary>
    /// Generate number sequence like "123456789"
    /// </summary>
    private string GenerateDigitSequence(int count)
    {
        var baseString = "";
        var sb = new StringBuilder();

        while (sb.Length < MaxCharacters)
        {
            sb.Append("1234567890");
        }
        baseString = sb.ToString().Trim();
        sb.Clear();

        var insertPoint = 0;
        while (sb.Length < MaxCharacters)
        {
            var subString = baseString.Substring(insertPoint, count);
            sb.Append(subString);
            sb.Append(' ');
            insertPoint += count;
        }
        var result = sb.ToString().Trim();
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    private static void GenerateSingleKeyPattern(StringBuilder sb, List<char> digits, int repetitions)
    {
        foreach (var digit in digits)
        {
            sb.Append(new string(digit, repetitions) + " ");
        }
    }

    private string GenerateHorizontalPatterns(int repeatCount)
    {
        var fingerPairs = new List<string> { "10", "29", "38", "47", "56", };
        var sb = new StringBuilder();

        while (sb.Length < MaxCharacters)
        {
            for (var i = 0; i <= repeatCount; i++)
            {
                sb.Append(fingerPairs[_random.Next(0, fingerPairs.Count - 1)]);
            }

        }

        var result = sb.ToString().Trim();
        return result[..Math.Min(sb.Length, MaxCharacters)].Trim();
    }

    /// <summary>
    /// Generate third stage string
    /// </summary>
    private List<string> GenerateStage2Drills(int count = 10, int difficulty = 2)
    {
        var drills = new List<string>();

        for (var i = 0; i < count; i++)
        {
            var template = Stage2Templates[_random.Next(Stage2Templates.Count)];
            var result = ReplacePatterns(template, difficulty);
            drills.Add(result);
        }

        return drills;
    }

    private string GenerateRealWords(int count)
    {
        // Example: generate a numeric drill with real words (could be numbers as words)
        return string.Join(" ", Enumerable.Range(1, count).Select(i => i.ToString()));
    }

    /// <summary>
    /// Generate forth stage string - all keys and numbers mixed
    /// </summary>
    private List<string> GenerateStage3Drills(int count = 15, int difficulty = 2)
    {
        var drills = new List<string>();

        for (var i = 0; i < count; i++)
        {
            var template = Stage3Templates[_random.Next(Stage3Templates.Count)];
            var numericPart = GenerateNumericPattern(difficulty + 1);
            var result = string.Format(template, numericPart);

            // add random point
            if (_random.NextDouble() > 0.7)
            {
                result += _random.NextDouble() > 0.5 ? ";" : ".";
            }

            drills.Add(result);
        }

        return drills;
    }

    /// <summary>
    /// Replaces template patterns with actual numbers based on difficulty
    /// </summary>
    private string ReplacePatterns(string template, int difficulty)
    {
        var sb = new StringBuilder();
        var isEscape = true;

        foreach (var c in template)
        {
            if (c == '\\' && !isEscape)
            {
                isEscape = true;
                continue;
            }

            if (isEscape)
            {
                sb.Append(c);
                isEscape = false;
                continue;
            }

            switch (c)
            {
                case 'y': // year
                    sb.Append(GenerateYear(difficulty));
                    break;

                case 'M': // month
                    sb.Append(GenerateMonth(difficulty));
                    break;

                case 'd': // date
                    sb.Append(GenerateDay(difficulty));
                    break;

                case 'H': // 24 hours
                    sb.Append(Generate24Hour());
                    break;

                case 'h': // 12 hours
                    sb.Append(Generate12Hour());
                    break;

                case 'm': // minute
                case 's': // second
                    sb.Append(GenerateMinuteSec());
                    break;

                case 't': // AM/PM
                    sb.Append(_random.NextDouble() > 0.5 ? "AM" : "PM");
                    break;

                case '*': // mask character
                    sb.Append(_random.Next(10));
                    break;

                case '#': // random digit
                case '0': // fixed digit
                    sb.Append(_random.Next(10));
                    break;

                case ',': //
                    sb.Append(_random.NextDouble() > 0.3 ? "," : "");
                    break;

                case '.': // decimal point
                    sb.Append(".");
                    break;

                default:
                    sb.Append(c);
                    break;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    ///  Random year generator
    /// </summary>
    private string GenerateYear(int difficulty)
    {
        return difficulty switch
        {
            1 => _random.Next(2020, 2030).ToString(), // 简单：近期年份
            2 => _random.Next(1900, 2100).ToString(), // 中等：任意年份
            _ => _random.Next(1000, 3000).ToString()  // 复杂：宽范围年份
        };
    }

    /// <summary>
    ///  Random month generator
    /// </summary>
    private string GenerateMonth(int difficulty)
    {
        int month = _random.Next(1, 13);
        return difficulty == 1 ? month.ToString("D2") : month.ToString();
    }

    /// <summary>
    /// Random time generator
    /// </summary>
    private string GenerateDay(int difficulty)
    {
        int day = _random.Next(1, 32);
        return difficulty == 1 ? day.ToString("D2") : day.ToString();
    }

    /// <summary>
    /// 24 hour time generator
    /// </summary>
    private string Generate24Hour()
    {
        return _random.Next(0, 24).ToString("D2");
    }

    /// <summary>
    /// 12 Hours time generator
    /// </summary>
    private string Generate12Hour()
    {
        return _random.Next(1, 13).ToString("D2");
    }

    /// <summary>
    /// Minute/Second generator
    /// </summary>
    private string GenerateMinuteSec()
    {
        return _random.Next(0, 60).ToString("D2");
    }

    /// <summary>
    /// Complexity formatted generator
    /// </summary>
    private string GenerateNumericPattern(int complexity)
    {
        int patternType = _random.Next(1, 7);

        return patternType switch
        {
            1 => GenerateDecimal(complexity),        // decimal number
            2 => GenerateFormattedNumber(complexity), // format number
            3 => GenerateRange(complexity),          // range number
            4 => GenerateFraction(),                 // fraction number
            5 => GeneratePhoneNumber(),              // telephone number
            _ => GenerateDigitSequence(complexity)   // normal digit sequence
        };
    }

    /// <summary>
    /// Decimal number generator
    /// </summary>
    private string GenerateDecimal(int complexity)
    {
        int wholePart = _random.Next(0, complexity * 1000);
        int decimalPlaces = _random.Next(1, complexity + 2);
        int decimalPart = _random.Next(0, (int)Math.Pow(10, decimalPlaces));
        return $"{wholePart}.{decimalPart.ToString().PadRight(decimalPlaces, '0')}";
    }

    /// <summary>
    /// Formatted number generator, e.g."1,234,567.89", "$1,234.56", "12.3%"
    /// </summary>
    private string GenerateFormattedNumber(int complexity)
    {
        int number = _random.Next(1000, 1000000);

        return _random.Next(3) switch
        {
            0 => number.ToString("N0"),    // 千位分隔符
            1 => number.ToString("C"),     // 货币格式
            _ => (number / 10000.0).ToString("P1") // 百分比
        };
    }

    /// <summary>
    /// Number range generator
    /// </summary>
    private string GenerateRange(int complexity)
    {
        int start = _random.Next(0, 100 * complexity);
        int end = start + _random.Next(10, 100 * complexity);
        return $"{start}-{end}";
    }

    /// <summary>
    /// Fraction number generator
    /// </summary>
    private string GenerateFraction()
    {
        int numerator = _random.Next(1, 10);
        int denominator = _random.Next(2, 20);
        return $"{numerator}/{denominator}";
    }

    /// <summary>
    /// Telephone number generator
    /// </summary>
    private string GeneratePhoneNumber()
    {
        return _random.Next(2) == 0
            ? $"{_random.Next(100, 1000)}-{_random.Next(100, 1000)}-{_random.Next(1000, 10000)}"
            : $"+{_random.Next(1, 99)} {_random.Next(100, 1000)}-{_random.Next(100, 1000)}-{_random.Next(1000, 10000)}";
    }

    /// <summary>
    /// Generate space-separated number sequences like "123 456 789"
    /// </summary>
    /// <param name="difficulty">Number of digit groups to generate</param>
    /// <returns>Space-separated number string</returns>
    private string GenerateNumberSequence(int difficulty)
    {
        var sb = new StringBuilder();
        int startNumber = _random.Next(100, 900); // Start with 3-digit number

        var length = difficulty switch
        {
            1 => _random.Next(2, 5),  // easy：2-4
            2 => _random.Next(4, 7),  // medium：4-6
            _ => _random.Next(6, 10)  // difficult：6-9
        };
        for (int i = 0; i < length; i++)
        {
            if (sb.Length > 0)
                sb.Append(' ');

            int currentNumber = startNumber + i;
            sb.Append(currentNumber);

            // Check if adding another number would exceed max characters
            if (sb.Length + currentNumber.ToString().Length + 1 > MaxCharacters)
                break;
        }

        return sb.ToString();
    }

}