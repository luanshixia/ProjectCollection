using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Dreambuild.Mvc
{
    /// <summary>
    /// Control callback manager.
    /// </summary>
    public class ControlCallbackManager
    {
        public HttpContextBase HttpContext { get; private set; }

        public const string KeyPrefix = "wj-";
        public const string ParameterPrefix = "wjdata-";
        public const string IsCallbackKey = KeyPrefix + "cbk";
        public const string ControlNameKey = KeyPrefix + "ctrl";
        public const string ControlIdKey = KeyPrefix + "id";
        public const string CommandNameKey = KeyPrefix + "cmd";

        public ControlCallbackManager(HttpContextBase context)
        {
            this.HttpContext = context;
        }

        public bool IsCallback
        {
            get
            {
                return this.HttpContext.Request.Params[IsCallbackKey] == "true";
            }
        }

        public string ControlName
        {
            get
            {
                return this.HttpContext.Request.Params[ControlNameKey];
            }
        }

        public string ControlId
        {
            get
            {
                return this.HttpContext.Request.Params[ControlIdKey];
            }
        }

        public string CommandName
        {
            get
            {
                return this.HttpContext.Request.Params[CommandNameKey];
            }
        }

        public RouteValueDictionary CommandParameter
        {
            get
            {
                var dict = new RouteValueDictionary();
                this.HttpContext.Request.Params.AllKeys
                    .Where(k => k.StartsWith(ParameterPrefix))
                    .ForEach(k =>
                    {
                        var k1 = k.Substring(ParameterPrefix.Length);
                        dict[k1] = this.HttpContext.Request.Params[k];
                    });
                return dict;
            }
        }

        protected void Write(string contentType, Action write)
        {
            var context = this.HttpContext;
            context.Response.Clear();
            context.Response.ContentType = contentType;
            write();
            context.Response.End();
        }

        public void WriteJson(object data)
        {
            Write("application/json", () =>
            {
                string json = System.Web.Helpers.Json.Encode(data);
                this.HttpContext.Response.Write(json);
            });
        }

        public void WriteContent(string content, string contentType = "text/plain")
        {
            Write(contentType, () =>
            {
                this.HttpContext.Response.Write(content);
            });
        }
    }
}
