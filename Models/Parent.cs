using System.Collections.Generic;

namespace TheCCPConnection.Models
{
    public class Parent : Decider
    {
        public virtual ICollection<Student> Students { get; set; }
    }
}