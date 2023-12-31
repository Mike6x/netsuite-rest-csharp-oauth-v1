﻿using System;
using System.IO;
using System.Net;
using System.Text;
using OAuth;
using Newtonsoft.Json;

namespace netsuiteToken
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string NS_realm = "xxxx";
                string url = "https://xxxx.restlets.api.netsuite.com/app/site/hosting/restlet.nl?script=SCRIPT_ID&deploy=1";
                HttpWebRequest request = null;

                String header = "Authorization: OAuth ";
                string consumer_id = "your consumerid";
                string consumer_secret = "your consumer_secre ";
                string token_id = "your token_id";
                string token_secret = "you token_secret";
                string param = "&type=serial&modify_to=2023-03-01&modify_from=2023-03-01&serial_num=";


                Uri uri = new Uri((url + param));
                OAuthBase req = new OAuthBase();
                string normalized_url;
                string normalized_params;
                string nonce = req.GenerateNonce();
                string time = req.GenerateTimeStamp();
                string signature = req.GenerateSignature(uri, consumer_id, consumer_secret, token_id, token_secret, "GET",
                                     time, nonce, out normalized_url, out normalized_params);
                if (signature.Contains("+"))
                {
                    signature = signature.Replace("+", "%2B");
                }

                // Construct the OAuth header		
                header += "oauth_signature=\"" + signature + "\",";
                header += "oauth_version=\"1.0\",";
                header += "oauth_nonce=\"" + nonce + "\",";
                header += "oauth_signature_method=\"HMAC-SHA256\",";
                header += "oauth_consumer_key=\"" + consumer_id + "\",";
                header += "oauth_token=\"" + token_id + "\",";
                header += "oauth_timestamp=\"" + time + "\",";
                header += "realm=\"" + NS_realm + "\"";


                request = (HttpWebRequest)WebRequest.Create((url + param));
                request.ContentType = "application/json";
                request.Method = "GET";
                request.Headers.Add(header);
                var response = request.GetResponse();
                var characterSet = ((HttpWebResponse)response).CharacterSet;
                var responseEncoding = characterSet == ""
                    ? Encoding.UTF8
                    : Encoding.GetEncoding(characterSet ?? "utf-8");
                var responsestream = response.GetResponseStream();
                if (responsestream == null)
                {
                    throw new ArgumentNullException(nameof(characterSet));
                }
                using (responsestream)
                {
                    var reader = new StreamReader(responsestream, responseEncoding);
                    var result = reader.ReadToEnd();
                    Console.WriteLine(@"result: " + result);
                }

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
