<%@ Page Title="关于我们" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="CityCMS.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        关于
    </h2>
    <div style="width: 500px">
        <p>
            本站点是使用 CityCMS 内容管理系统搭建的。<br />
            <span style="font-size: 11px; font-family: Tahoma; color: #999999">This site is built
                on top of CityCMS Content Management System.</span>
        </p>
        <p>
            CityCMS 是一个轻量、便捷、先进的内容管理系统，基于微软 .NET 平台，全面引入先进的快速 Web 开发框架 Microsoft ASP.NET Web
            Pages 2.0。您也可以选择传统的 Microsoft ASP.NET Web Forms 4.0 进行开发。<br />
            <span style="font-size: 11px; font-family: Tahoma; color: #999999">CityCMS is a light-weighted,
                rapid, and modern CMS based on Microsoft .NET platform, and takes full advantages
                of Microsoft ASP.NET Web Pages 2.0, a modern rapid web development framework. You
                can also choose a more traditional development style by using ASP.NET Web Forms
                4.0. </span>
        </p>
        <p>
            <a href="Admin">进入后台</a></p>
    </div>
</asp:Content>
