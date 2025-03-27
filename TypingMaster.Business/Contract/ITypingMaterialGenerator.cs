using TypingMaster.Core.Models;

namespace TypingMaster.Business.Contract;

public interface ITypingMaterialGenerator
{
    Task<string> GenerateTestMaterial(SkillLevel level, int wordCount = 100);

    Task<string> GenerateTestMaterialFromArticle(string url);
    
    Task<string> GenerateTestMaterialFromKeywords(string[] keywords, int wordCount = 100);

    Task<string> GenerateKeyPracticeText(char[] keys, int wordCount = 50, int minWordLength = 2, int maxWordLength = 4);
}