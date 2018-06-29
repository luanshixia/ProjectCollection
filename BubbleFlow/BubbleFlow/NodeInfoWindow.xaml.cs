using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BubbleFlow
{
    public partial class NodeInfoWindow : Window
    {
        public NodeInfoWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public string GetNodeName()
        {
            return this.val_name.Text;
        }

        public string GetNodeUser()
        {
            return this.val_user.Text;
        }

        public string GetNodeRole()
        {
            return this.val_role.Text;
        }

        public void SetNodeName(string name)
        {
            this.val_name.Text = name;
        }

        public void SetNodeUser(string user)
        {
            this.val_user.Text = user;
        }

        public void SetNodeRole(string role)
        {
            this.val_role.Text = role;
        }
    }
}

