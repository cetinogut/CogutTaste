using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
       ICategoryRepository Category { get; }

       IFoodTypeRepository FoodType { get; }

       IMenuItemRepository MenuItem { get; }

       IAppUserRepository AppUser { get; }

       IShoppingCartRepository ShoppingCart { get; }

        IOrderDetailsRepository OrderDetails { get; }

        IOrderHeaderRepository OrderHeader { get; }

        ISP_Call SP_Call { get; }

        void Save();
        
    }
}
