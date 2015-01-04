using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace TongJi.Web.Security
{
    public class RoleCapabilities
    {
        public bool CanLogin { get; set; }
        public string InitialUrl { get; set; }
        public List<string> AllowedAreas { get; set; }
        public List<string> AllowedACUs { get; set; }
    }

    public static class RoleCapabilitiesManager
    {
        public static Dictionary<string, RoleCapabilities> Roles { get; set; }

        static RoleCapabilitiesManager()
        {
            Roles = new Dictionary<string, RoleCapabilities>();
        }

        public static bool CanRoleAccessUnit(string role, string unit)
        {
            if (role == "admin")
            {
                return true;
            }
            if (!RoleCapabilitiesManager.Roles.ContainsKey(role))
            {
                return true;
            }
            else
            {
                if (RoleCapabilitiesManager.Roles[role].AllowedACUs == null)
                {
                    return false;
                }
                else
                {
                    return RoleCapabilitiesManager.Roles[role].AllowedACUs.Contains(unit);
                }
            }
        }

        public static bool CanUserLogin(string user)
        {
            var roles = System.Web.Security.Roles.GetRolesForUser(user);
            return roles.All(r => RoleCapabilitiesManager.CanRoleLogin(r));
        }

        public static bool CanRoleLogin(string role)
        {
            if (role == "admin")
            {
                return true;
            }
            if (!RoleCapabilitiesManager.Roles.ContainsKey(role))
            {
                return true;
            }
            else
            {
                return RoleCapabilitiesManager.Roles[role].CanLogin;
            }
        }

        public static string GetInitialUrl(string user)
        {
            var roles = System.Web.Security.Roles.GetRolesForUser(user);
            if (roles.Length > 0)
            {
                var role = roles[0];
                if (RoleCapabilitiesManager.Roles.ContainsKey(role))
                {
                    return RoleCapabilitiesManager.Roles[role].InitialUrl;
                }
            }
            return "~";
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AccessControlUnitAttribute : AuthorizeAttribute
    {
        public string Name { get; set; }

        public AccessControlUnitAttribute(string name)
            : base()
        {
            Name = name;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            bool result1 = base.AuthorizeCore(httpContext);
            var roles = System.Web.Security.Roles.GetRolesForUser();
            bool result2 = roles.Any(r => RoleCapabilitiesManager.CanRoleAccessUnit(r, Name));
            bool result = result1 && result2;
            if (result == false)
            {
                TongJi.Web.Notifications.Notification.EnqueueMessage(string.Format("您所在的用户组没有访问此功能模块({0})的权限，请咨询您的管理员。", Name));
            }
            return result;
        }
    }

    // todo: 实现组合控制
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeExtendedAttribute : AuthorizeAttribute
    {
        public string ExcludeRoles { get; set; }
        public string ExcludeUsers { get; set; }

        public AuthorizeExtendedAttribute()
            : base()
        {
            ExcludeRoles = string.Empty;
            ExcludeUsers = string.Empty;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var roles = System.Web.Security.Roles.GetRolesForUser();
            var user = WebMatrix.WebData.WebSecurity.CurrentUserName;

            var inRoles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var inUsers = Users.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var exRoles = ExcludeRoles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var exUsers = ExcludeUsers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            bool result0 = WebMatrix.WebData.WebSecurity.IsAuthenticated;
            bool result1 = inRoles.Contains("*") || roles.Any(r => inRoles.Contains(r));
            bool result2 = inUsers.Contains("*") || inUsers.Contains(user);
            bool result3 = !exRoles.Contains("*") && roles.All(r => !exRoles.Contains(r));
            bool result4 = !exUsers.Contains("*") && !exUsers.Contains(user);

            bool result = result0 && (result1 || result2) && (result3 && result4);
            if (result == false)
            {
                TongJi.Web.Notifications.Notification.EnqueueMessage(string.Format("您所在的用户组没有访问此功能模块的权限，请咨询您的管理员。"));
            }
            return result;
        }
    }
}
