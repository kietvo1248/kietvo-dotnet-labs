using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Warehouse.Application.Interface.IRepository;
using Warehouse.Infrastructure.DBContext;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WarehouseDBContext _context;
        // Dictionary lưu trữ tạm thời các Repository đã được khởi tạo để tái sử dụng
        private readonly ConcurrentDictionary<Type, object> _repositories;

        public IUserRepository UserRepository { get; }

        public UnitOfWork(WarehouseDBContext context)
        {
            _context = context;
            _repositories = new ConcurrentDictionary<Type, object>();

            // Khởi tạo UserRepository
            UserRepository = new UserRepository(_context);
        }

        public IGenericRepository<T> GetGenericRepository<T>() where T : class
        {
            return (IGenericRepository<T>)_repositories.GetOrAdd(
                typeof(T),
                _ => new GenericRepository<T>(_context)
            );
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
