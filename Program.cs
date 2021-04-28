using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json; 

namespace authtest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Spotify API");
            AccessToken token = GetToken().Result;
            Console.WriteLine(String.Format("Access Token: {0}",token.access_token));
            
            //CHAMADA DE PODCASTS - TA PRONTA
            using(var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token.access_token);

                //Request Endpoint
                var request = client.GetAsync("https://api.spotify.com/v1/search?q=Livros&type=show&market=BR&limit=10&offset=5").Result;
                var response = request.Content.ReadAsStringAsync().Result;
                Console.WriteLine(response);
            }

            Console.ReadLine();
        }

        static async Task<AccessToken> GetToken()
        {
            Console.WriteLine("Getting Token");
            string clientId = "e44b07b1d2e14882a8d5e11f2774400a";
            string clientSecret = "b2a39e60fc9f48fcaf71be58d3867038";
            string credentials = String.Format("{0}:{1}",clientId,clientSecret);

            using(var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));

                //Prepare Request Body
                List<KeyValuePair<string,string>> requestData = new List<KeyValuePair<string,string>>();
                requestData.Add(new KeyValuePair<string,string>("grant_type","client_credentials"));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = await client.PostAsync("https://accounts.spotify.com/api/token",requestBody);
                var response = await request.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AccessToken>(response); 
            }
        }
    }
}
