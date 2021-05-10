using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using TheCCPConnection.DAL;
using TheCCPConnection.Models;
using TheCCPConnection.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;

namespace TheCCPConnection.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext identityContext = new ApplicationDbContext();

        private RequestContext requestContext = new RequestContext();

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            var users = await identityContext.Users.ToListAsync();
            var counselors = await requestContext.Counselors.ToListAsync();
            var advisors = await requestContext.Advisors.ToListAsync();
            var schools = await requestContext.Schools.ToListAsync();

            var userStore = new UserStore<ApplicationUser>(identityContext);
            var userManager = new UserManager<ApplicationUser>(userStore);
            List<IdentityUser> counselorUsers = new List<IdentityUser>();
            List<IdentityUser> advisorUsers = new List<IdentityUser>();

            foreach (var user in users)
            {
                bool isInAdvisorRole = userManager.IsInRole(user.Id, "Advisor");
                if (isInAdvisorRole)
                {
                    advisorUsers.Add(user);
                }
                bool isInCounselorRole = userManager.IsInRole(user.Id, "Counselor");
                if (isInCounselorRole)
                {
                    counselorUsers.Add(user);
                }
            }

            AdminDashboardVM dashboardData = new AdminDashboardVM()
            {
                CounselorUsers = counselorUsers,
                AdvisorUsers = advisorUsers,
                Counselors = counselors,
                Advisors = advisors,
                Schools = schools
            };

            ViewBag.UserProfileFound = false;

            return View(dashboardData);
        }
    }
}