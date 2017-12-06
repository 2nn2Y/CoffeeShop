using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Runtime.Validation;
using YT.Dto;

namespace YT.ThreeData.Dtos
{
  /// <summary>
  /// 
  /// </summary>
    public class GetOrderInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 权限过滤
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 截至
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
    }
    /// <summary>
    /// 获取销售报表
    /// </summary>
    public class GetSaleInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 权限过滤
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 产品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 截至
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
    }

    /// <summary>
    /// 获取报警inpout
    /// </summary>
    public class GetWarnInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 权限过滤
        /// </summary>
        public string Point { get; set; }
        /// <summary>
        /// 权限过滤
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 权限过滤
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 截至
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
    }

    /// <summary>
    /// 获取报警inpout
    /// </summary>
    public class GetWarnInfoInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 是否解决
        /// </summary>
        public bool? IsDeal { get; set; }
        /// <summary>
        /// 时间区间
        /// </summary>
        public int? Left { get; set; }
        /// <summary>
        /// 时间区间
        /// </summary>
        public int? Right { get; set; }
        /// <summary>
        /// 权限过滤
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 权限过滤
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 截至
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class GetSignInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 设备
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserNum { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 截至
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
    }

    /// <summary>
    /// 产品销量input
    /// </summary>
    public class GetProductSaleInput : PagedAndSortedInputDto, IShouldNormalize
    {
        
        /// <summary>
        /// 权限过滤
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 截至
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
    }
}