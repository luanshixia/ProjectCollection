using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using TongJi.Web.Models;

namespace UPlanWeb
{
    public class DbConfig
    {
        public static void InitializeWebBasics()
        {
            Database.SetInitializer<BasicWebContext>(null);
            try
            {
                using (var context = new BasicWebContext())
                {
                    //((IObjectContextAdapter)context).ObjectContext.DeleteDatabase();
                    if (!context.Database.Exists())
                    {
                        // 创建不包含 Entity Framework 迁移架构的数据库
                        ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("无法初始化 Web Basics 数据库。", ex);
            }
        }
    }
}