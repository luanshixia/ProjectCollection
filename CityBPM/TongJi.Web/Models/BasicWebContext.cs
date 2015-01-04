using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TongJi.Web.CMS;
using TongJi.Web.Communication;
using TongJi.Web.Flow;
using TongJi.Web.Forms;
using TongJi.Web.Security;

namespace TongJi.Web.Models
{
    public class BasicWebContext : DbContext
    {
        public BasicWebContext()
            : base("WebBasicsConnection")
        {
        }

        public BasicWebContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        // Security

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<TongJi.Web.Security.Action> Actions { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }

        // Workflow

        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<FlowNode> FlowNodes { get; set; }
        public DbSet<FlowInstance> FlowInstances { get; set; }
        public DbSet<FlowInstanceAction> FlowInstanceActions { get; set; }
        public DbSet<FlowChartTextLabel> FlowChartTextLabels { get; set; }

        // Forms

        public DbSet<Field> Fields { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormInstance> FormInstances { get; set; }
        public DbSet<WorkflowForm> WorkflowForms { get; set; }
        public DbSet<Layout> Layouts { get; set; }
        public DbSet<FlowNodeLayout> FlowNodeLayouts { get; set; }

        // Site

        public DbSet<SiteMail> SiteMails { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        // CMS

        public DbSet<File> Files { get; set; }
    }

    //public class FlowNodeFlowNode
    //{
    //    [Key, Column(Order = 1)]
    //    public int FlowNode_ID { get; set; }
    //    [Key, Column(Order = 2)]
    //    public int FlowNode_ID1 { get; set; }
    //    public Guid? FlowNode_ID_1 { get; set; }
    //    public Guid? FlowNode_ID1_1 { get; set; }
    //}

    public class BasicWebManager
    {
        public static void New<T>(T record) where T : class
        {
            using (var context = new BasicWebContext())
            {
                context.Set<T>().Add(record);
                context.SaveChanges();
            }
        }

        public static void NewAll<T>(IEnumerable<T> records) where T : class
        {
            using (var context = new BasicWebContext())
            {
                foreach (var record in records)
                {
                    context.Set<T>().Add(record);
                }
                context.SaveChanges();
            }
        }

        public static T Get<T>(object id) where T : class
        {
            using (var context = new BasicWebContext())
            {
                return context.Set<T>().Find(id);
            }
        }

        public static List<T> GetAll<T>() where T : class
        {
            using (var context = new BasicWebContext())
            {
                return context.Set<T>().AsNoTracking().ToList();
            }
        }

        public static List<T> Query<T>(System.Linq.Expressions.Expression<Func<T, bool>> condition) where T : class
        {
            using (var context = new BasicWebContext())
            {
                return context.Set<T>().Where(condition).ToList();
            }
        }

        public static void Delete<T>(object id) where T : class
        {
            using (var context = new BasicWebContext())
            {
                var record = context.Set<T>().Find(id);
                context.Set<T>().Remove(record);
                context.SaveChanges();
            }
        }

        public static void DeleteAll<T>(System.Linq.Expressions.Expression<Func<T, bool>> selector) where T : class
        {
            using (var context = new BasicWebContext())
            {
                var records = context.Set<T>().Where(selector);
                foreach (var record in records)
                {
                    context.Set<T>().Remove(record);
                }
                context.SaveChanges();
            }
        }

        public static void Update<T>(object id, Action<T> action) where T : class
        {
            using (var context = new BasicWebContext())
            {
                var record = context.Set<T>().Find(id);
                action(record);
                context.SaveChanges();
            }
        }

        public static void UpdateAll<T>(System.Linq.Expressions.Expression<Func<T, bool>> selector, Action<T> action) where T : class
        {
            using (var context = new BasicWebContext())
            {
                var records = context.Set<T>().Where(selector);
                foreach (var record in records)
                {
                    action(record);
                }
                context.SaveChanges();
            }
        }
    }
}

namespace TongJi.Web
{
    public static class Utils
    {
        public static string AppDataPath
        {
            get
            {
                return System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
            }
        }
    }
}
