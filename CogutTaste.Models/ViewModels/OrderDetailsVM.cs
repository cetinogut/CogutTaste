using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.Models.ViewModels
{
    public class OrderDetailsVM
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
