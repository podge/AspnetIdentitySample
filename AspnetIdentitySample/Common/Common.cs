using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspnetIdentitySample.Common
{
    public class Common
    {
        private static MyDbContext db;
        private static UserManager<ApplicationUser> manager;

        public Common()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public static List<Country> getEUCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.EU));
            return Countries.OrderBy(c => c.CountryName).ToList();
        }

        public static List<Country> getLowRiskCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.LR));
            return Countries.OrderBy(c => c.CountryName).ToList();
        }

        public static List<Country> getHighRiskCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.HR));
            return Countries.OrderBy(c => c.CountryName).ToList();
        }

        public static List<Country> getAllCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries);
            return Countries.OrderBy(c => c.CountryName).ToList();
        }

        public static List<Country> get3rdCountries()
        {
            List<Country> Countries = new List<Country>();
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.LR));
            Countries.AddRange(db.Countries.Where(c => c.Location == Country.CountryType.HR));
            return Countries.OrderBy(c => c.CountryName).ToList();
        }
    }
}