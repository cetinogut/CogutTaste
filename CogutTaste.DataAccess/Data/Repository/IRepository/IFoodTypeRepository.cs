using CogutTaste.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository.IRepository
{
    public interface IFoodTypeRepository : IRepository<FoodType>
    {
        IEnumerable<SelectListItem> GetFoodTypeListForDropDown();

        void Update(FoodType foodType);
    }
}
