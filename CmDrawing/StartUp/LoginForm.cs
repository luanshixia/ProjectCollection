using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace StartUp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            LoginManager.CheckingResult += LoginManager_CheckingResult;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            if (username.Length > 0 && password.Length > 0)
            {
                LoginManager.Login(username, password);
            }
        }

        void LoginManager_CheckingResult(CadLoaderModel model)
        {
            if (model.Success)
            {
                int id = model.ProductID;
                string file = FileManager.Loader.Replace(".dll", "") + ".cfg";
                var lines = System.IO.File.ReadAllLines(FileManager.CurrentFolder + file);
                lines[1] = "ID=" + id;
                System.IO.File.WriteAllLines(FileManager.CurrentFolder + file, lines);
                MessageBox.Show(string.Format("登录成功，有效期至 {0}。", model.Expire.ToShortDateString()), "提示");
                this.Close();
            }
            else
            {
                MessageBox.Show("登录失败。可能的原因：\n1. 用户不存在\n2. 密码错误\n3. 账号已过期", "提示");
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (LoginManager.CurrentLogin == null || LoginManager.CurrentLogin.Success == false)
            {
                Application.Exit();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }

    public static class LoginManager
    {
        public static CadLoaderModel CurrentLogin { get; set; }

        public static event Action<CadLoaderModel> CheckingResult;
        public static void OnCheckingResult(CadLoaderModel model)
        {
            CurrentLogin = model;
            if (CheckingResult != null)
            {
                CheckingResult(model);
            }
        }

        public static void Login(string username, string password)
        {
            string query = string.Format("?username={0}&password={1}", username, password);
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            client.DownloadStringAsync(new Uri(FileManager.Server + query));
        }

        static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Result))
            {
                CadLoaderModel model = e.Result.ParseXml<CadLoaderModel>();
                OnCheckingResult(model);
            }
            else
            {
                CadLoaderModel model = new CadLoaderModel { Success = false };
                OnCheckingResult(model);
            }
        }

        public static T ParseXml<T>(this string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (System.IO.StringReader sr = new System.IO.StringReader(xml))
            {
                T result = (T)serializer.Deserialize(sr);
                return result;
            }
        }
    }

    public class CadLoaderModel
    {
        public bool Success { get; set; }
        public int ProductID { get; set; }
        public DateTime Expire { get; set; }
    }
}
