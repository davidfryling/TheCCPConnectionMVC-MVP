using System.Collections.Generic;
using System.Web.Mvc;
using TheCCPConnection.Models;

namespace TheCCPConnection.ViewModels
{
    public class CreateAdvisorVM
    {
        public Advisor Advisor { get; set; }

        public List<SelectListItem> Schools { get; set; }
    }
}