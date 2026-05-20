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
        public async Task<ProductResponse?> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.GetGenericRepository<Product>().GetByIdAsync(id);
            if (product == null) return null;

            return new ProductResponse
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                QuantityInStock = product.QuantityInStock,
                Price = product.Price,
                CategoryId = product.CategoryId
            };
        }
        public async Task<ProductResponse> CreateProductAsync(ProductRequest request)
        {
            var category = await _unitOfWork.GetGenericRepository<Category>().GetByIdAsync(request.CategoryId);
            if (category == null) throw new Exception("Category not found");

            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Sku = request.SKU,
                Name = request.name,
                Price = request.price,
                QuantityInStock = 0, // Mặc định khi tạo mới sẽ là 0
                CategoryId = request.CategoryId
            };
            await _unitOfWork.GetGenericRepository<Product>().AddAsync(newProduct);
            await _unitOfWork.SaveChangesAsync();

            return new ProductResponse
            {
                Id = newProduct.Id,
                Sku = newProduct.Sku,
                Name = newProduct.Name,
                Price = newProduct.Price,
                QuantityInStock = newProduct.QuantityInStock,
                CategoryId = newProduct.CategoryId
            };
        }
        public async Task<bool> UpdateProductAsync(Guid id, ProductRequest request)
        {
            var productRepo = _unitOfWork.GetGenericRepository<Product>();
            var Product = await productRepo.GetByIdAsync(id);
            var existCategory = await _unitOfWork.GetGenericRepository<Category>().GetByIdAsync(request.CategoryId);
            if (Product == null || existCategory == null) return false;

            Product.Name = request.name;
            Product.Sku = request.SKU;
            Product.Price = request.price;
            Product.CategoryId = request.CategoryId;

            productRepo.Update(Product);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var productRepo = _unitOfWork.GetGenericRepository<Product>();
            var Product = await productRepo.GetByIdAsync(id);

        //chưa triển khai
            return true;
        }
    }
}
