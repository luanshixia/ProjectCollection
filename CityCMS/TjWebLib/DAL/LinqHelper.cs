using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongJi.Web.DAL
{
    public static class LinqHelper
    {
        private static WorkflowDataContext _workflow = new WorkflowDataContext();
        public static WorkflowDataContext Workflow { get { return _workflow; } }

        private static CMSDataContext _cms;
        public static CMSDataContext CMS { get { return _cms; } }

        //static LinqHelper()
        //{
        //    var path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
        //    var cms_connection = string.Format("data source=.\\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename={0}App_Data\\Repository.mdf;User Instance=true", path);
        //    _cms = new CMSDataContext(cms_connection);
        //    if (!_cms.DatabaseExists())
        //    {
        //        _cms.CreateDatabase();
        //    }
        //}

        public static void Initialize(string connectionName = null, string webConfigPath = "~")
        {
            if (connectionName == null)
            {
                var path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                var cms_connection = string.Format("data source=.\\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename={0}App_Data\\Repository.mdf;User Instance=true", path);
                _cms = new CMSDataContext(cms_connection);
                if (!_cms.DatabaseExists())
                {
                    _cms.CreateDatabase();
                }
            }
            else
            {
                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(webConfigPath);
                var section = config.ConnectionStrings;
                _cms = new CMSDataContext(section.ConnectionStrings[connectionName].ConnectionString);
                if (!_cms.DatabaseExists())
                {
                    _cms.CreateDatabase();
                }
            }
        }
    }
}

namespace System.Linq
{
    public static class TjExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }
    }
}

namespace System.Xml.Linq
{
    public static class TjExtensions
    {
        public static string AttValue(this XElement xe, string attName)
        {
            return xe.Attribute(attName) == null ? string.Empty : xe.Attribute(attName).Value;
        }

        public static void SetAttValue(this XElement xe, string attName, string attValue)
        {
            if (xe.Attribute(attName) == null)
            {
                xe.Add(new XAttribute(attName, attValue));
            }
            else
            {
                xe.Attribute(attName).Value = attValue;
            }
        }
    }
}
