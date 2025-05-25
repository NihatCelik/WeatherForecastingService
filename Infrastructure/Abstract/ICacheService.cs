namespace Infrastructure.Abstract;

public interface ICacheService
{
    T Get<T>(string key);

    Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan duration);

    void Set<T>(string key, T value, TimeSpan duration);

    void Remove(string key);
}