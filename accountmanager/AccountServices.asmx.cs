using System;
using System.Web.Services;
using System.Data;

// We need these to talk to MySQL
using MySql.Data.MySqlClient;

namespace accountmanager
{
    /// <summary>
    /// Summary description for AccountServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class AccountServices : System.Web.Services.WebService
    {
        [WebMethod]
        public int NumberOfAccounts()
        {
            string sqlConnectString =
                System.Configuration.ConfigurationManager
                .ConnectionStrings["myDB"]
                .ConnectionString;

            string sqlSelect = "SELECT * FROM users";

            MySqlConnection sqlConnection =
                new MySqlConnection(sqlConnectString);

            MySqlCommand sqlCommand =
                new MySqlCommand(sqlSelect, sqlConnection);

            MySqlDataAdapter sqlDa =
                new MySqlDataAdapter(sqlCommand);

            DataTable sqlDt = new DataTable();

            sqlDa.Fill(sqlDt);

            return sqlDt.Rows.Count;
        }

        [WebMethod]
        public bool LeaveGroup(string username, int groupId)
        {
            string sqlConnectString =
                System.Configuration.ConfigurationManager
                .ConnectionStrings["myDB"]
                .ConnectionString;

            string sqlDelete =
                @"DELETE FROM groupmembers
                  WHERE Username = @username
                  AND GroupID = @groupId";

            using (MySqlConnection sqlConnection =
                   new MySqlConnection(sqlConnectString))
            using (MySqlCommand sqlCommand =
                   new MySqlCommand(sqlDelete, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@username", username);
                sqlCommand.Parameters.AddWithValue("@groupId", groupId);

                sqlConnection.Open();

                int rowsAffected = sqlCommand.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
    }
}