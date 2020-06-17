using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CogutTaste.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : Controller // ControllerBase olarak oluşturulmuştu biz controller e değiştirdik
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment; // bu bizzim wwwroot root folderimizdir
        public MenuItemController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.MenuItem.GetAll(null, null, "Category,FoodType") }); //burada UnitofWotk un GEtAll fonkssiyonun kullanacağız ve elimizfe category id ve foodtype id warken bunların ismini getirmek için INCLUDE özeeliğini kullanıyoruz.. Yazdığımız parametreler ile model isimler itamamen aynı olmalı..
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var objFromDb = _unitOfWork.MenuItem.GetFirstOrDefault(u => u.Id == id);
                if (objFromDb == null)
                {
                    return Json(new { success = false, message = "Error while deleting." });
                }

                // burada image yüklenip yüklenmediğini kontrol ediyoruz..
                var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, objFromDb.Image.TrimStart('\\')); // root folderimize gidiyoruz..
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _unitOfWork.MenuItem.Remove(objFromDb);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            return Json(new { success = true, message = "Delete success." });
        }

    }
}