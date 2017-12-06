using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using Abp.Domain.Repositories;
using Abp.Json;
using Abp.Logging;
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
        private readonly IRepository<StoreOrder> _orderRepository;
        
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="useRepository"></param>
        /// <param name="orderRepository"></param>
        public WechatController(ICacheManager cacheManager, IRepository<StoreUser> useRepository, IRepository<StoreOrder> orderRepository)
        {
            _cacheManager = cacheManager;
            _useRepository = useRepository;
            _orderRepository = orderRepository;
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
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public WxPayData GetNotifyData(Stream stram)
        {
            //接收从微信后台POST过来的数据
            System.IO.Stream s = stram;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();
            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(builder.ToString());
            }
            catch (Exception ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                HttpContext.Current.Response.Write(res.ToXml());
                HttpContext.Current.Response.End();
            }
            return data;
        }
        /// <summary>
        /// 通知
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Notify()
        {
            WxPayData notifyData = GetNotifyData(HttpContext.Current.Request.InputStream);

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData t = new WxPayData();
                t.SetValue("return_code", "FAIL");
                t.SetValue("return_msg", "支付结果中微信订单号不存在");
                HttpContext.Current.Response.Write(t.ToXml());
                HttpContext.Current.Response.End();
            }
            var code = notifyData.GetValue("return_code").ToString();
            var rcode = notifyData.GetValue("result_code").ToString();
            if (code.ToUpper().Equals("SUCCESS") && rcode.ToUpper().Equals("SUCCESS"))
            {
                var num= notifyData.GetValue("out_trade_no").ToString();
                var order = await _orderRepository.FirstOrDefaultAsync(c => c.OrderNum.Equals(num));
                if (order != null)
                {
                    order.WechatOrder = notifyData.GetValue("transaction_id").ToString();
                    order.PayState = true;
                    await _orderRepository.UpdateAsync(order);
                    WxPayData res = new WxPayData();
                    res.SetValue("return_code", "SUCCESS");
                    res.SetValue("return_msg", "OK");
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Write(res.ToXml());
                    HttpContext.Current.Response.End();
                }
            }
            else
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "第三方订单不存在");
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Write(res.ToXml());
                HttpContext.Current.Response.End();
            }
           
            return Ok();
        }
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> QrCode(QrcodeInput input)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(c => c.OrderNum.Equals(input.OrderNo));
            var fast = order == null ? "000" : order.FastCode;
            if (order == null)
            {
                order = new StoreOrder()
                {
                    DeviceNum = input.AssetId,
                    OrderNum = input.OrderNo,
                    OrderState = null,
                    PayState = null,
                    OrderType = OrderType.Ice,
                    ProductId = input.ProductNum,
                    NoticeUrl = input.Notify_Url,
                    Key = input.Key
                };
                order = await _orderRepository.InsertAsync(order);
            }
            var codeUrl =
                $"http://card.youyinkeji.cn/?#/detail/{order.ProductId}-{order.DeviceNum}-{fast}-{order.OrderNum}-{1}";
            return Json(codeUrl);
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
            var user = await _useRepository.FirstOrDefaultAsync(c => c.OpenId.Equals(openId));
            if (user == null) throw new UserFriendlyException("用户信息不存在");
            return user;
        }
        /// <summary>
        /// ice制作成功回掉
        /// </summary>
        /// <returns></returns>
        public async Task IceProduct(IceCallInput input)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(c =>
                    c.OrderNum.Equals(input.OrderNo) && c.DeviceNum.Equals(input.AssetId));
            if (order==null) throw new UserFriendlyException("该订单不存在");
            if (!order.PayState.HasValue||!order.PayState.Value) throw new UserFriendlyException("该订单未支付");
            if (input.DeliverStatus.Equals("0"))
            {
                order.OrderState = true;
            }
            else
            {
                order.OrderState = false;
                order.Reson = input.DeliverStatus;
            }
        }
        /// <summary>
        /// ice制作成功回掉
        /// </summary>
        /// <returns></returns>
        public async Task JackProduct(JackCallInput input)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(c =>
                    c.OrderNum.Equals(input.Id) && c.DeviceNum.Equals(input.Vmc));
            if (order == null) throw new UserFriendlyException("该订单不存在");
            if (!order.PayState.HasValue || !order.PayState.Value) throw new UserFriendlyException("该订单未支付");
                order.OrderState = true;
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
            var result = HttpHandler.PostJson<JObject>(url, JsonConvert.SerializeObject(new
            {
                openid = openId
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
            var temp =
             $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={WxPayConfig.Appid}&secret={WxPayConfig.Appsecret}&code={code}&grant_type=authorization_code";
            var ut = await HttpHandler.GetAsync<JObject>(temp);
            if (ut.GetValue("access_token") == null) throw new UserFriendlyException("获取用户token失败");
            var token = ut.GetValue("access_token").ToString();
            var openid = ut.GetValue("openid").ToString();
            string url = $"https://api.weixin.qq.com/sns/userinfo?access_token={token}&openid={openid}&lang=zh_CN";
            var result = await HttpHandler.GetAsync<JObject>(url);
            var openId = result.GetValue("openid").ToString();
            var user = await _useRepository.FirstOrDefaultAsync(c => c.OpenId.Equals(openId));
            if (user == null)
            {
                var t = new StoreUser()
                {
                    Balance = 0,
                    OpenId = openId
                };
                await _useRepository.InsertAsync(t);
                result.Add("balance", 0);
            }
            else
            {
                result.Add("balance", user.Balance);
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