using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TongJi.Web.Forms;

namespace UPlanWeb.Controllers
{
    [Authorize]
    public class FormController : Controller
    {
        public ActionResult Success()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult List()
        {
            var list = FormManager.GetAllDbRecords().OrderBy(x=>x.Name).ToList();
            return View(list);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(Form model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid();
                FormManager.New(model);
                return RedirectToAction("List");
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(Guid id)
        {
            var model = FormManager.GetDbRecord(id);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Guid id, Form model)
        {
            if (ModelState.IsValid)
            {
                FormManager.UpdateDbRecord(id, form =>
                {
                    form.Name = model.Name;
                    form.DisplayType = model.DisplayType;
                    form.Type = model.Type;
                    form.Layout = model.Layout;
                    form.Style = model.Style;
                });
                return RedirectToAction("List");
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(Guid id)
        {
            FormManager.DeleteDbRecord(id);
            return RedirectToAction("List");
        }

        [Authorize(Roles = "admin")]
        public ActionResult FieldList(Guid id)
        {
            var model = FormManager.GetDbRecord(id);
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult FieldCreate(Guid id)
        {
            ViewBag.FormID = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult FieldCreate(Guid id, Field model)
        {
            ModelState.Remove("id");
            if (ModelState.IsValid)
            {
                FieldManager.New(model);
                return RedirectToAction("FieldList", new { id = id });
            }
            ViewBag.FormID = id;
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult FieldEdit(int id)
        {
            var model = FieldManager.GetDbRecord(id);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult FieldEdit(int id, Field model)
        {
            if (ModelState.IsValid)
            {
                FieldManager.UpdateDbRecord(id, field =>
                {
                    field.Name = model.Name;
                    field.Description = model.Description;
                    field.DataType = model.DataType;
                    field.Options = model.Options;
                    field.Validation = model.Validation;
                    field.Order = model.Order;
                    field.Style = model.Style;
                });
                return RedirectToAction("FieldList", new { id = model.FormID });
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult FieldDelete(int id)
        {
            var field = FieldManager.GetDbRecord(id);
            FieldManager.DeleteDbRecord(id);
            return RedirectToAction("FieldList", new { id = field.FormID });
        }

        public ActionResult Fill(Guid id)
        {
            var form = FormManager.GetDbRecord(id);
            ViewBag.Form = form;
            return View();
        }

        [HttpPost]
        public ActionResult Fill(Guid id, FormCollection collection)
        {
            var data = collection.AllKeys.ToDictionary(x => x, x => collection[x]);
            FormInstanceManager.New(id, System.Web.Helpers.Json.Encode(data), 0);
            TongJi.Web.Notifications.Notification.EnqueueMessage("表单数据已保存。");
            var form = FormManager.GetDbRecord(id);
            ViewBag.Form = form;
            return View(data);
        }

        public ActionResult Preview(int id)
        {
            var inst = FormInstanceManager.GetDbRecord(id);
            var form = FormManager.GetDbRecord(inst.FormID);
            var data = System.Web.Helpers.Json.Decode<Dictionary<string, string>>(inst.Data);

            ViewBag.Form = form;
            ViewBag.Inst = inst;
            return View(data);
        }

        public ActionResult Modify(int id, bool partial = false, bool showButton = true)
        {
            var inst = FormInstanceManager.GetDbRecord(id);
            var form = FormManager.GetDbRecord(inst.FormID);
            var data = System.Web.Helpers.Json.Decode<Dictionary<string, string>>(inst.Data);

            ViewBag.partial = partial;
            ViewBag.showButton = showButton;
            ViewBag.Form = form;
            ViewBag.Inst = inst;
            return View(data);
        }

        [HttpPost]
        public ActionResult Modify(int id, FormCollection collection, bool partial = false, bool showButton = true)
        {
            var data = collection.AllKeys.ToDictionary(x => x, x => collection[x]);
            var inst = FormInstanceManager.GetDbRecord(id);
            var form = FormManager.GetDbRecord(inst.FormID);

            FormInstanceManager.UpdateDbRecord(id, record =>
            {
                record.Data = System.Web.Helpers.Json.Encode(data);
            });
            TongJi.Web.Notifications.Notification.EnqueueMessage("表单数据已保存。");

            ViewBag.partial = partial;
            ViewBag.showButton = showButton;
            ViewBag.Form = form;
            ViewBag.Inst = inst;
            return View(data);
        }

        public ActionResult Data(int id)
        {
            var inst = FormInstanceManager.GetDbRecord(id);
            return Content(inst.Data, "application/json");
        }

        [Authorize(Roles = "admin")]
        public ActionResult InstList(Guid? id)
        {
            var insts = id == null ? FormInstanceManager.GetAllDbRecords() : FormInstanceManager.Query(x => x.FormID == id);
            return View(insts);
        }

        [Authorize(Roles = "admin")]
        public ActionResult InstDetail(int id)
        {
            var inst = FormInstanceManager.GetDbRecord(id);
            var model = System.Web.Helpers.Json.Decode<Dictionary<string, string>>(inst.Data);
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult InstDelete(int id)
        {
            FormInstanceManager.DeleteDbRecord(id);
            return null;
        }

    }
}
