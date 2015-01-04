using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Web.Models;

namespace TongJi.Web.Security
{
    public class UserManager
    {
        public static string CurrentUserDisplayName
        {
            get
            {
                string user = WebMatrix.WebData.WebSecurity.CurrentUserName;
                string name = GetDbRecord(user).RealName;
                return string.IsNullOrEmpty(name) ? user : name;
            }
        }

        public static UserProfile GetDbRecord(string username)
        {
            using (var context = new BasicWebContext())
            {
                return context.UserProfiles.AsNoTracking().Single(x => x.UserName == username);
            }
        }

        public static List<UserProfile> GetAllDbRecords()
        {
            using (var context = new BasicWebContext())
            {
                return context.UserProfiles.AsNoTracking().ToList();
            }
        }

        public static List<UserProfile> Query(System.Linq.Expressions.Expression<Func<UserProfile, bool>> condition)
        {
            using (var context = new BasicWebContext())
            {
                return context.UserProfiles.Where(condition).ToList();
            }
        }

        public static void UpdateDbRecord(string username, Action<UserProfile> action)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.UserProfiles.Single(x => x.UserName == username);
                action(record);
                context.SaveChanges();
            }
        }
    }

    public class DepartmentManager
    {
        public static Department GetDbRecord(int id)
        {
            using (var context = new BasicWebContext())
            {
                return context.Departments.Find(id);
            }
        }

        public static List<Department> GetAllDbRecords()
        {
            using (var context = new BasicWebContext())
            {
                return context.Departments.AsNoTracking().ToList();
            }
        }

        public static List<Department> Query(System.Linq.Expressions.Expression<Func<Department, bool>> condition)
        {
            using (var context = new BasicWebContext())
            {
                return context.Departments.Where(condition).ToList();
            }
        }

        public static void DeleteDbRecord(int id)
        {
            // todo: 有条件删除部门
        }

        public static void UpdateDbRecord(int id, Action<Department> action)
        {
            using (var context = new BasicWebContext())
            {
                var record = context.Departments.Find(id);
                action(record);
                context.SaveChanges();
            }
        }

        public static void New(string name, string description, int parentID)
        {
            using (var context = new BasicWebContext())
            {
                Department record = new Department { Name = name, Description = description, ParentID = parentID };
                context.Departments.Add(record);
                context.SaveChanges();
                System.Web.Security.Roles.CreateRole("d_" + name);
            }
        }

        public static void AssignSubDepartment(int id, int sub)
        {
            UpdateDbRecord(sub, record =>
            {
                record.ParentID = id;
            });
        }

        public static void AddUser(int id, string username, string title, bool isInCharge)
        {
            using (var context = new BasicWebContext())
            {
                Position record = new Position { Username = username, DepartmentID = id, Title = title, IsInCharge = isInCharge };
                context.Positions.Add(record);
                context.SaveChanges();
                Department department = context.Departments.Find(id);
                System.Web.Security.Roles.AddUserToRole(username, "d_" + department.Name);
            }
        }

        public static void RemoveUser(int id, string username)
        {
            using (var context = new BasicWebContext())
            {
                Position record = context.Positions.FirstOrDefault(x => x.DepartmentID == id && x.Username == username);
                if (record != null)
                {
                    context.Positions.Remove(record);
                    context.SaveChanges();
                    Department department = context.Departments.Find(id);
                    System.Web.Security.Roles.RemoveUserFromRole(username, "d_" + department.Name);
                }
            }
        }

        public static List<PositionViewModel> GetUsers(int id)
        {
            using (var context = new BasicWebContext())
            {
                return context.UserProfiles.Join(context.Positions.Where(x => x.DepartmentID == id), user => user.UserName, pos => pos.Username, (user, pos) => new PositionViewModel
                {
                    UserName = user.UserName,
                    RealName = user.RealName,
                    Gender = user.Gender,
                    Department = string.Empty,
                    Title = pos.Title,
                    IsInCharge = pos.IsInCharge
                }).ToList();
            }
        }

        public static List<Department> GetSubDepartments(int id)
        {
            return Query(x => x.ParentID == id);
        }

        public static Department GetParentDepartment(int id)
        {
            using (var context = new BasicWebContext())
            {
                if (id == 0)
                {
                    return new Department { Name = "<无>" };
                }
                else
                {
                    int parentID = context.Departments.Find(id).ParentID;
                    if (parentID == 0)
                    {
                        return new Department { Name = "<根部门>" };
                    }
                    return context.Departments.Find(parentID);
                }
            }
        }
    }
}
