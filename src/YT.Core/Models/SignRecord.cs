using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using YT.Storage;

namespace YT.Models
{
    /// <summary>
    /// 签到记录
    /// </summary>
    [Table("signrecord")]
    public class SignRecord : CreationAuditedEntity
    {
        /// <summary>
        /// 签到人id
        /// </summary>
        public string UserId { get; set; }
        public int PointId { get; set; }
        /// <summary>
        /// 点位
        /// </summary>
        public virtual Point Point { get; set; }
        /// <summary>
        /// 签到状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 维度
        /// </summary>
        public double Dimension { get; set; }
        /// <summary>
        /// 签到图片
        /// </summary>
        [ForeignKey("SignId")]
        public virtual ICollection<SignProfile> SignProfiles { get; set; }
    }
    /// <summary>
    /// 用户和点位对应关系
    /// </summary>
    [Table("UserPoint")]
    public class UserPoint : CreationAuditedEntity
    {
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        public string PointId { get; set; }
    }
    /// <summary>
    /// 签到图片记录
    /// </summary>
    [Table("SignProfile")]
    public class SignProfile : CreationAuditedEntity
    {
        /// <summary>
        /// 签到id
        /// </summary>
        public int SignId { get; set; }
        /// <summary>
        /// 微信资源id
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// dto
        /// 
        /// </summary>
        public virtual SignRecord Sign { get; set; }
        /// <summary>
        /// profile
        /// </summary>
        public Guid? ProfileId { get; set; }
        /// <summary>
        /// profiledto
        /// </summary>
        public virtual BinaryObject Profile { get; set; }
    }

    [Table("product")]
    public class Product : CreationAuditedEntity
    {
        public Product() { }

        public Product(int id, string name)
        {
            ProductId = id;
            ProductName = name;
        }

        /// <summary>
        /// 产品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        ///是否卡券
        /// </summary>
        public bool IsCard { get; set; }
        /// <summary>
        /// 抵扣金额
        /// </summary>
        public int? Cost { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string ImageUrl { get; set; }
    }
    /// <summary>
    /// 点位信息
    /// </summary>
    [Table("point")]
    public class Point : CreationAuditedEntity
    {
        /// <summary>
        /// 学校
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNum { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 点位
        /// </summary>
        public string PointName { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }
    }
    /// <summary>
    /// 订单信息
    /// </summary>
    [Table("order")]
    public class Order : CreationAuditedEntity
    {
        /// <summary>
        /// 设备号
        /// </summary>
        public string DeviceNum { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 产品好
        /// </summary>
        public int ProductNum { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime Date { get; set; }
    }
    /// <summary>
    /// 报警信息
    /// </summary>
    [Table("warn")]
    public class Warn : CreationAuditedEntity
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNum { get; set; }
        /// <summary>
        /// 警告编号
        /// </summary>
        public string WarnNum { get; set; }
        /// <summary>
        /// 警告内容
        /// </summary>
        public string WarnContent { get; set; }
        /// <summary>
        /// 报警时间
        /// </summary>
        public DateTime WarnTime { get; set; }
        /// <summary>
        /// 维修状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 故障解决时间
        /// </summary>
        public DateTime? DealTime { get; set; }
        /// <summary>
        /// 设置时间
        /// </summary>
        public DateTime? SetTime { get; set; }
    }
    /// <summary>
    /// 商城订单
    /// </summary>
    [Table("StoreOrder")]
    public class StoreOrder : CreationAuditedEntity
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 微信支付订单
        /// </summary>
        public string WechatOrder { get; set; }
        /// <summary>
        /// 订单结果描述
        /// </summary>
        public string Reson { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PayType PayType { get; set; }
        public OrderType OrderType { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public bool? PayState { get; set; }
        /// <summary>
        /// 使用代金券
        /// </summary>
        public Guid? UseCard { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public bool? OrderState { get; set; }
        /// <summary>
        /// 机器编号
        /// </summary>

        public string DeviceNum { get; set; }
        /// <summary>
        /// 回掉地址
        /// </summary>
        public string NoticeUrl { get; set; }
        /// <summary>
        /// 通信密钥
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
    
        /// <summary>
        /// 商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 快捷码
        /// </summary>
        public string FastCode { get; set; }
    }
    [Table("StoreUser")]
    public class StoreUser : CreationAuditedEntity
    {
        /// <summary>
        /// openid
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public int Balance { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>

        public string NickName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string ImageUrl { get; set; }
    }
    /// <summary>
    /// 用戶卡券
    /// </summary>
    [Table("UserCard")]
    public class UserCard : CreationAuditedEntity
    {
        /// <summary>
        /// openid
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 券名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 抵扣金额
        /// </summary>
        public  int Cost { get; set; }
        /// <summary>
        /// 唯一編號
        /// </summary>
        public  Guid Key { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
    }
}
