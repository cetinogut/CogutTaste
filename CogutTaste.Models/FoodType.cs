using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CogutTaste.Models
{
    public class FoodType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Food Type Adı")]
        public string Name { get; set; }
    }
}
