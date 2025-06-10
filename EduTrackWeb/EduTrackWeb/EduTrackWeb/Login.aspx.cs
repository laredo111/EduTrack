using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EduTrackWeb.BAL;

namespace EduTrackWeb
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string enteredPassword = txtPassword.Text.Trim();

            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_LoginUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string hash = reader["PasswordHash"].ToString();
                    string salt = reader["Salt"].ToString();
                    string fullName = reader["FullName"].ToString();
                    string roleName = reader["RoleName"].ToString();
                    int roleId = Convert.ToInt32(reader["RoleID"]);
                    int userId = Convert.ToInt32(reader["UserID"]);
                    string email = reader["Email"].ToString();

                    reader.Close();

                    Admin admin = new Admin();
                    bool valid = admin.VerifyPassword(enteredPassword, hash, salt);

                    if (valid)
                    {
                        Session["Username"] = username;
                        Session["FullName"] = fullName;
                        Session["Role"] = roleName;
                        Session["RoleID"] = roleId;
                        Session["UserID"] = userId;
                        Session["Email"] = email;

                        // ניווט לפי תפקיד (if-else)
                        if (roleName == "Admin")
                        {
                            Response.Redirect("AdminPanel.aspx");
                        }
                        else if (roleName == "Teacher")
                        {
                            Response.Redirect("TeacherDashboard.aspx");
                        }
                        else if (roleName == "Student")
                        {
                            Response.Redirect("StudentDashboard.aspx");
                        }
                        else
                        {
                            lblMessage.Text = "תפקיד לא מזוהה.";
                        }
                    }
                    else
                    {
                        lblMessage.Text = "סיסמה שגויה.";
                    }
                }
                else
                {
                    lblMessage.Text = "שם משתמש לא נמצא.";
                }
            }
        }
    }
}
