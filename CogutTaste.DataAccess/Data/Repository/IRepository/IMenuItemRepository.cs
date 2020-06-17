using CogutTaste.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository.IRepository
{
    public interface IMenuItemRepository : IRepository<MenuItem>
    {
        void Update(MenuItem menuItem); // bunu IRepositoryden farklı oalrak kendisi implement edecek...
    }
}
