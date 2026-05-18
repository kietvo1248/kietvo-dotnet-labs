using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Entities;

namespace Warehouse.Application.Interface.IRepository
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
