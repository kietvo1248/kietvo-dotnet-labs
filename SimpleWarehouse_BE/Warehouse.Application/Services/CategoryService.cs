using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Application.Interface.IRepository;
using Warehouse.Application.Interface.IService;
using Warehouse.Domain.Entities;

namespace Warehouse.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService (IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.GetGenericRepository<Category>().GetAllAsync();
            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.GetGenericRepository<Category>().GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CategoryRequest request)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            await _unitOfWork.GetGenericRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryResponse { Id = category.Id, Name = category.Name, Description = category.Description };
        }

        public async Task<bool> UpdateCategoryAsync(Guid id, CategoryRequest request)
        {
            var categoryRepo = _unitOfWork.GetGenericRepository<Category>();
            var category = await categoryRepo.GetByIdAsync(id);
            if (category == null) return false;

            category.Name = request.Name;
            category.Description = request.Description;
//ImApfrom
            categoryRepo.Update(category);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<int> DeleteCategoryAsync(Guid id)
        {
            // 1. Kiểm tra xem danh mục có tồn tại không
            var categoryRepo = _unitOfWork.GetGenericRepository<Category>();
            var category = await categoryRepo.GetByIdAsync(id);
            if (category == null) return -1; // ko tìm thấy

            // 2. KIỂM TRA SẢN PHẨM RÀNG BUỘC
            var allProducts = await _unitOfWork.GetGenericRepository<Product>().GetAllAsync();
            var associatedProductCount = allProducts.Count(p => p.CategoryId == id);

            if (associatedProductCount > 0)
            {
                return associatedProductCount; // Trả về số lượng sản phẩm đang có, ko cho xóa
            }

            categoryRepo.Delete(category);
            await _unitOfWork.SaveChangesAsync();
            return 0; // okem
        }
    }
}
