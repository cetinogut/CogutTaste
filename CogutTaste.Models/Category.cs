using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CogutTaste.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; } // Id ismi ortomatik olarak key olarak algılanır ve bir artırabilir olarak database tanımlanır.. ancak Id dışında bir ad veriyorsak data annotations ile key olduğunu belirtmemiz lazım

        [Required]
        [Display(Name="Category Adı")]
        public string Name { get; set; }

        [Display(Name = "Display Sırası")]
        public int DisplayOrder { get; set; }
    }
}
