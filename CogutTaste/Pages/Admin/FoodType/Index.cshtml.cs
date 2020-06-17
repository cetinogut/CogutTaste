using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CogutTaste.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CogutTaste.Pages.Admin.FoodType
{
    [Authorize(Roles = StaticValues.ManagerRole)] // sadece manager bu tabloyu görebilir
    public class IndexModel : PageModel
    {
        [BindProperty] // bunu elşediğimizde OnPost içine nesne geçirmemize gerek kalmadan otomatik olarak geçecek
        public Models.FoodType FoodTypeObj { get; set; }
        public void OnGet()
        {

        }
    }
}