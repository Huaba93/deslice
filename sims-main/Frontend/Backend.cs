using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using SIMS_Frontend.Classes;


namespace SIMS_Frontend
{
    public class Backend
    {
        public Backend()
        {
        }
        public static string GetJsonFromBackend(string url, string authToken)
        {
            HttpResponseMessage message = Backend.GetBackendService(url, authToken);
            if (message.StatusCode != HttpStatusCode.OK) { return message.StatusCode.ToString(); };
            string jsonResult;
            using (var task = Task.Run(() => message.Content.ReadAsStringAsync()))
            {
                task.Wait();
                jsonResult = task.Result;
            }
            return jsonResult;
        }
        public static string GetBackendServiceURL()
        {
#if DEBUG
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();
            secretProvider.TryGet("BackendUrl", out var BackendUrl);
#else
			string BackendUrl = Environment.GetEnvironmentVariable("BackendUrl");
#endif
            if (null == BackendUrl) { Console.WriteLine("FAILURE NO BACKENDURL"); Environment.Exit(2); }

            return BackendUrl;
        }
        public static ResponseObj PostBackendService(string jsonObject, string PostUri, string? authToken = null)
        {
            string AuthServiceUri = GetBackendServiceURL();
            HttpResponseMessage response;
            using (HttpClient client = new())
            {
                
                client.BaseAddress = new Uri(AuthServiceUri);
                StringContent content = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json");
                if (null != authToken) { content.Headers.Add("authToken", authToken);}
                using (var task = Task.Run(() => client.PostAsync(PostUri, content)))
                {
                    task.Wait();
                    response = task.Result;
                }
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ResponseObj(HttpStatusCode.Unauthorized);
            }
            string message;
            using (var task = Task.Run(() => response.Content.ReadAsStringAsync()))
            {
                task.Wait();
                message = task.Result;

            }
            ResponseObj resp = new ResponseObj(HttpStatusCode.OK);
            resp.Message = message;

            return resp;
        }
        public static HttpResponseMessage GetBackendService(string GetUri, string? authToken = null)
        {
            string BackendUrl = GetBackendServiceURL();
            HttpResponseMessage message;
            using (HttpClient httpClient = new()){
                using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, new Uri(BackendUrl  + GetUri)))
                {

                    requestMessage.Headers.Add("authToken", authToken);
                   
                    using (var task = Task.Run(() => httpClient.SendAsync(requestMessage)))
                    {
                        task.Wait();
                        message = task.Result;
                    }
                }
            }


            return message;
        }

        public static ResponseObj GetAuthToken(string Username, string Password)
        {
            string AuthServiceUri = Authentication.GetAuthServiceURL();
            AuthObj auth = new AuthObj(Username, Password);
            string json = JsonConvert.SerializeObject(auth);
            ResponseObj resp = Authentication.PostAuthService(json, "/auth");
            return resp;

        }


    }
    

}

