using OnlineJournal.Model;
using OnlineJournal.ViewModel;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace OnlineJournal.API
{
    class WebAPI
    {
        private static GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public static Task<HttpResponseMessage> GetCall(string url)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string apiUrl = ConfigurationManager.AppSettings["baseUrl"] + url;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.GetAsync(apiUrl);
                    response.Wait();
                    return response;
                }
            }
            catch
            {
                throw;
            }
        }

        public static Task<HttpResponseMessage> PutCall(string url, object updateObjcet)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                url += (url.Contains("?") ? "&email=" : "?email=") + Global.Account.Email;
                string apiUrl = ConfigurationManager.AppSettings["baseUrl"] + url;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var jsonContent = new JavaScriptSerializer().Serialize(updateObjcet);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = client.PutAsync(apiUrl, content);
                    response.Wait();
                    return response;
                }
            }
            catch
            {
                throw;
            }
        }

        public static Task<HttpResponseMessage> PostCall(string url, object newObject)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                url += (url.Contains("?") ? "&email=" : "?email=") + Global.Account.Email;
                string apiUrl = ConfigurationManager.AppSettings["baseUrl"] + url;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var jsonContent = new JavaScriptSerializer().Serialize(newObject);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(apiUrl, content);
                    response.Wait();
                    return response;
                }
            }
            catch
            {
                throw;
            }
        }

        public static Task<HttpResponseMessage> DeleteCall(string url)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                url += "&email=" + Global.Account.Email;
                string apiUrl = ConfigurationManager.AppSettings["baseUrl"] + url;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.DeleteAsync(apiUrl);
                    response.Wait();
                    return response;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
