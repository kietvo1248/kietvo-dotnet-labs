using System;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Application.Interface.IRepository;
using Warehouse.Application.Interface.IService;
using Warehouse.Domain.Entities;

namespace Warehouse.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(IUnitOfWork unitOfWork, IJwtProvider jwtProvider)
        {
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null) return false;

            string saltAndHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = saltAndHash,
                Email = request.Email,
                Role = "Staff"
            };

            await _unitOfWork.GetGenericRepository<User>().AddAsync(newUser);
            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            // Tìm User dựa trên Username
            var user = await _unitOfWork.UserRepository.GetByUsernameAsync(request.Username);
            if (user == null) return null;

            // check mật khẩu  người dùng 
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid) return null;

            // Nếu khớp, tiến hành cấp phát Token quyền truy cập
            var token = _jwtProvider.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            };
        }

    }
}
