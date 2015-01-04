using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TongJi.Web.Flow;
using TongJi.Web.Forms;
using TongJi.Web.Models;
using TongJi.Web.Maintenance;

namespace UPlanWeb.Controllers
{
    public class SetupController : Controller
    {
        public ActionResult Restore()
        {
            Setup.LoadTable<Workflow>();
            Setup.LoadTable<FlowNode>();
            Setup.LoadTable<FlowChartTextLabel>();
            Setup.LoadTable<Form>();
            Setup.LoadTable<Field>();
            Setup.LoadTable<WorkflowForm>();
            Setup.LoadTable<Layout>();
            Setup.LoadTable<FlowNodeLayout>();
            Setup.LoadTable<MenuItem>();
            return Content("操作已成功。");
        }

        public ActionResult Backup()
        {
            Setup.SaveTable<Workflow>();
            Setup.SaveTable<FlowNode>();
            Setup.SaveTable<FlowChartTextLabel>();
            Setup.SaveTable<Form>();
            Setup.SaveTable<Field>();
            Setup.SaveTable<WorkflowForm>(); 
            Setup.SaveTable<Layout>();
            Setup.SaveTable<FlowNodeLayout>();
            Setup.SaveTable<MenuItem>();
            return Content("操作已成功。");
        }

        //public ActionResult MigrateIdentityToGuid()
        //{
        //    Setup.MigrateIdentityToGuid();
        //    return Content("操作已成功。");
        //}

    }
}
