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
