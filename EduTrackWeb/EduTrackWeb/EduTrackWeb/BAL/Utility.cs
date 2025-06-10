using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace EduTrackWeb.BAL
{
    public class Utility
    {
        string MainConnection = ConfigurationManager.ConnectionStrings["EduTrackConnection"].ConnectionString;


        #region פונקציות אבטחה

        public string GenerateSalt()
        {
            var saltBytes = new byte[16];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(bytes);
            }
        }

        public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            string hashToCheck = HashPassword(enteredPassword, storedSalt);
            return hashToCheck == storedHash;
        }

        #endregion

        #region פונקציות עזר למסד נתונים

        public string Check(string str)
        {
            if (str == null) return "";
            return str.Replace("'", "''").Trim();
        }

        public int ExecuteSql(string sql)
        {
            using (SqlConnection con = new SqlConnection(MainConnection))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public DataSet GetDataSet(string sql)
        {
            using (SqlConnection con = new SqlConnection(MainConnection))
            using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        public string[] GetSingleRow(DataSet ds)
        {
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];
            string[] values = new string[ds.Tables[0].Columns.Count];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = row[i].ToString();
            }

            return values;
        }

        #endregion

        #region Stored Procedure Support
        public DataSet RunStoredProcedure(string spName, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(MainConnection))
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
        }
        #endregion
        #region Validations

        public static bool IsValidPassword(string password, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "יש להזין סיסמה.";
                return false;
            }

            if (password.Length < 8)
            {
                errorMessage = "הסיסמה חייבת להכיל לפחות 8 תווים.";
                return false;
            }

            // בעתיד אפשר להוסיף תנאים נוספים כאן

            errorMessage = null;
            return true;
        }
    }
    #endregion

}

