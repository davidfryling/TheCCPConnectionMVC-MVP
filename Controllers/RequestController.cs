using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheCCPConnection.DAL;
using TheCCPConnection.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using TheCCPConnection.ViewModels;

namespace TheCCPConnection.Controllers
{
    public class RequestController : Controller
    {
        // private variables for controller
        private ApplicationDbContext identityContext = new ApplicationDbContext();

        private RequestContext requestContext = new RequestContext();

        // GET: Request
        //public async Task<ActionResult> Index()
        //{
        //    var requests = db.Requests.Include(r => r.Course).Include(r => r.Student);
        //    return View(await requests.ToListAsync());
        //}

        // GET: Request/Details/id
        [Authorize(Roles = "Admin,Student,Parent,Counselor,Advisor")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = await requestContext.Requests.FindAsync(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: Request/Add
        [Authorize(Roles = "Admin,Student")]
        public ActionResult Add()
        {
            // create list of active terms for dropdown - currently hard coded, but eventually set it up so admin create add and drop windows that will be used to populate this list based on datetime
            ViewBag.ActiveTerms = new SelectList(requestContext.Terms.Where(t => t.Name.Equals("Spring 2021")).ToList(), "Name", "Name");

            return View();
        }

        // POST: Request/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> Add([Bind(Include = "Subject, Number, Section, Synonym, Credits, Title, Days, Times, InstructorName, SchoolID, TermID")] Course course, [Bind(Include = "Timestamp, StudentID, CourseID, Type, Active")] Request request)
        {
            // get idenitity user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            try
            {
                if (ModelState.IsValid)
                {
                    Student student = await requestContext.Students.FindAsync(userId);

                    var studentSchools  = student.StudentSchools.ToList();
                    foreach (StudentSchool studentSchool in studentSchools)
                    {
                        if (studentSchool.School.Type == SchoolType.College)
                        {
                            course.SchoolID = studentSchool.School.ID;
                            break;
                        }
                    }

                    var terms = await requestContext.Terms.ToListAsync();
                    string termSelected = Request.Form["ActiveTerms"].ToString();
                    foreach (Term term in terms)
                    {
                        if (term.Name == termSelected)
                        {
                            course.TermID = term.ID;
                            break;
                        }
                    }
                    requestContext.Courses.Add(course);

                    request.Timestamp = DateTime.Now;
                    request.StudentID = student.ID;
                    request.CourseID = course.ID;
                    request.Type = RequestType.Add;
                    request.Active = true;
                    requestContext.Requests.Add(request);
                    await requestContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Student");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(course);
        }

        // GET: Request/Drop/requestId
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> Drop(int? requestId)
        {
            if (requestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = await requestContext.Requests.FindAsync(requestId);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Request/Drop
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> Drop([Bind(Include = "Timestamp, StudentID, CourseID, Type, Active")] Request request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    request.Timestamp = DateTime.Now;
                    request.Type = RequestType.Drop;
                    request.Active = true;
                    requestContext.Requests.Add(request);
                    await requestContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Student");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View();
        }

        // GET: Request/Cancel/requestId
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> StudentCancel(int? requestId)
        {
            if (requestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = await requestContext.Requests.FindAsync(requestId);
            Student student = await requestContext.Students.FindAsync(request.StudentID);
            StudentCancel studentCancel= new StudentCancel()
            {
                Request = request,
                Student = student
            };
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(studentCancel);
        }

        // POST: Request/Cancel/requestId
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult> StudentCancel([Bind(Include = "Timestamp, Reason, StudentID, RequestID")] StudentCancel studentCancel)
        {
            Request requestToUpdate = await requestContext.Requests.FindAsync(studentCancel.RequestID);
            if (TryUpdateModel(requestToUpdate, "",
               new string[] { "Active" }))
            {
                try
                {
                    requestToUpdate.Active = false;
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

                try
                {
                    if (ModelState.IsValid)
                    {
                        ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                        Guid userId = Guid.Parse(user.Id);

                        Student student = await requestContext.Students.FindAsync(userId);

                        studentCancel.Timestamp = DateTime.Now;
                        studentCancel.StudentID = student.ID;
                        requestContext.StudentCancels.Add(studentCancel);
                        await requestContext.SaveChangesAsync();

                        return RedirectToAction("Index", "Student");
                    }
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
                return View();
            }
            return View(studentCancel);
        }

        // GET: Request/ParentApprove/requestId+parentId
        [Authorize(Roles = "Admin,Parent")]
        public async Task<ActionResult> ParentApprove(int? requestId, Guid? parentId)
        {
            if (requestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = await requestContext.Requests.FindAsync(requestId);
            Parent parent = await requestContext.Parents.FindAsync(parentId);
            ParentDecision parentApproval = new ParentDecision()
            {
                Request = request,
                Parent = parent
            };
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(parentApproval);
        }

        // POST: Request/ParentApprove
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<ActionResult> ParentApprove([Bind(Include = "Timestamp, Approved, Reason, ParentID, RequestID")] ParentDecision parentApproval)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    parentApproval.Timestamp = DateTime.Now;
                    parentApproval.Approved = true;
                    requestContext.ParentDecisions.Add(parentApproval);
                    await requestContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Parent");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(parentApproval);
        }

        // GET: Request/ParentDeny/requestId+parentId
        [Authorize(Roles = "Admin,Parent")]
        public async Task<ActionResult> ParentDeny(int? requestId, Guid? parentId)
        {
            if (requestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request reqeust = await requestContext.Requests.FindAsync(requestId);
            Parent parent = await requestContext.Parents.FindAsync(parentId);
            ParentDecision parentDenial = new ParentDecision()
            {
                Request = reqeust,
                Parent = parent
            };
            if (reqeust == null)
            {
                return HttpNotFound();
            }
            return View(parentDenial);
        }

        // POST: Request/ParentDeny
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<ActionResult> ParentDeny([Bind(Include = "Timestamp, Approved, Reason, ParentID, RequestID")] ParentDecision parentApproval)
        {
            Request requestToUpdate = await requestContext.Requests.FindAsync(parentApproval.RequestID);
            if (TryUpdateModel(requestToUpdate, "",
               new string[] { "Active" }))
            {
                try
                {
                    requestToUpdate.Active = false;
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

                try
                {
                    if (ModelState.IsValid)
                    {
                        parentApproval.Timestamp = DateTime.Now;
                        parentApproval.Approved = false;
                        requestContext.ParentDecisions.Add(parentApproval);
                        await requestContext.SaveChangesAsync();

                        return RedirectToAction("Index", "Parent");
                    }
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
                return View(parentApproval);
            }
            return View(parentApproval);
        }

        // GET: Request/CounselorApprove/requestId+counselorId
        [Authorize(Roles = "Admin,Counselor")]
        public async Task<ActionResult> CounselorApprove(int? requestId, Guid? counselorId)
        {
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            if (requestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = await requestContext.Requests.FindAsync(requestId);
            Counselor counselor = await requestContext.Counselors.FindAsync(counselorId);
            CounselorDecision counselorApproval = new CounselorDecision()
            {
                Request = request,
                Counselor = counselor
            };
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(counselorApproval);
        }

        // POST: Request/CounselorApprove
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Counselor")]
        public async Task<ActionResult> CounselorApprove([Bind(Include = "Timestamp, Approved, Reason, CounselorID, RequestID")] CounselorDecision counselorApproval)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    counselorApproval.Timestamp = DateTime.Now;
                    counselorApproval.Approved = true;
                    requestContext.CounselorDecisions.Add(counselorApproval);
                    await requestContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Counselor");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(counselorApproval);
        }

        // GET: Request/CounselorDeny/requestId+counselorId
        [Authorize(Roles = "Admin,Counselor")]
        public async Task<ActionResult> CounselorDeny(int? requestId, Guid? counselorId)
        {
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            if (requestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request reqeust = await requestContext.Requests.FindAsync(requestId);
            Counselor counselor = await requestContext.Counselors.FindAsync(counselorId);
            CounselorDecision counselorDenial = new CounselorDecision()
            {
                Request = reqeust,
                Counselor = counselor
            };
            if (reqeust == null)
            {
                return HttpNotFound();
            }
            return View(counselorDenial);
        }

        // POST: Request/CounselorDeny
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Counselor")]
        public async Task<ActionResult> CounselorDeny([Bind(Include = "Timestamp, Approved, Reason, CounselorID, RequestID")] CounselorDecision counselorApproval)
        {
            Request requestToUpdate = await requestContext.Requests.FindAsync(counselorApproval.RequestID);
            if (TryUpdateModel(requestToUpdate, "",
               new string[] { "Active" }))
            {
                try
                {
                    requestToUpdate.Active = false;
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

                try
                {
                    if (ModelState.IsValid)
                    {
                        counselorApproval.Timestamp = DateTime.Now;
                        counselorApproval.Approved = false;
                        requestContext.CounselorDecisions.Add(counselorApproval);
                        await requestContext.SaveChangesAsync();

                        return RedirectToAction("Index", "Counselor");
                    }
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
                return View(counselorApproval);
            }
            return View(counselorApproval);
        }

        // GET: Request/AdvisorApprove/requestId+advisorId
        [Authorize(Roles = "Admin,Advisor")]
        public async Task<ActionResult> AdvisorApprove(int? requestId, Guid? advisorId)
        {
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            if (requestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = await requestContext.Requests.FindAsync(requestId);
            Advisor advisor = await requestContext.Advisors.FindAsync(advisorId);
            AdvisorDecision advisorApproval = new AdvisorDecision()
            {
                Request = request,
                Advisor = advisor
            };
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(advisorApproval);
        }

        // POST: Request/AdvisorApprove
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Advisor")]
        public async Task<ActionResult> AdvisorApprove([Bind(Include = "Timestamp, Registered, Reason, AdvisorID, RequestID")] AdvisorDecision advisorApproval)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    advisorApproval.Timestamp = DateTime.Now;
                    advisorApproval.Registered = true;
                    requestContext.AdvisorDecisions.Add(advisorApproval);
                    await requestContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Advisor");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(advisorApproval);
        }

        // GET: Request/AdvisorDeny/requestId+advisorId
        [Authorize(Roles = "Admin,Advisor")]
        public async Task<ActionResult> AdvisorDeny(int? requestId, Guid? advisorId)
        {
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            if (requestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request reqeust = await requestContext.Requests.FindAsync(requestId);
            Advisor advisor = await requestContext.Advisors.FindAsync(advisorId);
            AdvisorDecision advisorDenial = new AdvisorDecision()
            {
                Request = reqeust,
                Advisor = advisor
            };
            if (reqeust == null)
            {
                return HttpNotFound();
            }
            return View(advisorDenial);
        }

        // POST: Request/AdvisorDeny
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Advisor")]
        public async Task<ActionResult> AdvisorDeny([Bind(Include = "Timestamp, Registered, Reason, AdvisorID, RequestID")] AdvisorDecision advisorApproval)
        {
            Request requestToUpdate = await requestContext.Requests.FindAsync(advisorApproval.RequestID);
            if (TryUpdateModel(requestToUpdate, "",
               new string[] { "Active" }))
            {
                try
                {
                    requestToUpdate.Active = false;
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

                try
                {
                    if (ModelState.IsValid)
                    {
                        advisorApproval.Timestamp = DateTime.Now;
                        advisorApproval.Registered = false;
                        requestContext.AdvisorDecisions.Add(advisorApproval);
                        await requestContext.SaveChangesAsync();

                        return RedirectToAction("Index", "Counselor");
                    }
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
                return View(advisorApproval);
            }
            return View(advisorApproval);
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
