using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Application.Interface.IRepository;
using Warehouse.Application.Interface.IService;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Enum;

namespace Warehouse.Application.Services
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync()
        {
            var transactions = await _unitOfWork.GetGenericRepository<InventoryTransaction>().GetAllAsync();
            var products = await _unitOfWork.GetGenericRepository<Product>().GetAllAsync();
            var users = await _unitOfWork.GetGenericRepository<User>().GetAllAsync();

            return transactions.Select(t => {
                var p = products.FirstOrDefault(prod => prod.Id == t.ProductId);
                var u = users.FirstOrDefault(usr => usr.Id == t.UserId);
                return MapToResponse(t, p, u);
            }).OrderByDescending(t => t.TransactionDate);
        }

        public async Task<IEnumerable<TransactionResponse>> GetTransactionsByProductIdAsync(Guid productId)
        {
            var transactions = await _unitOfWork.GetGenericRepository<InventoryTransaction>().GetAllAsync();
            var product = await _unitOfWork.GetGenericRepository<Product>().GetByIdAsync(productId);
            var users = await _unitOfWork.GetGenericRepository<User>().GetAllAsync();

            return transactions
                .Where(t => t.ProductId == productId)
                .Select(t => {
                    var u = users.FirstOrDefault(usr => usr.Id == t.UserId);
                    return MapToResponse(t, product, u);
                }).OrderByDescending(t => t.TransactionDate);
        }

        public async Task<(int Status, TransactionResponse? Data)> ProcessTransactionAsync(TransactionRequest request, Guid userId, TransactionType type)
        {
            var productRepo = _unitOfWork.GetGenericRepository<Product>();
            var transactionRepo = _unitOfWork.GetGenericRepository<InventoryTransaction>();

            // 1. Kiểm tra xem sản phẩm có tồn tại hoặc có đang bị xóa mềm ẩn đi không
            var product = await productRepo.GetByIdAsync(request.ProductId);
            if (product == null || product.IsDeleted) return (-1, null); // Lỗi -1: Không tìm thấy sản phẩm

            // 2. tăng / giảm số lượng kho
            if (type == TransactionType.Import)
            {
                product.QuantityInStock += request.Quantity;
            }
            else if (type == TransactionType.Export)
            {
                //Kiểm tra lượng tồn kho hiện tại xem có đủ để thực hiện lệnh xuất kho không
                if (product.QuantityInStock < request.Quantity) return (-2, null); // Lỗi -2: Số lượng tồn kho không đủ

                product.QuantityInStock -= request.Quantity;
            }

            var transaction = new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                UserId = userId,
                Type = type.ToString(), // Chuyển đổi thành chuỗi 'Import' hoặc 'Export' 
                Quantity = request.Quantity,
                TransactionDate = DateTime.UtcNow // fix UTC
            };

            productRepo.Update(product);
            await transactionRepo.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.GetGenericRepository<User>().GetByIdAsync(userId);
            return (0, MapToResponse(transaction, product, user)); // 0: ok
        }

        private TransactionResponse MapToResponse(InventoryTransaction t, Product? p, User? u)
        {
            return new TransactionResponse
            {
                Id = t.Id,
                ProductId = t.ProductId,
                ProductName = p?.Name ?? "Sản phẩm không tồn tại hoặc đã bị ẩn",
                UserId = t.UserId,
                UserName = u?.Username ?? "Hệ thống nội bộ",
                Type = t.Type,
                Quantity = t.Quantity,
                TransactionDate = t.TransactionDate,
                NewQuantityInStock = p?.QuantityInStock ?? 0
            };
        }
    }
}