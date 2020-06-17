using CogutTaste.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncrementCount(ShoppingCart shoppingCart, int count);
        int DecrementCount(ShoppingCart shoppingCart, int count);
    }
}
 