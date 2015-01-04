using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebMatrix.WebData;

using TongJi.Web.Forms;
using TongJi.Web.Flow;

namespace UPlanWeb.Controllers
{
    [Authorize]
    public class WorkflowController : Controller
    {
        [Authorize(Roles = "admin")]
        public ActionResult AddRole(Guid id, string role)
        {
            FlowNodeManager.AddRole(id, role);
            return RedirectToAction("SetNodeRoles", new { id = id });
        }

        [Authorize(Roles = "admin")]
        public ActionResult AddUser(Guid id, string user)
        {
            FlowNodeManager.AddUser(id, user);
            return RedirectToAction("SetNodeRoles", new { id = id });
        }

        [Authorize(Roles = "admin")]
        public ActionResult DeleteRole(Guid id, string role)
        {
            FlowNodeManager.DeleteRole(id, role);
            return RedirectToAction("SetNodeRoles", new { id = id });
        }

        [Authorize(Roles = "admin")]
        public ActionResult DeleteUser(Guid id, string user)
        {
            FlowNodeManager.DeleteUser(id, user);
            return RedirectToAction("SetNodeRoles", new { id = id });
        }

        public ActionResult FlowChart(int id)
        {
            var inst = FlowInstanceManager.GetDbRecord(id);
            var node = FlowNodeManager.GetDbRecord(inst.CurrentNodeID);
            ViewBag.id = id;
            ViewBag.nodeName = node.Name;
            ViewBag.instName = inst.Name;
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult FlowEditor()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult FlowList()
        {
            var list = WorkflowManager.GetFlowList().OrderBy(x => x.CreateTime).ToList();
            return View(list);
        }

        public ActionResult FlowStep(int id)
        {
            var inst = FlowInstanceManager.GetDbRecord(id);
            var flow = WorkflowManager.GetDbRecord(inst.WorkflowID);
            var node = FlowNodeManager.GetDbRecord(inst.CurrentNodeID);
            bool allowed = FlowNodeManager.IsUserAllowed(inst.CurrentNodeID, WebSecurity.CurrentUserName);
            bool completed = inst.Completed;
            int formInstID = FormManager.GetFormInstanceOfFlowInstance(id);
            var layoutID = LayoutManager.GetLayoutOfFlowNode(inst.CurrentNodeID);

            ViewBag.inst = inst;
            ViewBag.flow = flow;
            ViewBag.node = node;
            ViewBag.allowed = allowed;
            ViewBag.completed = completed;
            ViewBag.formInstID = formInstID;
            ViewBag.hasLayout = layoutID != null;
            return View();
        }

        public ActionResult FlowStepJump(int id)
        {
            var inst = FlowInstanceManager.GetDbRecord(id);
            bool allowed = FlowNodeManager.IsUserAllowed(inst.CurrentNodeID, WebSecurity.CurrentUserName);
            if (!allowed)
            {
                return RedirectToAction("TodoList", "Workflow");
            }
            //if (inst.WorkflowID == Guid.Parse(UPlanWeb.Models.UPlanDb.WorkflowID_Project))
            //{
            //    return RedirectToAction("StepJump", "ProjectFlow", new { id = id });
            //}
            //else if (inst.WorkflowID == Guid.Parse(UPlanWeb.Models.UPlanDb.WorkflowID_UnitMutate))
            //{
            //    return RedirectToAction("Page", "UnitMutateFlow", new { id = id });
            //}
            return RedirectToAction("StepJump", string.Format("Flow{0}", inst.WorkflowID.ToString().Replace("-", "_")), new { id = id });
            //return RedirectToAction("FlowStep", new { id = id });
        }

        [Authorize(Roles = "admin")]
        public ActionResult FlowDelete(Guid id)
        {
            //WorkflowManager.DeleteDbRecord(id);
            return RedirectToAction("FlowList");
        }

        public ActionResult FlowGoOn(int id, string to, string returnUrl, bool isFinish = false)
        {
            FlowInstanceManager.GoOn(id, WebSecurity.CurrentUserName, to);
            if (isFinish)
            {
                FlowInstanceManager.GoOn(id, WebSecurity.CurrentUserName, "");
            }

            TongJi.Web.Notifications.Notification.EnqueueMessage("操作已成功。");
            if (string.IsNullOrEmpty(returnUrl))
            {
                return null;
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        public ActionResult FlowGoBack(int id, string returnUrl)
        {
            FlowInstanceManager.GoBack(id, WebSecurity.CurrentUserName);
            TongJi.Web.Notifications.Notification.EnqueueMessage("流程已经回退到上一步。您没有权限进一步回退，详情请咨询管理员。");
            if (string.IsNullOrEmpty(returnUrl))
            {
                return null;
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult FlowModify(Guid id)
        {
            ViewBag.id = id;
            ViewBag.flowName = WorkflowManager.GetDbRecord(id).Name;
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult GroupModify(Guid id)
        {
            var workflow = WorkflowManager.GetDbRecord(id);
            var groupList = new List<string> { "", "建筑类", "市政类", "村镇项目", "自定义", "其他" };
            ViewBag.groupList = groupList;
            return PartialView(workflow);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult GroupModify(Guid id, string group)
        {
            WorkflowManager.EditGroup(id, group);
            return RedirectToAction("FlowList");
            //return RedirectToAction("NodeListOfFlow", new { id = id });
        }

        public ActionResult FlowTracking(int id)
        {
            var inst = FlowInstanceManager.GetDbRecord(id);
            var list = FlowInstanceManager.TrackBack(id);
            ViewBag.inst = inst;
            return View(list);
        }

        public ActionResult GetFlowJson(Guid id)
        {
            var data = WorkflowManager.GetJsonObject(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInstJson(int id)
        {
            var data = FlowInstanceManager.GetJsonObject(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewFlowFromJson(string name, string data)
        {
            // 创建流程数据库
            var flow = System.Web.Helpers.Json.Decode<WorkflowJsonObject>(data);
            WorkflowManager.New(name, flow);
            return null;
        }

        public ActionResult NewFlowInst()
        {
            //ViewBag.FlowList = WorkflowManager.GetFlowList();
            ViewBag.FlowList = WorkflowManager.GetFlowList(WebSecurity.CurrentUserName);
            return View();
        }

        [HttpPost]
        public ActionResult NewFlowInst(Guid id, string name, string number, string comment)
        {
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("name", "实例名称不可为空。");
            }
            if (id == Guid.Empty)
            {
                ModelState.AddModelError("id", "请选择项目流程。");
            }
            if (ModelState.IsValid)
            {
                var flowID = id;
                var instID = WorkflowManager.NewInstance(flowID, name, number, comment);
                var formID = FormManager.GetMainFormOfWorkflow(flowID);
                if (formID != null)
                {
                    FormManager.NewInstance(formID.Value, string.Empty, instID);
                }
                FlowInstanceManager.GoOn(instID, WebSecurity.CurrentUserName);
                TongJi.Web.Notifications.Notification.EnqueueMessage("新建流程已成功。");
                return RedirectToAction("FlowStepJump", new { id = instID });
            }
            ViewBag.FlowList = WorkflowManager.GetFlowList();
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult NodeListOfFlow(Guid id)
        {
            var flow = WorkflowManager.GetDbRecord(id);
            var list = WorkflowManager.GetNodeList(id);
            ViewBag.flow = flow;
            return View(list);
        }

        public ActionResult Forbidden()
        {
            return View();
        }

        public ActionResult PersonAction(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                user = WebSecurity.CurrentUserName;
            }
            var list = FlowInstanceManager.UserAction(user);
            return View(list);
        }

        [Authorize(Roles = "admin")]
        public ActionResult SaveFlow(string data)
        {
            // 修改流程数据库
            var flow = System.Web.Helpers.Json.Decode<WorkflowJsonObject>(data);
            WorkflowManager.UpdateFromJson(flow);
            return null;
        }

        [Authorize(Roles = "admin")]
        public ActionResult FlowInstanceList(Guid? id)
        {
            var instances = id == null ? FlowInstanceManager.GetAllRecords() : FlowInstanceManager.Query(x => x.WorkflowID == id);
            instances = instances.OrderByDescending(x => x.StartTime).ToList();
            return View(instances);
        }

        [Authorize(Roles = "admin")]
        public ActionResult SetNodeRoles(Guid id)
        {
            var node = FlowNodeManager.GetDbRecord(id);
            var roles = System.Web.Security.Roles.GetAllRoles();
            var users = roles.SelectMany(r => System.Web.Security.Roles.GetUsersInRole(r)).Distinct().ToList();
            ViewBag.node = node;
            ViewBag.roles = roles;
            ViewBag.users = users;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult SetNodeRoles(Guid id, string submit, string role, string user)
        {
            if (submit == "添加用户")
            {
                FlowNodeManager.AddUser(id, user);
            }
            else if (submit == "删除用户")
            {
                FlowNodeManager.DeleteUser(id, user);
            }
            else if (submit == "添加角色")
            {
                FlowNodeManager.AddRole(id, role);
            }
            else if (submit == "删除角色")
            {
                FlowNodeManager.DeleteRole(id, role);
            }
            return RedirectToAction("SetNodeRoles");
        }

        public ActionResult TodoList()
        {
            var list = WorkflowManager.GetTodoList(WebSecurity.CurrentUserName);
            return View(list);
        }

        public ActionResult ActiveDoneList()
        {
            var list = WorkflowManager.GetActiveDoneList(WebSecurity.CurrentUserName);
            return View(list);
        }

        public ActionResult CompletedDoneList()
        {
            var list = WorkflowManager.GetCompletedDoneList(WebSecurity.CurrentUserName).OrderByDescending(x => x.EndTime).ToList();
            Dictionary<string, List<TodoListItem>> groups = new Dictionary<string, List<TodoListItem>>();
            for (int i = 0; i < 3; i++)
            {
                var date = DateTime.Now.AddMonths(-i);
                string key = string.Format("{0} 年 {1} 月", date.Year, date.Month);
                groups[key] = list.Where(x => TongJi.Maths.MonthDifference(DateTime.Now, x.EndTime) == i).ToList();
            }
            groups["更早之前"] = list.Where(x => TongJi.Maths.MonthDifference(DateTime.Now, x.EndTime) >= 3).ToList();
            return View(groups);
        }

        [Authorize(Roles = "admin")]
        public ActionResult SetForm(Guid id)
        {
            ViewBag.forms = FormManager.GetAllDbRecords();
            ViewBag.form = FormManager.GetMainFormOfWorkflow(id).ToString();
            return PartialView();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult SetForm(Guid id, Guid formID)
        {
            FormManager.SetMainFormOfWorkflow(id, formID);
            return RedirectToAction("FlowList");
            //FormManager.AddFormOfWorkflow(id, formID);
            //return RedirectToAction("NodeListOfFlow", new { id = id });
        }

        [Authorize(Roles = "admin")]
        public ActionResult FormDelete(int id)
        {
            var workflowid = FormManager.GetWorkFlowId(id);
            FormManager.DeleteRecord(id);
            return RedirectToAction("SetForm", new { id = workflowid });
        }

        [Authorize(Roles = "admin")]
        public ActionResult SetNodeLayout(Guid id)
        {
            ViewBag.nodeID = id;
            ViewBag.flowID = FlowNodeManager.GetDbRecord(id).WorkflowID;
            ViewBag.layoutID = LayoutManager.GetLayoutOfFlowNode(id).ToString();
            ViewBag.layouts = LayoutManager.GetAllDbRecords();
            return PartialView();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult SetNodeLayout(Guid id, Guid layoutID)
        {
            LayoutManager.SetLayoutOfFlowNode(id, layoutID);
            Guid flowID = FlowNodeManager.GetDbRecord(id).WorkflowID;
            return RedirectToAction("NodeListOfFlow", new { id = flowID });
        }

        [Authorize(Roles = "admin")]
        public ActionResult SetCurrentNode(int id)
        {
            var instance = FlowInstanceManager.GetDbRecord(id);
            var flowId = instance.WorkflowID;
            var nodes = WorkflowManager.GetNodeList(flowId);
            var dict = nodes.ToDictionary(x => x.ID, x => x.NodeName);
            ViewBag.nodes = dict;
            return View(instance);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult SetCurrentNode(int id, Guid nodeId)
        {
            FlowInstanceManager.SetCurrentNode(id, nodeId);
            return RedirectToAction("FlowInstanceList");
        }

        [Authorize(Roles = "admin")]
        public ActionResult EditFormContent(int id)
        {
            var instance = FlowInstanceManager.GetDbRecord(id);
            ViewBag.formInstID = FormManager.GetFormInstanceOfFlowInstance(id);
            return View(instance);
        }
    }
}
