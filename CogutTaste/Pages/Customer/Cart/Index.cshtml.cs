using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models;
using CogutTaste.Models.ViewModels;
using CogutTaste.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CogutTaste.Pages.Customer.Cart
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderDetailsCartVM OrderDetailsCartVM { get; set; }
        public void OnGet()
        {
            OrderDetailsCartVM = new OrderDetailsCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                listCart = new List<ShoppingCart>() // eğer claim nulls sa yani user logged in olmadan cart a tıklarsa patlamasın diye boşta olsa listCArt objesini oluşturduk
            };

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null) // yan iuser logged in olduysa git veritabanından shopping cart itemlerin igetir
            {
                IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value);

                if (cart != null)
                {
                    OrderDetailsCartVM.listCart = cart.ToList();
                }

                foreach (var cartList in OrderDetailsCartVM.listCart)
                {
                    cartList.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == cartList.MenuItemId);
                    OrderDetailsCartVM.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
                }
            }
        }

        public IActionResult OnPostPlus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostMinus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            if (cart.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();

                var cnt = _unitOfWork.ShoppingCart.
                                GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(StaticValues.ShoppingCart, cnt); // güncel sayıyı yularıda alıp session a yükleyeceğiz
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
                _unitOfWork.Save();

            }


            return RedirectToPage("/Customer/Cart/Index");
        }


        public IActionResult OnPostRemove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();

            var cnt = _unitOfWork.ShoppingCart.
                               GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(StaticValues.ShoppingCart, cnt);
            return RedirectToPage("/Customer/Cart/Index");
        }
    }
}