<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="AdminPanel.aspx.cs" Inherits="EduTrakWeb.Pages.AdminPanel" Title="לוח ניהול" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="text-center">לוח ניהול</h2>

    <!-- 🟢 הודעות מערכת בראש הדף -->
    <asp:Label ID="lblMessage" runat="server" CssClass="alert-message" EnableViewState="false" />
    <asp:Label ID="lblMajorMessage" runat="server" CssClass="alert-message" EnableViewState="false" />

    <!-- ניהול משתמשים -->
    <div>
        <button type="button" class="toggle-button" onclick="toggleSection('usersListSection')">+ הצג רשימת משתמשים</button>
    </div>

    <div id="usersListSection" class="section mt-3">
        <asp:Repeater ID="rptUsers" runat="server">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>שם משתמש</th>
                            <th>שם מלא</th>
                            <th>אימייל</th>
                            <th>תפקיד</th>
                            <th>מחיקה</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Username") %></td>
                    <td><%# Eval("FullName") %></td>
                    <td><%# Eval("Email") %></td>
                    <td><%# Eval("RoleName") %></td>
                    <td>
                        <asp:Button ID="btnDelete" runat="server" Text="🗑️" CommandName="Delete" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-danger btn-sm" OnCommand="DeleteUser_Command" />
                        <asp:Button ID="btnEdit" runat="server" Text="✏️" CommandName="Edit" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-warning btn-sm me-1" OnCommand="EditUser_Command" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>

    <button type="button" class="toggle-button" onclick="toggleSection('userSection')">+ הוספת משתמש חדש</button>

    <div id="userSection" class="section mt-3">
        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control mb-2" Placeholder="שם משתמש" />
        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control mb-2" TextMode="Password" Placeholder="סיסמה" />
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control mb-2" Placeholder="אימייל" />
        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control mb-2" Placeholder="שם מלא" />
        <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control mb-2" />
        <asp:Button ID="btnRegister" runat="server" Text="צור משתמש" CssClass="btn btn-primary" OnClick="btnRegister_Click" />
    </div>

    <!-- ניהול מגמות -->
    <div>
        <button type="button" class="toggle-button" onclick="toggleSection('majorsSection')">+ ניהול מגמות</button>
    </div>

    <div id="majorsSection" class="section mt-3">
        <h4>ניהול מגמות</h4>

        <asp:TextBox ID="txtMajorName" runat="server" CssClass="form-control mb-2" Placeholder="שם מגמה" />
        <asp:TextBox ID="txtYear" runat="server" CssClass="form-control mb-2" Placeholder="משך בשנים" TextMode="Number" />
        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control mb-2" Placeholder="תיאור" />

        <asp:Button ID="btnSaveMajor" runat="server" Text="צור מגמה" CssClass="btn btn-primary mb-3" OnClick="btnSaveMajor_Click" />

        <asp:Repeater ID="rptMajors" runat="server">
            <HeaderTemplate>
                <table class="table table-bordered mt-3">
                    <thead>
                        <tr>
                            <th>שם מגמה</th>
                            <th>שנים</th>
                            <th>תיאור</th>
                            <th>פעולות</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("MajorName") %></td>
                    <td><%# Eval("Year") %></td>
                    <td><%# Eval("Description") %></td>
                    <td>
                        <asp:Button runat="server" Text="✏️" CommandName="Edit" CommandArgument='<%# Eval("MajorID") %>' CssClass="btn btn-warning btn-sm me-1" OnCommand="EditMajor_Command" />
                        <asp:Button runat="server" Text="🗑️" CommandName="Delete" CommandArgument='<%# Eval("MajorID") %>' CssClass="btn btn-danger btn-sm" OnCommand="DeleteMajor_Command" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>

    <script type="text/javascript">
        function toggleSection(id) {
            var section = document.getElementById(id);
            section.style.display = (section.style.display === "none") ? "block" : "none";
        }
        window.onload = function () {
            // מצא את כל ההודעות
            var alerts = document.getElementsByClassName('alert-message');
            for (var i = 0; i < alerts.length; i++) {
                if (alerts[i].innerText.trim() !== "") {
                    (function (alertElement) {
                        setTimeout(function () {
                            alertElement.style.display = 'none';
                        }, 5000); // 5000 מילישניות = 5 שניות
                    })(alerts[i]);
                }
            }
        };
    </script>
</asp:Content>
