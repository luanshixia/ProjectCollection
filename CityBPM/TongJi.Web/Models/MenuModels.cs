using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongJi.Web.Models
{
    public class MenuItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public int ParentID { get; set; }
        public int Order { get; set; }
        public DateTime CreateTime { get; set; }
        public string AllowedRoles { get; set; }
    }

    public class MenuItemJsonObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public int Level { get; set; }
        public bool Selected { get; set; }
        public List<MenuItemJsonObject> Children { get; set; }
    }

    public static class MenuManager
    {
        public static string[] MenuItemTypes = { "Folder", "Url", "ActionLink", "RouteLink" };

        public static Dictionary<int, string> GetAllOtherMenuItemNames(int currentID)
        {
            using (var context = new BasicWebContext())
            {
                var dict = context.MenuItems.ToDictionary(x => x.ID, x => x.Name);
                dict[0] = "<root>";
                dict.Remove(currentID);
                return dict;
            }
        }

        public static MenuItem GetDbRecord(int id)
        {
            using (var context = new BasicWebContext())
            {
                return context.MenuItems.Find(id);
            }
        }

        public static void NewMenuItem(string name, string description, string type, string action, string icon, int parentID, int order, string allowedRoles)
        {
            using (var context = new BasicWebContext())
            {
                MenuItem item = new MenuItem
                {
                    Name = name,
                    Description = description,
                    Type = type,
                    Action = action,
                    Icon = icon,
                    ParentID = parentID,
                    Order = order,
                    CreateTime = DateTime.Now,
                    AllowedRoles = allowedRoles
                };
                context.MenuItems.Add(item);
                context.SaveChanges();
            }
        }

        public static void EditMenuItem(int id, string name, string description, string type, string action, string icon, int parentID, int order, string allowedRoles)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.MenuItems.Find(id);
                record.Name = name;
                record.Description = description;
                record.Type = type;
                record.Action = action;
                record.Icon = icon;
                record.ParentID = parentID;
                record.Order = order;
                record.AllowedRoles = allowedRoles;
                context.SaveChanges();
            }
        }

        public static void DeleteMenuItem(int id)
        {
            using (var context = new BasicWebContext())
            {
                context.MenuItems.Remove(context.MenuItems.Find(id));
                context.SaveChanges();
            }
        }

        public static List<MenuItem> GetSubMenuItems(int id)
        {
            using (var context = new BasicWebContext())
            {
                return context.MenuItems.Where(x => x.ParentID == id).ToList();
            }
        }

        public static List<MenuItemJsonObject> GetTree()
        {
            var tree = GetSubMenuItems(0).FormTree(0);
            tree.ForEach(t => t.Children = GetSubMenuItems(t.ID).FormTree(1));
            return tree;
        }

        private static List<MenuItemJsonObject> FormTree(this IEnumerable<MenuItem> items, int level)
        {
            return items.Where(x => IsUserAllowed(WebMatrix.WebData.WebSecurity.CurrentUserName, string.Empty, x.AllowedRoles)).OrderBy(x => x.Order).Select(x => new MenuItemJsonObject
            {
                ID = x.ID,
                Name = x.Name,
                Description = x.Description,
                Type = x.Type,
                Action = x.Action,
                Icon = x.Icon,
                Level = level
            }).ToList();
        }

        public static bool IsUserAllowed(string user, string allowedUsers, string allowedRoles)
        {
            if (string.IsNullOrEmpty(user))
            {
                return false;
            }
            if (allowedUsers == "*" || allowedRoles == "*")
            {
                return true;
            }
            else
            {
                var users = allowedUsers.Split(',');
                var roles = allowedRoles.Split(',');
                if (users.Contains(user) || roles.Any(x => (!string.IsNullOrEmpty(x)) && System.Web.Security.Roles.IsUserInRole(user, x)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
