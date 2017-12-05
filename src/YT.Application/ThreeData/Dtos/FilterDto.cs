using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;

namespace YT.ThreeData.Dtos
{
    /// <summary>
    /// 过滤dto
    /// </summary>
   public class FilterDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public int ProductId { get; set; }
    }
    /// <summary>
    /// 报警集合
    /// </summary>

    public class WarnDto
    {
        /// <summary>
        /// 正常
        /// </summary>
        public List<MobileWarnDto> Normal { get; set; }
        /// <summary>
        /// 异常
        /// </summary>
        public List<MobileWarnDto> Anomaly { get; set; }
    }
    /// <summary>
    /// 报警信息
    /// </summary>
    public class MobileWarnDto : EntityDto
    {
        /// <summary>
        /// 设备id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备名
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 签到地点列表
    /// </summary>
    public class SignPointDto:EntityDto
    {
        /// <summary>
        /// 位置
        /// </summary>
        public string Point { get; set; }
        /// <summary>
        /// 点位id
        /// </summary>
        public string PointId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
    }
}
