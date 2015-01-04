<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="CityCMS.Admin.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        内容管理
    </h2>
    <p>
        允许您编辑网站的栏目设置，并在相应栏目中发表文章。
    </p>
    <div id="root" runat="server">
    </div>
</asp:Content>
