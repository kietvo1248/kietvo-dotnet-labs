using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Application.DTO;

namespace Warehouse.Application.Interface.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse?> GetCategoryByIdAsync(Guid id);
        Task<CategoryResponse> CreateCategoryAsync(CategoryRequest request);
        Task<bool> UpdateCategoryAsync(Guid id, CategoryRequest request);

        Task<int> DeleteCategoryAsync(Guid id);
    }
}
