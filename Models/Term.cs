using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class Term
    {
        public int ID { get; set; }

        [Required]
        [StringLength(12)]
        public string Name { get; set; }
    }
}