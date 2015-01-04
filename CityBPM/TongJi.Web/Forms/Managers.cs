using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using TongJi.Web.Models;

namespace TongJi.Web.Forms
{
    public class FormManager
    {
        public const int DisplayType_Standard = 0;
        public const int DisplayType_Mixed = 1;
        public const int DisplayType_Custom = 2;

        public static string[] DataTypes = { "String", "Text", "Number", "Enum", "Boolean", "Date", "Time", "DateTime", "File", "Image", "User", "Department" };
        public static string[] DisplayTypes = { "标准布局", "用标准布局填写，用自定义布局查看", "自定义布局" };
        public static string[] FormTypes = { "申请表", "审核内容", "证书", "其他" };

        public static List<SelectListItem> SelectList(IEnumerable<string> items, string value, bool useNumberValue = false)
        {
            if (useNumberValue)
            {
                return items.Select((x, i) => new SelectListItem { Text = x, Value = i.ToString(), Selected = x == value }).ToList();
            }
            else
            {
                return items.Select(x => new SelectListItem { Text = x, Value = x, Selected = x == value }).ToList();
            }
        }

        public static List<SelectListItem> SelectList(Dictionary<string, string> items, string value)
        {
            return items.Select(x => new SelectListItem { Text = x.Value, Value = x.Key, Selected = x.Key == value }).ToList();
        }

