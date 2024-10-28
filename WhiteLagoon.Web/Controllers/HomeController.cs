using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.Models;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM
            {
                VillaList = _db.Villas.Include(v => v.VillaAmenity).ToList(),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };
            return View(homeVM);
        }
        [HttpPost]
        public IActionResult Index(HomeVM homeVM)
        {
            homeVM.VillaList = _db.Villas.Include(v => v.VillaAmenity);
            foreach(var villa in homeVM.VillaList)
            {
                if(villa.Id%2 == 0)
                {
                    villa.IsAvailable = false;
                }                
            }

            return View(homeVM);
        }
        public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
        {
            var villaList = _db.Villas.Include(v => v.VillaAmenity).ToList();
            foreach (var villas in villaList)
            {
                if (villas.Id % 2 == 0)
                {
                    villas.IsAvailable = false;
                }
            }
            HomeVM homeVM = new ()
            {
                CheckInDate = checkInDate,
                VillaList = villaList,
                Nights = nights                
            };

            return PartialView("_VillaList",homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult Error()
        {
            return View();
        }
    }
}
