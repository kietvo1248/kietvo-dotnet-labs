using Warehouse.Domain.Entities;
using System.Threading.Tasks;

namespace Warehouse.Application.Interface.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}
