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
        public virtual ICollection<PetFile> PetFiles { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Species Species { get; set; }
        public virtual Gender Gender { get; set; }
        [Required]
        [Display(Name = "Species")]
        public int SpeciesId { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public int GenderId { get; set; }
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
        [Required]
        [Display(Name = "Pet")]
        public int PetID { get; set; }

        public virtual Pet Pet { get; set; }

        public RabiesVaccination(DateTime dateOfRabiesVaccination, DateTime dateOfValidityFrom, DateTime dateOfValidityTo, int petId)
        {
            this.DateOfRabiesVaccination = dateOfRabiesVaccination;
            this.DateOfValidityFrom = dateOfValidityFrom;
            this.DateOfValidityTo = dateOfValidityTo;
            this.PetID = petId;
        }

        public RabiesVaccination()
        {

        }
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

        [Required]
        [Display(Name = "Pet")]
        public int PetID { get; set; }

        public virtual Pet Pet { get; set; }
    }
    public class Consignor
    {
        public int ConsignorId { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Address1 { get; set; }
        [Required]
        public String Address2 { get; set; }
        [Required]
        public String Address3 { get; set; }
        [Required]
        public String Address4 { get; set; }
        [Required]
        public String Telephone { get; set; }
    }
    public class Consignee
    {
        public int ConsigneeId { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Address1 { get; set; }
        [Required]
        public String Address2 { get; set; }
        [Required]
        public String Address3 { get; set; }
        [Required]
        public String Address4 { get; set; }
        [Required]
        public String PostCode { get; set; }
        [Required]
        public String Telephone { get; set; }
    }
    public class Species
    {
        public int SpeciesId { get; set; }
        public String SpeciesName { get; set; }
    }
    public class Gender
    {
        public int GenderId { get; set; }
        public String GenderName { get; set; }
    }
    public class Identification
    {
        public int IdentificationId { get; set; }
        public int IdentificationTypeId { get; set; }
        public String IdentificationNumber { get; set; }
        public DateTime IdentificationDate { get; set; }
    }
    public class IdentificationType
    {
        public int IndentificationTypeId { get; set; }
        public String IdentificationTypeName { get; set; }
    }
    public class Certificate
    {
        public int CertId { get; set; }
        public Consignor Consignor { get; set; }
        public Consignee Consignee { get; set; }
        public virtual ICollection<Pet> Pets { get; set; }
        public String CountryOfOrigin { get; set; }
        public String ISOCode { get; set; }
        public String CommodityDescription { get; set; }


    }
    public class PetFile
    {
        public int PetFileId { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public FileType FileType { get; set; }
        public int PersonId { get; set; }
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
        public DbSet<RabiesVaccination> RabiesVaccinations { get; set; }
        public DbSet<Bloodtest> Bloodtests { get; set; }
        public DbSet<PetFile> PetFiles { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<Gender> Gender { get; set; }
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