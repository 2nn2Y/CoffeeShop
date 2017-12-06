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
    /// 设备报警dto
    /// </summary>
  public  class DeviceWarnDto:EntityDto
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNum { get; set; }
        /// <summary>
        /// 设备名
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 报警类型
        /// </summary>
        public string WarnType { get; set; }
        /// <summary>
        /// 报警数量
        /// </summary>
        public int WarnCount { get; set; }
        /// <summary>
        /// 处理刷俩
        /// </summary>
        public int DealCount { get; set; }
        /// <summary>
        /// 平均处理时间
        /// </summary>
        public double PerTime { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 开始和i时间
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 戒指时间
        /// </summary>
        public DateTime? End { get; set; }
    }
    /// <summary>
    /// 报警详情信息
    /// </summary>
    public class WarnDetailDto
    {
        /// <summary>
        /// key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNum { get; set; }
        /// <summary>
        /// 点位名称
        /// </summary>
        public string PointName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 警告类型
        /// </summary>
        public string WarnType { get; set; }
        /// <summary>
        /// 报警日期
        /// </summary>
        public DateTime? WarnDate { get; set; }
        /// <summary>
        /// 解决日期
        /// </summary>
        public DateTime? SolveDate { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? SetTime { get; set; }
        /// <summary>
        /// 解决时长
        /// </summary>
        public int SolveTime { get; set; }
        /// <summary>
        /// 未解决时间
        /// </summary>
        public double UnSolveTime { get; set; }
        /// <summary>
        /// 是否解决
        /// </summary>
        public bool IsSolve { get; set; }
        /// <summary>
        /// 运维人员
        /// </summary>
        public string UserName { get; set; }
    }
}
