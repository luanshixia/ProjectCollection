<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ChannelEdit.aspx.cs" Inherits="CityCMS.Admin.ChannelEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        内容管理
    </h2>
    <p>
        允许您编辑网站的栏目设置，并在相应栏目中发表文章。
    </p>
    <p>
        在文本框中输入并单击确定以更改栏目名称等，或者单击删除栏目以删除当前栏目。<br />
        当且仅当栏目下没有子栏目和文章时，才能删除栏目。
    </p>
    <p>
        栏目名称
        <asp:TextBox ID="txtChannelName" runat="server"></asp:TextBox>
    </p>
    <p>
        同级次序
        <asp:TextBox ID="txtBrotherIndex" runat="server"></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtBrotherIndex"
            ValidationExpression="[0-9]" ForeColor="Red" ErrorMessage="输入非负整数"></asp:RegularExpressionValidator>
    </p>
    <p>
        父级栏目
        <asp:DropDownList ID="selectFather" runat="server" Width="200">
        </asp:DropDownList>
    </p>
    <p>
        <%--        <asp:CheckBox ID="cbHasMain" Text="显示最新文章而不是文章列表" runat="server" /><br/>
        --%>
        显示模式
        <asp:DropDownList ID="selectDisplay" runat="server" Width="200">
            <asp:ListItem>文章列表</asp:ListItem>
            <asp:ListItem>最新文章</asp:ListItem>
            <asp:ListItem>子栏目列表</asp:ListItem>
            <asp:ListItem>子栏目文章列表（合并）</asp:ListItem>
            <asp:ListItem>子栏目文章列表（分列）</asp:ListItem>
            <asp:ListItem>链接</asp:ListItem>
            <asp:ListItem>加载网页</asp:ListItem>
            <asp:ListItem>博客式</asp:ListItem>
        </asp:DropDownList>
    </p>
    <p>
        链接地址
        <asp:TextBox ID="txtLink" runat="server" Width="200"></asp:TextBox>
        （针对“链接”和“加载页面”两个选项）
    </p>
    <p>
        <asp:Button ID="btnOk" runat="server" Text="确定" OnClick="btnOk_Click" />
        <asp:Button ID="btnDel" runat="server" Text="删除栏目" OnClick="btnDel_Click" />
        <asp:Button ID="btnBack" runat="server" Text="返回" OnClick="btnBack_Click" />
    </p>
    <div id="root" runat="server">
    </div>
</asp:Content>
