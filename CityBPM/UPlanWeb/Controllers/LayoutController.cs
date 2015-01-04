using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TongJi.Web.Forms;
using TongJi.Web.Flow;

namespace UPlanWeb.Controllers
{
    public class LayoutController : Controller
    {
        [Authorize(Roles = "admin")]
        public ActionResult List()
        {
            var list = LayoutManager.GetAllDbRecords();
            return View(list);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            LayoutManager.New(Guid.NewGuid(), "新布局", string.Empty, string.Empty);
            return RedirectToAction("List");
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(Guid id)
        {
            var model = LayoutManager.GetDbRecord(id);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Guid id, Layout model)
        {
            if (ModelState.IsValid)
            {
                LayoutManager.UpdateDbRecord(id, layout =>
                {
                    layout.Name = model.Name;
                    layout.Markup = model.Markup;
                    layout.Style = model.Style;
                });
                return RedirectToAction("List");
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(Guid id)
        {
            LayoutManager.DeleteDbRecord(id);
            return RedirectToAction("List");
        }

        public ActionResult ShowByFlowInst(int id, bool partial = false, bool showButton = true)
        {
            int formInstID = FormManager.GetFormInstanceOfFlowInstance(id);
            var formInst = FormInstanceManager.GetDbRecord(formInstID);
            var form = FormManager.GetDbRecord(formInst.FormID);
            var flowInst = FlowInstanceManager.GetDbRecord(formInst.FlowInstanceID);
            var layoutID = LayoutManager.GetLayoutOfFlowNode(flowInst.CurrentNodeID);
            var markup = layoutID == null ? form.Layout : LayoutManager.GetDbRecord(layoutID.Value).Markup;

            ViewBag.partial = partial;
            ViewBag.showButton = showButton;
            ViewBag.Form = form;
            ViewBag.markup = markup;
            ViewBag.formInstID = formInstID;
            var data = System.Web.Helpers.Json.Decode<Dictionary<string, string>>(formInst.Data);
            return View("Show", data);
        }

        [HttpPost]
        public ActionResult ShowByFlowInst(int id, FormCollection collection, bool partial = false, bool showButton = true)
        {
            int formInstID = FormManager.GetFormInstanceOfFlowInstance(id);
            var formInst = FormInstanceManager.GetDbRecord(formInstID);
            var form = FormManager.GetDbRecord(formInst.FormID);
            var flowInst = FlowInstanceManager.GetDbRecord(formInst.FlowInstanceID);
            var layoutID = LayoutManager.GetLayoutOfFlowNode(flowInst.CurrentNodeID);
            var markup = layoutID == null ? form.Layout : LayoutManager.GetDbRecord(layoutID.Value).Markup;

            var data = System.Web.Helpers.Json.Decode<Dictionary<string, string>>(formInst.Data);
            data = data == null ? new Dictionary<string, string>() : data;
            collection.AllKeys.ForEach(x => data[x] = collection[x]);
            FormInstanceManager.UpdateDbRecord(formInstID, record =>
            {
                record.Data = System.Web.Helpers.Json.Encode(data);
            });
            TongJi.Web.Notifications.Notification.EnqueueMessage("表单数据已保存。");

            ViewBag.partial = partial;
            ViewBag.showButton = showButton;
            ViewBag.Form = form;
            ViewBag.markup = markup;
            ViewBag.formInstID = formInstID;
            return View("Show", data);
        }

        public ActionResult ShowByID(Guid id, int flowInstID, bool partial = false, bool showButton = true)
        {
            int formInstID = FormManager.GetFormInstanceOfFlowInstance(flowInstID);
            var formInst = FormInstanceManager.GetDbRecord(formInstID);
            var form = FormManager.GetDbRecord(formInst.FormID);
            var markup = LayoutManager.GetDbRecord(id).Markup;

            ViewBag.partial = partial;
            ViewBag.showButton = showButton;
            ViewBag.Form = form;
            ViewBag.markup = markup;
            ViewBag.formInstID = formInstID;
            var data = System.Web.Helpers.Json.Decode<Dictionary<string, string>>(formInst.Data);
            return View("Show", data);
        }

        [HttpPost]
        public ActionResult ShowByID(Guid id, int flowInstID, FormCollection collection, bool partial = false, bool showButton = true)
        {
            int formInstID = FormManager.GetFormInstanceOfFlowInstance(flowInstID);
            var formInst = FormInstanceManager.GetDbRecord(formInstID);
            var form = FormManager.GetDbRecord(formInst.FormID);
            var markup = LayoutManager.GetDbRecord(id).Markup;

            var data = System.Web.Helpers.Json.Decode<Dictionary<string, string>>(formInst.Data);
            data = data == null ? new Dictionary<string, string>() : data;
            collection.AllKeys.ForEach(x => data[x] = collection[x]);
            FormInstanceManager.UpdateDbRecord(formInstID, record =>
            {
                record.Data = System.Web.Helpers.Json.Encode(data);
            });
            TongJi.Web.Notifications.Notification.EnqueueMessage("表单数据已保存。");

            ViewBag.partial = partial;
            ViewBag.showButton = showButton;
            ViewBag.Form = form;
            ViewBag.markup = markup;
            ViewBag.formInstID = formInstID;
            return View("Show", data);
        }
    }
}
