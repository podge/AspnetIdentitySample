using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AspnetIdentitySample
{
    // This is useful if you do not want to tear down the database each time you run the application.
    // You want to create a new database if the Model changes
    // public class MyDbInitializer : DropCreateDatabaseIfModelChanges<MyDbContext>
    public class MyDbInitializer : DropCreateDatabaseAlways<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        private void InitializeIdentityForEF(MyDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var myinfo = new MyUserInfo() { FirstName = "Pranav", LastName = "Rastogi" };
            string name = "Admin";
            string password = "123456";
            string test = "test";

            //Create Role Test and User Test
            RoleManager.Create(new IdentityRole(test));
            UserManager.Create(new ApplicationUser() { UserName = test });

            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists(name))
            {
                var roleresult = RoleManager.Create(new IdentityRole(name));
            }

            //Create User=Admin with password=123456
            var user = new ApplicationUser();
            user.UserName = name;
            user.HomeTown = "Seattle";
            user.MyUserInfo = myinfo;
            var adminresult = UserManager.Create(user, password);

            //Add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, name);
            }

            //Create User=Podge with password=123456
            user = new ApplicationUser();
            user.UserName = "Podge";
            user.HomeTown = "Lucan";
            user.MyUserInfo = new MyUserInfo() { FirstName = "Padraic", LastName = "Lavin" };

            // Create Species
            Species cat = new Species();
            cat.SpeciesName = "Cat";
            cat.ScientificName = "Feline";
            Species dog = new Species();
            dog.SpeciesName = "Dog";
            dog.ScientificName = "Canine";

            // Create Gender
            Gender male = new Gender();
            male.GenderName = "Male";
            Gender female = new Gender();
            female.GenderName = "Female";
            Gender neutered = new Gender();
            neutered.GenderName = "Male (Neutered)";
            Gender spayed = new Gender();
            spayed.GenderName = "Female (Spayed)";

            // Create IdentificationSystem
            IdentificationSystem microchip = new IdentificationSystem();
            microchip.IdentificationSystemName = "Transponder";
            IdentificationSystem tattoo = new IdentificationSystem();
            tattoo.IdentificationSystemName = "Tattoo";

            // Create Pets
            ICollection<Pet> pets = new List<Pet>();

            for(int i=1; i<=3; i++){
                Pet Pet = new Pet();
                Pet.Name = "Rosco"+i;
                Pet.Breed = "Norwegian Elkhound";
                Pet.Colour = "Grey";
                Pet.DateOfBirth = new DateTime(2010, 1, 1);
                Pet.IdentificationSystem = microchip;
                Pet.MicrochipNumber = "9851200000100"+i.ToString("D2");
                Pet.DateOfMicrochipping = new DateTime(2010, 2, 1);
                Pet.Species = dog;
                Pet.Gender = male;
                pets.Add(Pet);

                Pet Pet2 = new Pet();
                Pet2.Name = "Puddy"+i;
                Pet2.Breed = "Tomcat";
                Pet2.Colour = "Tortoiseshell";
                Pet2.DateOfBirth = new DateTime(2012, 1, 1);
                Pet2.IdentificationSystem = tattoo;
                Pet2.MicrochipNumber = "9851200000200"+i.ToString("D2");
                Pet2.DateOfMicrochipping = new DateTime(2012, 3, 1);
                Pet2.Species = cat;
                Pet2.Gender = female;
                pets.Add(Pet2);
            }

            user.Pets = pets;

            adminresult = UserManager.Create(user, "123456");

            // Countries
            // EU Member States
            context.Countries.Add(new Country("Austria", "IE", Country.CountryType.EU));
            context.Countries.Add(new Country("Belgium", "BE", Country.CountryType.EU));
            context.Countries.Add(new Country("Bulgaria", "BG", Country.CountryType.EU));
            context.Countries.Add(new Country("Croatia", "HR", Country.CountryType.EU));
            context.Countries.Add(new Country("Cyprus", "CY", Country.CountryType.EU));
            context.Countries.Add(new Country("Czech Republic", "CZ", Country.CountryType.EU));
            context.Countries.Add(new Country("Denmark", "DK", Country.CountryType.EU));
            context.Countries.Add(new Country("Estonia", "EE", Country.CountryType.EU));
            context.Countries.Add(new Country("Finland", "FI", Country.CountryType.EU));
            context.Countries.Add(new Country("France", "FR", Country.CountryType.EU));
            context.Countries.Add(new Country("Germany", "DE", Country.CountryType.EU));
            context.Countries.Add(new Country("Greece", "EL", Country.CountryType.EU));
            context.Countries.Add(new Country("Hungary", "HU", Country.CountryType.EU));
            context.Countries.Add(new Country("Ireland", "IE", Country.CountryType.EU));
            context.Countries.Add(new Country("Italy", "IT", Country.CountryType.EU));
            context.Countries.Add(new Country("Latvia", "LV", Country.CountryType.EU));
            context.Countries.Add(new Country("Lithuania", "LT", Country.CountryType.EU));
            context.Countries.Add(new Country("Luxembourg", "LU", Country.CountryType.EU));
            context.Countries.Add(new Country("Malta", "MT", Country.CountryType.EU));
            context.Countries.Add(new Country("Netherlands", "NL", Country.CountryType.EU));
            context.Countries.Add(new Country("Poland", "PL", Country.CountryType.EU));
            context.Countries.Add(new Country("Portugal", "PT", Country.CountryType.EU));
            context.Countries.Add(new Country("Romania", "RO", Country.CountryType.EU));
            context.Countries.Add(new Country("Slovakia", "SK", Country.CountryType.EU));
            context.Countries.Add(new Country("Slovenia", "SI", Country.CountryType.EU));
            context.Countries.Add(new Country("Spain", "ES", Country.CountryType.EU));
            context.Countries.Add(new Country("Sweden", "SE", Country.CountryType.EU));
            context.Countries.Add(new Country("United Kingdom", "UK", Country.CountryType.EU));
            
            // Territories
            context.Countries.Add(new Country("Andorra", "AD", Country.CountryType.LR));
            context.Countries.Add(new Country("Switzerland", "CH", Country.CountryType.LR));
            context.Countries.Add(new Country("Faeroe Islands", "FO", Country.CountryType.LR));
            context.Countries.Add(new Country("Gibraltar", "GI", Country.CountryType.LR));
            context.Countries.Add(new Country("Greenland", "GL", Country.CountryType.LR));
            context.Countries.Add(new Country("Iceland", "IS", Country.CountryType.LR));
            context.Countries.Add(new Country("Liechtenstein", "LI", Country.CountryType.LR));
            context.Countries.Add(new Country("Monaco", "MC", Country.CountryType.LR));
            context.Countries.Add(new Country("Norway", "NO", Country.CountryType.LR));
            context.Countries.Add(new Country("San Marino", "SM", Country.CountryType.LR));
            context.Countries.Add(new Country("Vatican City State", "VA", Country.CountryType.LR));

            // Low Risk Third Countries
            context.Countries.Add(new Country("Ascension Island", "AC", Country.CountryType.LR));

            context.Countries.Add(new Country("United Arab Emirates", "AE", Country.CountryType.LR));
            context.Countries.Add(new Country("Antigua and Barbuda", "AG", Country.CountryType.LR));
            context.Countries.Add(new Country("Argentina", "AR", Country.CountryType.LR));
            context.Countries.Add(new Country("Australia", "AU", Country.CountryType.LR));
            context.Countries.Add(new Country("Aruba", "AW", Country.CountryType.LR));
            context.Countries.Add(new Country("Bosnia and Herzegovina", "BA", Country.CountryType.LR));
            context.Countries.Add(new Country("Barbados", "BB", Country.CountryType.LR));
            context.Countries.Add(new Country("Bahrain", "BH", Country.CountryType.LR));
            context.Countries.Add(new Country("Bermuda", "BM", Country.CountryType.LR));
            context.Countries.Add(new Country("Bonaire, Sint Eustatius and Saba (the BES Islands)", "BQ", Country.CountryType.LR));
            context.Countries.Add(new Country("Belarus", "BY", Country.CountryType.LR));
            context.Countries.Add(new Country("Canada", "CA", Country.CountryType.LR));
            context.Countries.Add(new Country("Chile", "CL", Country.CountryType.LR));
            context.Countries.Add(new Country("Curaçao", "CW", Country.CountryType.LR));
            context.Countries.Add(new Country("Fiji", "FJ", Country.CountryType.LR));
            context.Countries.Add(new Country("Falkland Islands", "FK", Country.CountryType.LR));
            context.Countries.Add(new Country("Hong Kong", "HK", Country.CountryType.LR));
            context.Countries.Add(new Country("Jamaica", "JM", Country.CountryType.LR));
            context.Countries.Add(new Country("Japan", "JP", Country.CountryType.LR));
            context.Countries.Add(new Country("Saint Kitts and Nevis", "KN", Country.CountryType.LR));
            context.Countries.Add(new Country("Cayman Islands", "KY", Country.CountryType.LR));
            context.Countries.Add(new Country("Saint Lucia", "LC", Country.CountryType.LR));
            context.Countries.Add(new Country("Montserrat", "MS", Country.CountryType.LR));
            context.Countries.Add(new Country("Mauritius", "MU", Country.CountryType.LR));
            context.Countries.Add(new Country("Mexico", "MX", Country.CountryType.LR));
            context.Countries.Add(new Country("Malaysia", "MY", Country.CountryType.LR));
            context.Countries.Add(new Country("New Caledonia", "NC", Country.CountryType.LR));
            context.Countries.Add(new Country("New Zealand", "NZ", Country.CountryType.LR));
            context.Countries.Add(new Country("French Polynesia", "PF", Country.CountryType.LR));
            context.Countries.Add(new Country("Saint Pierre and Miquelon", "PM", Country.CountryType.LR));
            context.Countries.Add(new Country("Russia", "RU", Country.CountryType.LR));
            context.Countries.Add(new Country("Singapore", "SG", Country.CountryType.LR));
            context.Countries.Add(new Country("Saint Helena", "SH", Country.CountryType.LR));
            context.Countries.Add(new Country("Sint Maarten", "SX", Country.CountryType.LR));
            context.Countries.Add(new Country("Trinidad and Tobago", "TT", Country.CountryType.LR));
            context.Countries.Add(new Country("Taiwan", "TW", Country.CountryType.LR));

            context.Countries.Add(new Country("United States of America", "US", Country.CountryType.LR));

            context.Countries.Add(new Country("American Samoa", "AS", Country.CountryType.LR));
            context.Countries.Add(new Country("Guam", "GU", Country.CountryType.LR));
            context.Countries.Add(new Country("Northern Mariana Islands", "MP", Country.CountryType.LR));
            context.Countries.Add(new Country("Puerto Rico", "PR", Country.CountryType.LR));
            context.Countries.Add(new Country("US Virgin Islands", "VI", Country.CountryType.LR));
            context.Countries.Add(new Country("Saint Vincent and the Grenadines", "VC", Country.CountryType.LR));
            context.Countries.Add(new Country("British Virgin Islands", "VG", Country.CountryType.LR));
            context.Countries.Add(new Country("Vanuatu", "VU", Country.CountryType.LR));
            context.Countries.Add(new Country("Wallis and Futuna", "WF", Country.CountryType.LR));
            context.Countries.Add(new Country("Mayotte", "YT", Country.CountryType.LR));


            // High Risk Third Countries
            context.Countries.Add(new Country("China", "CN", Country.CountryType.HR));

            context.SaveChanges();

            Consignor Consignor = new Consignor();
            Consignor.ConsignorName = "Consignor Lavin";
            Consignor.Address1 = "Address1";
            Consignor.Address2 = "Address2";
            Consignor.Address3 = "Address3";
            Consignor.Address4 = "Address4";
            Consignor.Postcode = "Postcode";
            Consignor.Telephone = "123456789";
            Consignor.CountryId = 1;
            Consignor.User = user;

            context.Consignors.Add(Consignor);
            context.SaveChanges();

            Consignee Consignee = new Consignee();
            Consignee.ConsigneeName = "Consignee Lavin";
            Consignee.Address1 = "Address1";
            Consignee.Address2 = "Address2";
            Consignee.Address3 = "Address3";
            Consignee.Address4 = "Address4";
            Consignee.Postcode = "Postcode";
            Consignee.Telephone = "123456789";
            Consignee.User = user;

            context.Consignees.Add(Consignee);
            context.SaveChanges();


            // Rabies Vaccinations
            var myPets = from s in context.Pets
                       where (s.User.Id == user.Id)
                       select s;
           
            foreach (Pet item in myPets)
            {
                RabiesVaccination rv = new RabiesVaccination();
                rv.BatchNo = "batch";
                rv.DateOfRabiesVaccination = new DateTime(2015, 01, 01);
                rv.DateOfValidityFrom = new DateTime(2015, 01, 01);
                rv.DateOfValidityTo = new DateTime(2016, 01, 01);
                rv.Manufacturer = "Nobivac";
                rv.Pet = item;
                rv.PetID = item.Id;
                context.RabiesVaccinations.Add(rv);

                Bloodtest bt = new Bloodtest();
                bt.DateOfBloodtest = new DateTime(2015, 02, 01);
                bt.Pet = item;
                bt.PetID = item.Id;
                bt.Result = true;

                context.Bloodtests.Add(bt);
            }

            context.SaveChanges();

        }
    }
}