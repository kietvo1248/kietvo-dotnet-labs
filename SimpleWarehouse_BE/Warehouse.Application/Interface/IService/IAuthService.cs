
using System.Threading.Tasks;
using Warehouse.Application.DTO;

namespace Warehouse.Application.Interface.IService
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
    }
}
