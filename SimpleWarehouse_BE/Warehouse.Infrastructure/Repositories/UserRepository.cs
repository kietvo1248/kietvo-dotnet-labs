using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Warehouse.Application.Interface.IRepository;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.DBContext;

namespace Warehouse.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(WarehouseDBContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
