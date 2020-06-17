using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CogutTaste.Pages.Admin.Category
{
    [Authorize(Roles = StaticValues.ManagerRole)] // sadece manager bu tabloyu görebilir
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Models.Category CategoryObj { get; set; }

        public IActionResult OnGet(int? id)
        {
            CategoryObj = new Models.Category();
            if (id != null)
            {
                CategoryObj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
                if (CategoryObj == null)
                {
                    return NotFound();
                }
            }
            return Page();

        }


        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (CategoryObj.Id == 0)
            {
                _unitOfWork.Category.Add(CategoryObj);
            }
            else
            {
                _unitOfWork.Category.Update(CategoryObj);
            }
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}