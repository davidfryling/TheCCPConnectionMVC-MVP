using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheCCPConnection.Models
{
    public abstract class Decider
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
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }

        [Display(Name = "Name")]
        public string FirstLast
        {
            get { return FirstName + " " + LastName; }
        }

        [Display(Name = "Name")]
        public string LastFirst
        {
            get { return LastName + ", " + FirstName; }
        }
    }
}