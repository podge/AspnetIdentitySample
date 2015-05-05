using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspnetIdentitySample.Controllers
{
    public class PetCheckController : Controller
    {
        private MyDbContext db;
        private UserManager<ApplicationUser> manager;
        public PetCheckController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: PetCheck
        public async Task<ActionResult> Index(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = await db.Pets.FindAsync(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            if (pet.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            ViewBag.errorList = errors(pet);
            return View(pet);
            //return View();
        }

        public List<String> errors(Pet pet)
        {
            List<String> errorList = new List<string>();
            List<RabiesVaccination> vaxList = new List<RabiesVaccination>();
            List<Bloodtest> testList = new List<Bloodtest>();

            if (null == pet.RabiesVaccinations || pet.RabiesVaccinations.Count == 0)
            {
                errorList.Add("No rabies Vaccinations found.");
            }
            else
            {
                // Sort Rabies Vaccinations
                vaxList = pet.RabiesVaccinations.OrderBy(o => o.DateOfValidityFrom).ToList();
            }

            if (null == pet.FAVNBloodTests || pet.FAVNBloodTests.Count == 0)
            {
                errorList.Add("No bloodtests found.");
            }
            else
            {
                // Only get successful blood tests
                testList = pet.FAVNBloodTests.Where(x => x.Result == true).ToList();
                // Sort blood tests by date
                testList = testList.OrderByDescending(o => o.DateOfBloodtest).ToList();
                // Get most recent successful blood test;
                errorList.Add("Most recent successful bloodtest " + pet.FAVNBloodTests.First().DateOfBloodtest);

                errorList.Add("Pet is eligible for entry on or after " + pet.FAVNBloodTests.First().DateOfBloodtest.AddMonths(3));
            }

            
            return errorList;
        }
    }
}