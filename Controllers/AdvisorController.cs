using System;
using System.Web;
using System.Collections.Generic;
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

namespace TheCCPConnection.Controllers
{
    public class AdvisorController : Controller
    {
        // private variables for controller
        private ApplicationDbContext identityContext = new ApplicationDbContext();

        private RequestContext requestContext = new RequestContext();

        // GET: Advisor/Index
        [Authorize(Roles = "Admin,Advisor")]
        public async Task<ActionResult> Index()
        {
            // get idenitity user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            // establish dB connections
            var advisors = await requestContext.Advisors.ToListAsync();
            var students = await requestContext.Students.ToListAsync();
            var courses = await requestContext.Courses.ToListAsync();
            var reqeusts = await requestContext.Requests.ToListAsync();
            var parentDecisions = await requestContext.ParentDecisions.ToListAsync();
            var counselorDecisions = await requestContext.CounselorDecisions.ToListAsync();
            var advisorDecisions = await requestContext.AdvisorDecisions.ToListAsync();
            var studentCancels = await requestContext.StudentCancels.ToListAsync();
            var advisorSchools = await requestContext.AdvisorSchools.ToListAsync();
            var studentSchools = await requestContext.StudentSchools.ToListAsync();

            // instantiate VM to be passed to view
            AdvisorDashboardVM dashboardData = new AdvisorDashboardVM()
            {
                Advisors = advisors,
                Students = students,
                Courses = courses,
                Requests = reqeusts,
                ParentDecisions = parentDecisions,
                CounselorDecisions = counselorDecisions,
                AdvisorDecisions = advisorDecisions,
                StudentCancels = studentCancels,
                AdvisorSchools = advisorSchools,
                StudentSchools = studentSchools
            };

            ViewBag.AddType = RequestType.Add;
            ViewBag.DropType = RequestType.Drop;

            // generate a list of students that go to this counselor's high school
            Advisor advisor = new Advisor();
            List<School> advisorHighschools = new List<School>();
            List<Student> advisorStudents = new List<Student>();

            if (advisors.Count() > 0)
            {
                foreach (Advisor advisorInList in advisors)
                {
                    if (advisorInList.ID == userId)
                    {
                        advisor = advisorInList;
                        ViewBag.AdvisorID = advisor.ID;
                        break;
                    }
                }
            }
            else
            {
                ViewBag.AdvisorID = null;
            }

            if (advisorSchools.Count() > 0)
            {
                foreach (AdvisorSchool advisorSchool in advisorSchools)
                {
                    if (advisorSchool.AdvisorID == advisor.ID && advisorSchool.School.Type == SchoolType.HighSchool)
                    {
                        advisorHighschools.Add(advisorSchool.School);
                        break;
                    }
                }
            }
            else
            {
                ViewBag.HighSchools = null;
            }

            if (studentSchools.Count() > 0)
            {
                foreach (StudentSchool studentSchool in studentSchools)
                {
                    foreach (School advisorHighschool in advisorHighschools)
                    if (studentSchool.SchoolID == advisorHighschool.ID)
                    {
                        advisorStudents.Add(studentSchool.Student);
                    }
                }
                ViewBag.AdvisorStudents = advisorStudents;
            }

            ViewBag.ParentDecisionMade = false;

            ViewBag.CounselorDecisionMade = false;

            ViewBag.AdvisorDecisionMade = false;

            return View(dashboardData);
        }

        // GET: Advisor/Details/id
        [Authorize(Roles = "Admin,Advisor,Counselor,Parent,Student")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advisor advisor = await requestContext.Advisors.FindAsync(id);
            if (advisor == null)
            {
                return HttpNotFound();
            }
            return View(advisor);
        }

        // GET: Advisor/Create/id
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Advisor advisor = new Advisor()
            {
                ID = id
            };
            
            var schools = await requestContext.Schools.ToListAsync();
            var schoolListItems = new List<SelectListItem>();
            foreach (School school in schools)
            {
                schoolListItems.Add(new SelectListItem { Text = school.Name.ToString(), Value = school.ID.ToString() });
            }

            CreateAdvisorVM viewModelData = new CreateAdvisorVM()
            {
                Advisor = advisor,
                Schools = schoolListItems
            };

