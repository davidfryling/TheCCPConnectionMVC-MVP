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

namespace TheCCPConnection
{
    public class StudentController : Controller
    {
        // private variables for controller
        private ApplicationDbContext identityContext = new ApplicationDbContext();

        private RequestContext requestContext = new RequestContext();

        // GET: Student/Index
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> Index()
        {
            // get idenitity user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);
            
            // establish dB connections
            var students = await requestContext.Students.ToListAsync();
            var parents = await requestContext.Parents.ToListAsync();
            var courses = await requestContext.Courses.ToListAsync();
            var reqeusts = await requestContext.Requests.ToListAsync();
            var parentDecisions = await requestContext.ParentDecisions.ToListAsync();
            var counselorDecisions = await requestContext.CounselorDecisions.ToListAsync();
            var advisorDecisions = await requestContext.AdvisorDecisions.ToListAsync();
            var studentCancels = await requestContext.StudentCancels.ToListAsync();

            // instantiate VM to be passed to view
            StudentDashboardVM dashboardData = new StudentDashboardVM()
            {
                Students = students,
                Parents = parents,
                Courses = courses,
                Requests = reqeusts,
                ParentDecisions = parentDecisions,
                CounselorDecisions = counselorDecisions,
                AdvisorDecisions = advisorDecisions,
                StudentCancels = studentCancels
            };

            ViewBag.AddType = RequestType.Add;
            ViewBag.DropType = RequestType.Drop;

            if (students.Count() > 0)
            {
                foreach (Student student in students)
                {
                    if (student.ID == userId)
                    {
                        ViewBag.StudentID = student.ID;
                        break;
                    }
                }
            }
            else
            {
                ViewBag.StudentID = null;
            }

            if (students.Count() > 0)
            {
                foreach (Student student in students)
                {
                    if (student.ID == userId)
                    {
                        if (student.ParentID != null)
                        {
                            ViewBag.ParentConnected = true;
                            break;
                        }
                        else
                        {
                            ViewBag.ParentConnected = false;
                            break;
                        }
                    }
                }
            }

            ViewBag.ParentDecisionMade = false;

            ViewBag.CounselorDecisionMade = false;

            ViewBag.AdvisorDecisionMade = false;

            return View(dashboardData);
        }

