using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class InventoryTransaction
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } = string.Empty; 
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }

        // Quan hệ Nhiều - 1: Giao dịch liên kết tới một Sản phẩm cụ thể
        public virtual Product Product { get; set; } = null!;

        // Quan hệ Nhiều - 1: Giao dịch được thực hiện bởi một Người dùng cụ thể
        public virtual User User { get; set; } = null!;
    }
}
