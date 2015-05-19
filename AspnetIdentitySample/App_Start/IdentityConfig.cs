﻿using AspnetIdentitySample.Models;
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
            microchip.IdentificationSystemName = "Microchip";
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
                //Pet.RabiesVaccinations = new List<RabiesVaccination>();
                //Pet.RabiesVaccinations.Add(new RabiesVaccination(new DateTime(2011, 01, 01), new DateTime(2011, 01, 01), new DateTime(2012, 01, 01), i));
                //Pet.RabiesVaccinations.Add(new RabiesVaccination(new DateTime(2012, 01, 01), new DateTime(2012, 01, 01), new DateTime(2013, 01, 01), i));
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

        }
    }
}