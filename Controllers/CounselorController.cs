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
    public class CounselorController : Controller
    {
        // private variables for controller
        private ApplicationDbContext identityContext = new ApplicationDbContext();

        private RequestContext requestContext = new RequestContext();

        // GET: Counselor/Index
        [Authorize(Roles = "Admin,Counselor")]
        public async Task<ActionResult> Index()
        {
            // get idenitity user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            // establish dB connections
            var counselors = await requestContext.Counselors.ToListAsync();
            var students = await requestContext.Students.ToListAsync();
            var courses = await requestContext.Courses.ToListAsync();
            var reqeusts = await requestContext.Requests.ToListAsync();
            var parentDecisions = await requestContext.ParentDecisions.ToListAsync();
            var counselorDecisions = await requestContext.CounselorDecisions.ToListAsync();
            var advisorDecisions = await requestContext.AdvisorDecisions.ToListAsync();
            var studentCancels = await requestContext.StudentCancels.ToListAsync();
            var counselorSchools = await requestContext.CounselorSchools.ToListAsync();
            var studentSchools = await requestContext.StudentSchools.ToListAsync();

            // instantiate VM to be passed to view
            CounselorDashboardVM dashboardData = new CounselorDashboardVM()
            {
                Counselors = counselors,
                Students = students,
                Courses = courses,
                Requests = reqeusts,
                ParentDecisions = parentDecisions,
                CounselorDecisions = counselorDecisions,
                AdvisorDecisions = advisorDecisions,
                StudentCancels = studentCancels,
                CounselorSchools = counselorSchools,
                StudentSchools = studentSchools
            };

            ViewBag.AddType = RequestType.Add;
            ViewBag.DropType = RequestType.Drop;

            // generate a list of students that go to this counselor's high school
            Counselor counselor = new Counselor();
            School counselorHighschool = new School();
            List<Student> counselorStudents = new List<Student>();

            if (counselors.Count() > 0)
            {
                foreach (Counselor counselorInList in counselors)
                {
                    if (counselorInList.ID == userId)
                    {
                        counselor = counselorInList;
                        ViewBag.CounselorID = counselor.ID;
                        break;
                    }
                }
            }
            else
            {
                ViewBag.CounselorID = null;
            }

            if (counselorSchools.Count() > 0)
            {
                foreach (CounselorSchool counselorSchool in counselorSchools)
                {
                    if (counselorSchool.CounselorID == counselor.ID && counselorSchool.School.Type == SchoolType.HighSchool)
                    {
                        counselorHighschool = counselorSchool.School;
                        break;
                    }
                }
            }
            else
            {
                ViewBag.HighSchoolID = null;
            }

            if (studentSchools.Count() > 0)
            {
                foreach (StudentSchool studentSchool in studentSchools)
                {
                    if (studentSchool.SchoolID == counselorHighschool.ID)
                    {
                        counselorStudents.Add(studentSchool.Student);
                    }
                }
                ViewBag.CounselorStudents = counselorStudents;
            }

            ViewBag.ParentDecisionMade = false;

            ViewBag.CounselorDecisionMade = false;

            ViewBag.AdvisorDecisionMade = false;

            return View(dashboardData);
        }

        // GET: Counselor/Details/id
        [Authorize(Roles = "Admin,Advisor,Counselor,Parent,Student")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Counselor counselor = await requestContext.Counselors.FindAsync(id);
            if (counselor == null)
            {
                return HttpNotFound();
            }
            return View(counselor);
        }

        // GET: Counselor/Create/id
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Counselor counselor = new Counselor()
            {
                ID = id
            };

            var schools = await requestContext.Schools.ToListAsync();
            var schoolListItems = new List<SelectListItem>();
            foreach (School school in schools)
            {
                schoolListItems.Add(new SelectListItem { Text = school.Name.ToString(), Value = school.ID.ToString() });
            }

            CreateCounselorVM viewModelData = new CreateCounselorVM()
            {
                Counselor = counselor,
                Schools = schoolListItems
            };
            if (counselor == null)
            {
                return HttpNotFound();
            }
            return View(viewModelData);
        }

        // POST: Counselor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Include = "ID, LastName, FirstName, EmailAddress")] Counselor counselor, [Bind(Include = "CounselorID, SchoolID")] List<CounselorSchool> counselorSchools, CreateCounselorVM createCounselorVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var school in createCounselorVM.Schools)
                    {
                        if (school.Selected)
                        {
                            var counselorSchool = new CounselorSchool();
                            counselorSchool.CounselorID = counselor.ID;
                            counselorSchool.SchoolID = Guid.Parse(school.Value);
                            requestContext.CounselorSchools.Add(counselorSchool);
                        }
                    }

                    requestContext.Counselors.Add(counselor);

                    await requestContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Admin");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(createCounselorVM);
        }

        // GET: Counselor/Edit/id
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Counselor counselor = await requestContext.Counselors.FindAsync(id);

            var schools = await requestContext.Schools.ToListAsync();
            var schoolListItems = new List<SelectListItem>();
            foreach (School school in schools)
            {
                schoolListItems.Add(new SelectListItem { Text = school.Name.ToString(), Value = school.ID.ToString() });
            }

            foreach (CounselorSchool counselorSchool in requestContext.CounselorSchools)
            {
                if (counselorSchool.CounselorID == id)
                {
                    foreach (SelectListItem schoolListItem in schoolListItems)
                    {
                        if (Guid.Parse(schoolListItem.Value) == counselorSchool.SchoolID)
                        {
                            schoolListItem.Selected = true;
                            break;
                        }
                    }
                }
            }

            CreateCounselorVM viewModelData = new CreateCounselorVM()
            {
                Counselor = counselor,
                Schools = schoolListItems
            };

            if (counselor == null)
            {
                return HttpNotFound();
            }
            return View(viewModelData);
        }

        // POST: Counselor/Edit/id
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditCounselor(Guid? id, CreateCounselorVM createCounselorVM)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Counselor counselorToUpdate = await requestContext.Counselors.FindAsync(id);

            List<CounselorSchool> counselorSchoolsToUpdate = new List<CounselorSchool>();
            foreach (CounselorSchool school in requestContext.CounselorSchools)
            {
                if (school.CounselorID == counselorToUpdate.ID)
                {
                    counselorSchoolsToUpdate.Add(school);
                }
            }

            if (TryUpdateModel(createCounselorVM.Counselor, "",
               new string[] { "LastName", "FirstName", "EmailAddress" }))
            {
                try
                {
                    foreach (SelectListItem school in createCounselorVM.Schools)
                    {
                        bool mathchFound = false;
                        foreach (CounselorSchool schoolToUpdate in counselorSchoolsToUpdate)
                        {
                            if (Guid.Parse(school.Value) == schoolToUpdate.SchoolID)
                            {
                                mathchFound = true;
                                if (!school.Selected)
                                {
                                    requestContext.CounselorSchools.Remove(schoolToUpdate);
                                    break;
                                }
                                else break;
                            }
                        }
                        if (school.Selected && !mathchFound)
                        {
                            var newSchoolToAdd = new CounselorSchool();
                            newSchoolToAdd.CounselorID = counselorToUpdate.ID;
                            newSchoolToAdd.SchoolID = Guid.Parse(school.Value);
                            requestContext.CounselorSchools.Add(newSchoolToAdd);
                        }
                    }

                    // map viewmodel changes to model
                    counselorToUpdate.LastName = createCounselorVM.Counselor.LastName;
                    counselorToUpdate.FirstName = createCounselorVM.Counselor.FirstName;
                    counselorToUpdate.EmailAddress = createCounselorVM.Counselor.EmailAddress;
                    UpdateModel(counselorToUpdate);
                    requestContext.SaveChanges();
                    return RedirectToAction("Index", "Admin");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(createCounselorVM);
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
