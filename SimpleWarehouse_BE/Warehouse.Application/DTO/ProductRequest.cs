using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Application.DTO
{
    public class ProductRequest
    {
        public string SKU { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public decimal price { get; set; }
        public Guid CategoryId { get; set; }
    }
}
