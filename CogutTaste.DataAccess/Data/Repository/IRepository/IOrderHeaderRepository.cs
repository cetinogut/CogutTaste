using CogutTaste.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader); // bunu IRepositoryden farklı oalrak kendisi implement edecek...
    }
}
