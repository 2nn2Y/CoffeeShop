using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using YT.Models;

namespace YT.ThreeData.Dtos
{
    /// <summary>
    /// 咖啡产品类
    /// </summary>
    public class CoffeeProduct
    {
        /// <summary>
        /// 
        /// </summary>
        public CoffeeProduct() { }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="des"></param>
        /// <param name="image"></param>
        /// <param name="price"></param>
        public CoffeeProduct(int id, string name, string des, string image, int price)
        {
            Id = id;
            ProductName = name;
            Description = des;
            ImageUrl = image;
            Price = price;
        }
        /// <summary>
        /// 产品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 图片连接
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }

    }
    /// <summary>
    /// 创建订单input
    /// </summary>
    public class InsertOrderInput
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public  OrderType OrderType { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }

  
        /// <summary>
        /// 是否使用优惠券
        /// </summary>
        public Guid? Key { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string Order { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 快捷码
        /// </summary>
        public string FastCode { get; set; }
    }
    /// <summary>
    /// 订单详情
    /// </summary>
    public class StoreOrderDto : EntityDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 口味
        /// </summary>
        public string FastCode { get; set; }
    }
    /// <summary>
    /// 取货input
    /// </summary>
    public class PickOrderInput
    {
        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNum { get; set; }
    }
    /// <summary>
    /// 产品和卡券dto
    /// </summary>
    public class ProductAndCardDto
    {
        /// <summary>
        /// 产品信息
        /// </summary>
        public  Product Product { get; set; }
        /// <summary>
        /// 卡圈信息
        /// </summary>
        public List<CardInfo> Cards { get; set; }
    }
    /// <summary>
    /// 卡券信息dto
    /// </summary>
    public class CardInfo
    {
        /// <summary>
        /// key
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// xianshi
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public int Cost { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }
    }
}
