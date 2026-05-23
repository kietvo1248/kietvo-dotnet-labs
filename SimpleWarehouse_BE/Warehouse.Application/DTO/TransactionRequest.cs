using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Enum;

namespace Warehouse.Application.DTO
{
    public class TransactionRequest
    {
        public Guid ProductId { get; set; }
        public TransactionType Type { get; set; } // Sử dụng Enum (0 = Import, 1 = Export)
        public int Quantity { get; set; }
    }
}
