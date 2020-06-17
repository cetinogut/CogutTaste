using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CogutTaste.Models
{

    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;// count un her zaman birden başlamasını istediğimizden constructor da 1 ile başlattık
        }
        public int Id { get; set; }

        public int MenuItemId { get; set; }

        [NotMapped]
        [ForeignKey("MenuItemId")] // alt satırdaki MenuItem tablosuna FK olarak bağlıyoruz..
        public virtual MenuItem MenuItem { get; set; }

        public string ApplicationUserId { get; set; } // string çünkü ıd GUID stringi

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }


        [Range(1, 100, ErrorMessage = "Please select a count between 1 and 100")]
        public int Count { get; set; }
    }
}
