using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Serialization;
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
        /// 微信支付回掉
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Notify()
        {
            //            < xml >
            //  < appid >< ![CDATA[wx2421b1c4370ec43b]] ></ appid >
            //  < attach >< ![CDATA[支付测试]] ></ attach >
            //  < bank_type >< ![CDATA[CFT]] ></ bank_type >
            //  < fee_type >< ![CDATA[CNY]] ></ fee_type >
            //  < is_subscribe >< ![CDATA[Y]] ></ is_subscribe >
            //  < mch_id >< ![CDATA[10000100]] ></ mch_id >
            //  < nonce_str >< ![CDATA[5d2b6c2a8db53831f7eda20af46e531c]] ></ nonce_str >
            //  < openid >< ![CDATA[oUpF8uMEb4qRXf22hE3X68TekukE]] ></ openid >
            //  < out_trade_no >< ![CDATA[1409811653]] ></ out_trade_no >
            //  < result_code >< ![CDATA[SUCCESS]] ></ result_code >
            //  < return_code >< ![CDATA[SUCCESS]] ></ return_code >
            //  < sign >< ![CDATA[B552ED6B279343CB493C5DD0D78AB241]] ></ sign >
            //  < sub_mch_id >< ![CDATA[10000100]] ></ sub_mch_id >
            //  < time_end >< ![CDATA[20140903131540]] ></ time_end >
            //  < total_fee > 1 </ total_fee >
            //< coupon_fee >< ![CDATA[10]] ></ coupon_fee >
            //< coupon_count >< ![CDATA[1]] ></ coupon_count >
            //< coupon_type >< ![CDATA[CASH]] ></ coupon_type >
            //< coupon_id >< ![CDATA[10000]] ></ coupon_id >
            //< coupon_fee >< ![CDATA[100]] ></ coupon_fee >
            //  < trade_type >< ![CDATA[JSAPI]] ></ trade_type >
            //  < transaction_id >< ![CDATA[1004400740201409030005092168]] ></ transaction_id >
            //</ xml >
            var stream = HttpContext.Current.Request.InputStream;
            var result = (WeChatXml)(new XmlSerializer(typeof(WeChatXml)).Deserialize(stream));
            var output = string.Empty;
            if (result.return_code.ToUpper().Equals("SUCCESS")&&result.result_code.ToUpper().Equals("SUCCESS"))
            {
                var order = await _orderRepository.FirstOrDefaultAsync(c => c.OrderNum.Equals(result.out_trade_no));
                if (order != null)
                {
                    order.WechatOrder = result.transaction_id;
                    order.PayState = true;
                    await _orderRepository.UpdateAsync(order);

                    output = @"<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg> </xml>";
                }
            }
            else
            {
                output = @"<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[订单不成功]]></return_msg> </xml>";
            }

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(output);
            HttpContext.Current.Response.End();
            return null;
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
