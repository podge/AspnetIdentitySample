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
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Breed { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string Colour { get; set; }
        [Required]
        [Display(Name = "Identification System")]
        public int IdentificationSystemId { get; set; }
        [Required]
        [Display(Name = "Identification Number")]
        [StringLength(15, MinimumLength = 9)]
        public string MicrochipNumber { get; set; }
        [Display(Name = "Identification Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfMicrochipping { get; set; }
        public virtual ICollection<RabiesVaccination> RabiesVaccinations { get; set; }
        public virtual ICollection<Bloodtest> FAVNBloodTests { get; set; }
        public virtual ICollection<PetFile> PetFiles { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Species Species { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual IdentificationSystem IdentificationSystem { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<Irregularity> Irregularities { get; set; }
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
        [StringLength(20, MinimumLength = 1)]
        public string Manufacturer { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string BatchNo { get; set; }
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
        [Range(1, int.MaxValue, ErrorMessage = "You must select a pet.")]
        [Display(Name = "Pet")]
        public int PetID { get; set; }

        public virtual Pet Pet { get; set; }

        public RabiesVaccination(DateTime dateOfRabiesVaccination, DateTime dateOfValidityFrom, DateTime dateOfValidityTo, Pet pet, string batchNo, string manufacturer)
        {
            this.DateOfRabiesVaccination = dateOfRabiesVaccination;
            this.DateOfValidityFrom = dateOfValidityFrom;
            this.DateOfValidityTo = dateOfValidityTo;
            this.BatchNo = batchNo;
            this.Manufacturer = manufacturer;
            this.PetID = pet.Id;
            this.Pet = pet;
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
        [Display(Name = "Consignor")]
        public String ConsignorName { get; set; }
        [Required]
        public String Address1 { get; set; }
        [Required]
        public String Address2 { get; set; }
        [Required]
        public String Address3 { get; set; }
        [Required]
        public String Address4 { get; set; }
        [Required]
        public String Postcode { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country.")]
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        [Required]
        public String Telephone { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string DropdownName
        {
            get { return ConsignorName + " " + Address1 + " " + Address2 + " " + Address3 + " " + Address4 + " " + Postcode; }
        }
    }
    public class Consignee
    {
        public int ConsigneeId { get; set; }
        [Required]
        [Display(Name = "Consignee")]
        public String ConsigneeName { get; set; }
        [Required]
        public String Address1 { get; set; }
        [Required]
        public String Address2 { get; set; }
        [Required]
        public String Address3 { get; set; }
        [Required]
        public String Address4 { get; set; }
        [Required]
        public String Postcode { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country.")]
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        [Required]
        public String Telephone { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string DropdownName
        {
            get { return ConsigneeName + " " + Address1 + " " + Address2 + " " + Address3 + " " + Address4 + " " + Postcode; }
        }
    }
    public class Species
    {
        public int SpeciesId { get; set; }
        public String SpeciesName { get; set; }
        public String ScientificName { get; set; }
    }
    public class Gender
    {
        public int GenderId { get; set; }
        public String GenderName { get; set; }
    }
    public class IdentificationSystem
    {
        public int IdentificationSystemId { get; set; }
        public String IdentificationSystemName { get; set; }
    }
    public class Certificate
    {
        public int CertificateId { get; set; }
        [Required]
        public int ConsignorId { get; set; }
        [Required]
        public int ConsigneeId { get; set; }
        [Required]
        public List<int> PetIDs { get; set; }
        public Consignor Consignor { get; set; }
        public Consignee Consignee { get; set; }
        public int Pet1 { get; set; }
        public int Pet2 { get; set; }
        public int Pet3 { get; set; }
        public int Pet4 { get; set; }
        public int Pet5 { get; set; }
        [Required]
        public String CountryOfOrigin { get; set; }
        [Required]
        public String ISOCode { get; set; }
        [Required]
        public String CommodityDescription { get; set; }
        public bool Paid { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Pet> Pets { get; set; }
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
    public class StripeChargeModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string Email { get; set; }

        // These fields are optional and are not 
        // required for the creation of the token
        public string CardHolderName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressPostcode { get; set; }
        public string AddressCountry { get; set; }
    }
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryISOCode { get; set; }
        public CountryType Location { get; set; } // EU LR or HR
        public enum CountryType { EU, LR, HR}
        //public virtual List<Consignor> Consignors { get; set; }
        //public virtual List<Consignee> Consignees { get; set; }
        public Country()
        {

        }
        public Country(string name, string iso, CountryType location)
        {
            this.CountryName = name;
            this.CountryISOCode = iso;
            this.Location = location;
        }
    }
    public class Irregularity
    {
        public int IrregularityId { get; set; }
        [Required]
        public int IrregularityCode { get; set; }
        [Required]
        public string IrregularityText { get; set; }
        [Required]
        [Display(Name = "Date Raised")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateRaised { get; set; }
        [Display(Name = "Date Resolved")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateResolved { get; set; }
        public virtual ICollection<Pet> Pets { get; set; }
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
            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            //modelBuilder.Entity<Pet>().HasMany(p => p.RabiesVaccinations).WithOptional().WillCascadeOnDelete();
            //modelBuilder.Entity<Pet>().HasMany(p => p.FAVNBloodTests).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<Pet>().HasMany(p => p.PetFiles).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<Consignor>().HasRequired(c => c.Country).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Consignee>().HasRequired(c => c.Country).WithMany().WillCascadeOnDelete(false);
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<MyUserInfo> MyUserInfo { get; set; }
        public DbSet<RabiesVaccination> RabiesVaccinations { get; set; }
        public DbSet<Bloodtest> Bloodtests { get; set; }
        public DbSet<PetFile> PetFiles { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<IdentificationSystem> IdentificationSystem { get; set; }
        public DbSet<Certificate> Certificate { get; set; }
        public DbSet<Consignee> Consignees { get; set; }
        public DbSet<Consignor> Consignors { get; set; }
        public DbSet<Irregularity> Irregularities { get; set; }
        public DbSet<Country> Countries { get; set; }
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
            if (Pet != null)
            {
                if (DateGiven >= Pet.DateOfBirth && DateGiven <= DateTime.Today)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    String error = "";
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
            else
            {
                return ValidationResult.Success;
            }            
        }
    }
}