        // GET: Student/Details/id
        [Authorize(Roles = "Admin,Advisor,Counselor,Parent,Student")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await requestContext.Students.FindAsync(id);
            ViewBag.Schools = student.StudentSchools.ToList();
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Student/Create
        [Authorize(Roles = "Admin,Student")]
        public ActionResult Create()
        {
            // create list of school types for dropdown
            ViewBag.Colleges = new SelectList(requestContext.Schools.Where(r => r.Type == SchoolType.College).ToList(), "Name", "Name");
            ViewBag.HighSchools = new SelectList(requestContext.Schools.Where(r => r.Type == SchoolType.HighSchool).ToList(), "Name", "Name");

            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> Create([Bind(Include = "ID, LastName, FirstName, CID, EmailAddress, GradYear")] Student student, [Bind(Include = "StudentID, SchoolID")] StudentSchool studentCollege, [Bind(Include = "StudentID, SchoolID")] StudentSchool studentHighSchool)
        {
            // create list of school types for dropdown
            ViewBag.Colleges = new SelectList(requestContext.Schools.Where(r => r.Type == SchoolType.College).ToList(), "Name", "Name");
            ViewBag.HighSchools = new SelectList(requestContext.Schools.Where(r => r.Type == SchoolType.HighSchool).ToList(), "Name", "Name");

            // get idenitity user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            try
            {
                if (ModelState.IsValid)
                {
                    var schools = await requestContext.Schools.ToListAsync();

                    string collegeName = Request.Form["Colleges"].ToString();
                    foreach (School school in schools)
                    {
                        if (school.Name == collegeName)
                        {
                            studentCollege.StudentID = userId;
                            studentCollege.SchoolID = school.ID;
                            requestContext.StudentSchools.Add(studentCollege);
                            break;
                        }
                    }

                    string highSchoolName = Request.Form["HighSchools"].ToString();
                    foreach (School school in schools)
                    {
                        if (school.Name == highSchoolName)
                        {
                            studentHighSchool.StudentID = userId;
                            studentHighSchool.SchoolID = school.ID;
                            requestContext.StudentSchools.Add(studentHighSchool);
                            break;
                        }
                    }
                    
                    student.ID = userId;
                    requestContext.Students.Add(student);

                    await requestContext.SaveChangesAsync();
                    
                    return RedirectToAction("Index", "Student");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        // GET: Student/Connect/id
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> Connect()
        {
            // get idenitity user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            Student student = await requestContext.Students.FindAsync(userId);

            ConnectVM connectViewData = new ConnectVM()
            {
                Student = student
            };

            if (student == null)
            {
                return HttpNotFound();
            }
            return View(connectViewData);
        }

        // POST: Student/Connect/id
        [HttpPost, ActionName("Connect")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        [ValidateInput(true)]
        public async Task<ActionResult> ConnectToParent()
        {
            // get idenitity user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            Student studentToUpdate = await requestContext.Students.FindAsync(userId);

            ConnectVM connectViewData = new ConnectVM()
            {
                Student = studentToUpdate
            };

            int pin = Int32.Parse(Request.Form["ParentPIN.ID"]);
            ParentPIN parentPin = await requestContext.ParentPINs.FindAsync(pin);
            try
            {
                Parent parentToConnect = parentPin.Parent;
                studentToUpdate.ParentID = parentToConnect.ID;
            }
            catch (NullReferenceException /* nrex */)
            {
                //Log the error (uncomment nrex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to find parent by PIN. Has your parent created their account and profile yet?");
            }

            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "ParentID" }))
            {
                try
                {
                    requestContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(connectViewData);
        }

        // GET: Student/Edit/id
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            ViewBag.Colleges = new SelectList(requestContext.Schools.Where(r => r.Type == SchoolType.College).ToList(), "Name", "Name");
            ViewBag.HighSchools = new SelectList(requestContext.Schools.Where(r => r.Type == SchoolType.HighSchool).ToList(), "Name", "Name");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await requestContext.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/id
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> EditStudent(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student studentToUpdate = await requestContext.Students.FindAsync(id);

            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "LastName", "FirstName", "CID", "SSID", "EmailAddress", "GradYear" }))
            {
                try
                {
                    var studentSchoolsToUpdate = studentToUpdate.StudentSchools.ToList();
                    foreach (StudentSchool schoolToUpdate in studentSchoolsToUpdate)
                    {
                        var schools = await requestContext.Schools.ToListAsync();

                        string collegeName = Request.Form["Colleges"].ToString();
                        if (collegeName != "" && schoolToUpdate.School.Type == SchoolType.College)
                        {
                            foreach (School school in schools)
                            {
                                if (school.Name == collegeName)
                                {
                                    requestContext.StudentSchools.Remove(schoolToUpdate);
                                    var newSchoolToAdd = new StudentSchool();
                                    newSchoolToAdd.StudentID = studentToUpdate.ID;
                                    newSchoolToAdd.SchoolID = school.ID;
                                    requestContext.StudentSchools.Add(newSchoolToAdd);
                                    break;
                                }
                            }
                        }

                        string highSchoolName = Request.Form["HighSchools"].ToString();
                        if (highSchoolName != "" && schoolToUpdate.School.Type == SchoolType.HighSchool)
                        {
                            foreach (School school in schools)
                            {
                                if (school.Name == highSchoolName)
                                {
                                    requestContext.StudentSchools.Remove(schoolToUpdate);
                                    var newSchoolToAdd = new StudentSchool();
                                    newSchoolToAdd.StudentID = studentToUpdate.ID;
                                    newSchoolToAdd.SchoolID = school.ID;
                                    requestContext.StudentSchools.Add(newSchoolToAdd);
                                    break;
                                }
                            }
                        }
                    }
                    requestContext.SaveChanges();
                    return RedirectToAction("Index", "Student");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Student/UpdateID/id
        [Authorize(Roles = "Admin,Counselor")]
        public async Task<ActionResult> UpdateID(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await requestContext.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/id
        [HttpPost, ActionName("UpdateID")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Counselor")]
        public async Task<ActionResult> UpdateStudentId (Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student studentToUpdate = await requestContext.Students.FindAsync(id);

            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "SSID" }))
            {
                try
                {
                    requestContext.SaveChanges();
                    return RedirectToAction("Index", "Counselor");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Student/UpdateMaxCredits/id
        [Authorize(Roles = "Admin,Counselor")]
        public async Task<ActionResult> UpdateMaxCredits(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await requestContext.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/id
        [HttpPost, ActionName("UpdateMaxCredits")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Counselor")]
        public async Task<ActionResult> UpdateStudentMaxCredits(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student studentToUpdate = await requestContext.Students.FindAsync(id);

            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "MaxCredits" }))
            {
                try
                {
                    requestContext.SaveChanges();
                    return RedirectToAction("Index", "Counselor");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(studentToUpdate);
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
