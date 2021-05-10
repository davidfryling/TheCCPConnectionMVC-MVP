using System;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class MaxCredit
    {
        public int ID { get; set; }

        [DataType(DataType.Date)]
        public DateTime TimeStamp { get; set; }

        public int SubTotal { get; set; }
        
        public Guid StudentID { get; set; }
        public virtual Student Student { get; set; }

        public int TermID { get; set; }
        public virtual Term Term { get; set; }

        public SchoolType Type { get; set; }
    }
}