using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using RestSharp.Authenticators;

namespace WebApplication13.DAL
{
    public class SmsDAL
    {
        private static IRestRequest CreateRequestBody(string phoneNumber, string message, string sender = "UrbanThai", string scheduled_delivery = "", string force = "")
        {

            if (String.IsNullOrEmpty(phoneNumber) || String.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Phone Number or Message is missing");
            }
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(new { msisdn = phoneNumber, message = message, sender = sender, scheduled_delivery = "", force = "" });

            return request;
        }

        private static HttpBasicAuthenticator CreateBasicAuth(string apiKey, string secretKey)
        {
            if (String.IsNullOrEmpty(apiKey) || String.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentException("Api Ket or Secret Key is missing");
            }
            var authen = new HttpBasicAuthenticator(apiKey, secretKey);

            return authen;
        }

        public static void SendOTP(string phoneNumber, string message)
        {
            var client = new RestClient("https://api-v2.thaibulksms.com/sms");
            string apiKey = "lVS7pGfx_R3gmokFWeUA6UXWZX5Yo0";
            string secretKey = "apI09Q5WdjHVLL0i_3ARRj0KnzmzCh";
            client.Authenticator = CreateBasicAuth(apiKey, secretKey);
            client.Timeout = -1;
            var request = CreateRequestBody(phoneNumber, message);

            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);


        }
    }
}