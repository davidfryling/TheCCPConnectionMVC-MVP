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
    public class ParentController : Controller
    {
        // private variables for controller
        private ApplicationDbContext identityContext = new ApplicationDbContext();

        private RequestContext requestContext = new RequestContext();

        // GET: Parent/Index
        [Authorize(Roles = "Admin,Parent")]
        public async Task<ActionResult> Index()
        {
            // get idenitity user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            // establish dB connections
            var students = await requestContext.Students.ToListAsync();
            var parents = await requestContext.Parents.ToListAsync();
            var parentPins = await requestContext.ParentPINs.ToListAsync();
            var courses = await requestContext.Courses.ToListAsync();
            var reqeusts = await requestContext.Requests.ToListAsync();
            var parentDecisions = await requestContext.ParentDecisions.ToListAsync();
            var counselorDecisions = await requestContext.CounselorDecisions.ToListAsync();
            var advisorDecisions = await requestContext.AdvisorDecisions.ToListAsync();
            var studentCancels = await requestContext.StudentCancels.ToListAsync();

            // instantiate VM to be passed to view
            ParentDashboardVM dashboardData = new ParentDashboardVM()
            {
                Parents = parents,
                Students = students,
                Courses = courses,
                Requests = reqeusts,
                ParentDecisions = parentDecisions,
                CounselorDecisions = counselorDecisions,
                AdvisorDecisions = advisorDecisions,
                StudentCancels = studentCancels
            };

            ViewBag.AddType = RequestType.Add;
            ViewBag.DropType = RequestType.Drop;

            if (parents.Count() > 0)
            {
                foreach (Parent parent in parents)
                {
                    if (parent.ID == userId)
                    {
                        ViewBag.ParentID = parent.ID;
                        break;
                    }
                }
            }
            else
            {
                ViewBag.ParentID = null;
            }
  
            if (students.Count() > 0)
            {
                ViewBag.StudentIDs = new List<Student>();

                foreach (Student student in students)
                {
                    if (student.ParentID == userId)
                    {
                        ViewBag.StudentIds.Add(student);
                    }
                }
            }
            else
            {
                ViewBag.StudentIDs = null;
            }

            if (parentPins.Count() > 0)
            {
                foreach (ParentPIN pin in parentPins)
                {
                    if (pin.ParentID == userId)
                    {
                        ViewBag.ParentPIN = pin.ID;
                        break;
                    }
                }
            }
            else
            {
                ViewBag.ParentPIN = null;
            }

            ViewBag.ParentDecisionMade = false;

            ViewBag.CounselorDecisionMade = false;

            ViewBag.AdvisorDecisionMade = false;

            return View(dashboardData);
        }

        // GET: Parent/Details/id
        [Authorize(Roles = "Admin,Advisor,Counselor,Parent,Student")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var students = await requestContext.Students.ToListAsync();

            // assign students of parent to a list for use in view
            if (students.Count() > 0)
            {
                ViewBag.StudentIDs = new List<Student>();

                foreach (Student student in students)
                {
                    if (student.ParentID == id)
                    {
                        ViewBag.StudentIds.Add(student);
                    }
                }
            }
            else
            {
                ViewBag.StudentIDs = null;
            }

            Parent parent = await requestContext.Parents.FindAsync(id);
            if (parent == null)
            {
                return HttpNotFound();
            }
            return View(parent);
        }

        // GET: Parent/Create
        [Authorize(Roles = "Admin,Parent")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Parent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<ActionResult> Create([Bind(Include = "ID, LastName, FirstName, EmailAddress")] Parent parent, [Bind(Include = "ParentID")] ParentPIN pin)
        {
            // get idenitity user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Guid userId = Guid.Parse(user.Id);

            try
            {
                if (ModelState.IsValid)
                {
                    parent.ID = userId;
                    requestContext.Parents.Add(parent);

                    pin.ParentID = userId;
                    requestContext.ParentPINs.Add(pin);

                    await requestContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Parent");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(parent);
        }

        // GET: Parent/Edit/id
        [Authorize(Roles = "Admin,Parent")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parent parent = await requestContext.Parents.FindAsync(id);
            if (parent == null)
            {
                return HttpNotFound();
            }
            return View(parent);
        }

        // POST: Parent/Edit/id
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<ActionResult> EditParent(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parent parentToUpdate = await requestContext.Parents.FindAsync(id);

            if (TryUpdateModel(parentToUpdate, "",
               new string[] { "LastName", "FirstName", "EmailAddress" }))
            {
                try
                {
                    requestContext.SaveChanges();

                    return RedirectToAction("Index", "Parent");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(parentToUpdate);
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
