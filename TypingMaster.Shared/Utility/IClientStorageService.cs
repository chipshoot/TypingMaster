namespace TypingMaster.Shared.Utility;

public interface IClientStorageService
{
    void SetItem<T>(string key, T value);

    T GetItem<T>(string key);

    void RemoveItem(string key);
}