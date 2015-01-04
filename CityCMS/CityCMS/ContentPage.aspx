<%@ Page Title="" Language="C#" MasterPageFile="~/Public.Master" AutoEventWireup="true"
    CodeBehind="ContentPage.aspx.cs" Inherits="CityCMS.ContentPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="sidebar" runat="server" style="float: left; width: 200px">
    </div>
    <div id="content" runat="server" style="padding: 20px; margin-left: 200px">
    </div>
    <script type="text/javascript">
        var width = 600;
        $(document).ready(function () {
            // for IE
            $('img').each(function () {
                if (this.width > width) {
                    var ratio = this.width / this.height;
                    this.width = width;
                    this.height = this.width / ratio;
                }
            });
            // for Firefox/Chrome/Safari/Opera
            $('img').load(function () {
                if (this.width > width) {
                    var ratio = this.width / this.height;
                    this.width = width;
                    this.height = this.width / ratio;
                }
            });
        });

        $(document).ready(function () {
            $('#btnPost').click(function (event) {
                if ($('textarea').val().length < 6) {
                    event.preventDefault();
                    $('#btnPost').parent().append('<span style=\"color:red\">请认真填写评论。</span>');
                }
            });
        });
    </script>
</asp:Content>
