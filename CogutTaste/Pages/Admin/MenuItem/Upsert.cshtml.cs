using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models.ViewModels;
using CogutTaste.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CogutTaste.Pages.Admin.MenuItem
{
    [Authorize(Roles = StaticValues.ManagerRole)] // sadece manager bu tabloyu görebilir
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment; // bu bizzim wwwroot root folderimizdir

        public UpsertModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment )
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public MenuItemVM MenuItemObj { get; set; }

        public IActionResult OnGet(int? id)
        {
            MenuItemObj = new MenuItemVM()
            {

                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown(),
                FoodTypeList = _unitOfWork.FoodType.GetFoodTypeListForDropDown(),
                MenuItem = new Models.MenuItem()
            };
            if (id != null)
            {
                MenuItemObj.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(u => u.Id == id);
                if (MenuItemObj == null)
                {
                    return NotFound();
                }
            }
            return Page();

        }


        public IActionResult OnPost()
        {
            string webRootPath = _hostingEnvironment.WebRootPath; // bizi wwwroot folderine götürecek.. Burada images folderi altına menuItems adında bir klasör oluşturup resimleri oraya kaydedeceğiz.
            var files = HttpContext.Request.Form.Files; // kullanıcı tarafındna yüklenecek resim ve dosyalar için bir değişken tanımlıyoruz..


            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (MenuItemObj.MenuItem.Id == 0) // yeni bir menu ite mekliyoruzçç
            {
                string fileName = Guid.NewGuid().ToString(); // kullanıcını ismini yüklediği resmin ismin değiştirdik. global unique
                var uploads = Path.Combine(webRootPath, @"images\menuItems");
                var extension = Path.GetExtension(files[0].FileName); // dosya uzantısını aldık

                using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) // create this file inte server in this path location
                {
                    files[0].CopyTo(fileStream);
                }
                MenuItemObj.MenuItem.Image = @"\images\menuItems\" + fileName + extension;

                _unitOfWork.MenuItem.Add(MenuItemObj.MenuItem); // burada database e ekledik.. eğer var olan bir image i günceliyorsak aşağıya gidecek
            }
            else
            {
                //Edit Menu Item
                var objFromDb = _unitOfWork.MenuItem.Get(MenuItemObj.MenuItem.Id);
                if (files.Count > 0) 
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\menuItems");
                    var extension = Path.GetExtension(files[0].FileName);

                    var imagePath = Path.Combine(webRootPath, objFromDb.Image.TrimStart('\\')); // var olan file i silmek için path e ihtiyaç var..

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }


                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    MenuItemObj.MenuItem.Image = @"\images\menuItems\" + fileName + extension;
                }
                else // burası resim yüklenmeden sadece category veya price değiştiriliyorsa bakmak için kullanılır..
                {
                    MenuItemObj.MenuItem.Image = objFromDb.Image;
                }


                _unitOfWork.MenuItem.Update(MenuItemObj.MenuItem);
            }
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}