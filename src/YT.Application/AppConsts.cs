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
    public class CacheName
    {
        /// <summary>
        /// �˵�Ȩ��
        /// </summary>
        public const string MenuCache = "Milk.Cache.Menu";
        /// <summary>
        /// ΢��token
        /// </summary>
        public const string WeChatToken = "Milk.WeChat.ACCESS_TOKEN";
        /// <summary>
        /// ȫ��token
        /// </summary>
        public const string FullToken = "Milk.WeChat.FullToken";
        /// <summary>
        /// jsapiticket
        /// </summary>
        public const string TicketToken = "Milk.WeChat.Jsapi_Ticket";
        /// <summary>
        /// ��������Ա
        /// </summary>
        public const string OrgUser = "Milk.WeChat.OrgUser";
        /// <summary>
        /// Ȩ��const
        /// </summary>
        public const string PermissionCache = "Milk.Cache.Permission";
        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public const string ProductCache = "Milk.Three.ProductCache";
        /// <summary>
        /// ��λ����
        /// </summary>
        public const string PointCache = "Milk.Three.PointCache";
        /// <summary>
        /// �û���λ����
        /// </summary>
        public const string UserPointCache = "Milk.Three.UserPointCache";

    }
}
