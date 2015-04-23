using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AspnetIdentitySample.Models
{
    public class ApplicationUser : IdentityUser
    {
        // HomeTown will be stored in the same table as Users
        public string HomeTown { get; set; }
        public virtual ICollection<Pet> Pets { get; set; }
  
        // FirstName & LastName will be stored in a different table called MyUserInfo
        public virtual MyUserInfo MyUserInfo { get; set; }
    }

    public class MyUserInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Pet
    {
        public int Id { get; set; }
        //public string Description { get; set; }
        //public bool IsDone { get; set; }
        //public virtual ApplicationUser User { get; set; }

        //public int PetID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Breed { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 9)]
        public string MicrochipNumber { get; set; }
        public virtual ICollection<RabiesVaccination> RabiesVaccinations { get; set; }
        public virtual ICollection<Bloodtest> FAVNBloodTests { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
    public class RabiesVaccination
    {
        public int RabiesVaccinationID { get; set; }
        [Required]
        [ValidateDateRange]
        [Display(Name = "Date given")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfRabiesVaccination { get; set; }
        [Required]
        [Display(Name = "Date valid from")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfValidityFrom { get; set; }
        [Required]
        [Display(Name = "Date valid to")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfValidityTo { get; set; }

        [Display(Name = "Pet")]
        public int PetID { get; set; }

        public virtual Pet Pet { get; set; }
    }
    public class Bloodtest
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Date of blood taken")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBloodtest { get; set; }
        public Boolean Result { get; set; }

        [Display(Name = "Pet")]
        public int PetID { get; set; }

        public virtual Pet Pet { get; set; }
    }
    public class MyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Change the name of the table to be Users instead of AspNetUsers
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");
        }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<MyUserInfo> MyUserInfo { get; set; }

        public System.Data.Entity.DbSet<AspnetIdentitySample.Models.RabiesVaccination> RabiesVaccinations { get; set; }

        public System.Data.Entity.DbSet<AspnetIdentitySample.Models.Bloodtest> Bloodtests { get; set; }
    }

    public class ValidateDateRange : ValidationAttribute
    {
        private MyDbContext db = new MyDbContext();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // your validation logic
            DateTime DateGiven = (DateTime)value;
            RabiesVaccination rvax = (RabiesVaccination)validationContext.ObjectInstance;
            Pet Pet = db.Pets.FirstOrDefault(x => x.Id == rvax.PetID);
            if (DateGiven >= Pet.DateOfBirth && DateGiven <= DateTime.Today)
            {
                return ValidationResult.Success;
            }
            else
            {
                String error="";
                if (DateGiven < Pet.DateOfBirth)
                {
                    error = "Date must be after the pets date of birth " + Pet.DateOfBirth + ".";
                }
                else if (DateGiven > DateTime.Today)
                {
                    error = "Date can not be in the future.";
                }
            return new ValidationResult(error);                
            }
        }
    }


}