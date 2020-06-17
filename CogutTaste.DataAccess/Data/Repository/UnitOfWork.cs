using CogutTaste.DataAccess.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository
{
   public class UnitOfWork :IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            FoodType = new FoodTypeRepository(_db);
            MenuItem = new MenuItemRepository(_db);
            AppUser = new AppUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            SP_Call = new SP_Call(_db);
        }

        public ICategoryRepository Category { get; private set; }
        public IFoodTypeRepository FoodType { get; private set; }
        public IMenuItemRepository MenuItem { get; private set; }
        public IAppUserRepository AppUser { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public ISP_Call SP_Call { get; private set; }


        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
