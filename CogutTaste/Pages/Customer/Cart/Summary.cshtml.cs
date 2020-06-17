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
using Stripe;

namespace CogutTaste.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public OrderDetailsCartVM detailsCart { get; set; }
        public IActionResult OnGet()
        {
            detailsCart = new OrderDetailsCartVM()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailsCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                detailsCart.listCart = cart.ToList();
            }

            foreach (var cartList in detailsCart.listCart)
            {
                cartList.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == cartList.MenuItemId);
                detailsCart.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
            }

            ApplicationUser applicationUser = _unitOfWork.AppUser.GetFirstOrDefault(c => c.Id == claim.Value);
            detailsCart.OrderHeader.PickupName = applicationUser.FullName;
            detailsCart.OrderHeader.PickUpTime = DateTime.Now;
            detailsCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            return Page();
        }

        public IActionResult OnPost(string stripeToken) // yukarıda bindproperty yaptığımızdan burada nesne geçirmeye gerek yok..
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            detailsCart.listCart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList();

            detailsCart.OrderHeader.PaymentStatus = StaticValues.PaymentStatusPending;
            detailsCart.OrderHeader.OrderDate = DateTime.Now;
            detailsCart.OrderHeader.UserId = claim.Value;
            detailsCart.OrderHeader.Status = StaticValues.PaymentStatusPending;
            detailsCart.OrderHeader.PickUpTime = Convert.ToDateTime(detailsCart.OrderHeader.PickUpDate.ToShortDateString() + " " + detailsCart.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _unitOfWork.OrderHeader.Add(detailsCart.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in detailsCart.listCart)
            {
                item.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = detailsCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                detailsCart.OrderHeader.OrderTotal += (orderDetails.Count * orderDetails.Price);
                _unitOfWork.OrderDetails.Add(orderDetails);

            }
            detailsCart.OrderHeader.OrderTotal = Convert.ToDouble(String.Format("{0:.##}", detailsCart.OrderHeader.OrderTotal)); // veri tabanına order total in cent bölümünü iki basamaklı olarak yuvarlar..
            _unitOfWork.ShoppingCart.RemoveRange(detailsCart.listCart); //cart daki bütün ürünleri silmek için IRepository ve Repository e sonradan ekledik
            HttpContext.Session.SetInt32(StaticValues.ShoppingCart, 0);
            _unitOfWork.Save();

            if (stripeToken != null)
            {

                var options = new ChargeCreateOptions
                {
                    //Amount is in cents
                    Amount = Convert.ToInt32(detailsCart.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order ID : " + detailsCart.OrderHeader.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options); // kredi kart ıburada charge edilir

                detailsCart.OrderHeader.TransactionId = charge.Id; // stripe tranaction ıd yi db kaydettik

                if (charge.Status.ToLower() == "succeeded") // başarılı ödeme yapıldıysa
                {
                    //email 
                    detailsCart.OrderHeader.PaymentStatus = StaticValues.PaymentStatusApproved;
                    detailsCart.OrderHeader.Status = StaticValues.StatusSubmitted;
                }
                else
                {
                    detailsCart.OrderHeader.PaymentStatus = StaticValues.PaymentStatusRejected;
                }
            }
            else
            {
                detailsCart.OrderHeader.PaymentStatus = StaticValues.PaymentStatusRejected;
            }
            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = detailsCart.OrderHeader.Id });

        }

    }
}