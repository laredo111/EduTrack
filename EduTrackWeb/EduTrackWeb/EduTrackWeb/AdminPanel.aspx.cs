using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EduTrackWeb.BAL; // שימוש במחלקות Utility ו-Admin
namespace EduTrakWeb.Pages
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        #region page load
        protected void Page_Load(object sender, EventArgs e)
        {
            // בדיקה אם המשתמש מחובר ויש לו הרשאת אדמין (RoleID = 1)
            if (Session["roleId"] == null || Session["roleId"].ToString() != "1")
            {
                Response.Redirect("login.aspx");
                return;
            }

            // הטענה  רק בפעם הראשונה שטוענים את העמוד (ולא בכל לחיצה)
            if (!IsPostBack)
            {
                LoadRoles();
                LoadUsersList();
                LoadMajorsList(); // השורה החדשה
            }
        }

        /// טוען את טבלת התפקידים לתוך ה־DropDownList

        private void LoadRoles()
        {
            Admin admin = new Admin(); // יצירת מופע מהמחלקה Admin
            DataSet ds = admin.GetRoles(); // שליפת כל התפקידים מהמסד

            ddlRole.DataSource = ds; // חיבור הנתונים לרשימה
            ddlRole.DataTextField = "RoleName"; // מה יוצג למשתמש
            ddlRole.DataValueField = "RoleID"; // מה יישמר כערך
            ddlRole.DataBind(); // בניית הרשימה בפועל

            ddlRole.Items.Insert(0, new ListItem("בחר תפקיד", "0")); // שורה ראשונה ריקה
        }

        private void LoadUsersList()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT u.UserID, u.Username, u.FullName, u.Email, r.RoleName
                       FROM Users u
                       INNER JOIN Roles r ON u.RoleID = r.RoleID
                       WHERE u.IsActive = 1";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptUsers.DataSource = dt;
                rptUsers.DataBind();
            }
        }
        private void LoadMajorsList()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT MajorID, MajorName, Year, Description FROM Majors WHERE IsValid = 1";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptMajors.DataSource = dt;
                rptMajors.DataBind();
            }
        }

        #endregion
        #region Users
        /// פעולה שנקראת כשלוחצים על "צור משתמש"

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            int roleId = int.Parse(ddlRole.SelectedValue);
            if (roleId == 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "אנא בחר תפקיד תקף";
                return;
            }

            string password = txtPassword.Text;
            Admin admin = new Admin();

            // ✳️ אם מדובר בעדכון:
            if (ViewState["EditUserID"] != null)
            {
                int userId = (int)ViewState["EditUserID"];

                // עדכון פרטי המשתמש (ללא שינוי סיסמה)
                bool updated = admin.UpdateUser(
                    userId,
                    txtFullName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    roleId
                );

                if (updated)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = "המשתמש עודכן בהצלחה.";
                    ViewState["EditUserID"] = null;
                    btnRegister.Text = "צור משתמש";
                    LoadUsersList();
                }
                else
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "שגיאה בעדכון המשתמש.";
                }

                return;
            }

            // ✳️ אם מדובר ביצירת משתמש חדש:
            if (!Utility.IsValidPassword(password, out string passwordError))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = passwordError;
                return;
            }

            if (admin.IsUsernameExists(txtUsername.Text.Trim()))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "שם המשתמש כבר קיים במערכת.";
                return;
            }

            if (admin.IsEmailExists(txtEmail.Text.Trim()))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "כתובת האימייל כבר רשומה.";
                return;
            }

            bool success = admin.RegisterNewUser(
                txtUsername.Text.Trim(),
                txtPassword.Text,
                txtEmail.Text.Trim(),
                txtFullName.Text.Trim(),
                roleId
            );

            if (success)
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "משתמש נוצר בהצלחה!";
                LoadUsersList();
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "אירעה שגיאה ביצירת המשתמש. נסה שוב.";
            }
        }


        /// פעולה שנקראת כשלוחצים על מחק

        protected void DeleteUser_Command(object sender, CommandEventArgs e)
        {
            int userId = int.Parse(e.CommandArgument.ToString());
            Admin admin = new Admin();

            if (admin.DeactivateUser(userId))
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "המשתמש נמחק בהצלחה.";
                LoadUsersList(); // טען מחדש את הרשימה
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "שגיאה במחיקת המשתמש.";
            }
        }
        protected void EditUser_Command(object sender, CommandEventArgs e)
        {
            int userId = int.Parse(e.CommandArgument.ToString());
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT Username, FullName, Email, RoleID FROM Users WHERE UserID = @UserID AND IsActive = 1";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtUsername.Enabled = false;
                    txtFullName.Text = reader["FullName"].ToString();
                    txtEmail.Text = reader["Email"].ToString();
                    ddlRole.SelectedValue = reader["RoleID"].ToString();

                    // נשמור את ה־ID של המשתמש בעמוד (למשל ב־ViewState)
                    ViewState["EditUserID"] = userId;

                    // תוכל לשנות טקסט כפתור:
                    btnRegister.Text = "עדכן משתמש";
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "openSection", "toggleSection('userSection');", true);
            }

            // פתח את ה־DIV של הטופס
            ScriptManager.RegisterStartupScript(this, GetType(), "openUserForm", "toggleSection('userSection');", true);

        }
        #endregion
        #region Majors
        // כפתור לשמירת מגמה
        protected void btnSaveMajor_Click(object sender, EventArgs e)
        {
            string name = txtMajorName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (!int.TryParse(txtYear.Text, out int year))
            {
                lblMajorMessage.Text = "שנה לא תקינה.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd;

                if (ViewState["EditMajorID"] != null)
                {
                    // עדכון מגמה קיימת
                    int majorId = (int)ViewState["EditMajorID"];
                    cmd = new SqlCommand("UPDATE Majors SET MajorName=@Name, Year=@Year, Description=@Desc WHERE MajorID=@ID", conn);
                    cmd.Parameters.AddWithValue("@ID", majorId);

                    ViewState["EditMajorID"] = null;
                    btnSaveMajor.Text = "צור מגמה";
                }
                else
                {
                    Admin admin = new Admin();
                    int existingId = admin.GetExistingMajorId(name);

                    if (existingId != -1)
                    {
                        // שחזור מגמה קיימת שנמחקה
                        cmd = new SqlCommand("UPDATE Majors SET Year=@Year, Description=@Desc, IsValid=1 WHERE MajorID=@ID", conn);
                        cmd.Parameters.AddWithValue("@ID", existingId);
                    }
                    else
                    {
                        // יצירה רגילה
                        cmd = new SqlCommand("INSERT INTO Majors (MajorName, Year, Description) VALUES (@Name, @Year, @Desc)", conn);
                        cmd.Parameters.AddWithValue("@Name", name);
                    }
                    // יצירת מגמה חדשה
                  //  cmd = new SqlCommand("INSERT INTO Majors (MajorName, Year, Description) VALUES (@Name, @Year, @Desc)", conn);
                }

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Desc", description);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    lblMajorMessage.ForeColor = System.Drawing.Color.Green;
                    lblMajorMessage.Text = "הפעולה בוצעה בהצלחה!";

                    // איפוס השדות
                    txtMajorName.Text = "";
                    txtYear.Text = "";
                    txtDescription.Text = "";
                    btnSaveMajor.Text = "צור מגמה";

                }
                catch (SqlException ex)
                {
                    lblMajorMessage.ForeColor = System.Drawing.Color.Red;
                    lblMajorMessage.Text = "שגיאה: " + ex.Message;
                }

                LoadMajorsList();
            }
        }

        //לחצו עריכת מגמות
        protected void EditMajor_Command(object sender, CommandEventArgs e)
        {
            int majorId = int.Parse(e.CommandArgument.ToString());
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT MajorName, Year, Description FROM Majors WHERE MajorID = @ID AND IsValid = 1";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", majorId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtMajorName.Text = reader["MajorName"].ToString();
                    txtYear.Text = reader["Year"].ToString();
                    txtDescription.Text = reader["Description"].ToString();

                    ViewState["EditMajorID"] = majorId;
                    btnSaveMajor.Text = "עדכן מגמה";
                }
            }

            // הצגת אזור המגמות
            ScriptManager.RegisterStartupScript(this, GetType(), "openMajorsSection", "toggleSection('majorsSection');", true);
        }


        //לחצן ביטול מגמה 
        protected void DeleteMajor_Command(object sender, CommandEventArgs e)
        {
            int majorId = int.Parse(e.CommandArgument.ToString());
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Majors SET IsValid = 0 WHERE MajorID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", majorId);

                conn.Open();
                cmd.ExecuteNonQuery();
                LoadMajorsList();
            }
        }
        #endregion

    }
}