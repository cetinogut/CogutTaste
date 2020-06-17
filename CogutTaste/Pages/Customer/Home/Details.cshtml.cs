using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models;
using CogutTaste.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CogutTaste.Pages.Customer.Home
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public ShoppingCart ShoppingCartObj { get; set; }

        public void OnGet(int id)
        {
            ShoppingCartObj = new ShoppingCart()
            {
                MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(includeProperties: "Category,FoodType", filter: c => c.Id == id),
                MenuItemId = id
            };
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity; // shooping cart için user id getireceğiz
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCartObj.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.ApplicationUserId == ShoppingCartObj.ApplicationUserId &&
                                            c.MenuItemId == ShoppingCartObj.MenuItemId);

                if (cartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(ShoppingCartObj);
                }
                else
                {
                    _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, ShoppingCartObj.Count);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == ShoppingCartObj.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(StaticValues.ShoppingCart, count);
                return RedirectToPage("Index");

            }
            else
            {
                ShoppingCartObj.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(includeProperties: "Category,FoodType", filter: c => c.Id == ShoppingCartObj.MenuItemId);
                return Page();
            }
        }
    }
}