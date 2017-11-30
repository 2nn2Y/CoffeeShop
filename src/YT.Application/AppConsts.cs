using System.Collections.Generic;
using YT.Dto;

namespace YT
{
    /// <summary>
    /// Ӧ�ò㳣������
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
        /// ��̬Ȩ������
        /// </summary>
        public static List<DictionaryDto> StaticPermissions =>

            new List<DictionaryDto>()
            {
                new DictionaryDto("create","����"),
                new DictionaryDto("edit","�༭"),
                new DictionaryDto("delete","ɾ��"),
                new DictionaryDto("chart","ͼ��"),
                new DictionaryDto("auth","��Ȩ"),
            };
   
    }
    /// <summary>
    /// ���涨��
    /// </summary>
    public class SystemCacheName
    {
        /// <summary>
        /// �˵�Ȩ��
        /// </summary>
        public const string MenuCache = "System.Cache.Menu";
   
        /// <summary>
        /// Ȩ��const
        /// </summary>
        public const string PermissionCache = "System.Cache.Permission";
    }
    /// <summary>
    /// ���涨��
    /// </summary>
    public class CoffeeCacheName
    {
        /// <summary>
        /// ΢��token
        /// </summary>
        public const string WeChatToken = "Coffee.WeChat.ACCESS_TOKEN";
        /// <summary>
        /// ȫ��token
        /// </summary>
        public const string UserToken = "Coffee.WeChat.UserToken";
        /// <summary>
        /// jsapiticket
        /// </summary>
        public const string TicketToken = "Coffee.WeChat.Jsapi_Ticket";
      
    }


    /// <summary>
    /// ���涨��
    /// </summary>
    public class OrgCacheName
    {
        /// <summary>
        /// ΢��token
        /// </summary>
        public const string WeChatToken = "Org.WeChat.ACCESS_TOKEN";
        /// <summary>
        /// ȫ��token
        /// </summary>
        public const string FullToken = "Org.WeChat.FullToken";
        /// <summary>
        /// jsapiticket
        /// </summary>
        public const string TicketToken = "Org.WeChat.Jsapi_Ticket";
        /// <summary>
        /// ��������Ա
        /// </summary>
        public const string OrgUser = "Org.WeChat.OrgUser";
        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public const string ProductCache = "Org.Three.ProductCache";
        /// <summary>
        /// ��λ����
        /// </summary>
        public const string PointCache = "Org.Three.PointCache";
        /// <summary>
        /// �û���λ����
        /// </summary>
        public const string UserPointCache = "Org.Three.UserPointCache";

    }
}
