using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TongJi.Web.Security
{
    public class LoginUser
    {
        public int UID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
    }

    public interface IUserTable
    {
        IEnumerable<LoginUser> GetUsers();
    }

    public static class LoginManager
    {
        private static LoginUser GetUser(IUserTable users, string username)
        {
            if (users.GetUsers().Any(x => x.Username == username))
            {
                var user = users.GetUsers().First(x => x.Username == username);
                return user;
            }
            else
            {
                return null;
            }
        }

        public static bool Validate(IUserTable users, string username, string password, out LoginUser validUser)
        {
            LoginUser user = GetUser(users, username);
            if (user == null)
            {
                validUser = null;
                return false;
            }
            else
            {
                if (user.Password == password)
                {
                    validUser = user;
                    return true;
                }
                else
                {
                    validUser = null;
                    return false;
                }
            }
        }

        public static bool IsAdministrator(int user_id)
        {
            return user_id == 1;
        }

        public static void Login(LoginUser user)
        {
            HttpContext.Current.Session["User"] = user;
        }

        public static string GetLoggedInUsername()
        {
            LoginUser user = HttpContext.Current.Session["User"] as LoginUser;
            if (user != null)
            {
                return user.Username;
            }
            else
            {
                return null;
            }
        }

        public static LoginUser GetLoggedInUser()
        {
            return HttpContext.Current.Session["User"] as LoginUser;
        }
    }

    //public interface ILoginManager
    //{
    //    LoginUser GetUser(string username);
    //    bool Validate(string username, string password, out LoginUser validUser);
    //    bool IsAdministrator(int user_id);
    //}

    //public interface IRegisterManager
    //{
    //    string[] GetUsernames();
    //    void Register(string username, string password);
    //}
}
