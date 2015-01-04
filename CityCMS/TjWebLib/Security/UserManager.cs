using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TongJi.Web.DAL;

namespace TongJi.Web.Security
{
    public class UserManager
    {
        public int UserId { get; private set; }

        public UserManager(int userId)
        {
            UserId = userId;
        }

        public static int NewUser(string username, string password)
        {
            entity_user user = new entity_user { username = username, password = password, info = new XElement("root"), enabled = true, time = DateTime.Now };
            LinqHelper.Workflow.entity_user.InsertOnSubmit(user);
            LinqHelper.Workflow.SubmitChanges();
            return LinqHelper.Workflow.entity_user.Max(x => x.id);
        }

        public entity_user GetDbRecord()
        {
            return LinqHelper.Workflow.entity_user.Single(x => x.id == UserId);
        }

        public void Disable()
        {
            GetDbRecord().enabled = false;
            LinqHelper.Workflow.SubmitChanges();
        }

        public void Enable()
        {
            GetDbRecord().enabled = true;
            LinqHelper.Workflow.SubmitChanges();
        }

        public void SetUserInfo(string key, string value)
        {
            var xe = GetDbRecord().info;
            xe.SetAttValue(key, value);
            GetDbRecord().info = XElement.Parse(xe.ToString());
            LinqHelper.Workflow.SubmitChanges();
        }

        public string GetUserInfo(string key)
        {
            return GetDbRecord().info.AttValue(key);
        }

        public void ChangePassword(string password)
        {
            GetDbRecord().password = password;
            LinqHelper.Workflow.SubmitChanges();
        }

        public bool ValidatePassword(string password)
        {
            return GetDbRecord().password == password;
        }
    }
}
