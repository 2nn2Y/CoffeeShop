using System.ComponentModel.DataAnnotations;

namespace YT.WebApi.Models
{
    public class LoginModel
    {

        [Required]
        public string UsernameOrEmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
    /// <summary>
    /// �ֻ���¼
    /// </summary>
    public class MobileLoginModel
    {
        [Required]
        public string Mobile { get; set; }

        [Required]
        public string Password { get; set; }
    }
    /// <summary>
    /// ��ά�����
    /// </summary>
    public class QrcodeInput
    {
        /// <summary>
        /// �������
        /// </summary>
        public string AssetId { get; set; }   
        /// <summary>
        /// ��Ʒ���
        /// </summary>
        public int ProductNum { get; set; }
        /// <summary>
        /// �ص�url
        /// </summary>
        public string Notify_Url { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// ��֤key
        /// </summary>
        public string Key { get; set; }
    }

    public class IceCallInput
    {
        /// <summary>
        /// �豸id
        /// </summary>
        public string AssetId { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// ״̬
        /// </summary>
        public string DeliverStatus { get; set; }
    }
    public class JackCallInput
    {
        /// <summary>
        /// �豸id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        public string Vmc { get; set; }
        /// <summary>
        /// ״̬
        /// </summary>
        public string Pid { get; set; }
        public string Mac { get; set; }
    }

    public class WeChatXml
    {
            public string appid { get; set; }
            public string attach { get; set; }
            public string bank_type { get; set; }
            public string fee_type { get; set; }
            public string is_subscribe { get; set; }
            public string mch_id { get; set; }
            public string openid { get; set; }
            public string out_trade_no { get; set; }
            public string result_code { get; set; }
            public string return_code { get; set; }
            public string sign { get; set; }
            public string sub_mch_id { get; set; }
            public string time_end { get; set; }
            public string total_fee { get; set; }
            public string coupon_fee { get; set; }
            public string coupon_count { get; set; }
            public string coupon_type { get; set; }
            public string trade_type { get; set; }
            public string transaction_id { get; set; }
    }
}
