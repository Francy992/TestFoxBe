namespace Database.Core;

public interface IRepository<T, TKey> where T : class, IEntityId<TKey>
{
    Task<T> GetById(TKey id);
    Task<List<T>> FindAll();
    Task Insert(T item);
    void Update(T item);
    void Delete(T item);
    void DeleteRange(IEnumerable<T> items);
}