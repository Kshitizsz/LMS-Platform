using LMS.Domain.Interfaces;
using LMS.Infrastructure.Persistence;

namespace LMS.Infrastructure.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(AppDbContext db) => _db = db;

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
            _repositories[type] = new GenericRepository<T>(_db);
        return (IRepository<T>)_repositories[type];
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public void Dispose() => _db.Dispose();
}