            if (advisor == null)
            {
                return HttpNotFound();
            }
            return View(viewModelData);
        }

        // POST: Advisor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Include = "ID, LastName, FirstName, EmailAddress")] Advisor advisor, [Bind(Include = "AdvisorID, SchoolID")] List<AdvisorSchool> advisorSchools, CreateAdvisorVM createAdvisorVM)
        {
            try
            {
                if (ModelState.IsValid)
                {    
                    foreach (var school in createAdvisorVM.Schools)
                    {
                        if (school.Selected)
                        {
                            var advisorSchool = new AdvisorSchool();
                            advisorSchool.AdvisorID = advisor.ID;
                            advisorSchool.SchoolID = Guid.Parse(school.Value);
                            requestContext.AdvisorSchools.Add(advisorSchool);
                        }
                    }

                    requestContext.Advisors.Add(advisor);

                    await requestContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Admin");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(createAdvisorVM);
        }

        // GET: Advisor/Edit/id
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advisor advisor = await requestContext.Advisors.FindAsync(id);

            var schools = await requestContext.Schools.ToListAsync();
            var schoolListItems = new List<SelectListItem>();
            foreach (School school in schools)
            {
                schoolListItems.Add(new SelectListItem { Text = school.Name.ToString(), Value = school.ID.ToString() });
            }

            foreach (AdvisorSchool advisorSchool in requestContext.AdvisorSchools)
            {
                if (advisorSchool.AdvisorID == id)
                {
                    foreach (SelectListItem schoolListItem in schoolListItems)
                    {
                        if (Guid.Parse(schoolListItem.Value) == advisorSchool.SchoolID)
                        {
                            schoolListItem.Selected = true;
                            break;
                        }
                    }
                }
            }

            CreateAdvisorVM viewModelData = new CreateAdvisorVM()
            {
                Advisor = advisor,
                Schools = schoolListItems
            };

            if (advisor == null)
            {
                return HttpNotFound();
            }
            return View(viewModelData);
        }

        // POST: Advisor/Edit/id
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditAdvisor(Guid? id, CreateAdvisorVM createAdvisorVM)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advisor advisorToUpdate = await requestContext.Advisors.FindAsync(id);

            List<AdvisorSchool> advisorSchoolsToUpdate = new List<AdvisorSchool>();
            foreach (AdvisorSchool school in requestContext.AdvisorSchools)
            {
                if (school.AdvisorID == advisorToUpdate.ID)
                {
                    advisorSchoolsToUpdate.Add(school);
                }
            }
            if (TryUpdateModel(createAdvisorVM.Advisor, "",
               new string[] { "LastName", "FirstName", "EmailAddress" }))
            {
                try
                {
                    foreach (SelectListItem school in createAdvisorVM.Schools)
                    {
                        bool mathchFound = false;
                        foreach (AdvisorSchool schoolToUpdate in advisorSchoolsToUpdate)
                        {
                            if (Guid.Parse(school.Value) == schoolToUpdate.SchoolID)
                            {
                                mathchFound = true;
                                if (!school.Selected)
                                {
                                    requestContext.AdvisorSchools.Remove(schoolToUpdate);
                                    break;
                                }
                                else break;
                            }
                        }
                        if (school.Selected && !mathchFound)
                        {
                            var newSchoolToAdd = new AdvisorSchool();
                            newSchoolToAdd.AdvisorID = advisorToUpdate.ID;
                            newSchoolToAdd.SchoolID = Guid.Parse(school.Value);
                            requestContext.AdvisorSchools.Add(newSchoolToAdd);
                        }
                    }

                    // map viewmodel changes to model
                    advisorToUpdate.LastName = createAdvisorVM.Advisor.LastName;
                    advisorToUpdate.FirstName = createAdvisorVM.Advisor.FirstName;
                    advisorToUpdate.EmailAddress = createAdvisorVM.Advisor.EmailAddress;
                    UpdateModel(advisorToUpdate);
                    requestContext.SaveChanges();
                    return RedirectToAction("Index", "Admin");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(advisorToUpdate);
        }

        // delete methods removed - no data will be deleted by any user

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                identityContext.Dispose();
                requestContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
