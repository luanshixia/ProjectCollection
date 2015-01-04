using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Web.DAL;

namespace TongJi.Web.Security
{
    public class UsergroupManager
    {
        public int UsergroupId { get; private set; }

        public UsergroupManager(int usergroupId)
        {
            UsergroupId = usergroupId;
        }

        public static int NewUsergroup(string name)
        {
            entity_usergroup group = new entity_usergroup { name = name };
            LinqHelper.Workflow.entity_usergroup.InsertOnSubmit(group);
            LinqHelper.Workflow.SubmitChanges();
            return LinqHelper.Workflow.entity_usergroup.Max(x => x.id);
        }

        public entity_usergroup GetDbRecord()
        {
            return LinqHelper.Workflow.entity_usergroup.Single(x => x.id == UsergroupId);
        }

        public void AddUser(int userId)
        {
            LinqHelper.Workflow.relation_user_usergroup.InsertOnSubmit(new relation_user_usergroup { fk_user_id = userId, fk_usergroup_id = UsergroupId });
            LinqHelper.Workflow.SubmitChanges();
        }

        public IEnumerable<int> GetUsers()
        {
            return LinqHelper.Workflow.relation_user_usergroup.Where(x => x.fk_usergroup_id == UsergroupId).Select(x => x.fk_user_id);
        }

        public void RemoveUser(int userId)
        {
            if (LinqHelper.Workflow.relation_user_usergroup.Where(x => x.fk_usergroup_id == UsergroupId).Any(x => x.fk_user_id == userId))
            {
                LinqHelper.Workflow.relation_user_usergroup.DeleteOnSubmit(LinqHelper.Workflow.relation_user_usergroup.First(x => x.fk_user_id == userId && x.fk_usergroup_id == UsergroupId));
                LinqHelper.Workflow.SubmitChanges();
            }
        }
    }
}
