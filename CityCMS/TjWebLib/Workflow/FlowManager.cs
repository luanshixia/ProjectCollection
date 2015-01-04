using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Web.DAL;

namespace TongJi.Web.Workflow
{
    public class FlowManager
    {
        public int FlowId { get; private set; }

        public FlowManager(int flowId)
        {
            FlowId = flowId;
        }

        public static int NewFlow(string name)
        {
            entity_flow flow = new entity_flow { name = name, create_time = DateTime.Now, modify_time = DateTime.Now, fk_form_id_main = -1 };
            LinqHelper.Workflow.entity_flow.InsertOnSubmit(flow);
            LinqHelper.Workflow.SubmitChanges();
            int id = LinqHelper.Workflow.entity_flow.Max(x => x.id);

            FlowManager fm = new FlowManager(id);
            fm.NewState("开始");
            fm.NewState("结束");

            return id;
        }

        public int NewState(string name)
        {
            entity_state state = new entity_state { name = name, fk_flow_id = FlowId, enabled = true };
            LinqHelper.Workflow.entity_state.InsertOnSubmit(state);
            LinqHelper.Workflow.SubmitChanges();
            return LinqHelper.Workflow.entity_state.Max(x => x.id);
        }

        public int NewUserOperation(string name, int userId, int startStateId, int endStateId)
        {
            entity_operation op = new entity_operation { name = name, fk_user_id = userId, fk_state_id_start = startStateId, fk_state_id_end = endStateId, enabled = true, fk_usergroup_id = -1 };
            LinqHelper.Workflow.entity_operation.InsertOnSubmit(op);
            LinqHelper.Workflow.SubmitChanges();
            return LinqHelper.Workflow.entity_operation.Max(x => x.id);
        }

        public int NewUsergroupOperation(string name, int usergroupId, int startStateId, int endStateId)
        {
            entity_operation op = new entity_operation { name = name, fk_usergroup_id = usergroupId, fk_state_id_start = startStateId, fk_state_id_end = endStateId, enabled = true, fk_user_id = -1 };
            LinqHelper.Workflow.entity_operation.InsertOnSubmit(op);
            LinqHelper.Workflow.SubmitChanges();
            return LinqHelper.Workflow.entity_operation.Max(x => x.id);
        }

        public void RemoveOperation(int operationId)
        {
            LinqHelper.Workflow.entity_operation.Single(x => x.id == operationId).enabled = false;
            LinqHelper.Workflow.SubmitChanges();
        }

        public void RemoveStateWithRelatedOperations(int stateId)
        {
            LinqHelper.Workflow.entity_state.Single(x => x.id == stateId).enabled = false;
            LinqHelper.Workflow.entity_operation.Where(x => x.fk_state_id_start == stateId || x.fk_state_id_end == stateId).ForEach(x => x.enabled = false);
            LinqHelper.Workflow.SubmitChanges();
        }

        public IEnumerable<int> GetAvailableOperationsOfState(int stateId)
        {
            return LinqHelper.Workflow.entity_operation.Where(x => x.fk_state_id_start == stateId && x.enabled).Select(x => x.id);
        }

        public IEnumerable<int> GetAvailableOperationsOfStateAndUser(int stateId, int userId)
        {
            return LinqHelper.Workflow.entity_operation.Where(x => x.fk_state_id_start == stateId && CanUserDoOperation(userId, x.id) && x.enabled).Select(x => x.id);
        }

        public bool CanUserDoOperation(int userId, int operationId)
        {
            var operation = LinqHelper.Workflow.entity_operation.Single(x => x.id == operationId);
            if (operation.fk_user_id > 0)
            {
                return operation.fk_user_id == userId;
            }
            else
            {
                return LinqHelper.Workflow.relation_user_usergroup.Where(x => x.fk_usergroup_id == operation.fk_usergroup_id).Select(x => x.fk_user_id).Contains(userId);
            }
        }

        public int GetNextState(int operationId)
        {
            return LinqHelper.Workflow.entity_operation.Single(x => x.id == operationId).fk_state_id_end;
        }

        public entity_flow GetDbRecord()
        {
            return LinqHelper.Workflow.entity_flow.Single(x => x.id == FlowId);
        }

        public void SetMainForm(int formId)
        {
            GetDbRecord().fk_form_id_main = formId;
            LinqHelper.Workflow.SubmitChanges();
        }
    }
}
