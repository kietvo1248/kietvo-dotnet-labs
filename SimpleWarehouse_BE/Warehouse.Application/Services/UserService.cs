using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Application.Interface.IRepository;
using Warehouse.Application.Interface.IService;
using Warehouse.Domain.Entities;

namespace Warehouse.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.GetGenericRepository<User>().GetAllAsync();
            return users.Select(u => new UserResponse
            {
                //Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role
            });
        }

        public async Task<bool> ChangeUserRoleAsync(Guid userId, ChangeRoleRequest request)
        {
            // Kiểm tra tính hợp lệ của quyền mới truyền lên
            var validRoles = new[] { "Admin", "Manager", "Staff" };
            if (!validRoles.Contains(request.NewRole)) return false;

            var userRepo = _unitOfWork.GetGenericRepository<User>();
            var user = await userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.Role = request.NewRole;
            userRepo.Update(user);

            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<string>> SeedSampleUsersAsync()
        {
            var createdUsers = new List<string>();
            var userGenericRepo = _unitOfWork.GetGenericRepository<User>();

            // Định nghĩa danh sách 2 tài khoản mẫu theo yêu cầu
            var sampleUsers = new[]
            {
                new { Username = "admin", Role = "Admin", Email = "admin@warehouse.com" },
                new { Username = "staff", Role = "Staff", Email = "staff@warehouse.com" }
            };

            // Băm mật khẩu mặc định "123" bằng BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123");

            foreach (var sample in sampleUsers)
            {
                // Kiểm tra xem tên tài khoản đã tồn tại trong DB chưa
                var existingUser = await _unitOfWork.UserRepository.GetByUsernameAsync(sample.Username);

                // Nếu chưa có thì tiến hành nạp vào DB
                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Username = sample.Username,
                        PasswordHash = hashedPassword,
                        Email = sample.Email,
                        Role = sample.Role
                    };

                    await userGenericRepo.AddAsync(newUser);
                    createdUsers.Add($"{sample.Username} ({sample.Role})");
                }
            }

            // Nếu có tài khoản được thêm mới thì thực hiện lưu xuống SQL Server
            if (createdUsers.Any())
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return createdUsers; // Trả về mảng danh sách để Controller báo cáo kết quả
        }
    }
}