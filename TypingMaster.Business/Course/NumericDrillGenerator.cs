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

    public bool EnableCapital { get; set; }

    public bool EnableSymbol { get; set; }

    public string GenerateDrill(NumericPracticePhases phases, int count)
    {
        if (count <= 0)
        {
            return string.Empty;
        }

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
                return GenerateStage2Drills(count);

            case NumericPracticePhases.FullIntegration:

                // Full key and number mix application
                return GenerateStage3Drills(count);

            case NumericPracticePhases.DomainSpecialization:

                // Domain specific numbers, e.g., phone, zip, scientific
                var domainTemplates = new List<string>
                {
                    "Phone: {0}", "ZIP: {0}", "ISBN: {0}", "EIN: {0}", "Score: {0}", "Temp: {0}C"
                };

                var template = domainTemplates[_random.Next(domainTemplates.Count)];
                var value = GenerateNumericPattern();
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
        var sb = new StringBuilder();

        while (sb.Length < MaxCharacters)
        {
            sb.Append("1234567890");
        }

        var baseString = sb.ToString().Trim();
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
            for (var i = 0; i < repeatCount; i++)
            {
                sb.Append(fingerPairs[_random.Next(0, fingerPairs.Count - 1)]);
            }

            sb.Append(' ');
        }

        var result = sb.ToString().Trim();
        return result[..Math.Min(sb.Length, MaxCharacters)].Trim();
    }

    /// <summary>
    /// Generate third stage string
    /// </summary>
    private string GenerateStage2Drills(int count = 10)
    {
        var drills = new List<string>();

        var rawText = string.Empty;
        while (rawText.Length <= MaxCharacters)
        {
            for (var i = 0; i <= count; i++)
            {
                var template = Stage2Templates[_random.Next(Stage2Templates.Count)];
                if (template.IsDateTimeTemplate())
                {
                    var year = _random.Next(1000, 3000);
                    var month = _random.Next(1, 13);
                    var day = _random.Next(1, 32);
                    var hour = _random.Next(0, 24);
                    var min = _random.Next(0, 60);
                    var second = _random.Next(0, 60);
                    var randomDate = new DateTime(year, month, day, hour, min, second);
                    var dateText = ModifyTextBasedOnFlagSetting(randomDate.ToString(template));
                    drills.Add(dateText);
                }
                else
                {
                    var result = ReplacePatterns(template);
                    drills.Add(result);
                }
            }

            rawText = $"{rawText} {string.Join(" ", drills).Trim()}";
        }

        var finalText = rawText[..Math.Min(rawText.Length, MaxCharacters)].Trim();
        return finalText;
    }

    /// <summary>
    /// Generate forth stage string - all keys and numbers mixed
    /// </summary>
    private string GenerateStage3Drills(int count = 10)
    {
        var drills = new List<string>();
        var rawText = string.Empty;

        while (rawText.Length <= MaxCharacters)
        {
            for (var i = 0; i < count; i++)
            {
                var template = Stage3Templates[_random.Next(Stage3Templates.Count)];
                var numericPart = GenerateNumericPattern();
                var result = string.Format(template, numericPart);

                // add random point
                if (_random.NextDouble() > 0.7)
                {
                    result += _random.NextDouble() > 0.5 ? ";" : ".";
                }

                drills.Add(result);
                rawText = $"{rawText} {string.Join(" ", drills).Trim()}";
            }
        }

        rawText = ModifyTextBasedOnFlagSetting(rawText);
        var finalText = rawText[..Math.Min(rawText.Length, MaxCharacters)].Trim();
        
        return finalText;
    }

    /// <summary>
    /// Replaces template patterns with actual numbers
    /// </summary>
    private string ReplacePatterns(string template)
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
                case '*': // mask character
                case '#': // random digit
                case '0': // fixed digit
                    sb.Append(_random.Next(10));
                    break;

                case ',': //
                    sb.Append(_random.NextDouble() > 0.3 ? "," : "");
                    break;

                case '.': // decimal point
                    sb.Append('.');
                    break;

                default:
                    sb.Append(c);
                    break;
            }
        }

        return ModifyTextBasedOnFlagSetting(sb.ToString());
    }

    /// <summary>
    /// Replace all symbols with spaces when EnableSymbol is false
    /// </summary>
    private string ModifyTextBasedOnFlagSetting(string input)
    {
        var result = input;
        if (!EnableSymbol)
        {
            var sb = new StringBuilder();

            foreach (var c in input)
            {
                // Keep letters, digits, and existing spaces
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                {
                    sb.Append(c);
                }
                else
                {
                    // Replace symbols with space
                    sb.Append(' ');
                }
            }

            // Clean up multiple consecutive spaces
            result = sb.ToString();
            while (result.Contains("  "))
            {
                result = result.Replace("  ", " ");
            }

            result = result.Trim();
        }

        if (!EnableCapital)
        {
            result = result.ToLower();

        }

        return result;
    }

    /// <summary>
    /// Complexity formatted generator
    /// </summary>
    private string GenerateNumericPattern()
    {
        int patternType = _random.Next(1, 7);

        return patternType switch
        {
            1 => GenerateDecimal(),        // decimal number
            2 => GenerateFormattedNumber(), // format number
            3 => GenerateRange(),          // range number
            4 => GenerateFraction(),                 // fraction number
            5 => GeneratePhoneNumber(),              // telephone number
            _ => GenerateDigitSequence(3)   // normal digit sequence
        };
    }

    /// <summary>
    /// Decimal number generator
    /// </summary>
    private string GenerateDecimal()
    {
        var wholePart = _random.Next(0, 1000);
        var decimalPlaces = _random.Next(1, 2);
        var decimalPart = _random.Next(0, (int)Math.Pow(10, decimalPlaces));
        return $"{wholePart}.{decimalPart.ToString().PadRight(decimalPlaces, '0')}";
    }

    /// <summary>
    /// Formatted number generator, e.g."1,234,567.89", "$1,234.56", "12.3%"
    /// </summary>
    private string GenerateFormattedNumber()
    {
        var number = _random.Next(1000, 1000000);

        return _random.Next(3) switch
        {
            0 => number.ToString("N0"),
            1 => number.ToString("C"),
            _ => (number / 10000.0).ToString("P1")
        };
    }

    /// <summary>
    /// Number range generator
    /// </summary>
    private string GenerateRange()
    {
        var start = _random.Next(0, 100);
        var end = start + _random.Next(10, 100);
        return $"{start}-{end}";
    }

    /// <summary>
    /// Fraction number generator
    /// </summary>
    private string GenerateFraction()
    {
        var numerator = _random.Next(1, 10);
        var denominator = _random.Next(2, 20);
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
}