        public static List<SelectListItem> SelectList(Dictionary<int, string> items, int value)
        {
            return items.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString(), Selected = x.Key == value }).ToList();
        }

        public static string StringSummary(string content, int length = 10)
        {
            return content.Length <= length ? content : content.Remove(length - 2) + "...";
        }

        public static Form GetDbRecord(Guid id)
        {
            using (var context = new BasicWebContext())
            {
                return context.Forms.Include("Fields").AsNoTracking().Single(x => x.ID == id);
            }
        }

        public static List<Form> GetAllDbRecords()
        {
            using (var context = new BasicWebContext())
            {
                return context.Forms.Include("Fields").AsNoTracking().ToList();
            }
        }

        public static List<Form> Query(System.Linq.Expressions.Expression<Func<Form, bool>> condition)
        {
            using (var context = new BasicWebContext())
            {
                return context.Forms.Where(condition).ToList();
            }
        }

        public static void DeleteDbRecord(Guid id)
        {
            using (var context = new BasicWebContext())
            {
                context.Forms.Remove(context.Forms.Find(id));
                context.SaveChanges();
            }
        }

        public static void UpdateDbRecord(Guid id, Action<Form> action)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.Forms.Find(id);
                action(record);
                context.SaveChanges();
            }
        }        

        public static void New(Form form)
        {
            using (var context = new BasicWebContext())
            {
                form.CreateTime = DateTime.Now;
                form.CreateUser = WebMatrix.WebData.WebSecurity.CurrentUserName;
                context.Forms.Add(form);
                context.SaveChanges();
            }
        }

        public static void New(Guid id, string name, int displayType, string layout, string style, string group, List<Field> fields)
        {
            using (var context = new BasicWebContext())
            {
                Form form = new Form();
                form.ID = id;
                form.Name = name;
                form.CreateTime = DateTime.Now;
                form.CreateUser = WebMatrix.WebData.WebSecurity.CurrentUserName;
                form.DisplayType = displayType;
                form.Layout = layout;
                form.Style = style;
                form.Group = group;
                form.Fields = fields;
                context.Forms.Add(form);
                context.SaveChanges();
            }
        }

        public static int NewInstance(Guid id, string data, int flowInstID = 0)
        {
            using (var context = new BasicWebContext())
            {
                FormInstance inst = new FormInstance();
                inst.FormID = id;
                inst.Data = data;
                inst.PostTime = DateTime.Now;
                inst.Username = WebMatrix.WebData.WebSecurity.CurrentUserName;
                inst.FlowInstanceID = flowInstID;
                context.FormInstances.Add(inst);
                context.SaveChanges();
                return context.FormInstances.Max(x => x.ID);
            }
        }

        public static Guid? GetMainFormOfWorkflow(Guid flowID)
        {
            using (var context = new BasicWebContext())
            {
                var wff = context.WorkflowForms.FirstOrDefault(x => x.WorkflowID == flowID);
                if (wff != null)
                {
                    return wff.FormID;
                }
                else
                {
                    return null;
                }
            }
        }

        public static int GetFormInstanceOfFlowInstance(int flowInstID)
        {
            using (var context = new BasicWebContext())
            {
                var formInst = context.FormInstances.FirstOrDefault(x => x.FlowInstanceID == flowInstID);
                if (formInst != null)
                {
                    return formInst.ID;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static Guid GetWorkFlowId(int id)
        {
            using (var context = new BasicWebContext())
            {
                return context.WorkflowForms.Find(id).WorkflowID;
            }
        }

        public static void DeleteRecord(int id)
        {
            using (var context = new BasicWebContext())
            {
                context.WorkflowForms.Remove(context.WorkflowForms.Find(id));
            }
        }

        public static void SetMainFormOfWorkflow(Guid flowID, Guid formID)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.WorkflowForms.FirstOrDefault(x => x.WorkflowID == flowID);
                if (record == null)
                {
                    WorkflowForm wff = new WorkflowForm();
                    wff.WorkflowID = flowID;
                    wff.FormID = formID;
                    context.WorkflowForms.Add(wff);
                }
                else
                {
                    record.FormID = formID;
                }
                context.SaveChanges();
            }
        }

        public static void AddFormOfWorkflow(Guid flowID, Guid formID)
        {
            using (var context = new BasicWebContext())
            {
                if (context.WorkflowForms.Any(x => x.WorkflowID == flowID && x.FormID == formID))
                {

                }
                else
                {
                    WorkflowForm wff = new WorkflowForm();
                    wff.WorkflowID = flowID;
                    wff.FormID = formID;
                    context.WorkflowForms.Add(wff);
                    context.SaveChanges();
                }
            }
        }
    }

    public class FieldManager
    {
        public static void New(Field record)
        {
            using (var context = new BasicWebContext())
            {
                context.Fields.Add(record);
                context.SaveChanges();
            }
        }

        public static Field GetDbRecord(int id)
        {
            using (var context = new BasicWebContext())
            {
                return context.Fields.Find(id);
            }
        }

        public static void DeleteDbRecord(int id)
        {
            using (var context = new BasicWebContext())
            {
                context.Fields.Remove(context.Fields.Find(id));
                context.SaveChanges();
            }
        }

        public static List<Field> GetAllDbRecords()
        {
            using (var context = new BasicWebContext())
            {
                return context.Fields.ToList();
            }
        }

        public static List<Field> Query(System.Linq.Expressions.Expression<Func<Field, bool>> condition)
        {
            using (var context = new BasicWebContext())
            {
                return context.Fields.Where(condition).ToList();
            }
        }

        public static void UpdateDbRecord(int id, Action<Field> action)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.Fields.Find(id);
                action(record);
                context.SaveChanges();
            }
        }
    }

    public class FormInstanceManager
    {
        public static void New(Guid formID, string data, int flowInstID)
        {
            using (var context = new BasicWebContext())
            {
                FormInstance record = new FormInstance();
                record.FormID = formID;
                record.Data = data;
                record.FlowInstanceID = flowInstID;
                record.Username = WebMatrix.WebData.WebSecurity.CurrentUserName;
                record.PostTime = DateTime.Now;
                context.FormInstances.Add(record);
                context.SaveChanges();
            }
        }

        public static FormInstance GetDbRecord(int id)
        {
            using (var context = new BasicWebContext())
            {
                return context.FormInstances.Find(id);
            }
        }

        public static void DeleteDbRecord(int id)
        {
            using (var context = new BasicWebContext())
            {
                context.FormInstances.Remove(context.FormInstances.Find(id));
                context.SaveChanges();
            }
        }

        public static List<FormInstance> GetAllDbRecords()
        {
            using (var context = new BasicWebContext())
            {
                return context.FormInstances.ToList();
            }
        }

        public static List<FormInstance> Query(System.Linq.Expressions.Expression<Func<FormInstance, bool>> condition)
        {
            using (var context = new BasicWebContext())
            {
                return context.FormInstances.Where(condition).ToList();
            }
        }

        public static void UpdateDbRecord(int id, Action<FormInstance> action)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.FormInstances.Find(id);
                action(record);
                context.SaveChanges();
            }
        }

        public static Dictionary<string, string> GetData(int id)
        {
            return System.Web.Helpers.Json.Decode<Dictionary<string, string>>(GetDbRecord(id).Data);
        }

        public static void SetData(int id, string key, string value)
        {
            var dict = GetData(id);
            dict[key] = value;
            UpdateDbRecord(id, inst =>
            {
                inst.Data = System.Web.Helpers.Json.Encode(dict);
            });
        }

        public static int GetLastID()
        {
            using (var context = new BasicWebContext())
            {
                return context.FormInstances.Max(x => x.ID);
            }
        }
    }

    public class LayoutManager
    {
        public static void New(Guid id, string name, string markup, string style)
        {
            using (var context = new BasicWebContext())
            {
                Layout record = new Layout();
                record.ID = id;
                record.Name = name;
                record.Markup = markup;
                record.Style = style;
                record.CreateUser = WebMatrix.WebData.WebSecurity.CurrentUserName;
                record.CreateTime = DateTime.Now;
                context.Layouts.Add(record);
                context.SaveChanges();
            }
        }

        public static Guid? GetLayoutOfFlowNode(Guid flowNodeID)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.FlowNodeLayouts.FirstOrDefault(x => x.FlowNodeID == flowNodeID);
                if (record != null)
                {
                    return record.LayoutID;
                }
                else
                {
                    return null;
                }
            }
        }

        public static void SetLayoutOfFlowNode(Guid flowNodeID, Guid layoutID)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.FlowNodeLayouts.FirstOrDefault(x => x.FlowNodeID == flowNodeID);
                if (record == null)
                {
                    FlowNodeLayout fnl = new FlowNodeLayout();
                    fnl.FlowNodeID = flowNodeID;
                    fnl.LayoutID = layoutID;
                    context.FlowNodeLayouts.Add(fnl);
                }
                else
                {
                    record.LayoutID = layoutID;
                }
                context.SaveChanges();
            }
        }

        public static Layout GetDbRecord(Guid id)
        {
            using (var context = new BasicWebContext())
            {
                return context.Layouts.Find(id);
            }
        }

        public static List<Layout> GetAllDbRecords()
        {
            using (var context = new BasicWebContext())
            {
                return context.Layouts.ToList();
            }
        }

        public static List<Layout> Query(System.Linq.Expressions.Expression<Func<Layout, bool>> condition)
        {
            using (var context = new BasicWebContext())
            {
                return context.Layouts.Where(condition).ToList();
            }
        }

        public static void UpdateDbRecord(Guid id, Action<Layout> action)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.Layouts.Find(id);
                action(record);
                context.SaveChanges();
            }
        }

        public static void DeleteDbRecord(Guid id)
        {
            using (var context = new BasicWebContext())
            {
                context.Layouts.Remove(context.Layouts.Find(id));
                context.SaveChanges();
            }
        }
    }
}
