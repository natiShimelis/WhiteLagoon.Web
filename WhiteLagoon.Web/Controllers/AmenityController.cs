using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AmenityController(ApplicationDbContext db)
        {
            _db = db;            
        }
        public IActionResult Index()
        {
            var amenities = _db.Amenities.Include(u=>u.Villa).ToList();
            return View(amenities);
        }
        public IActionResult Create()
        {
            AmenityVM AmenityVM = new()
            {
                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(AmenityVM);
            
        }
        [HttpPost]
        public IActionResult Create(AmenityVM obj)
        {            

            if (ModelState.IsValid)
            {
                _db.Amenities.Add(obj.Amenity);
                _db.SaveChanges();
                TempData["Success"] = "Amenity created successfully";
                return RedirectToAction("Index");
            }
            
            obj.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }
        public IActionResult Update(int amenityId)
        {
            AmenityVM AmenityVM = new()
            {
                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _db.Amenities.FirstOrDefault(v => v.Id == amenityId)
            };
            if (AmenityVM == null) 
            {
                return RedirectToAction("Error", "Home");
            }
            return View(AmenityVM);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM amenityVM)
        {

            if (ModelState.IsValid)
            {
                _db.Amenities.Update(amenityVM.Amenity);
                _db.SaveChanges();
                TempData["Success"] = "Amenity updated successfully";
                return RedirectToAction("Index");
            }

            amenityVM.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(amenityVM);
        }
        public IActionResult Delete(int amenityId)
        {
            AmenityVM AmenityVM = new()
            {
                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _db.Amenities.FirstOrDefault(v => v.Id == amenityId)
            };
            if (AmenityVM == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(AmenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM amenityVM)
        {
            Amenity? objFromDb = _db.Amenities
                .FirstOrDefault(v => v.Id == amenityVM.Amenity.Id);

            if (objFromDb is not null)
            {
                _db.Amenities.Remove(objFromDb);
                _db.SaveChanges();
                TempData["Success"] = "Amenity deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Amenity not found";
            return View();

        }
    }
}
