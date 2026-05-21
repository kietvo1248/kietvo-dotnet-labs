using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty; // Mã định danh duy nhất
        public string Name { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Guid CategoryId { get; set; }

        // Quan hệ Nhiều - 1: Sản phẩm thuộc về một danh mục
        public virtual Category Category { get; set; } = null!;

        // Quan hệ 1 - Nhiều: Sản phẩm có nhiều lịch sử giao dịch nhập/xuất
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
