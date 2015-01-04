using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

using TongJi.Web.Forms;
using TongJi.Web.Flow;

namespace UPlanWeb.Controllers
{
    public class ProjectFlowController : Controller
    {
        //
        // GET: /ProjectFlow/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StepJump(int id)
        {
            var inst = FlowInstanceManager.GetDbRecord(id);
            var node = FlowNodeManager.GetDbRecord(inst.CurrentNodeID);

            switch (node.Name)
            {
                case "窗口接件":
                    return RedirectToAction("Step1", new { id = id });
                case "审核申请材料":
                    return RedirectToAction("Step2", new { id = id });
                case "补正材料":
                    return RedirectToAction("Step3", new { id = id });
                case "出具不予受理决定书":
                    return RedirectToAction("Step4", new { id = id });
                case "审核是否符合地块控制":
                    return RedirectToAction("Step5", new { id = id });
                case "准备申请单元控制":
                    return RedirectToAction("Step6", new { id = id });
                case "输出规划条件":
                    return RedirectToAction("Step7", new { id = id });
                case "科长审批":
                    return RedirectToAction("Step8", new { id = id });
                case "会审":
                    return RedirectToAction("Step9", new { id = id });
                case "局长审批":
                    return RedirectToAction("Step10", new { id = id });
                case "整理规划条件":
                    return RedirectToAction("Step11", new { id = id });
                case "窗口出具准予行政许可决定书与规划条件":
                    return RedirectToAction("Step12", new { id = id });
                case "出具不予行政许可决定书":
                    return RedirectToAction("Step13", new { id = id });
                case "出具受理通知书":
                    return RedirectToAction("Step14", new { id = id });
                default:
                    return RedirectToAction("FlowEnd", new { id = id });
            }
        }

        public ActionResult Step1(int id)
        {
            
            return View();
        }

        public ActionResult Step2(int id)
        {
            return View();
        }

        public ActionResult Step3(int id)
        {
            return View();
        }

        public ActionResult Step4(int id)
        {
            return View();
        }

        public ActionResult Step5(int id)
        {
            return View();
        }

        public ActionResult Step6(int id)
        {
            return View();
        }

        public ActionResult Step7(int id)
        {
            return View();
        }

        public ActionResult Step8(int id)
        {
            return View();
        }

        public ActionResult Step9(int id)
        {
            return View();
        }

        public ActionResult Step10(int id)
        {
            return View();
        }

        public ActionResult Step11(int id)
        {
            return View();
        }

        public ActionResult Step12(int id)
        {
            return View();
        }

        public ActionResult Step13(int id)
        {
            return View();
        }

        public ActionResult Step14(int id)
        {
            return View();
        }

        public ActionResult FlowEnd(int id)
        {
            return View();
        }
    }
}
