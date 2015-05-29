using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using AutoCADCommands;

namespace TongJi.Drawing
{
    /// <summary>
    /// Menu.xaml 的交互逻辑
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            this.InitializeComponent();

            // 在此点之下插入创建对象所需的代码。
        }

        private void InvokeCommandMethod(Action method)
        {
            using (Autodesk.AutoCAD.ApplicationServices.DocumentLock doclock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                Interaction.SetActiveDocFocus();
                method();
            }
        }

        private void cmdHandler(object sender, RoutedEventArgs e)
        {
            Interaction.SetPickSet(new Autodesk.AutoCAD.DatabaseServices.ObjectId[0]);
            System.Reflection.MethodInfo mi = typeof(CommandManager).GetMethod((sender as Control).Tag.ToString());
            InvokeCommandMethod(InvokeWithExceptionMessage(mi));
        }

        private void usePickSetCmdHandler(object sender, RoutedEventArgs e)
        {
            System.Reflection.MethodInfo mi = typeof(CommandManager).GetMethod((sender as Control).Tag.ToString());
            InvokeCommandMethod(InvokeWithExceptionMessage(mi));
        }

        // 20110620
        private void cadCmdHandler(object sender, RoutedEventArgs e)
        {
            string cmd = (sender as Control).Tag.ToString();
            Interaction.SetActiveDocFocus();
            Interaction.StartCommand(cmd + " ");
        }

        // 20110618
        private Action InvokeWithExceptionMessage(System.Reflection.MethodInfo mi)
        {
            return () =>
            {
                try
                {
                    mi.Invoke(null, null);
                }
                catch (System.Exception ex)
                {
                    ex = ex.GetBaseException();
                    Interaction.WriteLine(ex.Message);
                }
            };
        }

        // 20120614 mod 加入动画
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            foreach (var child in (LayoutRoot.Content as StackPanel).Children)
            {
                var exp = child as Expander;
                if (exp != null)
                {
                    if (sender != exp)
                    {
                        if (exp.IsExpanded == true)
                        {
                            var content = exp.Content as FrameworkElement;
                            if (content != null)
                            {
                                var trans = new ScaleTransform(1, 1);
                                content.LayoutTransform = trans;
                                DoubleAnimation da = new DoubleAnimation(0, new Duration(TimeSpan.Parse("0:0:0.2")));
                                da.Completed += (s, args) => exp.IsExpanded = false;
                                trans.BeginAnimation(ScaleTransform.ScaleYProperty, da);
                            }                            
                        }
                    }
                    else
                    {
                        var content = exp.Content as FrameworkElement;
                        if (content != null)
                        {
                            var trans = new ScaleTransform(1, 0);
                            content.LayoutTransform = trans;
                            DoubleAnimation da = new DoubleAnimation(1, new Duration(TimeSpan.Parse("0:0:0.4")));
                            trans.BeginAnimation(ScaleTransform.ScaleYProperty, da);
                        }
                    }
                }
            }
        }
    }
}