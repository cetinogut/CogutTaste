using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository
{
    class AppUserRepository : Repository<ApplicationUser>, IAppUserRepository
    {
        private readonly ApplicationDbContext _db;

        public AppUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetCategoryListForDropDown()
        {
            return _db.Category.Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        public void Update(Category category)
        {
            var objFromDb = _db.Category.FirstOrDefault(s => s.Id == category.Id);

            objFromDb.Name = category.Name;
            objFromDb.DisplayOrder = category.DisplayOrder;

            _db.SaveChanges();

        }
    }
}
