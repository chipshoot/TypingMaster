using Microsoft.EntityFrameworkCore;
using System.Text;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Course;

public class NumericDrillGenerator : INumericDrillGenerator
{
    private readonly Random _random = new Random();

    private static readonly List<string> Stage2Templates =
    [
        "yyyy-MM-dd", "MM/dd/yyyy", "dd.MM.yy",
        "HH:mm:ss", "hh:mm tt", "H:m",
        "$#,###.00", "¥#,###", "€#,##0.00",
        "##.##%", "###%",
        "#/#", "##/##",
        "##.##°N, ##.##°E",
        "π=3.14159", "e≈2.71828", "φ=1.61803",
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
                return string.Empty;

            case NumericPracticePhases.SingleKeyFocus:
                // Simple repeated characters, e.g., "11111"
                return GenerateSimpleRepetition(count);

            case NumericPracticePhases.HorizontalCombination:
                // Simple patterns with keys, e.g., "12345"
                return GeneratePatterns(count);

            case NumericPracticePhases.VerticalCombos:
                // Another simple pattern, could be reversed or alternate
                return new string(Enumerable.Range(0, count)
                    .Select(i => (char)('0' + (i % 10))).ToArray());

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
                    "Phone: {0}", "ZIP: {0}", "ISBN: {0}", "EIN: {0}", "Score: {0}", "Temp: {0}°C"
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

    private string GenerateSimpleRepetition(int count)
    {
        return new string('1', count); // Example: simple repetition of '1'
    }

    private string GeneratePatterns(int count)
    {
        // Example: generate a simple numeric pattern like "1234567890"
        return string.Concat(Enumerable.Range(1, count % 10).Select(i => i.ToString()));
    }

    private string GenerateRealWords(int count)
    {
        // Example: generate a numeric drill with real words (could be numbers as words)
        return string.Join(" ", Enumerable.Range(1, count).Select(i => i.ToString()));
    }

    /// <summary>
    /// 生成阶段2练习字符串 - 实用数字模式
    /// </summary>
    public List<string> GenerateStage2Drills(int count = 10, int difficulty = 2)
    {
        var drills = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            string template = Stage2Templates[_random.Next(Stage2Templates.Count)];
            string result = ReplacePatterns(template, difficulty);
            drills.Add(result);
        }
        
        return drills;
    }

    /// <summary>
    /// 生成阶段3练习字符串 - 全键混合应用
    /// </summary>
    public List<string> GenerateStage3Drills(int count = 15, int difficulty = 2)
    {
        var drills = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            string template = Stage3Templates[_random.Next(Stage3Templates.Count)];
            string numericPart = GenerateNumericPattern(difficulty + 1);
            string result = string.Format(template, numericPart);
            
            // 添加随机标点
            if (_random.NextDouble() > 0.7)
            {
                result += _random.NextDouble() > 0.5 ? ";" : ".";
            }
            
            drills.Add(result);
        }
        
