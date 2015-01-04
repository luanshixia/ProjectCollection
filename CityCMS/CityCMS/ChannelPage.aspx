<%@ Page Title="" Language="C#" MasterPageFile="~/Public.Master" AutoEventWireup="true"
    CodeBehind="ChannelPage.aspx.cs" Inherits="CityCMS.ChannelPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function loadPage(pageUrl) {
            $('#content').fadeOut('fast');
            $.ajaxSetup({ cache: false }); // 禁止IE缓存
            $('#content').load(pageUrl, function () {
                $('#content').fadeIn('fast');
            });
        }
    </script>
    <div id="sidebar" runat="server" style="float: left; width: 200px">
    </div>
    <div id="content" runat="server" style="padding: 20px; margin-left: 200px" clientidmode="Static">
    </div>
</asp:Content>
