namespace Application.Hubs;

public interface IUserConnectionMap<in T>
{
    void Add(T key, string connectionId);
    IEnumerable<string> GetConnections(T? key);
    void Remove(T key, string connectionId);


}