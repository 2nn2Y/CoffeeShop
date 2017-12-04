using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;

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
        /// 价格
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 提货码
        /// </summary>
        public string Code { get; set; }
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
}
