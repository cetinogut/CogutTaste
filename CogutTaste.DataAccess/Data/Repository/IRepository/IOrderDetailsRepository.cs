using CogutTaste.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        void Update(OrderDetails orderDetails); // bunu IRepositoryden farklı oalrak kendisi implement edecek...
    }
}
