using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EduTrackWeb.BAL
{
    public class Admin : Utility
    {

        /// רושם משתמש חדש עם סיסמה מוצפנת ו־Salt, כולל RoleID

        public bool RegisterNewUser(string username, string password, string email, string fullName, int roleId)
        {

            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;
            string salt = GenerateSalt();
            string hash = HashPassword(password, salt);
            DateTime createdAt = DateTime.Now;

            using (SqlConnection conn = new SqlConnection(connStr))

            {
                string sql = @"INSERT INTO Users 
                        (Username, PasswordHash, Salt, Email, FullName, RoleID, CreatedAt) 
                       VALUES 
                        (@Username, @PasswordHash, @Salt, @Email, @FullName, @RoleID, @CreatedAt)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", hash);
                cmd.Parameters.AddWithValue("@Salt", salt);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@FullName", fullName); // ⬅️ עברית תעבוד כאן תקין
                cmd.Parameters.AddWithValue("@RoleID", roleId);
                cmd.Parameters.AddWithValue("@CreatedAt", createdAt);

                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }


        /// מחזיר את פרטי המשתמש לפי שם משתמש – כולל hash, salt ותפקיד

        public string[] GetUserByUsername(string username)
        {
            string sql = $"SELECT PasswordHash, Salt, RoleID, UserID, FullName, Email FROM Users WHERE Username = '{Check(username)}'";
            DataSet ds = GetDataSet(sql);
            return GetSingleRow(ds);
        }

        
        /// מחזיר את רשימת התפקידים מטבלת Roles
    
        public DataSet GetRoles()
        {
            string sql = "SELECT RoleID, RoleName FROM Roles ORDER BY RoleName";
            return GetDataSet(sql);
        }

        /// מחזיר את שם התפקיד לפי RoleID
    
        public string GetRoleName(int roleId)
        {
            string sql = $"SELECT RoleName FROM Roles WHERE RoleID = {roleId}";
            DataSet ds = GetDataSet(sql);
            string[] row = GetSingleRow(ds);
            return row != null ? row[0] : "";
        }
        /// בדיקה האם קיים שם משתמש 
        public bool IsUsernameExists(string username)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        /// בדיקה האם קיים שם כתובת דוא"ל  
        public bool IsEmailExists(string email)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        ///ביטול של משתמש   
        public bool DeactivateUser(int userId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }
        ///עדכון של משתמש 
        public bool UpdateUser(int userId, string fullName, string email, int roleId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"UPDATE Users
                       SET FullName = @FullName, Email = @Email, RoleID = @RoleID
                       WHERE UserID = @UserID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@RoleID", roleId);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }

        }
        public int GetExistingMajorId(string majorName)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT MajorID FROM Majors WHERE MajorName = @Name";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Name", majorName);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }


    }

}
