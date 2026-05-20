using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Application.DTO;

namespace Warehouse.Application.Interface.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(Guid id);
        //Task<ProductResponse?> GetProductByNameAsync(ProductRequest request);
        Task<ProductResponse> CreateProductAsync(ProductRequest request);
        Task<bool> UpdateProductAsync(Guid id, ProductRequest request);
        Task<bool> DeleteProductAsync(Guid id);
    }
}
