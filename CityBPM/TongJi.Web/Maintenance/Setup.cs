using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using TongJi.IO;
using TongJi.Web.Flow;
using TongJi.Web.Forms;
using TongJi.Web.Models;

namespace TongJi.Web.Maintenance
{
    public static class Setup
    {
        public static void SaveTable<T>() where T : class
        {
            var list = BasicWebManager.GetAll<T>();
            var xml = list.XmlEncode();
            var path = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/init/{0}.xml", typeof(T).Name));
            System.IO.File.WriteAllText(path, xml);
        }

        public static void LoadTable<T>() where T : class
        {
            var path = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/init/{0}.xml", typeof(T).Name));
            var xml = System.IO.File.ReadAllText(path);
            var list = xml.XmlDecode<List<T>>();
            BasicWebManager.NewAll(list);
        }

        //public static void MigrateIdentityToGuid()
        //{
        //    var ids = BasicWebManager.GetAll<FlowNode>().Select(x => x.ID).ToArray();
        //    var dict = ids.ToDictionary(x => x, x => Guid.NewGuid());
        //    BasicWebManager.UpdateAll<FlowNode>(x => true, record =>
        //    {
        //        record.ID1 = dict[record.ID];
        //    });
        //    BasicWebManager.UpdateAll<FlowInstance>(x => true, record =>
        //    {
        //        record.CurrentNodeID1 = dict[record.CurrentNodeID];
        //    });
        //    BasicWebManager.UpdateAll<FlowInstanceAction>(x => true, record =>
        //    {
        //        record.FromNodeID1 = dict[record.FromNodeID];
        //        record.ToNodeID1 = dict[record.ToNodeID];
        //    });
        //    BasicWebManager.UpdateAll<FlowNodeLayout>(x => true, record =>
        //    {
        //        record.FlowNodeID1 = dict[record.FlowNodeID];
        //    });
        //    BasicWebManager.UpdateAll<FlowNodeFlowNode>(x => true, record =>
        //    {
        //        record.FlowNode_ID_1 = dict[record.FlowNode_ID];
        //        record.FlowNode_ID1_1 = dict[record.FlowNode_ID1];
        //    });
        //}
    }
}
