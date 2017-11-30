using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.Extensions;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YT.Configuration
{
    public static class HttpHandler
    {
        /// <summary>
        /// 异步GET请求，
        /// </summary>
        /// <param name="url"></param> 
        /// <returns></returns>
        public static async Task<string> GetAsync(string url)
        {
            using (var client = new HttpClient())
            {
                //获取客户端信息
                //  var user = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
                //  client.DefaultRequestHeaders.Add("user-agent", user);//添加HTTP头
                return await client.GetStringAsync(url).ConfigureAwait(false);
            }
        }
        public static async Task<T> GetAsync<T>(string url) where T : class, new()
        {
            using (var client = new HttpClient())
            {
                //获取客户端信息
                //  var user = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
                //  client.DefaultRequestHeaders.Add("user-agent", user);//添加HTTP头
                var result = await client.GetStringAsync(url).ConfigureAwait(false);
                return !result.IsNullOrWhiteSpace() ? JsonConvert.DeserializeObject<T>(result) : null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T PostJson<T>(string url, string json) where T:class ,new ()
        {
            string strUrl = url;

            //创建一个HTTP请求  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
            //Post请求方式  
            request.Method = "POST";
            //内容类型
            request.ContentType = "application/x-www-form-urlencoded";
            //设置参数，并进行URL编码 
            string paraUrlCoded = json;//System.Web.HttpUtility.UrlEncode(jsonParas);   
            byte[] payload;
            //将Json字符串转化为字节  
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength   
            request.ContentLength = payload.Length;
            //发送请求，获得请求流 
            Stream writer;
            try
            {
                writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
            }
            catch (Exception)
            {
                writer = null;
            }
            //将请求参数写入流
            if (writer != null)
            {
                writer.Write(payload, 0, payload.Length);
                writer.Close();//关闭请求流
            }
            string postContent = "";

            //获得响应流
            var response = (HttpWebResponse)request.GetResponse();
            Stream s = response.GetResponseStream();
            StreamReader sRead = new StreamReader(s);
            postContent = sRead.ReadToEnd();
            sRead.Close();
            return JsonConvert.DeserializeObject<T>(postContent);
        }


        /// <summary>
        /// 异步Post请求，
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string url, List<KeyValuePair<string, string>> values = null) where T : class, new()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response;
                if (values != null)
                {
                    var content = new FormUrlEncodedContent(values);
                    response = await client.PostAsync(url, content).ConfigureAwait(false);
                }
                else
                {
                    response = await client.PostAsync(url, null).ConfigureAwait(false);
                }
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return !result.IsNullOrWhiteSpace() ? JsonConvert.DeserializeObject<T>(result) : null;
            }
        }
        /// <summary>
        /// 异步Post请求，
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, List<KeyValuePair<string, string>> values = null)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(url, content).ConfigureAwait(false);
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        /// <summary>  
        /// 指定Post地址使用Get 方式获取全部字符串  
        /// </summary>  
        /// <param name="url">请求后台地址</param>  
        /// <returns></returns>  
        public static string Post(string url)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取内容  
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
