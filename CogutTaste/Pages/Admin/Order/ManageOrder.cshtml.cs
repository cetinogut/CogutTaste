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

    public class ManageOrderModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManageOrderModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public List<OrderDetailsVM> orderDetailsVM { get; set; }


        public void OnGet()
        {
            orderDetailsVM = new List<OrderDetailsVM>();

            List<OrderHeader> OrderHeaderList = _unitOfWork.OrderHeader
                .GetAll(o => o.Status == StaticValues.StatusSubmitted || o.Status == StaticValues.StatusInProcess)
                .OrderByDescending(u => u.PickUpTime).ToList();


            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsVM individual = new OrderDetailsVM
                {
                    OrderHeader = item,
                    OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == item.Id).ToList()
                };
                orderDetailsVM.Add(individual);
            }
        }

        public IActionResult OnPostOrderPrepare(int orderId) // when click Start Cooking
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId);
            orderHeader.Status = StaticValues.StatusInProcess;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder");
        }

        public IActionResult OnPostOrderReady(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId);
            orderHeader.Status = StaticValues.StatusReady;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder");
        }

        public IActionResult OnPostOrderCancel(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId);
            orderHeader.Status = StaticValues.StatusCancelled;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder");
        }

        public IActionResult OnPostOrderRefund(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId);
            //refund amount
            var options = new RefundCreateOptions
            {
                Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                Reason = RefundReasons.RequestedByCustomer,
                Charge = orderHeader.TransactionId // chargeId charge a değişmiş.. video eski
            };
            var service = new RefundService();
            Refund refund = service.Create(options);

            orderHeader.Status = StaticValues.StatusRefunded;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder");

            
        }

    }
}