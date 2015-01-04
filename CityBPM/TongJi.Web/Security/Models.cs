using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//
// =================================================
// TongJi.Web.Security 通用权限管理体系
// =================================================
//
// 在此之前，所有系统大体采用一种简易的专用权限体系：
// 用一个表存储以下形式的配置记录：
// [角色] - [能否进行/获取] - [某个操作/资源]
//
// 而在 RBAC 中，需要明确：
// 三个实体：User|Role|Resource
// 两个关系：UserRole|RoleResource
//
// 又基于资源-动作分离原则，
// 我们把 Resource 拆分为 Resource 和 Action
// 权限配置即是 RoleAction 关系，我们称为 Access
//

namespace TongJi.Web.Security
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? CreateTime { get; set; }
        public bool? IsForbidden { get; set; }

        [Display(Name = "真实姓名")]
        public string RealName { get; set; }
        [Display(Name = "性别")]
        public string Gender { get; set; }
        [Display(Name = "出生日期")]
        public DateTime? Birthday { get; set; }
        [Display(Name = "电话号码")]
        public string Phone { get; set; }
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
        [Display(Name = "通信地址")]
        public string Address { get; set; }
        [Display(Name = "兴趣爱好")]
        public string Hobby { get; set; }
        [Display(Name = "自我评价")]
        public string PersonalStatement { get; set; }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(RealName) ? UserName : RealName;
            }
        }
    }

    // 1 - RBAC 相关
    //
    // 借用 WebSecurity 中的以下实体和关系
    // User
    // Role
    // UserRole
    //

    public class Resource
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class Action
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ResourceID { get; set; }
    }

    public class Access
    {
        public int ID { get; set; }
        public string Role { get; set; }
        public int ActionID { get; set; }
        public bool IsNegative { get; set; }
    }

    // 2 - 组织结构相关

    //public class Person // mod 20130605
    //{
    //    public int ID { get; set; }
    //    public string Username { get; set; } // 关联 WebSecurity 用户名
    //    public string Realname { get; set; }
    //    public string Gender { get; set; }
    //    public DateTime Birthday { get; set; }
    //    public string Phone { get; set; }
    //    public string Email { get; set; }
    //    public string Address { get; set; }
    //    public string Hobby { get; set; }
    //    public string PersonalStatement { get; set; }
    //    //public List<Position> Positions { get; set; }
    //}

    public class Department
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "描述")]
        public string Description { get; set; }
        [Display(Name = "上级")]
        public int ParentID { get; set; }
    }

    //[Obsolete]
    //public class Position
    //{
    //    public int ID { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public int DepartmentID { get; set; }
    //    public List<Person> Persons { get; set; }
    //}

    public class Position // newly 20130605
    {
        public int ID { get; set; }
        [Display(Name = "用户")]
        public string Username { get; set; }
        [Display(Name = "部门")]
        public int DepartmentID { get; set; }
        [Required]
        [Display(Name = "职位")]
        public string Title { get; set; }
        [Display(Name = "是否负责人")]
        public bool IsInCharge { get; set; }
    }

    public class PositionViewModel
    {
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public bool IsInCharge { get; set; }
    }

    // 3 - 组织结构的另一种实现形式：用户组模式

    //public class PersonGroup
    //{
    //    public int ID { get; set; }
    //    public string Name { get; set; }
    //    public int ParentID { get; set; }
    //    public string GroupType { get; set; } // 组是部门还是职位
    //}
}