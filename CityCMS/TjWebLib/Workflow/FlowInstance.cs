using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Web.DAL;

namespace TongJi.Web.Workflow
{
    public class FlowInstance
    {
        public int FlowId { get; private set; }
        public int InstanceId { get; private set; }
        public int ProjectId { get; private set; }

        public FlowInstance(int instanceId)
        {
            InstanceId = instanceId;
            FlowId = LinqHelper.Workflow.entity_flow_inst.Single(x => x.id == instanceId).fk_flow_id;
            ProjectId = LinqHelper.Workflow.entity_flow_inst.Single(x => x.id == instanceId).fk_project_id;
        }

        public static int NewFlowInstance(int flowId, int projectId)
        {
            int startStateId = LinqHelper.Workflow.entity_state.First(x => x.fk_flow_id == flowId && x.name == "开始").id;
            entity_flow_inst inst = new entity_flow_inst { fk_flow_id = flowId, fk_project_id = projectId, fk_state_id = startStateId };
            LinqHelper.Workflow.entity_flow_inst.InsertOnSubmit(inst);
            LinqHelper.Workflow.SubmitChanges();
            return LinqHelper.Workflow.entity_flow_inst.Max(x => x.id);
        }
    }
}
