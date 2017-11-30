using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT.ThreeData.Dtos
{
    /// <summary>
    /// 订单详情
    /// </summary>
  public  class OrderDetail
    {
        /// <summary>
        /// 机器号
        /// </summary>
        public string DeviceNum { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 产品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public string PayType { get; set; }
    }
    /// <summary>
    /// 产品销售dto
    /// </summary>
    public class ProductSaleDto
    {
        /// <summary>
        /// 支付类型
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// chengshi
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 点位id
        /// </summary>
        public string DeviceNum { get; set; }
        /// <summary>
        /// 点位名
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 销售额
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 截至
        /// </summary>
        public DateTime? End { get; set; }
    }
    /// <summary>
    /// 时间区域统计
    /// </summary>
    public class TimeAreaDto
    {
        /// <summary>
        /// ctor
        /// </summary>
        public TimeAreaDto() { }
      /// <summary>
      /// ctor
      /// </summary>
      /// <param name="area"></param>
      /// <param name="temp"></param>
        public TimeAreaDto(string area,string temp)
        {
            Area = area;
            Temp = temp;
        }
        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }    
        /// <summary>
        /// 临时数据
        /// </summary>
        public string Temp { get; set; }
        /// <summary>
        /// 点位
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 销售额
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 戒指
        /// </summary>
        public DateTime? End { get; set; }
    }
}
