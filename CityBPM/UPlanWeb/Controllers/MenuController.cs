using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TongJi.Web.Models;

namespace UPlanWeb.Controllers
{
    public class MenuController : Controller
    {
        [Authorize(Roles = "admin")]
        public ActionResult Manage(int id = 0)
        {
            var model = MenuManager.GetSubMenuItems(id);
            ViewBag.id = id;
            var record = MenuManager.GetDbRecord(id);
            ViewBag.record = record;
            if (id == 0)
            {
                ViewBag.name = "<root>";
                ViewBag.pid = 0;
                ViewBag.pname = "<root>";
            }
            else
            {
                ViewBag.name = record.Name;
                ViewBag.pid = record.ParentID;
                ViewBag.pname = record.ParentID == 0 ? "<root>" : MenuManager.GetDbRecord(record.ParentID).Name;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult NewMenuItem(string name, string description, string type, string action, string icon, string allowedRoles, int parentID = 0, int order = 0)
        {
            MenuManager.NewMenuItem(name, description, type, action, icon, parentID, order, allowedRoles);
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditMenuItem(int id, string name, string description, string type, string action, string icon, string allowedRoles, int parentID = 0, int order = 0)
        {
            MenuManager.EditMenuItem(id, name, description, type, action, icon, parentID, order, allowedRoles);
            return RedirectToAction("Manage");
        }

        [Authorize(Roles = "admin")]
        public ActionResult DeleteMenuItem(int id)
        {
            MenuManager.DeleteMenuItem(id);
            return RedirectToAction("Manage");
        }

        public ActionResult Show()
        {
            var model = MenuManager.GetTree();
            return PartialView(model);
        }

    }
}
