using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MyKatanaNotif.MyStartup))]

namespace MyKatanaNotif
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var notif = new NotifyIcon();
            notif.BalloonTipTitle = "Katana app";
            notif.BalloonTipText = "Katana app is running...";
            notif.Text = "Katana app";
            notif.Icon = new System.Drawing.Icon(GetResource("MyKatanaNotif.Katana.ico"));
            notif.Visible = true;
            notif.ShowBalloonTip(2000);

            var menuExit = new MenuItem("Exit");
            menuExit.Click += (sender, e) => Application.Exit();
            var childen = new MenuItem[] { menuExit };
            notif.ContextMenu = new ContextMenu(childen);
            notif.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    notif.ShowBalloonTip(2000);
                }
            });

            StartKatana();
            Application.Run();
        }

        private static void StartKatana()
        {
            Microsoft.Owin.Hosting.WebApp.Start<MyStartup>("http://localhost:9000");
        }

        private static System.IO.Stream GetResource(string resourceName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(resourceName);
        }
    }

    public class MyStartup
    {
        public void Configuration(IAppBuilder app)
        {
            // New code: Add the error page middleware to the pipeline. 
            app.UseErrorPage();

            app.Run(context =>
            {
                // New code: Throw an exception for this URI path.
                if (context.Request.Path.Value == "/fail")
                {
                    throw new Exception("Random exception");
                }

                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("Hello, world.");
            });
        }
    }
}
