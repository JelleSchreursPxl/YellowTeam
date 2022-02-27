using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Console1
{
    class Program
    {
        public static string bearerToken;
        public static string responseString = "";
        public static HttpWebResponse response = null;

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appSettings.json")
               .Build();

            try
            {
                GetToken(config);

                while (true)
                {
                    Console.Write("Enter city: ");
                    var input = Console.ReadLine();

                    var request = (HttpWebRequest)WebRequest.Create($"http://localhost:5000/api/seatholders/{input}");
                    request.Accept = "application/json"; //"application/xml";
                    request.Method = "GET";

                    //Pass token in Authorization Header.
                    request.Headers["Authorization"] = "Bearer " + bearerToken;

                    using (response = (HttpWebResponse)request.GetResponse())
                    {
                        responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }

                    Console.WriteLine(responseString);
                }
                //Environment.Exit(0);
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Response);
                Console.WriteLine(ex.StackTrace);
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)ex.Response;
                    responseString = "Some error occured: " + response.StatusCode.ToString();
                }
                else
                {
                    responseString = "Some error occured: " + ex.Status.ToString();
                }
            }
        }

        private static void GetToken(IConfiguration config)
        {
            var request = (HttpWebRequest)WebRequest.Create(config.GetSection("baseURL").Value + "/token");
            request.Accept = "application/json"; //"application/xml";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            var formString = $"client_id={config.GetSection("credentials")["username"]}&client_secret={config.GetSection("credentials")["password"]}&grant_type=client_credentials";
            var data = Encoding.UTF8.GetBytes(formString);
            request.ContentLength = data.Length;

            // put form in request
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(data, 0, data.Length);
            dataStream.Close();

            using (response = (HttpWebResponse)request.GetResponse())
            {
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

            GetTokenFromResponse(responseString);
        }

        //Get token from response
        private static void GetTokenFromResponse(string response)
        {
            string[] stringArray = response.Split(',');
            foreach (string stringValue in stringArray)
            {
                if (stringValue.Contains("access_token"))
                {
                    //split string again to removes access_token in front of the actual token
                    string[] stringSecondArray = stringValue.Split(':');
                    //removes the paranthesises by using substring and assign it to member variable
                    bearerToken = stringSecondArray[1].Substring(1, stringSecondArray[1].Length - 2);
                }
            }
        }
    }
}
