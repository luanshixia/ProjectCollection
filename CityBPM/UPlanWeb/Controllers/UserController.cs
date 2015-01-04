using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

using TongJi.Web.Models;
using TongJi.Web.Security;

namespace UPlanWeb.Controllers
{
    public class UserController : Controller
    {
        [Authorize(Roles = "admin")]
        public ActionResult List()
        {
            using (var uc = new BasicWebContext())
            {
                var users = uc.UserProfiles.ToList();
                return View(users);
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 尝试注册用户
                //try
                //{
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                using (var uc = new BasicWebContext())
                {
                    var userProfile = uc.UserProfiles.Single(x => x.UserName == model.UserName);
                    userProfile.CreateTime = DateTime.Now;
                    userProfile.IsForbidden = false;
                    uc.SaveChanges();
                }
                return RedirectToAction("List");
                //}
                //catch (MembershipCreateUserException e)
                //{
                //    ModelState.AddModelError("", "创建账户出错。");
                //}
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult ManageRoles()
        {
            var roles = Roles.GetAllRoles().ToList();
            var roleUsers = roles.ToDictionary(r => r, r => Roles.GetUsersInRole(r).ToList());
            using (var uc = new BasicWebContext())
            {
                var users = uc.UserProfiles.Select(x => x.UserName).ToList();
                ViewBag.users = users;
            }
            ViewBag.roles = roles;
            ViewBag.roleUsers = roleUsers;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult NewRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError("roleName", "角色名不可为空");
            }
            else if (Roles.RoleExists(roleName))
            {
                ModelState.AddModelError("roleName", "角色名已存在");
            }

            if (ModelState.IsValid)
            {
                Roles.CreateRole(roleName);
            }
            return RedirectToAction("ManageRoles");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddUserToRole(string user, string role)
        {
            if (!Roles.IsUserInRole(user, role))
            {
                Roles.AddUserToRole(user, role);
            }
            return RedirectToAction("ManageRoles");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUserFromRole(string user, string role)
        {
            Roles.RemoveUserFromRole(user, role);
            return RedirectToAction("ManageRoles");
        }

        public ActionResult Center()
        {
            return View();
        }

        public ActionResult CompleteInfo()
        {
            var model = UserManager.GetDbRecord(WebSecurity.CurrentUserName);
            return View(model);
        }

        [HttpPost]
        public ActionResult CompleteInfo(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                UserManager.UpdateDbRecord(WebSecurity.CurrentUserName, user =>
                {
                    user.RealName = model.RealName;
                    user.Gender = model.Gender;
                    user.Birthday = model.Birthday;
                    user.Phone = model.Phone;
                    user.Email = model.Email;
                    user.Address = model.Address;
                    user.Hobby = model.Hobby;
                    user.PersonalStatement = model.PersonalStatement;
                });
                return RedirectToAction("Center");
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult EditInfo(string id)
        {
            var model = UserManager.GetDbRecord(id);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditInfo(string id, UserProfile model)
        {
            if (ModelState.IsValid)
            {
                UserManager.UpdateDbRecord(id, user =>
                {
                    user.RealName = model.RealName;
                    user.Gender = model.Gender;
                    user.Birthday = model.Birthday;
                    user.Phone = model.Phone;
                    user.Email = model.Email;
                    user.Address = model.Address;
                    user.Hobby = model.Hobby;
                    user.PersonalStatement = model.PersonalStatement;
                });
                return RedirectToAction("List");
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult ToggleForbidden(string id)
        {
            if (id != "root")
            {
                UserManager.UpdateDbRecord(id, user =>
                {
                    if (user.IsForbidden == true)
                    {
                        user.IsForbidden = false;
                    }
                    else
                    {
                        user.IsForbidden = true;
                    }
                });
            }
            return RedirectToAction("List");
        }
    }
}
