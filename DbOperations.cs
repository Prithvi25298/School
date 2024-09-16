using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SchoolPortalV2.Operations
{
    public class DbOperations
    {
        // Retrieve connection string from your application configuration file (Web.config or App.config)
        string connectionString = ConfigurationManager.ConnectionStrings["SchoolDBConnectionString"].ConnectionString;
        public DataSet GetList(string spname)
        {
            DataSet ds = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(spname, connection);
                command.CommandType = CommandType.StoredProcedure;
                // Create a SqlDataAdapter to fetch data into the DataSet
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                // Open the connection
                connection.Open();
                // Fill the DataSet with data
                adapter.Fill(ds);
            }
            return ds;
        }
        public DataSet GetListWithParameters(string spname,List<SqlParameter> parms)
        {
            DataSet dataSet = new DataSet();
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(spname, connection))
                {
                    command.CommandType = CommandType.StoredProcedure; // or CommandType.Text if it’s a raw query

                    if (parms != null)
                    {
                        foreach (SqlParameter param in parms)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataSet);
                }
            }

            return dataSet;
        }
        public DataTable GetDetailsForEdit(string spname, List<SqlParameter> parameters)
        {
            // Connection string to your SQL Server database
            string connectionString = ConfigurationManager.ConnectionStrings["SchoolDBConnectionString"].ConnectionString;
            // Create a new DataTable to hold the result
            DataTable dataTable = new DataTable();
            try
            {
                // Create a new SqlConnection using the connection string
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Create a new SqlCommand
                    using (SqlCommand cmd = new SqlCommand(spname, connection))
                    {
                        // Set the command type to stored procedure
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        if (parameters != null)
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                cmd.Parameters.Add(parameter);
                            }
                        }

                        // Create a new SqlDataAdapter
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            // Fill the DataTable with the data from the stored procedure
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here, like logging or throwing
                Console.WriteLine("Error: " + ex.Message);
            }

            // Return the populated DataTable
            return dataTable;
        }
        public int SaveModelData<T>(T model, string storedProcedureName,out string ErrorMsg)
        {
            var properties = typeof(T).GetProperties();
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (var property in properties)
            {
                parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(model) ?? DBNull.Value));
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters.ToArray());

                // Add output parameters
                SqlParameter isErrorParameter = new SqlParameter("@IsError", SqlDbType.Int);
                isErrorParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(isErrorParameter);

                SqlParameter errorMsgParameter = new SqlParameter("@ErrorMsg", SqlDbType.VarChar, 100);
                errorMsgParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(errorMsgParameter);

                // Executing the stored procedure
                command.ExecuteNonQuery();
                int isError = Convert.ToInt32(isErrorParameter.Value);
                ErrorMsg = errorMsgParameter.Value.ToString();
                connection.Close();
                return isError;
            }           
        }

        public string RenderViewToString(string viewName, object model, ControllerContext controllerContext)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}