using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestPOS.DBContext.Services
{
    public class DBHelper
    {
        //http://goldenoil.dyndns-office.com:818/API/values/?whid=1
        private static SqlConnection getConnectionString() // Should be gotten from config in secure storage.
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "it.hurts.when.IP";
            builder.UserID = "someDBUser";
            builder.Password = "someDBPassword";
            builder.InitialCatalog = "someDB";
            return new SqlConnection(builder.ConnectionString);
        }

        public static Uri GetUriConnection()
        {
            return new Uri("http://goldenoil.dyndns-office.com:818/API/");
        }

        public static async Task<DataTable> GetInfoFromApi()
        {
            // Initialization.  
            DataTable responseObj = new DataTable();

            // HTTP GET.  
            using (var client = new HttpClient())
            {
                // Setting Base address.  
                client.BaseAddress = GetUriConnection();

                // Setting content type.  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Initialization.  
                HttpResponseMessage response = new HttpResponseMessage();

                // HTTP GET  
                response = await client.GetAsync("values/?whid=1").ConfigureAwait(false);

                // Verification  
                if (response.IsSuccessStatusCode)
                {
                    // Reading Response.  
                    string result = response.Content.ReadAsStringAsync().Result;
                    //responseObj = JsonConvert.DeserializeObject<DataTable>(result);
                }
            }

            return responseObj;
        }
         public static List<T> GetData<T>()
        {
            List<T> list = new List<T>();
            var data = GetInfoFromApi();
            var retVal = ConvertToList<T>(data.Result);

            return list;
        }

        public static List<T> ExecuteSP<T>(string SPName, List<SqlParameter> Params)
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (SqlConnection Connection = getConnectionString())
                {
                    // Open connection
                    Connection.Open();

                    // Create command from params / SP
                    SqlCommand cmd = new SqlCommand(SPName, Connection);

                    // Add parameters
                    cmd.Parameters.AddRange(Params.ToArray());
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Make datatable for conversion
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                    da.Dispose();

                    // Close connection
                    Connection.Close();
                }

                // Convert to list of T
                var retVal = ConvertToList<T>(dataTable);
                return retVal;
            }
            catch (SqlException e)
            {
                Console.WriteLine("ConvertToList Exception: " + e.ToString());
                return new List<T>();
            }
        }

        /// <summary>
        /// Converts datatable to List<someType> if possible.
        /// </summary>
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            try // Necesarry unfotunately.
            {
                var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();

                var properties = typeof(T).GetProperties();

                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();

                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name))
                        {
                            if (row[pro.Name].GetType() == typeof(System.DBNull)) pro.SetValue(objT, null, null);
                            else pro.SetValue(objT, row[pro.Name], null);
                        }
                    }

                    return objT;
                }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to write data to list. Often this occurs due to type errors (DBNull, nullables), changes in SP's used or wrongly formatted SP output.");
                Console.WriteLine("ConvertToList Exception: " + e.ToString());
                return new List<T>();
            }
        }
    }
}
