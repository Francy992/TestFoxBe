namespace Database.Core;

public interface IUnitOfWork : IDisposable
{
    Task SaveChanges();
}