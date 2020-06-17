using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            var orderHeaderFromDb = _db.OrderHeader.FirstOrDefault(m => m.Id == orderHeader.Id);

            //menuItemFromDb.Name = menuItem.Name;
            //menuItemFromDb.CategoryId = menuItem.CategoryId;
            //menuItemFromDb.Description = menuItem.Description;
            //menuItemFromDb.FoodTypeId = menuItem.FoodTypeId;
            //menuItemFromDb.Price = menuItem.Price;
            //if (menuItem.Image != null) // if image is uploaded before
            //{
            //    menuItemFromDb.Image = menuItem.Image;
            //}

            // yukarıda property leri tek tek update ediyouz.. MenuItem de böyle id.. Eğer bütün bir nesneyi güncelleyecek isek aşağıdaki gibi yapabiliri

            _db.OrderHeader.Update(orderHeaderFromDb);
            _db.SaveChanges();

        }
    }
}
