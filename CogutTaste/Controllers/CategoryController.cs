using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CogutTaste.Controllers // bu bir api. 
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CategoryController : Controller // ControllerBase olarak oluşturulmuştu biz controller e değiştirdik
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.Category.GetAll() }); // normalde böyle çalışıyordu.. proje sonunda SP kullanarak aynı işi yaptık.. Aşğıdaki satırı ekledik..
            //return Json(new { data = _unitOfWork.SP_Call.ReturnList<Category>("usp_GetAllCategory", null) }); // sistemi yeni kurarken db de sp olmazsa hata vereceğinden yeniden eski haline çevirdim..
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Category.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }

    }
}