using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models;
using CogutTaste.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CogutTaste.Pages.Customer.Home
{
    public class IndexModel : PageModel
    {

        // burada gelen customer e menu item listesini göstermemşiz lazım..  Bunun için menu item listesi ve kategori listesni burada tanımlayıp get de dolduracağız.. MenuItem upsert modelinde olduğu gibi farklı bir VM de yapablirdik.. Burada bu yöntemi kullandı..
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<MenuItem> MenuItemList { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }

        public void OnGet()
        {

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                int shoppingCartCount = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(StaticValues.ShoppingCart, shoppingCartCount);
            }

            MenuItemList = _unitOfWork.MenuItem.GetAll(null, null, "Category,FoodType"); // foodtype ve catagori adlarının gelmesi için yaptık
            CategoryList = _unitOfWork.Category.GetAll(null, q => q.OrderBy(c => c.DisplayOrder), null); // ayrıca bir liste daha göstereceğiz kategorilere göre..
        }
    }

}