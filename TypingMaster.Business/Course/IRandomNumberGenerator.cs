namespace TypingMaster.Business.Course;

public interface IRandomNumberGenerator
{
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
}

public class RandomNumberGenerator : IRandomNumberGenerator
{
    private static readonly Random Random = new();

    public int Next(int maxValue) => Random.Next(maxValue);
    public int Next(int minValue, int maxValue) => Random.Next(minValue, maxValue);
}