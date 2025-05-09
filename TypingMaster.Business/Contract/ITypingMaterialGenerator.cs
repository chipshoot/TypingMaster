using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business.Contract;

public interface ITypingMaterialGenerator
{
    Task<string> GenerateTestMaterial(SkillLevel level, int wordCount = 100);

    Task<string> GenerateTestMaterialFromArticle(string url);
    
    Task<string> GenerateTestMaterialFromKeywords(string[] keywords, int wordCount = 100);

    Task<string> GenerateKeyPracticeText(char[] keys, int wordCount = 50, int minWordLength = 2, int maxWordLength = 4);


    /// <summary>
    /// Gets or sets the result of the process, including status, error messages, and additional information.
    /// </summary>
    ProcessResult ProcessResult { get; set; }
}