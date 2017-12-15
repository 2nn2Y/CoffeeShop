using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Web.Models;
using Abp.WebApi.Controllers;
using Castle.Core.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YT.Configuration;
using YT.Models;
using YT.Storage;
using YT.ThreeData;
using YT.WebApi.Models;

namespace YT.WebApi.Controllers
{
    /// <summary>
    /// token相关
    /// </summary>
    [AbpAllowAnonymous]
    public class SignController : AbpApiController
    {
        private const string CorpId = "ww480b7545345a38f7";
        private const string Secret = "SjF1kHm8EZfb35rNPn8wiTt-iTrJPegpzXI14yHwseg";
        private const string SecretA = "yKribg1WopTnC1vLrvFT5vObFXZVUou6XAtt8sqgiJc";
        private readonly ICacheManager _cacheManager;
        private readonly IAppFolders _appFolders;
        private readonly IBinaryObjectManager _objectManager;
        private readonly IRepository<Warn> _warnRepository;
        private readonly IRepository<UserPoint> _userPointRepository;
        private readonly IRepository<SignProfile> _profileRepository;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="appFolders"></param>
        /// <param name="objectManager"></param>
        /// <param name="warnRepository"></param>
        /// <param name="profileRepository"></param>
        /// <param name="userPointRepository"></param>
        public SignController(ICacheManager cacheManager,
            IAppFolders appFolders,
            IBinaryObjectManager objectManager,
            IRepository<Warn> warnRepository,
            IRepository<SignProfile> profileRepository,
            IRepository<UserPoint> userPointRepository)
        {
            _cacheManager = cacheManager;
            _appFolders = appFolders;
            _objectManager = objectManager;
            _warnRepository = warnRepository;
            _profileRepository = profileRepository;
            _userPointRepository = userPointRepository;
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
        /// 获取 accesstoken
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetFullTokenFromCache()
        {
            var result = await _cacheManager.GetCache(OrgCacheName.FullToken).GetAsync(OrgCacheName.FullToken,
                async () => await GetFullToken());
            return result;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<dynamic> GetInfoByCode(string code)
        {
            var token = await GetTokenFromCache();
            string url = $"https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={token}&code={code}";
            var result = await HttpHandler.GetAsync<JObject>(url);
            if (result.GetValue("user_ticket") == null)
            {
                return result;
            }
            var ticket = result.GetValue("user_ticket").ToString();
            var info = $"https://qyapi.weixin.qq.com/cgi-bin/user/getuserdetail?access_token={token}";
            var r = HttpHandler.PostJson<JObject>(info, JsonConvert.SerializeObject(new
            {
                user_ticket = ticket
            }));
            return r;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public async Task<AjaxResponse> GetInfoByTicket(string ticket)
        {
            var token = await GetTokenFromCache();
            var info = $"https://qyapi.weixin.qq.com/cgi-bin/user/getuserdetail?access_token={token}";
            var r = await HttpHandler.PostAsync<JObject>(info, new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("user_ticket", ticket)
            });
            return new AjaxResponse(r);
        }
        /// <summary>
        /// 获取验证信息
        /// </summary>
        /// <returns></returns>
        public async Task<AjaxResponse> Config(string url)
        {
            var result =
                await
                    _cacheManager.GetCache(OrgCacheName.TicketToken)
                        .GetAsync(OrgCacheName.TicketToken, () => GetJsapiTicket());
            var noncestr = Guid.NewGuid().ToString("D").Split('-').Last().ToLower();
            var timestamp = CreatenTimestamp();
            string sign = GetSignature(result, noncestr, timestamp, url);
            return new AjaxResponse(new
            {
                appId = "ww480b7545345a38f7",
                timestamp = timestamp,
                nonceStr = noncestr,
                signature = sign.ToLower()
            });
        }
        /// <summary>
        /// 获取机构人员 从缓存
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetOrgUserFromCache()
        {
            var result =
               await
                   _cacheManager.GetCache(OrgCacheName.OrgUser)
                       .GetAsync(OrgCacheName.OrgUser, () => GetOrgUsers());
            return result;
        }
        /// <summary>
        /// 获取机构人员
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> GetOrgUsers()
        {
            var token = await GetTokenFromCache();
            string url = $"https://qyapi.weixin.qq.com/cgi-bin/user/simplelist?access_token={token}&department_id=1&fetch_child=1&status=0";
            var result = await HttpHandler.GetAsync<JObject>(url);
            if (result.GetValue("userlist") == null)
            {
                throw new UserFriendlyException("获取人员失败");
            }
            var ticket = result.GetValue("userlist");
            return ticket.OfType<JObject>()
                .Select(t => t.GetValue("userid").ToString().ToLower()).ToList();
        }
        /// <summary>
        /// 同步图片资源
        /// </summary>
        /// <returns></returns>
        public async Task GenderImage()
        {
            var records = await _profileRepository.GetAllListAsync(c => !c.ProfileId.HasValue);
            foreach (var profile in records)
            {
                var guid = await GetMultimedia(profile.MediaId);
                profile.ProfileId = guid;
                await _profileRepository.UpdateAsync(profile);
            }
        }

        /// <summary>
        /// 获取报警数据
        /// </summary>
        /// <returns></returns>
        public async Task GenderWarning()
        {

            var p =
         new QueryParams("WARNING");
            var t =
            p.ReturnParams();
            var result = HttpHandler.Post(p.Warn + "?" + t);
            var temp = JsonConvert.DeserializeObject<ResultItem>(result);
            await InsertWarn(temp.Record);
        }
        /// <summary>
        /// 插入报警数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task InsertWarn(dynamic list)
        {
            var temp = list as object[][];
            if (temp != null)
            {
                var warns = await _warnRepository.GetAllListAsync(c => !c.State);

                foreach (var warn in warns)
                {
                    if(warn.SetTime.HasValue)continue;
                    if (!temp.Any(c => c[0].ToString() == warn.DeviceNum && c[2].ToString() == warn.WarnContent))
                    {
                        warn.SetTime = DateTime.Now;
                        warn.State = true;
                        await _warnRepository.UpdateAsync(warn);
                    }
                }

                foreach (var item in list)
                {
                    if (item[0] == null || item[1] == null || item[2] == null || item[3] == null) continue;
                    if (warns.Any(c => c.DeviceNum.Equals(item[0].ToString()) && c.WarnContent.Equals(item[2].ToString()))) continue;
                    DateTime t = DateTime.Parse(item[3].ToString());
                    var war = new Warn()
                    {
                        DeviceNum = item[0].ToString(),
                        State = false,
                        WarnContent = item[2].ToString(),
                        WarnNum = item[1].ToString(),
                        WarnTime = t.AddHours(-8)
                    };
                    await _warnRepository.InsertAsync(war);
                }
            }
        }
        /// <summary>
        /// 轮询报警
        /// </summary>
        /// <returns></returns>
        public async Task ForWarn()
        {
            var warns = await _warnRepository.GetAllListAsync(c => !c.State);
            var users = await GetUserPointsFromCache();
            var t = await GetOrgUserFromCache();
            foreach (var ou in t)
            {
                var user = users.Where(w => w.UserId.ToLower() == ou.ToLower()).Select(c => c.PointId).ToList();
                var forwarn = warns.Where(c => user.Contains(c.DeviceNum));
                var temp = "";
                forwarn.ForEach(warn =>
                {
                    var info = YtConsts.Types.FirstOrDefault(c => c.Type.Equals(warn.WarnNum));
                    var time = (DateTime.Now - warn.WarnTime).TotalHours;
                   
                    time = Math.Round(time, 2);
                    if (time <= 2)
                    {
                        temp += $@"设备:{warn.DeviceNum}在{warn.WarnTime}时间出现{info?.Chinese}类型的问题已持续{time}小时,请尽快处理\n
                                 <a href='https://open.weixin.qq.com/connect/oauth2/authorize?appid=ww480b7545345a38f7&redirect_uri=http://wx.youyinkeji.cn/#/author&response_type=code&scope=snsapi_userinfo&agentid=1000002&state=STATE#wechat_redirect'>报警处理中心</a>";

                    }
                });
                if (!temp.IsNullOrWhiteSpace())
                {
                    await SendMessage(temp, ou);
                }

            }

        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> SendMessage(string content, string user)
        {
            var users = await GetOrgUserFromCache();
            if (!users.Contains(user)) return null;
            var token = await GetTokenFromCache();
            string url = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={token}";
            var result = HttpHandler.PostJson<JObject>(url, JsonConvert.SerializeObject(new
            {
                touser = user,
                msgtype = "text",
                agentid = 1000002,
                text = new
                {
                    content = content
                }
            }));
            return result;
        }
        /// <summary>
        /// 下载保存多媒体文件,返回多媒体保存路径  
        /// </summary>
        /// <param name="media"></param>
        /// <returns></returns>
        private async Task<Guid> GetMultimedia(string media)
        {
            var token = await GetTokenFromCache();
            string url =
                $"https://qyapi.weixin.qq.com/cgi-bin/media/get?access_token={token}&media_id={media}";
            Guid name = Guid.NewGuid();
            var path = "/Files/Wechat/";
            string ext = string.Empty;
            Stream stream = DownLoadFielToMemoryStream(url, out ext);
            if (stream != null)
            {
                var length = stream.Length;
                string fp = Path.Combine(_appFolders.ImageFolder, "WeChat/");
                if (!Directory.Exists(fp))
                {
                    Directory.CreateDirectory(fp);
                }
                var temp = $"{name:N}.{ext}";
                var filepath = Path.Combine(fp, temp);
                using (var fileStream = File.Create(filepath))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                    fileStream.Flush();
                }
                stream.Close();
                await _objectManager.SaveAsync(new BinaryObject()
                {
                    Id = name,
                    Name = temp,
                    Url = path + temp,
                    Suffix = "." + ext,
                    Size = length
                });
            }
            return name;
        }
        #region 工具类
        /// <summary>
        /// 创建时间戳
        /// </summary>
        /// <returns></returns>
        private static long CreatenTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        /// 从url读取内容到内存MemoryStream流中
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        private static Stream DownLoadFielToMemoryStream(string url, out string ext)
        {
            var wreq = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = wreq.GetResponse() as HttpWebResponse;
            var filename = response.GetResponseHeader("Content-disposition");
            ext = filename.Split("=").Last().Split(".").Last().Replace("\"", "");
            MemoryStream ms = null;
            using (var stream = response.GetResponseStream())
            {
                Byte[] buffer = new Byte[response.ContentLength];
                int offset = 0, actuallyRead = 0;
                do
                {
                    actuallyRead = stream.Read(buffer, offset, buffer.Length - offset);
                    offset += actuallyRead;
                }
                while (actuallyRead > 0);
                ms = new MemoryStream(buffer);
            }
            response.Close();
            return ms;
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
        /// 获取token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetJsapiTicket()
        {

            var token = await GetTokenFromCache();
            var url = $"https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token={token}";
            var result = await HttpHandler.GetAsync<dynamic>(url);
            if (result != null)
            {
                return result.ticket;
            }
            throw new UserFriendlyException("票据不存在");
        }

        private async Task<List<UserPoint>> GetUserPointsFromCache()
        {
            return await _cacheManager.GetCache(OrgCacheName.UserPointCache)
                .GetAsync(OrgCacheName.UserPointCache, async () => await GetUserPointsFromDb());
        }
        /// <summary>
        /// db product
        /// </summary>
        /// <returns></returns>
        private async Task<List<UserPoint>> GetUserPointsFromDb()
        {
            var result = await _userPointRepository.GetAllListAsync();
            return result;
        }
        /// <summary>
        /// 获取fulltoken
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetFullToken()
        {
            var url =
              $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={CorpId}&corpsecret={Secret}";
            var result = await HttpHandler.GetAsync<dynamic>(url);
            return result.access_token;
        }
        /// <summary>
        /// 获取accesstoken
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetAccessToken()
        {

            var url =
              $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={CorpId}&corpsecret={SecretA}";
            var result = await HttpHandler.GetAsync<dynamic>(url);
            return result.access_token;
        }
        #endregion
    }

}
