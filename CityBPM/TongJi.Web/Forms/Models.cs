using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace TongJi.Web.Forms
{
    public class Field
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "字段名称")]
        public string Name { get; set; }
        [Display(Name = "显示名称")]
        public string DisplayName { get; set; }
        [Display(Name = "字段描述")]
        public string Description { get; set; }
        [Display(Name = "数据类型")]
        public string DataType { get; set; }
        [Display(Name = "选项字典")]
        public string Options { get; set; }
        [Display(Name = "验证规则")]
        public string Validation { get; set; }
        [AllowHtml]
        [Display(Name = "字段样式")]
        public string Style { get; set; }
        [Display(Name = "字段顺序")]
        public int Order { get; set; }
        public Guid FormID { get; set; }
    }

    public class Form
    {
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "表单名称")]
        public string Name { get; set; }
        [Display(Name = "呈现方式")]
        public int DisplayType { get; set; }
        [AllowHtml]
        [Display(Name = "表单布局")]
        public string Layout { get; set; }
        [AllowHtml]
        [Display(Name = "表单样式")]
        public string Style { get; set; }
        public string Type { get; set; }
        public string Group { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public List<Field> Fields { get; set; }
    }

    public class Layout // newly 20130703
    {
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "布局名称")]
        public string Name { get; set; }
        [AllowHtml]
        [Display(Name = "布局模板")]
        public string Markup { get; set; }
        [Display(Name = "布局样式")]
        public string Style { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class FlowNodeLayout // newly 2013703
    {
        public int ID { get; set; }
        //public int FlowNodeID { get; set; }
        public Guid FlowNodeID { get; set; }
        public Guid LayoutID { get; set; }
    }

    public class FormInstance
    {
        public int ID { get; set; }
        public Guid FormID { get; set; }
        public string Data { get; set; }
        public DateTime PostTime { get; set; }
        public string Username { get; set; }
        public int FlowInstanceID { get; set; } // newly 20130607 一个表单定义可以绑定到多个流程，然而一个表单实例只能从属于一个流程实例。
    }

    public class WorkflowForm
    {
        public int ID { get; set; }
        public Guid WorkflowID { get; set; }
        public Guid FormID { get; set; }
    }
}
