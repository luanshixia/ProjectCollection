<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ContentEdit.aspx.cs" Inherits="CityCMS.Admin.ContentEdit" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--    <script type="text/javascript">
        var msg_unload = "请确认您的文章已经保存。";
        var UnloadConfirm = {};
        UnloadConfirm.set = function (confirm_msg) {
            window.onbeforeunload = function (event) {
                event = event || window.event;
                event.returnValue = confirm_msg;
            }
        }

        UnloadConfirm.clear = function () {
            window.onbeforeunload = function () { };
        }

        UnloadConfirm.set(msg_unload);
        //document.getElementsByTagName('form').item(0).setAttribute('onsubmit', 'UnloadConfirm.clear()');
    </script>--%>
    <%--    <script type="text/javascript">
        function previewContent() {
            $('#content_preview').html($('#txtContent').val());
        }

        $(document).ready(function () {
            previewContent();
        });
    </script>
    --%>
    <script type="text/javascript" src="../ckeditor/ckeditor.js"></script>
    <h2>
        内容管理 - 编辑文章
    </h2>
    <p>
        允许您编辑网站的栏目设置，并在相应栏目中发表文章。
    </p>
    <p>
        标题
        <asp:TextBox ID="txtTitle" runat="server" Width="685px"></asp:TextBox>
    </p>
    <p>
        作者
        <asp:TextBox ID="txtAuthor" runat="server" Width="685px"></asp:TextBox>
    </p>
    <p>
        栏目
        <asp:DropDownList ID="selectChannel" runat="server" Width="164px">
        </asp:DropDownList>
        <asp:CheckBox ID="cbFixToTop" Text="在栏目中置顶" runat="server" />
        <asp:CheckBox ID="cbAllowComment" Text="允许评论" runat="server" />
        <asp:Button ID="btnDel" runat="server" Text="删除文章" OnClick="btnDel_Click" OnClientClick="if(confirm('确实要删除这篇文章吗？')){document.getElementById('MainContent_hidden1').value = 'del';}" />
    </p>
    <div>
        <asp:TextBox ID="txtContent" runat="server" Width="710px" Height="700px" TextMode="MultiLine"
            ClientIDMode="Static" Style="border: 1px solid gray; font-size: 12px; font-family: Cosolas" rows="20" class="ckeditor"></asp:TextBox>
    </div>
    <%--    <div id="content_preview" style="width: 450px; height: 500px; overflow: scroll; border: 1px solid gray"
        contenteditable="true">
    </div>
    --%>
    <div>
        <asp:Button ID="btnSaveAndBack" runat="server" Text="保存并返回" OnClick="btnSaveAndBack_Click" />
        <asp:Button ID="btnSave" runat="server" Text="保存" OnClick="btnSave_Click" />
        <asp:Button ID="btnBack" runat="server" Text="返回" OnClick="btnBack_Click" />
        <input id="hidden1" type="hidden" runat="server" />
    </div>
    <div class="block" style="padding: 10px">
        <p>
            从磁盘选择文件以上传并作为附件。</p>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="btnUpload" runat="server" Text="上传" OnClick="btnUpload_Click" /><br />
    </div>
    <div id="root" runat="server">
    </div>
</asp:Content>
