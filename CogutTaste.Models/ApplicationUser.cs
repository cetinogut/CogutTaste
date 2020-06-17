using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CogutTaste.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Adı Soyadı")]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [NotMapped] // veri tabanına eklenmeyecek..
        public string FullName { get { return FirstName + " " + LastName; } }
    }
}
