using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Web.Security;

namespace TongJi.Web
{
    public class TjWebPage : System.Web.UI.Page
    {
        public virtual bool CanUserAccess(LoginUser user)
        {
            return true;
        }

        protected override void OnLoad(EventArgs e)
        {
            CheckLogin();
            base.OnLoad(e);
        }

        protected virtual bool CheckLogin()
        {
            return true;
        }

        public static string Url(string relative)
        {
            var path = System.Web.HttpContext.Current.Request.ApplicationPath;
            if (path == "/")
            {
                return relative;
            }
            else
            {
                return System.Web.HttpContext.Current.Request.ApplicationPath + "/" + relative;
            }
        }
    }

    public class AdminPage : TjWebPage
    {
        public override bool CanUserAccess(LoginUser user)
        {
            return LoginManager.IsAdministrator(user.UID);
        }
    }
}
