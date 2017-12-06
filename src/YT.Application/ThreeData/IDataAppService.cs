using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using YT.Dto;
using YT.ThreeData.Dtos;

namespace YT.ThreeData
{
    /// <summary>
    /// 第三方接口
    /// </summary>
   public interface IDataAppService:IApplicationService
    {

        /// <summary>
        /// 获取成交订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
          Task<PagedResultDto<OrderDetail>> GetOrderDetails(GetOrderInput input);
        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ProductSaleDto>> GetProductsSale(GetProductSaleInput input);
        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ProductSaleDto>> GetDeviceProductsSale(GetOrderInput input);

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ProductSaleDto>> GetAreaProductsSale(GetSaleInput input);

        /// <summary>
        /// 获取支付渠道销售
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ProductSaleDto>> GetPayTypeSale(GetOrderInput input);

        /// <summary>
        /// 获取报警信息报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<WarnDetailDto>> GetWarns(GetWarnInfoInput input);
        /// <summary>
        /// 获取时间区域统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<TimeAreaDto>> GetTimeAreaSale(GetOrderInput input);

        /// <summary>
        /// 根据设备统计警告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
          Task<PagedResultDto<DeviceWarnDto>> GetWarnByDevice(GetWarnInput input);
        /// <summary>
        /// 根据设备统计警告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
          Task<PagedResultDto<DeviceWarnDto>> GetWarnByUser(GetOrderInput input);

        /// <summary>
        /// 获取人员签到统计
        /// </summary>
        /// <returns></returns>
         Task<PagedResultDto<SignStaticialDto>> GetSignsByUser(GetOrderInput input);


        /// <summary>
        /// 获取统计明细
        /// </summary>
        /// <returns></returns>
        Task<PagedResultDto<SignDetailDto>> GetSignsDetail(GetSignInput input);
        #region 导出
        /// <summary>
        /// 获取成交订单
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        Task<FileDto>  ExportOrderDetails(GetOrderInput orders);
        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        Task<FileDto> ExportProductsSale(GetProductSaleInput orders);

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        Task<FileDto> ExportDeviceProductsSale(GetOrderInput orders);


        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        Task<FileDto> ExportAreaProductsSale(GetSaleInput orders);


        /// <summary>
        /// 获取支付渠道销售
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        Task<FileDto> ExportPayTypeSale(GetOrderInput orders);


        /// <summary>
        /// 获取时间区域统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<FileDto> ExportTimeAreaSale(GetOrderInput input);

        /// <summary>
        /// 根据设备统计警告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
          Task<FileDto> ExportWarnByDevice(GetWarnInput input);

        /// <summary>
        /// 根据设备统计警告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<FileDto> ExportWarnByUser(GetOrderInput input);

        /// <summary>
        /// 获取人员签到统计
        /// </summary>
        /// <returns></returns>
         Task<FileDto> ExportSignsByUser(GetOrderInput input);

        /// <summary>
        /// 获取统计明细
        /// </summary>
        /// <returns></returns>
         Task<FileDto> ExportSignsDetail(GetSignInput input);
        /// <summary>
        /// 获取报警信息报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
          Task<FileDto> ExportWarns(GetWarnInfoInput input);

        #endregion
    }
}
