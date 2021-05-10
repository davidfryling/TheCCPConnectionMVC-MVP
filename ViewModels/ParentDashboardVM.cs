using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheCCPConnection.Models;

namespace TheCCPConnection.ViewModels
{
    public class ParentDashboardVM : DashboardVM
    {
        public List<Parent> Parents { get; set; }
        
        public List<Student> Students { get; set; }
    }
}