        return drills;
    }

    /// <summary>
    /// 替换模板中的模式字符为实际数字
    /// </summary>
    private string ReplacePatterns(string template, int difficulty)
    {
        StringBuilder sb = new StringBuilder();
        bool isEscape = false; // 用于处理转义字符
        
        foreach (char c in template)
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
                case 'y': // 年份
                    sb.Append(GenerateYear(difficulty));
                    break;
                case 'M': // 月份
                    sb.Append(GenerateMonth(difficulty));
                    break;
                case 'd': // 日期
                    sb.Append(GenerateDay(difficulty));
                    break;
                case 'H': // 24小时制小时
                    sb.Append(Generate24Hour());
                    break;
                case 'h': // 12小时制小时
                    sb.Append(Generate12Hour());
                    break;
                case 'm': // 分钟
                case 's': // 秒
                    sb.Append(GenerateMinuteSec());
                    break;
                case 't': // AM/PM
                    sb.Append(_random.NextDouble() > 0.5 ? "AM" : "PM");
                    break;
                case '*': // 掩码字符
                    sb.Append(_random.Next(10));
                    break;
                case '#': // 随机数字
                case '0': // 固定位置数字
                    sb.Append(_random.Next(10));
                    break;
                case ',': // 千位分隔符
                    sb.Append(_random.NextDouble() > 0.3 ? "," : "");
                    break;
                case '.': // 小数点
                    sb.Append(".");
                    break;
                case '°': // 度符号
                    sb.Append("°");
                    break;
                default:
                    sb.Append(c);
                    break;
            }
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// 生成随机年份
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
    /// 生成随机月份
    /// </summary>
    private string GenerateMonth(int difficulty)
    {
        int month = _random.Next(1, 13);
        return difficulty == 1 ? month.ToString("D2") : month.ToString();
    }

    /// <summary>
    /// 生成随机日期
    /// </summary>
    private string GenerateDay(int difficulty)
    {
        int day = _random.Next(1, 32);
        return difficulty == 1 ? day.ToString("D2") : day.ToString();
    }

    /// <summary>
    /// 生成24小时制时间
    /// </summary>
    private string Generate24Hour()
    {
        return _random.Next(0, 24).ToString("D2");
    }

    /// <summary>
    /// 生成12小时制时间
    /// </summary>
    private string Generate12Hour()
    {
        return _random.Next(1, 13).ToString("D2");
    }

    /// <summary>
    /// 生成分钟或秒
    /// </summary>
    private string GenerateMinuteSec()
    {
        return _random.Next(0, 60).ToString("D2");
    }

    /// <summary>
    /// 生成复杂的数字模式
    /// </summary>
    private string GenerateNumericPattern(int complexity)
    {
        int patternType = _random.Next(1, 7);
        
        return patternType switch
        {
            1 => GenerateDecimal(complexity),        // 小数
            2 => GenerateFormattedNumber(complexity), // 格式化数字
            3 => GenerateRange(complexity),          // 范围表示
            4 => GenerateFraction(),                 // 分数
            5 => GeneratePhoneNumber(),              // 电话号码
            _ => GenerateDigitSequence(complexity)   // 普通序列
        };
    }

    /// <summary>
    /// 生成数字序列
    /// </summary>
    private string GenerateDigitSequence(int difficulty)
    {
        int length = difficulty switch
        {
            1 => _random.Next(2, 5),  // 简单：2-4位
            2 => _random.Next(4, 7),  // 中等：4-6位
            _ => _random.Next(6, 10)  // 复杂：6-9位
        };
        
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(_random.Next(10));
        }
        return sb.ToString();
    }

    /// <summary>
    /// 生成小数
    /// </summary>
    private string GenerateDecimal(int complexity)
    {
        int wholePart = _random.Next(0, complexity * 1000);
        int decimalPlaces = _random.Next(1, complexity + 2);
        int decimalPart = _random.Next(0, (int)Math.Pow(10, decimalPlaces));
        return $"{wholePart}.{decimalPart.ToString().PadRight(decimalPlaces, '0')}";
    }

    /// <summary>
    /// 生成格式化数字
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
    /// 生成范围表示
    /// </summary>
    private string GenerateRange(int complexity)
    {
        int start = _random.Next(0, 100 * complexity);
        int end = start + _random.Next(10, 100 * complexity);
        return $"{start}-{end}";
    }

    /// <summary>
    /// 生成分数
    /// </summary>
    private string GenerateFraction()
    {
        int numerator = _random.Next(1, 10);
        int denominator = _random.Next(2, 20);
        return $"{numerator}/{denominator}";
    }

    /// <summary>
    /// 生成电话号码
    /// </summary>
    private string GeneratePhoneNumber()
    {
        return _random.Next(2) == 0
            ? $"{_random.Next(100, 1000)}-{_random.Next(100, 1000)}-{_random.Next(1000, 10000)}"
            : $"+{_random.Next(1, 99)} {_random.Next(100, 1000)}-{_random.Next(100, 1000)}-{_random.Next(1000, 10000)}";
    }
}

// 使用示例
public class Program
{
    public static void Main()
    {
        var generator = new NumericDrillGenerator();
        
        Console.WriteLine("=== Stage 2: Practical Number Patterns ===");
        var stage2Drills = generator.GenerateStage2Drills(8, 2);
        foreach (var drill in stage2Drills)
        {
            Console.WriteLine(drill);
        }
        
        Console.WriteLine("\n=== Stage 3: Mixed Application Drills ===");
        var stage3Drills = generator.GenerateStage3Drills(10, 2);
        foreach (var drill in stage3Drills)
        {
            Console.WriteLine(drill);
        }
    }
}
}