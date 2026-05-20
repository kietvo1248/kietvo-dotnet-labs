using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Application.DTO
{
    public class CategoryDeleteExceptionResponse
    {
        public string Message { get; set; } = string.Empty;
        public int AssociatedProductCount { get; set; }
    }
}
