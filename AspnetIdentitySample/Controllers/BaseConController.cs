using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspnetIdentitySample.Controllers
{
    public class BaseConController : Controller
    {
        public MyDbContext db;
        public UserManager<ApplicationUser> manager;

        public BaseConController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public List<Country> getEUCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.EU));
            return Countries.OrderBy(c => c.CountryName).ToList();
        }

        public List<Country> getLowRiskCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.LR));
            return Countries.OrderBy(c => c.CountryName).ToList();
        }

        public List<Country> getHighRiskCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.HR));
            return Countries.OrderBy(c => c.CountryName).ToList();
        }

        public List<Country> getAllCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries);
            return Countries.OrderBy(c => c.CountryName).ToList();
        }

        public List<Country> get3rdCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.LR));
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.HR));
            return Countries.OrderBy(c => c.CountryName).ToList();
        }
    }    
}