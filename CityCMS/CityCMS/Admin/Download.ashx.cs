using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TongJi.Web.CMS;

namespace CityCMS.Admin
{
    /// <summary>
    /// Download 的摘要说明
    /// </summary>
    public class Download : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int fileId = Convert.ToInt32(context.Request.QueryString["fileId"]);
            var record = ContentManager.GetAttachmentDbRecord(fileId);
            string downFile = ContentManager.AttachmentPath + record.filename;
            //context.Response.ContentType = "application/octet-stream";
            context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(record.name, System.Text.Encoding.UTF8));
            context.Response.WriteFile(downFile);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}