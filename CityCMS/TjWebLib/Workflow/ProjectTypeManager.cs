//using System;
//using System.Collections.Generic;
//using System.Data.Linq;
//using System.Linq;
//using System.Text;

//using TongJi.Web.DAL;

//namespace TongJi.Web.Workflow
//{
//    public class ProjectTypeManager
//    {
//        public int ProjectTypeId { get; private set; }

//        public ProjectTypeManager(int projectTypeId)
//        {
//            ProjectTypeId = projectTypeId;
//        }

//        public static int NewProjectType(string name)
//        {
//            entity_project_type type = new entity_project_type { name = name, fk_flow_id_main = -1 };
//            LinqHelper.Workflow.entity_project_type.InsertOnSubmit(type);
//            LinqHelper.Workflow.SubmitChanges();
//            return LinqHelper.Workflow.entity_project_type.Max(x => x.id);
//        }

//        public entity_project_type GetDbRecord()
//        {
//            return GetDbTable().Single(x => x.id == ProjectTypeId);
//        }

//        public Table<entity_project_type> GetDbTable()
//        {
//            return LinqHelper.Workflow.entity_project_type;
//        }
//    }
//}
