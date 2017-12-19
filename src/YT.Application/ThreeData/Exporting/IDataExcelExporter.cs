using System.Collections.Generic;
using YT.Authorization.Users.Dto;
using YT.Dto;
using YT.ThreeData.Dtos;

namespace YT.ThreeData.Exporting
{/// <summary>
/// 
/// </summary>
    public interface IDataExcelExporter
    {/// <summary>
     /// 到处用户连接信息
     /// </summary>
     /// <param name="userListDtos"></param>
     /// <returns></returns>
        FileDto ExportToFile(List<UserListDto> userListDtos);

   
      
        /// <summary>
        /// 获取成交订单
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportOrderDetails(List<OrderDetail> orders);
        /// <summary>
        /// 到处订单
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        FileDto ExportStoreOrders(List<StoreOrderListDto> orders);

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportProductsSale(List<ProductSaleDto> orders);
     

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportDeviceProductsSale(List<ProductSaleDto> orders);
        

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportAreaProductsSale(List<ProductSaleDto> orders);
      

        /// <summary>
        /// 获取支付渠道销售
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportPayTypeSale(List<ProductSaleDto> orders);
     

        /// <summary>
        /// 获取时间区域统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
         FileDto ExportTimeAreaSale(List<TimeAreaDto> input);

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportWarnByDevice(List<DeviceWarnDto> orders);

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportWarnByUser(List<DeviceWarnDto> orders);

        /// <summary>
        /// 导出签到详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        FileDto ExportSignsByUser(List<SignStaticialDto> input);

        /// <summary>
        /// 导出签到详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        FileDto ExportSignsDetail(List<SignDetailDto> input);
        /// <summary>
        /// 到处报警信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        FileDto ExportWarns(List<WarnDetailDto> input);
    }
}