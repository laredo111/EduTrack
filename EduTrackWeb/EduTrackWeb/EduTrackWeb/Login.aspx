<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="EduTrackWeb.Login" Title="התחברות" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mx-auto mt-5" style="max-width: 400px;">
        <h2 class="text-center">התחברות למערכת</h2>

        <div class="mb-3">
            <asp:Label ID="lblUsername" runat="server" Text="שם משתמש:"></asp:Label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <asp:Label ID="lblPassword" runat="server" Text="סיסמה:"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="התחבר" OnClick="btnLogin_Click" CssClass="btn btn-primary w-100" />

        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="d-block mt-3 text-center"></asp:Label>
    </div>
</asp:Content>