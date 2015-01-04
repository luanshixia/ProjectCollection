using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TongJi.Web.Security;

namespace UPlanWeb.Controllers
{
    [Authorize(Roles = "admin")]
    public class DepartmentController : Controller
    {
        public ActionResult Page(int id = 0)
        {
            var model = DepartmentManager.GetDbRecord(id);
            if (model == null)
            {
                model = new Department { Name = "<根部门>", ParentID = 0 };
            }
            ViewBag.Parent = DepartmentManager.GetParentDepartment(id);
            ViewBag.Subs = DepartmentManager.GetSubDepartments(id);
            ViewBag.Users = DepartmentManager.GetUsers(id);
            return View(model);
        }

        public ActionResult NewSub(int id = 0)
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult NewSub(int id, Department model)
        {
            if (ModelState.IsValid)
            {
                DepartmentManager.New(model.Name, model.Description, id);
                return RedirectToAction("Page", new { id });
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.id = id;
            var allDeparts = DepartmentManager.GetAllDbRecords();
            allDeparts.Insert(0, new Department { ID = 0, Name = "<根部门>" });
            ViewBag.AllDepartments = allDeparts;
            var model = DepartmentManager.GetDbRecord(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, Department model)
        {
            if (ModelState.IsValid)
            {
                DepartmentManager.UpdateDbRecord(id, department =>
                {
                    department.Name = model.Name;
                    department.Description = model.Description;
                    department.ParentID = model.ParentID;
                });
                return RedirectToAction("Page", new { id });
            }
            return View(model);
        }

        public ActionResult AddUser(int id)
        {
            ViewBag.id = id;
            ViewBag.AllUsers = UserManager.GetAllDbRecords();
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddUser(int id, Position model)
        {
            if (ModelState.IsValid)
            {
                DepartmentManager.AddUser(id, model.Username, model.Title, model.IsInCharge);
                return RedirectToAction("Page", new { id });
            }
            return View(model);
        }

        public ActionResult RemoveUser(int id, string user)
        {
            DepartmentManager.RemoveUser(id, user);
            return RedirectToAction("Page", new { id });
        }

    }
}
