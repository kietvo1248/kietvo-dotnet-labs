using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Domain.Enum;

namespace Warehouse.Application.Interface.IService
{
    public interface IInventoryTransactionService
    {
        Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync();

        Task<IEnumerable<TransactionResponse>> GetTransactionsByProductIdAsync(Guid productId);

        Task<(int Status, TransactionResponse? Data)> ProcessTransactionAsync(TransactionRequest request, Guid userId, TransactionType type);
    }
}