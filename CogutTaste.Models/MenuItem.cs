using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CogutTaste.Models
{
    public class MenuItem
    {
        public int Id { get; set; } // Burada Key data annotation yazmadık.. Çünkü prop Id olarak adlandırıldığından otomatik olarak veritabanına primary key olarak atancak

        [Required (ErrorMessage = "Ürün adını girmek zorundasınız...")]
        [Display(Name = "Food Type Adı")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        [Required(ErrorMessage = "Ürün fiyatını girmek zorundasınız...")]
        [Range(1, int.MaxValue, ErrorMessage = "Price should be greater than $1")] // Range min ve max değer aralığını bildirir
        public double Price { get; set; }

        [Display(Name = "Category Tipi")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }// bu ve üst satır FK yı kurar ve tablolar arası ilişkiyi sağlar..

        [Display(Name = "Food Tipi")]
        public int FoodTypeId { get; set; }

        [ForeignKey("FoodTypeId")]
        public virtual FoodType FoodType { get; set; }
    }
}
