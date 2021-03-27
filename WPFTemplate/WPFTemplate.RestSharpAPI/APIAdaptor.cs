using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTemplate.RestSharpAPI.Model;

namespace WPFTemplate.RestSharpAPI
{
    public static class APIAdaptor
    {
        #region Properties
        public static readonly RestClient client;

        public static UserModel User { get; set; }
        public static readonly string ServerIPAddress = ConfigurationManager.AppSettings["ServerIPAddress"];
        
        private static List<RestResponseCookie> cookies;
        private const int requestTimeoutTime = 5 * 60 * 1000;
        #endregion

        #region Constructors
        static APIAdaptor()
        {
            client = new RestClient(ServerIPAddress);
            client.Timeout = requestTimeoutTime;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 获取登录取得的Cookie并放入RestClient中
        /// 多并发访问时使用
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static RestClient GetClient(List<RestResponseCookie> cookies)
        {
            var client = new RestClient(ServerIPAddress);
            client.CookieContainer = new System.Net.CookieContainer();
            cookies.ForEach(c => client.CookieContainer.Add(new System.Net.Cookie(c.Name, c.Value, c.Path, c.Domain)));
            client.Timeout = requestTimeoutTime;
            return client;
        }

        #region Common Methods
        /// <summary>
        /// 同步调用后台GET方法
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="parameters">接口参数</param>
        /// <returns></returns>
        public static IRestResponse SendGetRequest(string url, Dictionary<string,string> parameters = null)
        {
            var request = FormRequest(url, Method.GET);
            if(parameters!= null)
            {
                foreach(var item in parameters)
                {
                    request.AddParameter(item.Key, item.Value);
                }
            }
            return client.Execute(request);
        }

        /// <summary>
        /// 异步调用后台GET方法
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="parameters">接口参数</param>
        /// <returns></returns>
        public async static Task<IRestResponse> SendGetRequestAsync(string url, Dictionary<string, string> parameters = null)
        {
            var clientAsync = GetClient(cookies);
            var request = FormRequest(url, Method.GET);
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    request.AddParameter(item.Key, item.Value);
                }
            }
            return await clientAsync.ExecuteAsync(request);
        }

        /// <summary>
        /// 同步调用后台POST方法
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="parameters">接口参数</param>
        /// <param name="requestBody">接口Json数据</param>
        /// <returns></returns>
        public static IRestResponse SendPostRequest(string url, Dictionary<string, string> parameters = null,object requestBody = null, string path = "")
        {
            var request = FormRequest(url, Method.POST, "application/json");
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    request.AddParameter(item.Key, item.Value);
                }
            }
            if (requestBody != null)
            {
                request.AddJsonBody(requestBody);
            }
            if (!string.IsNullOrEmpty(path))
            {
                request.AddFile("file", path, "multipart/form-data");
            }
            return client.Execute(request);
        }

        /// <summary>
        /// 异步调用后台POST方法
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="parameters">接口参数</param>
        /// <param name="requestBody">接口Json数据</param>
        /// <returns></returns>
        public async static Task<IRestResponse> SendPostRequestAsync(string url, Dictionary<string, string> parameters = null, object requestBody = null, string path = "")
        {
            var clientAsync = GetClient(cookies);
            var request = FormRequest(url, Method.POST, "application/json");
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    request.AddParameter(item.Key, item.Value);
                }
            }
            if (requestBody != null)
            {
                request.AddJsonBody(requestBody);
            }
            if (!string.IsNullOrEmpty(path))
            {
                request.AddFile("file", path, "multipart/form-data");
            }
            return await client.ExecuteAsync(request);
        }
        #endregion

        public static IRestResponse Login(string url, Dictionary<string,string> parameters = null, object requestBody = null)
        {
            var request = FormRequest(url, Method.POST, "application/json");
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    request.AddParameter(item.Key, item.Value);
                }
            }
            if (requestBody != null)
            {
                request.AddJsonBody(requestBody);
            }
            var response = client.Execute(request);

            cookies = response.Cookies.ToList();
            return response;
        }

        /// <summary>
        /// 组装request的私有方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private static RestRequest FormRequest(string url, Method method,string contentType = "application/x-www-form-urlencoded")
        {
            var request = new RestRequest(url, method);
            request.AddHeader("content-Type", contentType);
            return request;
        }
        #endregion
    }
}
