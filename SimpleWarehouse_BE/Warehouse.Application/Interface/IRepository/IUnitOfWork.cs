using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Application.Interface.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        // Hàm đa hình giúp lấy nhanh Repository của bất kỳ thực thể nào (Category, Product,...)
        IGenericRepository<T> GetGenericRepository<T>() where T : class;

        // Gọi trực tiếp UserRepository để xử lý logic Auth
        IUserRepository UserRepository { get; }

        // Hàm chốt chặn cuối cùng để lưu toàn bộ thay đổi xuống Database
        Task<int> SaveChangesAsync();
    }
}
