using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IOrderRepository
    {
        void CreateOrder(Order order);
    }
}
