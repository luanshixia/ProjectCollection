using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace TongJi.Web.Controls
{
    public enum FormDataType
    {
        String = 0,
        Int32,
        Double,
        DateTime,
        Enum,
        LargeText
    }

    public class TjForm : WebControl
    {
        private Dictionary<string, WebControl> _inputs = new Dictionary<string, WebControl>();
        private Dictionary<string, Label> _errors = new Dictionary<string, Label>();
        private Dictionary<string, Func<string, bool>> _serverValidators = new Dictionary<string, Func<string, bool>>();
        private Dictionary<string, FormDataType> _dataTypes = new Dictionary<string, FormDataType>();

        public TjForm(params string[] keys)
        {
            foreach (string key in keys)
            {
                _inputs.Add(key, new TextBox());
                _errors.Add(key, new Label { ForeColor = System.Drawing.Color.Red });
                _serverValidators.Add(key, x => true);
                _dataTypes.Add(key, FormDataType.String);
            }
        }

        public TjForm(Dictionary<string, FormDataType> fields)
        {
            foreach (var field in fields)
            {
                if (field.Value == FormDataType.Enum)
                {
                    _inputs.Add(field.Key, new DropDownList { Width = 200 });
                }
                else
                {
                    TextBox tb = new TextBox { Width = 300 };
                    if (field.Value == FormDataType.LargeText)
                    {
                        tb.TextMode = TextBoxMode.MultiLine;
                    }
                    _inputs.Add(field.Key, tb);
                }
                _errors.Add(field.Key, new Label { ForeColor = System.Drawing.Color.Red });
                _serverValidators.Add(field.Key, x => true);
                _dataTypes.Add(field.Key, field.Value);

                if (field.Value == FormDataType.Double || field.Value == FormDataType.Int32)
                {
                    SetServerValidator(field.Key, ServerValidators.NumberValidator, "无效数字");
                }
                else if (field.Value == FormDataType.DateTime)
                {
                    SetServerValidator(field.Key, ServerValidators.DateTimeValidator, "无效日期和时间");
                }
            }
        }

        public void SetDropDownList(string key, string[] items)
        {
            if (_dataTypes[key] == FormDataType.Enum)
            {
                var control = _inputs[key] as DropDownList;
                items.ToList().ForEach(x => control.Items.Add(x));
            }
        }

        public event EventHandler Submit;
        public event EventHandler Cancel;

        // 给原始验证函数包装上显示错误提示的功能。
        public void SetServerValidator(string key, Func<string, bool> serverValidator, string errorMessage)
        {
            _serverValidators[key] = x =>
            {
                if (serverValidator(x))
                {
                    _errors[key].Text = string.Empty;
                    return true;
                }
                else
                {
                    _errors[key].Text = errorMessage;
                    return false;
                }
            };
        }

        public void Lock(string key)
        {
            _inputs[key].Attributes["disabled"] = "true";
        }

        public HtmlDivision GetForm()
        {
            HtmlDivision div = new HtmlDivision();

            foreach (var item in _inputs)
            {
                HtmlParagraph p = new HtmlParagraph();
                div.Controls.Add(p);
                p.Controls.Add(new Label { Text = item.Key });
                p.Controls.Add(new HtmlBreak());
                p.Controls.Add(item.Value);
                p.Controls.Add(_errors[item.Key]);
            }

            Button btnSubmit = new Button { Text = "提交" };
            btnSubmit.Click += (sender, e) =>
            {
                if (_serverValidators.All(x => x.Value(GetText(x.Key))))
                {
                    if (Submit != null) Submit(sender, e);
                }
            };
            Button btnCancel = new Button { Text = "返回" };
            btnCancel.Click += (sender, e) => { if (Cancel != null) Cancel(sender, e); };
            div.Controls.Add(btnSubmit);
            div.Controls.Add(btnCancel);

            return div;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            GetForm().RenderControl(writer);
        }

        public string GetText(string key)
        {
            var control = _inputs[key];
            if (control is TextBox)
            {
                return (control as TextBox).Text;
            }
            else if (control is DropDownList)
            {
                return (control as DropDownList).Text;
            }
            return string.Empty;
        }

        public object GetValue(string key)
        {
            var dataType = _dataTypes[key];
            var value = GetText(key);
            if (dataType == FormDataType.String)
            {
                return value;
            }
            else if (dataType == FormDataType.Double)
            {
                return Convert.ToDouble(value);
            }
            else if (dataType == FormDataType.Int32)
            {
                return (int)Convert.ToDouble(value);
            }
            else if (dataType == FormDataType.DateTime)
            {
                return Convert.ToDateTime(value);
            }
            else if (dataType == FormDataType.Enum)
            {
                return value;
            }
            return null;
        }

        public void SetValue(string key, object value)
        {
            var control = _inputs[key];
            if (control is TextBox)
            {
                (control as TextBox).Text = value.ToString();
            }
            else if (control is DropDownList)
            {
                var list = control as DropDownList;
                list.SelectedIndex = list.Items.IndexOf(list.Items.FindByText(value.ToString()));
            }
        }
    }

    public static class ServerValidators
    {
        public static bool NecessaryFieldValidator(string text)
        {
            return !string.IsNullOrEmpty(text);
        }

        public static bool NumberValidator(string text)
        {
            double num;
            return double.TryParse(text, out num);
        }

        public static bool DateTimeValidator(string text)
        {
            DateTime dt;
            return DateTime.TryParse(text, out dt);
        }
    }
}
