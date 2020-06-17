using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models;
using CogutTaste.Models.ViewModels;
using CogutTaste.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace CogutTaste.Pages.Admin.Order
{
    public class OrderDetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public OrderDetailsVM OrderDetailsVM { get; set; }


        public void OnGet(int id)
        {
            OrderDetailsVM = new OrderDetailsVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(m => m.Id == id),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(m => m.OrderId == id).ToList()

            };

            OrderDetailsVM.OrderHeader.ApplicationUser = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == OrderDetailsVM.OrderHeader.UserId);
        }



        public IActionResult OnPostOrderConfirm(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId);
            orderHeader.Status = StaticValues.StatusCompleted;
            _unitOfWork.Save();
            return RedirectToPage("OrderList");
        }

        public IActionResult OnPostOrderCancel(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId);
            orderHeader.Status = StaticValues.StatusCancelled;
            _unitOfWork.Save();
            return RedirectToPage("OrderList");
        }

        public IActionResult OnPostOrderRefund(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId);
            //refund amount
            var options = new RefundCreateOptions
            {
                Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                Reason = RefundReasons.RequestedByCustomer,
                Charge = orderHeader.TransactionId
            };
            var service = new RefundService();
            Refund refund = service.Create(options);

            orderHeader.Status = StaticValues.StatusRefunded;
            _unitOfWork.Save();
            return RedirectToPage("OrderList");
        }
    }
}