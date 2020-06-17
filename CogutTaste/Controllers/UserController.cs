using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CogutTaste.Controllers // bu bir api. 
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller // ControllerBase olarak oluşturulmuştu biz controller e değiştirdik
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.AppUser.GetAll() });
        }

        [HttpPost] // delete olduğu gibi id gelmeyecek.. bunu FromBody den method içine aşağıdaki gibi alacağız.. Bu bir GUID string olacak
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now ) // user kilitlenmiş onu aşağıda açalım
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else // user kilitli değil kilitleyelim 100 yıl boyunca
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            }
           
            _unitOfWork.Save();
            return Json(new { success = true, message = "operation successful" });
        }

    }
}