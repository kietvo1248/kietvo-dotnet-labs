using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Application.DTO;

namespace Warehouse.Application.Interface.IService
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<bool> ChangeUserRoleAsync(Guid userId, ChangeRoleRequest request);

      
        Task<IEnumerable<string>> SeedSampleUsersAsync();
    }
}
