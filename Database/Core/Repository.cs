using Microsoft.EntityFrameworkCore;

namespace Database.Core;

public class Repository<T, TKey> : IRepository<T, TKey> where T : class, IEntityId<TKey>
{
    protected readonly DbSet<T> DbSet;

    protected Repository(DbContext dbContext)
    {
        DbSet = dbContext.Set<T>();
    }
    
    protected virtual IQueryable<T> GetDefaultQuery()
    {
        return DbSet.AsQueryable().AsNoTracking();
    }
        
    public async Task<T> GetById(TKey id)
    {
        return await GetDefaultQuery().FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<List<T>> FindAll()
    {
        return await GetDefaultQuery().ToListAsync();
    }

    public async Task Insert(T item)
    {
        if (item == null)
            return;

        await DbSet.AddAsync(item);
    }
    
    
    public void Update(T item)
    {
        if (item == null)
            return;

        DbSet.Update(item);
    }
    
    public void Delete(T item)
    {
        if (item == null)
            return;
        
        DbSet.Remove(item);
    }

    public void DeleteRange(IEnumerable<T> items)
    {
        if (items == null)
            return;
        
        DbSet.RemoveRange(items);
    }
}