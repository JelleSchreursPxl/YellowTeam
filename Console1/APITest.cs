using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Console1
{
    class APITest
    {
        private static string URL = "http://localhost:5000/api/seatholders";
        // private static string TOKENURL = "http://localhost:5002/connect/token";
        // private static string BEARER = "Authorization:Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkE2NTIzMDlGNkIwNzM1QUQzODZEMjQ4M0ZEQjY2OTZDIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NDU2NDY0NjQsImV4cCI6MTY0NTY1MDA2NCwiaXNzIjoiaHR0cDovL2lkZW50aXR5IiwiY2xpZW50X2lkIjoiZGF0YXZpeiIsImp0aSI6IkU5RjQwMERCNUYxQTg2N0U2RTA2RDUzNjRGOUVGMDNFIiwiaWF0IjoxNjQ1NjQ2NDY0LCJzY29wZSI6WyJrcmMtZ2VuayJdfQ.VSGEzLH4QFOvXMJoJKxpTyrVGHq8t1gqdBhMqr64KSW3O86reMWaRaL-iz5NdM84ldaPYmkVA1rrE6ip07WUWHh-mZD3dURc5BSNtKdB8oDg9rLt2WpGpK0hj2toQTUzp0Xs76Svqd7Zud7Wb_X1xwysgt-XGFyVCJAQfE-jsXwTlRCRZBpYq48zELLWIXiCSBQV1ZPa_ntBBlgPsvaZlJtKSFjRlep5iLRuiM1iH2QgkSrThXFIpOrQk2niMyPP6Z5lhEk9P5NvedWPD2Bunb74eWubE_R7Bm-QUvkdUy7O0ARpDLWjUg_hBGnbO1z1lsHVsxTSukMWCaB2vhovbg";
        public static async void GetDataWithoutAuthentication()
        {
            var authCredential = Encoding.UTF8.GetBytes("{userTest}:{passTest}");
            using(var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authCredential));
                client.BaseAddress = new Uri(URL);
                HttpResponseMessage response = await client.GetAsync(URL);

                if(response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var rawResponse = readTask.GetAwaiter().GetResult();
                    Console.WriteLine(rawResponse);
                }
                Console.WriteLine("Complete");

                // client.Headers.Add(BEARER);       
                // client.Headers.Add("Content-Type:application/json");
                // client.Headers.Add("Accept:application/json");

                // var result = client.DownloadString(URL);
                // Console.WriteLine(Environment.NewLine + result);
            }
        }
    }

}