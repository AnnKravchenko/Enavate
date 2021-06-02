using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;


namespace WebAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set these values:
            // e.g. https://yourorg.crm.dynamics.com
            string url = "https://orgb5345919.api.crm4.dynamics.com";
            var resourceUri = "a9bbe5b7-81d8-4686-bedf-2adbb1ad9134";
            // e.g. you@yourorg.onmicrosoft.com
            string userName = "anna.kravchenko@testwebapi2.onmicrosoft.com";
            // e.g. y0urp455w0rd
            string password = "TestWebAPI2";
            
            // Azure Active Directory registered app clientid for Microsoft samples
            string clientIdq = "3cdb629e-5430-4ae1-b91e-f48a4ce72ce6";

            var userCredential = new UserCredential(userName, password);
            string apiVersion = "9.2";
            string webApiUrl = $"{url}/api/data/v{apiVersion}";

            var serviceUrl = "https://orgb5345919.api.crm4.dynamics.com";
            var clientId = "1b6e9d73-912c-43fb-99a4-13bb0061fd87";
            var secret = "hC16ek4mljOg.x~..M56c525~~wgCxlIM6";

            var authCtx = new AuthenticationContext("https://login.microsoftonline.com/a9bbe5b7-81d8-4686-bedf-2adbb1ad9134");
            var creds = new ClientCredential(clientId, secret);
            var result = authCtx.AcquireToken(serviceUrl, creds);
            var accessToken = result.AccessToken;

            //Authenticate using IdentityModel.Clients.ActiveDirectory
            //var authParameters = AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(webApiUrl)).Result;
            //var authContext = new AuthenticationContext(authParameters.Authority, false);
            //var authResult = authContext.AcquireToken(resourceUri, clientId, userCredential);
            //var authHeader = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            var authHeader = new AuthenticationHeaderValue("Bearer", accessToken);

            SQL s = new SQL();
            JArray accounts = new JArray(s.ConnectionData);
            
            for (int i = 0; i < accounts.Count; i++)
            {
                using (var client = new HttpClient())
                {
                
                    
                    try
                    {
                        client.BaseAddress = new Uri(webApiUrl);
                        client.DefaultRequestHeaders.Authorization = authHeader;
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        Console.WriteLine(accounts[i]);

                        if (accounts[i]["accountid"] == null)//create record
                        {
                            client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                            HttpContent content = new StringContent(accounts[i].ToString(), Encoding.UTF8, "application/json"); //JObject of the data to be posted.
                            HttpResponseMessage response = await client.PostAsync($"{client.BaseAddress}/accounts", content);
                            string json = await response.Content.ReadAsStringAsync();
                            JObject returnedData = JObject.Parse(json);
                            Console.WriteLine(returnedData);
                            s.UpdateSqlRecord(returnedData);
                            
                        }
                        else // update record
                        {
                            HttpContent content = new StringContent(accounts[i].ToString(), Encoding.UTF8, "application/json"); //JObject of the data to be posted.
                            HttpResponseMessage response = await PatchAsync(client, $"{client.BaseAddress}/accounts({accounts[i]["accountid"].ToString()})", content);
                            Console.WriteLine(response.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.Message);

                    }
                }
            }
            Console.ReadLine();
            s.Dispose();
        }
        public static async Task<HttpResponseMessage> PatchAsync(HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };
            Console.WriteLine("request\n "+request.ToString());
            var response = await client.SendAsync(request);
            return response;
        }
    }
}