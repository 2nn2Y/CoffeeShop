using System;
using System.Collections.Generic;

namespace YT.ThreeData.Dtos
{
    /// <summary>
    /// 
    /// </summary>
   public class SignDto
    {
        /// <summary>
        /// 签到人id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PointId { get; set; }
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
        /// 
        /// </summary>
        public  List<SignImageAndInfo> SignProfiles { get; set; }
    }
    /// <summary>
    /// temp
    /// </summary>
    public class SignImageAndInfo
    {
        /// <summary>
        /// media
        /// </summary>
        public string MedeaId { get; set; }
        /// <summary>
        /// url
        /// </summary>
        public string Src { get; set; }
    }

    /// <summary>
    /// 签到统计dto
    /// </summary>
    public class SignStaticialDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 日签到
        /// </summary>
        public int DaySign { get; set; }
        /// <summary>
        /// 签到次数
        /// </summary>
        public int TimeSign { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 结束
        /// </summary>
        public DateTime? End { get; set; }
    }

    /// <summary>
    /// 签到详情dto
    /// </summary>
    public class SignDetailDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 签到地
        /// </summary>
        public string SignLocation { get; set; }
        /// <summary>
        /// 签到次数
        /// </summary>
        public bool IsSign { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// 维度
        /// </summary>
        public double? Dimension { get; set; }
        /// <summary>
        /// 签到图片
        /// </summary>
        public List<string> Profiles { get; set; }
        /// <summary>
        /// 签到时间
        /// </summary>
        public DateTime? CreationTime { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 结束
        /// </summary>
        public DateTime? End { get; set; }
    }
}
