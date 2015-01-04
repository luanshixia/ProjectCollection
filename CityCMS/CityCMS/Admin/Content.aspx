<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Content.aspx.cs" Inherits="CityCMS.Admin.Content" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        内容管理 - <asp:Label ID="lblId" runat="server" /> - 文章列表
    </h2>
    <p>
        允许您编辑网站的栏目设置，并在相应栏目中发表文章。
    </p>
    <div id="root" runat="server">
    </div>
</asp:Content>
