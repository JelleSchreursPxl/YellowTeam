using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Console1
{
    class Program
    {
        static void Main(string[] args)
        {
            string ResponseString = "";
            HttpWebResponse response = null;
            HttpWebResponse response2 = null;
            IConfiguration Config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();

            try
            {
                var baseURL = Config.GetSection("baseURL").Value;
                var request = (HttpWebRequest)WebRequest.Create(baseURL + "/token");
                // request.Accept = "application/json"; //"application/xml";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";

		        //Get credentials from config.
                // var dusername = EncryptionService.Decrypt(Config.GetSection("credentials")["username"]);
                // var dpassword = EncryptionService.Decrypt(Config.GetSection("credentials")["password"]);
                // var dusername = Config.GetSection("credentials")["username"];
                // var dpassword = Config.GetSection("credentials")["password"];

                Credentials cred = new Credentials()
                {
                    client_id = Config.GetSection("credentials")["username"],
                    client_secret = Config.GetSection("credentials")["password"],
                    grant_type = "client_credentials",
                    scope = "krc-genk"
                };

                var myContent = JsonConvert.SerializeObject(cred);

                var data = Encoding.ASCII.GetBytes(myContent);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (response = (HttpWebResponse)request.GetResponse())
                {
                    ResponseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

		        //Get the token from the /token end-point and call another end-point.
                Token token = JsonConvert.DeserializeObject<Token>(ResponseString);

                var request2 = (HttpWebRequest)WebRequest.Create(baseURL + "/ProcessData");
                request2.Accept = "application/json"; //"application/xml";
                request2.Method = "POST";
				
		        //Pass token in Authorization Header.
                request2.Headers["Authorization"] = "Bearer " + token.token;

                using (response2 = (HttpWebResponse)request2.GetResponse())
                {
                    ResponseString = new StreamReader(response2.GetResponseStream()).ReadToEnd();
                }

                Console.WriteLine("Hello World, try!");
                Console.WriteLine(ResponseString);
                Environment.Exit(0);
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Response);
                Console.WriteLine(ex.StackTrace);
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)ex.Response;
                    ResponseString = "Some error occured: " + response.StatusCode.ToString();
                }
                else
                {
                    ResponseString = "Some error occured: " + ex.Status.ToString();
                }
            }
            Console.WriteLine("Hello World, test!");
        }
    }
}
