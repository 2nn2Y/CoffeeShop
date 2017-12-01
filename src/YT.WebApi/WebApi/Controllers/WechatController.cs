using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Web.Models;
using Abp.WebApi.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YT.Configuration;
using YT.WebApi.Models;

namespace YT.WebApi.Controllers
{
    /// <summary>
    /// h5微信接口
    /// </summary>
    public class WechatController : AbpApiController
    {

        private const string AppId = "wx734728844b17a945";
        private const string Secret = "56a6e5b18dd44b397d1c80383a39cd01";
        private const string MchId = "1481424252";
        private const string SaleKey = "1q2w3e4r5t6y7u8i9o0p1q2w3e4r5t6y";
        
        private readonly ICacheManager _cacheManager;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        public WechatController(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 获取 accesstoken
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTokenFromCache()
        {
            var result = await _cacheManager.GetCache(OrgCacheName.WeChatToken).GetAsync(OrgCacheName.WeChatToken,
                async () => await GetAccessToken());
            return result;
        }
        /// <summary>
        /// 创建时间戳
        /// </summary>
        /// <returns></returns>
        private static long CreatenTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public async Task Unifiedorder(JsPayInput input)
        {
            var order = Guid.NewGuid().ToString("N");

        var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
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
              $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={AppId}&secret={Secret}&code={code}&grant_type=authorization_code";
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
              $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={AppId}&secret={Secret}";
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
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<dynamic> GetInfoByCode(string code)
        {
            var model = await GetUserTokenFromCache(code);
            var token = model.GetValue("access_token").ToString();
            var openid = model.GetValue("openid").ToString();
            string url = $"https://api.weixin.qq.com/sns/userinfo?access_token={token}&openid={openid}&lang=zh_CN";
            var result = await HttpHandler.GetAsync<JObject>(url);
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
                appId = AppId,
                timestamp = timestamp,
                nonceStr = noncestr,
                signature = sign.ToLower()
            });
        }
    }
}
