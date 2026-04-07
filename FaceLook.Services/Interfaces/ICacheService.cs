namespace FaceLook.Services.Interfaces
{
    public interface ICacheService
    {
        void Add(string key, object data);
        bool TryGet<T>(string key, out T? data);
        T? Get<T>(string key) where T : class;
    }
}
