using System.Collections.Generic;
using YT.Dto;

namespace YT
{
    /// <summary>
    /// 应用层常量定义
    /// </summary>
    public class AppConsts
    {
        /// <summary>
        /// Default page size for paged requests.
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Maximum allowed page size for paged requests.
        /// </summary>
        public const int MaxPageSize = 1000;
        /// <summary>
        /// 静态权限类型
        /// </summary>
        public static List<DictionaryDto> StaticPermissions =>

            new List<DictionaryDto>()
            {
                new DictionaryDto("create","新增"),
                new DictionaryDto("edit","编辑"),
                new DictionaryDto("delete","删除"),
                new DictionaryDto("chart","图表"),
                new DictionaryDto("auth","授权"),
            };
   
    }
    /// <summary>
    /// 缓存定义
    /// </summary>
    public class SystemCacheName
    {
        /// <summary>
        /// 菜单权限
        /// </summary>
        public const string MenuCache = "System.Cache.Menu";
   
        /// <summary>
        /// 权限const
        /// </summary>
        public const string PermissionCache = "System.Cache.Permission";
    }
    /// <summary>
    /// 缓存定义
    /// </summary>
    public class CoffeeCacheName
    {
        /// <summary>
        /// 微信token
        /// </summary>
        public const string WeChatToken = "Coffee.WeChat.ACCESS_TOKEN";
        /// <summary>
        /// 全局token
        /// </summary>
        public const string UserToken = "Coffee.WeChat.UserToken";
        /// <summary>
        /// jsapiticket
        /// </summary>
        public const string TicketToken = "Coffee.WeChat.Jsapi_Ticket";
      
    }


    /// <summary>
    /// 缓存定义
    /// </summary>
    public class OrgCacheName
    {
        /// <summary>
        /// 微信token
        /// </summary>
        public const string WeChatToken = "Org.WeChat.ACCESS_TOKEN";
        /// <summary>
        /// 全局token
        /// </summary>
        public const string FullToken = "Org.WeChat.FullToken";
        /// <summary>
        /// jsapiticket
        /// </summary>
        public const string TicketToken = "Org.WeChat.Jsapi_Ticket";
        /// <summary>
        /// 机构下人员
        /// </summary>
        public const string OrgUser = "Org.WeChat.OrgUser";
        /// <summary>
        /// 产品缓存
        /// </summary>
        public const string ProductCache = "Org.Three.ProductCache";
        /// <summary>
        /// 点位缓存
        /// </summary>
        public const string PointCache = "Org.Three.PointCache";
        /// <summary>
        /// 用户点位缓存
        /// </summary>
        public const string UserPointCache = "Org.Three.UserPointCache";

    }
}
