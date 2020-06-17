using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository
{
    class FoodTypeRepository : Repository<FoodType>, IFoodTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public FoodTypeRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }
        public IEnumerable<SelectListItem> GetFoodTypeListForDropDown()
        {
            return _db.FoodTypes.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }

        public void Update(FoodType foodType)
        {
            var objFromDb = _db.FoodTypes.FirstOrDefault(x => x.Id == foodType.Id);
            objFromDb.Name = foodType.Name;
            
            _db.SaveChanges();
        }
    }
}
