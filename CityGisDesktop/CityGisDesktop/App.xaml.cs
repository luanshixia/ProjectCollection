using System;
using System.Windows;
using System.Windows.Threading;

namespace Dreambuild.Gis.Desktop
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static DispatcherOperationCallback exitFrameCallback = ExitFrame;

        ///<summary>
        ///Processes all UI messages currently in the message queue.
        ///</summary>
        public static void DoEvents()
        {
            // Create new nested message pump.

            DispatcherFrame nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than the that of UI event messages.

            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, exitFrameCallback, nestedFrame);

            // pump the nested message loop, the nested message loop will
            // immediately process the messages left inside the message queue.

            Dispatcher.PushFrame(nestedFrame);

            // If the "exitFrame" callback doesn't get finished, Abort it.

            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;

            // Exit the nested message loop.

            frame.Continue = false;
            return null;
        }

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);

        //    var config = new HttpSelfHostConfiguration("http://localhost:38384");

        //    config.Routes.MapHttpRoute(
        //        "API Default", "api/{controller}/{id}",
        //        new { id = RouteParameter.Optional });

        //    using (var server = new HttpSelfHostServer(config))
        //    {
        //        server.OpenAsync().Wait();
        //    }
        //}
    }
}
