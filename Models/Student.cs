using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheCCPConnection.Models
{ 
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ID { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Please capitalize the first letter. No spaces or special characters please.")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Please capitalize the first letter. No spaces or special characters please.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "College ID")]
        [StringLength(10, ErrorMessage = "ID number cannot be longer than 10 characters.")]
        public string CID { get; set; }

        [Display(Name = "State ID")]
        [StringLength(10, ErrorMessage = "ID number cannot be longer than 10 characters.")]
        public string SSID { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Graduation Year")]
        [Range(2022, 2040, ErrorMessage = "Please enter the full year (ex. 2022).")]
        public int GradYear { get; set; }

        [Display(Name = "Max Credits")]
        [Range(-30, 30)]
        public float MaxCredits { get; set; }

        public Guid? ParentID { get; set; }
        public virtual Parent Parent { get; set; }

        [Display(Name = "Schools")]
        public virtual ICollection<StudentSchool> StudentSchools { get; set; }

        [Display(Name = "Student Name")]
        public string FirstLast
        {
            get { return FirstName + " " + LastName; }
        }

        [Display(Name = "Student Name (Last, First)")]
        public string LastFirst
        {
            get { return LastName + ", " + FirstName; }
        }
    }
}