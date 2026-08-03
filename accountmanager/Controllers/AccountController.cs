using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace accountmanager.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public ActionResult Register(
            string username,
            string password,
            string firstName,
            string lastName,
            string email)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(email))
            {
                Response.StatusCode = 400;
                return Content(
                    "Username, password, first name, and email are required."
                );
            }

            string connectionString =
                ConfigurationManager
                    .ConnectionStrings["myDB"]
                    .ConnectionString;

            try
            {
                using (MySqlConnection connection =
                       new MySqlConnection(connectionString))
                {
                    connection.Open();

                    const string checkSql = @"
                        SELECT COUNT(*)
                        FROM users
                        WHERE Username = @Username;";

                    using (MySqlCommand checkCommand =
                           new MySqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue(
                            "@Username",
                            username.Trim()
                        );

                        checkCommand.Parameters.AddWithValue(
                            "@Email",
                            email.Trim()
                        );

                        int existingUsers =
                            Convert.ToInt32(
                                checkCommand.ExecuteScalar()
                            );

                        if (existingUsers > 0)
                        {
                            Response.StatusCode = 409;

                            return Content(
                                "That username or email already exists."
                            );
                        }
                    }

                    const string insertSql = @"
                        INSERT INTO users
                        (
                            Username,
                            Email,
                            FirstName,
                            LastName,
                            Password
                        )
                        VALUES
                        (
                            @Username,
                            @Email,
                            @FirstName,
                            @LastName,
                            @Password
                        );";

                    using (MySqlCommand insertCommand =
                           new MySqlCommand(insertSql, connection))
                    {
                        insertCommand.Parameters.AddWithValue(
                            "@Username",
                            username.Trim()
                        );

                        insertCommand.Parameters.AddWithValue(
                            "@Email",
                            email.Trim()
                        );

                        insertCommand.Parameters.AddWithValue(
                            "@FirstName",
                            firstName.Trim()
                        );

                        insertCommand.Parameters.AddWithValue(
                            "@LastName",
                            string.IsNullOrWhiteSpace(lastName)
                                ? (object)DBNull.Value
                                : lastName.Trim()
                        );

                        insertCommand.Parameters.AddWithValue(
                            "@Password",
                            password
                        );

                        int rowsInserted =
                            insertCommand.ExecuteNonQuery();

                        if (rowsInserted != 1)
                        {
                            Response.StatusCode = 500;
                            return Content("The account was not created.");
                        }
                    }
                }

                return Content("User created successfully.");
            }
            catch (MySqlException ex)
            {
                Response.StatusCode = 500;
                return Content("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("Server error: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                Response.StatusCode = 400;
                return Content("Username and password are required.");
            }

            string connectionString =
                ConfigurationManager
                    .ConnectionStrings["myDB"]
                    .ConnectionString;

            try
            {
                using (MySqlConnection connection =
                       new MySqlConnection(connectionString))
                {
                    connection.Open();

                    const string sql = @"
                        SELECT Username
                        FROM users
                        WHERE Username = @Username
                          AND Password = @Password
                        LIMIT 1;";

                    using (MySqlCommand command =
                           new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Username",
                            username.Trim()
                        );

                        command.Parameters.AddWithValue(
                            "@Password",
                            password
                        );

                        object result = command.ExecuteScalar();

                        if (result == null)
                        {
                            Response.StatusCode = 401;
                            return Content(
                                "Incorrect username or password."
                            );
                        }

                        Session["Username"] = result.ToString();

                        return Content("Login successful.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Response.StatusCode = 500;
                return Content("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("Server error: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return Content("Logout successful.");
        }

        [HttpGet]
        public ActionResult CurrentUser()
        {
            if (Session["Username"] == null)
            {
                Response.StatusCode = 401;
                return Content("Not logged in.");
            }

            return Content(Session["Username"].ToString());
        }
    }
}