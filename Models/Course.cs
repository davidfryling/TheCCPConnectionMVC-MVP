using System;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class Course
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Course Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Course Number")]
        public int Number { get; set; }

        [Required]
        [Display(Name = "Section Number")]
        public string Section { get; set; }

        [Required]
        [Display(Name = "Synonym Number")]
        public string Synonym { get; set; }

        [Required]
        [Display(Name = "Credit Hours")]
        [Range(1, 6)]
        public float Credits { get; set; }

        [Required]
        [Display(Name = "Course Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Course Days")]
        public string Days { get; set; }

        [Required]
        [Display(Name = "Course Times")]
        public string Times { get; set; }

        [Display(Name = "Course Instructor")]
        public string InstructorName { get; set; }

        public Guid SchoolID { get; set; }
        public virtual School School { get; set; }

        [Display(Name = "Course Term")]
        public int TermID { get; set; }
        public virtual Term Term { get; set; }

        [Display(Name = "Course Information")]
        public string FullCourse
        {
            get { return Title + " with " + InstructorName + " (" + Subject + "-" + Number + "-" + Section + "-" + Synonym + " " + FullDayTime + ")"; }
        }

        [Display(Name = "Days/Times")]
        public string FullDayTime
        {
            get { return Days + " " + Times; }
        }
    }
}