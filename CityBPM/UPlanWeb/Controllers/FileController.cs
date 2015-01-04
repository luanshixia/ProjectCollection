using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TongJi.Web.CMS;
using TongJi.Web.Models;

namespace UPlanWeb.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        public ActionResult List(string id)
        {
            List<File> list;
            if (id == null)
            {
                list = BasicWebManager.GetAll<File>();
            }
            else if (id.Contains('|'))
            {
                var parts = id.Split('|');
                var parts0 = parts[0];
                var parts1 = parts[1];
                list = BasicWebManager.Query<File>(x => x.Type == parts0 && x.Group == parts1);
            }
            else
            {
                list = BasicWebManager.Query<File>(x => x.Group == id);
            }
            list = list.OrderByDescending(x => x.PostTime).ToList();
            ViewBag.id = id;
            return View(list);
        }

        public ActionResult Detail(Guid id)
        {
            var model = BasicWebManager.Get<File>(id);
            ViewBag.id = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadToGroup(string id, string tags)
        {
            string type;
            string group;
            if (id == null)
            {
                id = string.Empty;
            }
            if (id.Contains('|'))
            {
                var parts = id.Split('|');
                type = parts[0];
                group = parts[1];
            }
            else
            {
                type = string.Empty;
                group = id;
            }
            Guid fileID = Guid.NewGuid();
            var file = Request.Files["file"];
            if (string.IsNullOrEmpty(file.FileName))
            {
                ModelState.AddModelError("file", "请选择文件");
            }
            if (ModelState.IsValid)
            {
                File record = new File { ID = fileID, Name = file.FileName, Group = group, Type = type, PostTime = DateTime.Now, PostUser = WebMatrix.WebData.WebSecurity.CurrentUserName, Tags = tags, Size = file.ContentLength };
                BasicWebManager.New(record);
                file.SaveAs(GetUploadFileName(fileID));
            }
            return RedirectToAction("List", new { id });
        }

        [HttpPost]
        public ActionResult Upload(Guid id, string tags)
        {
            var record = BasicWebManager.Get<File>(id);
            Guid fileID = id;
            var file = Request.Files["file"];
            if (string.IsNullOrEmpty(file.FileName))
            {
                ModelState.AddModelError("file", "请选择文件");
            }
            if (ModelState.IsValid)
            {
                if (record == null)
                {
                    record = new File { ID = fileID, Name = file.FileName, PostTime = DateTime.Now, PostUser = WebMatrix.WebData.WebSecurity.CurrentUserName, Tags = tags, Size = file.ContentLength };
                    BasicWebManager.New(record);
                }
                else // 不允许修改PostUser
                {
                    BasicWebManager.Update<File>(id, x =>
                    {
                        x.Name = file.FileName;
                        x.PostTime = DateTime.Now;
                        x.Tags = tags;
                        x.Size = file.ContentLength;
                    });
                }
                file.SaveAs(GetUploadFileName(fileID));
            }
            return RedirectToAction("Detail", new { id });
        }

        public ActionResult Delete(Guid id, string returnUrl)
        {
            BasicWebManager.Delete<File>(id);
            System.IO.File.Delete(GetUploadFileName(id));
            return Redirect(returnUrl);
        }

        public ActionResult Download(Guid id)
        {
            var record = BasicWebManager.Get<File>(id);
            return File(GetUploadFileName(id), "multipart/form-data", record.Name);
        }

        private string GetUploadFileName(Guid id)
        {
            return string.Format("{0}\\{1}.file", Server.MapPath("~/storage/upload_files"), id);
        }

    }
}
