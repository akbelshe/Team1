using System;
using System.Data;
using System.Web.Services;
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
        public bool CreateStudyGroup(
            string groupName,
            string course,
            string section,
            int createdBy)
        {
            string sqlConnectString =
                System.Configuration.ConfigurationManager
                .ConnectionStrings["myDB"]
                .ConnectionString;

            string sqlInsert =
                @"INSERT INTO studygroups
                  (GroupName, Course, Section, CreatedBy)
                  VALUES (@groupName, @course, @section, @createdBy)";

            using (MySqlConnection sqlConnection =
                   new MySqlConnection(sqlConnectString))
            using (MySqlCommand sqlCommand =
                   new MySqlCommand(sqlInsert, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@groupName", groupName);
                sqlCommand.Parameters.AddWithValue("@course", course);
                sqlCommand.Parameters.AddWithValue("@section", section);
                sqlCommand.Parameters.AddWithValue("@createdBy", createdBy);

                try
                {
                    sqlConnection.Open();

                    int rowsAffected =
                        sqlCommand.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
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
                sqlCommand.Parameters.AddWithValue(
                    "@username",
                    username);

                sqlCommand.Parameters.AddWithValue(
                    "@groupId",
                    groupId);

                sqlConnection.Open();

                int rowsAffected =
                    sqlCommand.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        [WebMethod]
        public bool PostGroupMessage(
            int groupId,
            string username,
            string messageText)
        {
            string sqlConnectString =
                System.Configuration.ConfigurationManager
                .ConnectionStrings["myDB"]
                .ConnectionString;

            string sqlInsert =
                @"INSERT INTO groupmessages
                  (GroupID, Username, MessageText)
                  VALUES (@groupId, @username, @messageText)";

            using (MySqlConnection sqlConnection =
                   new MySqlConnection(sqlConnectString))
            using (MySqlCommand sqlCommand =
                   new MySqlCommand(sqlInsert, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue(
                    "@groupId",
                    groupId);

                sqlCommand.Parameters.AddWithValue(
                    "@username",
                    username);

                sqlCommand.Parameters.AddWithValue(
                    "@messageText",
                    messageText);

                sqlConnection.Open();

                int rowsAffected =
                    sqlCommand.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        [WebMethod]
        public DataTable GetGroupMessages(int groupId)
        {
            string sqlConnectString =
                System.Configuration.ConfigurationManager
                .ConnectionStrings["myDB"]
                .ConnectionString;

            string sqlSelect =
                @"SELECT MessageID,
                         GroupID,
                         Username,
                         MessageText,
                         CreatedDate
                  FROM groupmessages
                  WHERE GroupID = @groupId
                  ORDER BY CreatedDate ASC";

            using (MySqlConnection sqlConnection =
                   new MySqlConnection(sqlConnectString))
            using (MySqlCommand sqlCommand =
                   new MySqlCommand(sqlSelect, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue(
                    "@groupId",
                    groupId);

                MySqlDataAdapter sqlDa =
                    new MySqlDataAdapter(sqlCommand);

                DataTable sqlDt =
                    new DataTable("GroupMessages");

                sqlDa.Fill(sqlDt);

                return sqlDt;
            }
        }
    }
}