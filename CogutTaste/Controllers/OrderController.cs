using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models;
using CogutTaste.Models.ViewModels;
using CogutTaste.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CogutTaste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
        // ilh önce kullanıcının durumuna bakıp admin ise bütün order leri customer ise onun verdiklerini göstereceğiz..
       
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize] // sadece login olanların order listesini görmesin isağlar..
        public IActionResult Get(string status = null) // status boş ise hepsini sıralar.. aşağıda else de diğer şartlar var..
        {
            List<OrderDetailsVM> orderListVM = new List<OrderDetailsVM>();

            IEnumerable<OrderHeader> OrderHeaderList;

            if (User.IsInRole(StaticValues.CustomerRole))
            {
                //retrieve all orders for that customer
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                OrderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.UserId == claim.Value, null, "ApplicationUser");
            }
            else // eger role customer değilse bütün orderleri alacağız
            {
                OrderHeaderList = _unitOfWork.OrderHeader.GetAll(null, null, "ApplicationUser");
            }

            if (status == "cancelled")
            {
                OrderHeaderList = OrderHeaderList.Where(o => o.Status == StaticValues.StatusCancelled || o.Status == StaticValues.StatusRefunded || o.Status == StaticValues.PaymentStatusRejected); // cancelled ve diğer cancel etme türleri refunded ile ödeme reddedildiler de sıralanacak
            }
            else
            {
                if (status == "completed")
                {
                    OrderHeaderList = OrderHeaderList.Where(o => o.Status == StaticValues.StatusCompleted);
                }
                else // status inprocess, ready for pickup veya submitted...
                {
                    OrderHeaderList = OrderHeaderList.Where(o => o.Status == StaticValues.StatusReady || o.Status == StaticValues.StatusInProcess || o.Status == StaticValues.StatusSubmitted || o.Status == StaticValues.PaymentStatusPending);
                }
            }

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsVM individual = new OrderDetailsVM
                {
                    OrderHeader = item,
                    OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == item.Id).ToList()
                };
                orderListVM.Add(individual);
            }

            return Json(new { data = orderListVM });
        }

    }
}