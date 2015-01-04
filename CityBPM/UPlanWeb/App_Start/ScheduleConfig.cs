using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TongJi.Web.Maintenance;

namespace UPlanWeb
{
    public class ScheduleConfig
    {
        public static void RegisterSchedules()
        {
            //ScheduledTask.Register(() =>
            //{
            //    var users = TongJi.Web.Security.UserManager.GetAllDbRecords().Select(x => x.UserName).ToList();
            //    foreach (var user in users)
            //    {
            //        TongJi.Web.Notifications.Notification.EnqueueMessage(user, "您有未完成的待办事项。");
            //    }
            //}, DateTime.Now, TimeSpan.FromSeconds(30));

            TongJi.Web.Notifications.Notification.MessageGenerate += () => // with Session
            {
                var user = WebMatrix.WebData.WebSecurity.CurrentUserName;
                var list = TongJi.Web.Flow.WorkflowManager.GetTodoList(user);
                if (list.Count > 0)
                {
                    TongJi.Web.Notifications.Notification.EnqueueMessage(user, "您有未完成的待办事项。");
                }
            };
        }
    }
}