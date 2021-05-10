using System;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class RequestWindow
    {
        public int ID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-DD-YYYY}", ApplyFormatInEditMode = true)]
        [Display(Name = "Open Date")]
        public DateTime OpenDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-DD-YYYY}", ApplyFormatInEditMode = true)]
        [Display(Name = "Close Date")]
        public DateTime CloseDate { get; set; }

        public Guid SchoolID { get; set; }
        public virtual School School { get; set; }

        public int TermID { get; set; }
        public virtual Term Term { get; set; }

        public RequestType Type { get; set; }
    }
}