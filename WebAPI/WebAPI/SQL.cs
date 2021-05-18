using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace WebAPI
{
    class SQL
    {
        public SQL()
        {
            string connectionString = @"Data Source=PC-AKRAVCHENKO2;Initial Catalog = accounts;Integrated Security=SSPI;";
            this.Connection = new SqlConnection(connectionString);
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
                                JObject acc = new JObject
                                {
                                    { "name",  reader["accountname"].ToString() },
                                    { "new_cash", reader["new_cash"].ToString() },
                                    { "emailaddress1", reader["emailaddress1"].ToString() }
                                    //{ "parentaccountid@odata.bind", "/accounts(f8ccd9f5-26a4-eb11-b1ac-000d3ab2f628)"}
                                };
                                if (reader["accountid"].ToString() != "")
                                {
                                    acc.Add("accountid", reader["accountid"].ToString());
                                }
                                accs.Add(acc);
                            }
                        }
                    }
                    cnn.Close();
                    
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
            SqlConnection cnn = this.Connection;
            try
            {

                cnn.Open();
                using (SqlCommand command = cnn.CreateCommand())
                {
                    command.CommandText = "update accounts.dbo.Clients set accountid=@id where accountname=@name and new_cash=@nc and emailadress1=@em";
                    command.Parameters.AddWithValue("@id", acc["accountid"].ToString());
                    command.Parameters.AddWithValue("@name", acc["name"].ToString());
                    command.Parameters.AddWithValue("@nc", acc["new_cash"].ToString());
                    command.Parameters.AddWithValue("@em", acc["emailaddress1"].ToString());
                    command.ExecuteNonQuery();

                }
                cnn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateSqlRecord Exception: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
