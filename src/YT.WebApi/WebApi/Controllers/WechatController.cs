using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Web.Models;
using Abp.WebApi.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YT.Configuration;
using YT.Models;
using YT.WebApi.Models;

namespace YT.WebApi.Controllers
{
    /// <summary>
    /// h5微信接口
    /// </summary>
    public class WechatController : AbpApiController
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<StoreUser> _useRepository;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="useRepository"></param>
        public WechatController(ICacheManager cacheManager, IRepository<StoreUser> useRepository)
        {
            _cacheManager = cacheManager;
            _useRepository = useRepository;
        }

        /// <summary>
        /// 获取 accesstoken
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTokenFromCache()
        {
            var result = await _cacheManager.GetCache(CoffeeCacheName.WeChatToken).GetAsync(CoffeeCacheName.WeChatToken,
                async () => await GetAccessToken());
            return result;
        }
        /// <summary>
        /// 微信支付回掉
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Notify()
        {
            // 本文顶部说的四个参数，最好进行URL解码
            var signature = HttpUtility.UrlDecode(HttpContext.Current.Request["msg_signature"] ?? string.Empty);
            var timestamp = HttpUtility.UrlDecode(HttpContext.Current.Request["timestamp"] ?? string.Empty);
            var nonce = HttpUtility.UrlDecode(HttpContext.Current.Request["nonce"] ?? string.Empty);
            var echo = HttpUtility.UrlDecode(HttpContext.Current.Request["echostr"] ?? string.Empty);

            // 验证结束后的返回值，一定不要带引号！！！
            var echoResult = string.Empty;

            // 将验证后的返回值写入响应流，这样可以去掉引号！！！
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(echoResult);
            HttpContext.Current.Response.End();
            return null;
        }
        /// <summary>
        /// 获取卡圈ticket
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCardTicketFromCache()
        {
            var result = await _cacheManager.GetCache(CoffeeCacheName.CardToken).GetAsync(CoffeeCacheName.CardToken,
               async () => await GetCardToken());
            return result;
         
        }

        private async Task<string> GetCardToken()
        {
            var token = await GetTokenFromCache();
            var url = $"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={token}&type=wx_card";
            var result = await HttpHandler.GetAsync<dynamic>(url);
            if (result != null)
            {
                return result.ticket;
            }
            throw new UserFriendlyException("card票据不存在");
        }

        /// <summary>
        /// 创建时间戳
        /// </summary>
        /// <returns></returns>
        private static long CreatenTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
        /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> GetUserBalance(string openId)
        {
            var user =await _useRepository.FirstOrDefaultAsync(c => c.OpenId.Equals(openId));
            if(user==null)throw new  UserFriendlyException("用户信息不存在");
            return user;
        }


        /// <summary>
        /// 获取用户卡圈列表
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        [HttpGet]

        public async Task<dynamic> GetUserCards(string openId)
        {
            var token = await GetTokenFromCache();
            var url = $"https://api.weixin.qq.com/card/user/getcardlist?access_token={token}";
            var result =  HttpHandler.PostJson<JObject>(url, JsonConvert.SerializeObject(new
            {
                openid=openId
            }));
            return result;
        }
     
