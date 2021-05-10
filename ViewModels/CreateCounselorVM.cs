using System.Collections.Generic;
using System.Web.Mvc;
using TheCCPConnection.Models;

namespace TheCCPConnection.ViewModels
{
    public class CreateCounselorVM
    {
        public Counselor Counselor { get; set; }

        public List<SelectListItem> Schools { get; set; }
    }
}