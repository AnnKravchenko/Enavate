using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace WebAPI
{
    class SQL : IDisposable
    {
        public SQL()
        {
            string connectionString = @"Data Source=PC-AKRAVCHENKO2;Initial Catalog = accounts;Integrated Security=SSPI;";
            this.Connection = new SqlConnection(connectionString);
        }
        public void Dispose()
        {
            this.Connection.Close();
            Console.WriteLine("Connection Closed");
        }
        public SqlConnection Connection { get; set; }
        public JArray ConnectionData
        {
            get
            {
                SqlConnection cnn = this.Connection;
                JArray accs = new JArray();
                try
                {
                    
                    cnn.Open();
                    Console.WriteLine("Connection Open ! ");
                    Console.WriteLine(cnn.Database);
                    using (SqlCommand command = new SqlCommand("SELECT  * FROM CLients", cnn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                JObject acc = new JObject { };
                                if (reader["accountid"].ToString() != "")
                                {
                                    acc.Add("accountid", reader["accountid"].ToString());
                                }
                                if (reader["new_cash"].ToString() != "")
                                {
                                    acc.Add("new_cash", Convert.ToDouble(reader["new_cash"]));
                                }
                                if (reader["accountname"].ToString() != "")
                                {
                                    acc.Add("name", reader["accountname"].ToString());
                                }
                                if (reader["emailaddress1"].ToString() != "")
                                {
                                    acc.Add("emailaddress1", reader["emailaddress1"].ToString());
                                }
                                accs.Add(acc);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ConnectionData Exception: " + ex.Message);
                    Console.ReadLine();
                }
                return accs;
            }
        }
        public void UpdateSqlRecord(JObject acc)
        {
            List<string> filters = new List<string>();
            SqlConnection cnn = this.Connection;
            try
            {
                using (SqlCommand command = cnn.CreateCommand())
                {
                    //command.CommandText = "update accounts.dbo.Clients set accountid=@id where ";
                    command.Parameters.AddWithValue("@id", acc["accountid"].ToString());
                    if (acc["name"].ToString()!="")
                    {
                        command.Parameters.AddWithValue("@name", acc["name"].ToString());
                        filters.Add("accountname=@name");
                    }
                    if (acc["new_cash"].ToString() != "")
                    {
                        Console.WriteLine("here");
                        command.Parameters.AddWithValue("@nc", Convert.ToDouble(acc["new_cash"]));
                        filters.Add("new_cash=@nc");
                    }
                    if (acc["emailaddress1"].ToString() != "")
                    {
                        command.Parameters.AddWithValue("@nc", acc["emailaddress1"].ToString());
                        filters.Add("emailaddress1=@em");
                    }
                    string filter = String.Join(" and ", filters);
                    command.CommandText= "update accounts.dbo.Clients set accountid=@id where "+filter;
                    Console.WriteLine(command.CommandText);
                    command.ExecuteNonQuery();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateSqlRecord Exception: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