        ///  <summary>
        ///  签名算法
        ///  </summary>
        ///  <param name="jsapiTicket">jsapi_ticket</param>
        ///  <param name="noncestr">随机字符串(必须与wx.config中的nonceStr相同)</param>
        ///  <param name="timestamp">时间戳(必须与wx.config中的timestamp相同)</param>
        ///  <param name="url">当前网页的URL，不包含#及其后面部分(必须是调用JS接口页面的完整URL)</param>
        /// <returns></returns>
        private string GetSignature(string jsapiTicket, string noncestr, long timestamp, string url)
        {
            var string1Builder = new StringBuilder();
            string1Builder.Append("jsapi_ticket=").Append(jsapiTicket).Append("&")
            .Append("noncestr=").Append(noncestr).Append("&")
            .Append("timestamp=").Append(timestamp).Append("&")
            .Append("url=").Append(url.IndexOf("#", StringComparison.Ordinal) >= 0 ?
            url.Substring(0, url.IndexOf("#", StringComparison.Ordinal)) : url);
            var result = string1Builder.ToString();
            return Sha1(result);
        }
        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="content">需要加密字符串</param>  
        /// <returns>返回40位UTF8 大写</returns>  
        private string Sha1(string content)
        {
            return Sha1(content, Encoding.UTF8);
        }
        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="content">需要加密字符串</param>  
        /// <param name="encode">指定加密编码</param>  
        /// <returns>返回40位大写字符串</returns>  
        private string Sha1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytesIn = encode.GetBytes(content);
                byte[] bytesOut = sha1.ComputeHash(bytesIn);
                sha1.Dispose();
                string result = BitConverter.ToString(bytesOut);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }
        /// <summary>
        /// 获取fulltoken
        /// </summary>
        /// <returns></returns>
        private static async Task<JObject> GetUserToken(string code)
        {
            var url =
              $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={WxPayConfig.Appid}&secret={WxPayConfig.Appsecret}&code={code}&grant_type=authorization_code";
            var result = await HttpHandler.GetAsync<JObject>(url);
            return result;
        }
        /// <summary>
        /// 获取accesstoken
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetAccessToken()
        {
            var url =
              $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={WxPayConfig.Appid}&secret={WxPayConfig.Appsecret}";
            var result = await HttpHandler.GetAsync<dynamic>(url);
            return result.access_token;
        }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetJsapiTicket()
        {

            var token = await GetTokenFromCache();
            var url = $"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={token}&type=jsapi";
            var result = await HttpHandler.GetAsync<dynamic>(url);
            if (result != null)
            {
                return result.ticket;
            }
            throw new UserFriendlyException("票据不存在");
        }

      
        /// <summary>
        /// 获取 accesstoken
        /// </summary>
        /// <returns></returns>
        public async Task<JObject> GetUserTokenFromCache(string code)
        {
            var result = await _cacheManager.GetCache(CoffeeCacheName.UserToken).GetAsync(CoffeeCacheName.UserToken,
                async () => await GetUserToken(code));
            return result;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<dynamic> GetInfoByCode(string code)
        {
            var model = await GetUserTokenFromCache(code);
            var token = model.GetValue("access_token").ToString();
            var openid = model.GetValue("openid").ToString();
            string url = $"https://api.weixin.qq.com/sns/userinfo?access_token={token}&openid={openid}&lang=zh_CN";
            var result = await HttpHandler.GetAsync<JObject>(url);
            var openId = result.GetValue("openid").ToString();
            var user =await _useRepository.FirstOrDefaultAsync(c=>c.OpenId.Equals(openId));
            if (user == null)
            {
                var t=new StoreUser()
                {
                    Balance=0,OpenId=openId
                };
                await _useRepository.InsertAsync(t);
                result.Add("balance", 0);
            }
            else
            {
                result.Add("balance",user.Balance);
            }
            return result;
        }
        /// <summary>
        /// 获取验证信息
        /// </summary>
        /// <returns></returns>
        public async Task<AjaxResponse> Config(string url)
        {
            var result =
                await
                    _cacheManager.GetCache(CoffeeCacheName.TicketToken)
                        .GetAsync(CoffeeCacheName.TicketToken, () => GetJsapiTicket());
            var noncestr = Guid.NewGuid().ToString("D").Split('-').Last().ToLower();
            var timestamp = CreatenTimestamp();
            string sign = GetSignature(result, noncestr, timestamp, url);
            return new AjaxResponse(new
            {
                appId = WxPayConfig.Appid,
                timestamp = timestamp,
                nonceStr = noncestr,
                signature = sign.ToLower()
            });
        }
    }
}
