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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            // 1. Gọi Generic Repository của Product thông qua UnitOfWork
            var productRepo = _unitOfWork.GetGenericRepository<Product>();

            // 2. Lấy danh sách thực thể thô từ Database
            var products = await productRepo.GetAllAsync();

            // 3. Thực hiện ánh xạ dữ liệu sang DTO (Manual Mapping theo nguyên lý KISS)
            var productDtos = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                QuantityInStock = p.QuantityInStock,
                Price = p.Price,
                CategoryId = p.CategoryId
            });

            return productDtos;
        }
    }
}
