using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using YT.Authorization.Users.Dto;
using YT.DataExporting.Excel.EpPlus;
using YT.Dto;
using YT.ThreeData.Dtos;

namespace YT.ThreeData.Exporting
{/// <summary>
/// 
/// </summary>
    public class DataExcelExporter : EpPlusExcelExporterBase, IDataExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="timeZoneConverter"></param>
        /// <param name="abpSession"></param>
        public DataExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }
        /// <summary>
        /// 导出用户信息
        /// </summary>
        /// <param name="userListDtos"></param>
        /// <returns></returns>
        public FileDto ExportToFile(List<UserListDto> userListDtos)
        {
            return CreateExcelPackage(
                "UserList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Users"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("UserName"),
                        L("PhoneNumber"),
                        L("Roles"),
                        L("LastLoginTime"),
                        L("Active"),
                        L("CreationTime")
                        );

                    AddObjects(
                        sheet, 2, userListDtos,
                        _ => _.Name,
                        _ => _.UserName,
                        _ => _.PhoneNumber,
                        _ => _.Roles.Select(r => r.RoleName).JoinAsString(", "),
                        _ => _timeZoneConverter.Convert(_.LastLoginTime, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.IsActive,
                        _ => _timeZoneConverter.Convert(_.CreationTime, _abpSession.TenantId, _abpSession.GetUserId())
                        );

                    //Formatting cells

                    var lastLoginTimeColumn = sheet.Column(8);
                    lastLoginTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    var creationTimeColumn = sheet.Column(10);
                    creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    for (var i = 1; i <= 10; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }

        /// <summary>
        /// 获取成交订单
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportOrderDetails(List<OrderDetail> orders)
        {
            return CreateExcelPackage(
                  "订单详情.xlsx",
                  excelPackage =>
                  {
                      var sheet = excelPackage.Workbook.Worksheets.Add("订单详情");
                      sheet.OutLineApplyStyle = true;
                      AddHeader(
                          sheet,
                         "机器号",
                         "订单号",
                         "产品名",
                         "价格",
                         "支付类型",
                         "成交时间"
                          );
                      AddObjects(
                          sheet, 2, orders,
                          _ => _.DeviceNum,
                          _ => _.OrderNum,
                          _ => _.ProductName,
                          _ => _.Price * 1.0 / 100,
                          _ => ChangeType(_.PayType),
                          _ => _timeZoneConverter.Convert(_.Date, _abpSession.TenantId, _abpSession.GetUserId())
                          );
                      var lastLoginTimeColumn = sheet.Column(6);
                      lastLoginTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                      for (var i = 1; i <= 10; i++)
                      {
                          sheet.Column(i).AutoFit();
                      }
                  });
        }
        /// <summary>
        /// 导出订单
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportStoreOrders(List<StoreOrderListDto> orders)
        {
            return CreateExcelPackage(
                   "订单详情.xlsx",
                   excelPackage =>
                   {
                       var sheet = excelPackage.Workbook.Worksheets.Add("订单详情");
                       sheet.OutLineApplyStyle = true;
                       AddHeader(
                           sheet,
                          "用户id",
                          "产品名",
                          "订单号",
                          "价格",
                          "支付类型",
                          "支付状态",
                          "订单状态",
                          "成交时间"
                           );
                       AddObjects(
                           sheet, 2, orders,
                           _ => _.UserName,
                           _ => _.ProductName,
                           _ => _.OrderNum,
                           _ => _.Price*1.0/100,
                           _ =>
                               _.PayType == PayType.BalancePay
                                   ? "余额支付"
                                   : (_.PayType == PayType.ActivityPay
                                       ? "活动支付"
                                       : (_.PayType == PayType.LinePay ? "在线支付" : "充值")),
                        _ => _.PayState,
                        _ => _.OrderState,
                           _ => _timeZoneConverter.Convert(_.DateTime, _abpSession.TenantId, _abpSession.GetUserId())
                       );
                       var lastLoginTimeColumn = sheet.Column(6);
                       lastLoginTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                       for (var i = 1; i <= 10; i++)
                       {
                           sheet.Column(i).AutoFit();
                       }
                   });
        }


        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportProductsSale(List<ProductSaleDto> orders)
        {
            return CreateExcelPackage(
                   "产品销量.xlsx",
                   excelPackage =>
                   {
                       var sheet = excelPackage.Workbook.Worksheets.Add("产品销量");
                       sheet.OutLineApplyStyle = true;
                       AddHeader(
                           sheet,
                          "产品名称",
                          "销量",
                          "总价",
                          "开始时间",
                          "结束时间"
                           );
                       AddObjects(
                           sheet, 2, orders,
                           _ => _.ProductName,
                           _ => _.Count,
                           _ => _.Price * 1.0 / 100,
                           _ => _timeZoneConverter.Convert(_.Start, _abpSession.TenantId, _abpSession.GetUserId()),
                           _ => _timeZoneConverter.Convert(_.End, _abpSession.TenantId, _abpSession.GetUserId())
                           );
                       var a = sheet.Column(4);
                       a.Style.Numberformat.Format = "yyyy-mm-dd";
                       var b = sheet.Column(5);
                       b.Style.Numberformat.Format = "yyyy-mm-dd";
                       for (var i = 1; i <= 10; i++)
                       {
                           sheet.Column(i).AutoFit();
                       }
                   });
        }

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportDeviceProductsSale(List<ProductSaleDto> orders)
        {
            return CreateExcelPackage(
                    "设备销量.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("设备销量");
                        sheet.OutLineApplyStyle = true;
                        AddHeader(
                            sheet,
                           "机器编号",
                           "设备点位",
                           "产品名称",
                           "销量",
                           "总价",
                           "开始时间",
                           "结束时间"
                            );
                        AddObjects(
                            sheet, 2, orders,
                            _ => _.DeviceNum,
                            _ => _.DeviceName,
                            _ => _.ProductName,
                            _ => _.Count,
                            _ => _.Price * 1.0 / 100,
                            _ => _timeZoneConverter.Convert(_.Start, _abpSession.TenantId, _abpSession.GetUserId()),
                            _ => _timeZoneConverter.Convert(_.End, _abpSession.TenantId, _abpSession.GetUserId())
                            );
                        var a = sheet.Column(6);
                        a.Style.Numberformat.Format = "yyyy-mm-dd";
                        var b = sheet.Column(7);
                        b.Style.Numberformat.Format = "yyyy-mm-dd";
                        for (var i = 1; i <= 10; i++)
                        {
                            sheet.Column(i).AutoFit();
                        }
                    });
        }

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportAreaProductsSale(List<ProductSaleDto> orders)
        {
            return CreateExcelPackage(
                    "区域销量.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("区域销量");
                        sheet.OutLineApplyStyle = true;
                        AddHeader(
                            sheet,
                           "区域",
                           "设备名称",
                           "产品名称",
                           "销量",
                           "总价",
                           "开始时间",
                           "结束时间"
                            );
                        AddObjects(
                            sheet, 2, orders,
                            _ => _.City,
                            _ => _.DeviceName,
                            _ => _.ProductName,
                            _ => _.Count,
                            _ => _.Price * 1.0 / 100,
                            _ => _timeZoneConverter.Convert(_.Start, _abpSession.TenantId, _abpSession.GetUserId()),
                            _ => _timeZoneConverter.Convert(_.End, _abpSession.TenantId, _abpSession.GetUserId())
                            );
                        var a = sheet.Column(5);
                        a.Style.Numberformat.Format = "yyyy-mm-dd";
                        var b = sheet.Column(6);
                        b.Style.Numberformat.Format = "yyyy-mm-dd";
                        for (var i = 1; i <= 10; i++)
                        {
                            sheet.Column(i).AutoFit();
                        }
                    });
        }

        /// <summary>
        /// 获取支付渠道销售
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportPayTypeSale(List<ProductSaleDto> orders)
        {
            return CreateExcelPackage(
                   "支付渠道.xlsx",
                   excelPackage =>
                   {
                       var sheet = excelPackage.Workbook.Worksheets.Add("支付渠道");
                       sheet.OutLineApplyStyle = true;
                       AddHeader(
                           sheet,
                          "支付渠道",
                          "区域",
                          "销量",
                          "总价",
                          "开始时间",
                          "结束时间"
                           );
                       AddObjects(
                           sheet, 2, orders,
                           _ => ChangeType(_.PayType),
                           _ => _.City,
                           _ => _.Count,
                           _ => _.Price * 1.0 / 100,
                           _ => _timeZoneConverter.Convert(_.Start, _abpSession.TenantId, _abpSession.GetUserId()),
                           _ => _timeZoneConverter.Convert(_.End, _abpSession.TenantId, _abpSession.GetUserId())
                           );
                       var a = sheet.Column(5);
                       a.Style.Numberformat.Format = "yyyy-mm-dd";
                       var b = sheet.Column(6);
                       b.Style.Numberformat.Format = "yyyy-mm-dd";
                       for (var i = 1; i <= 10; i++)
                       {
                           sheet.Column(i).AutoFit();
                       }
                   });
        }

        /// <summary>
        /// 获取时间区域统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public FileDto ExportTimeAreaSale(List<TimeAreaDto> input)
        {
            return CreateExcelPackage(
                  "时段销量.xlsx",
                  excelPackage =>
                  {
                      var sheet = excelPackage.Workbook.Worksheets.Add("时段销量");
                      sheet.OutLineApplyStyle = true;
                      AddHeader(
                          sheet,
                         "时段",
                         "区域",
                         "销量",
                         "总价",
                         "开始时间",
                         "结束时间"
                          );
                      AddObjects(
                          sheet, 2, input,
                          _ => _.Area,
                          _ => _.SchoolName,
                          _ => _.Count,
                          _ => _.Price * 1.0 / 100,
                          _ => _timeZoneConverter.Convert(_.Start, _abpSession.TenantId, _abpSession.GetUserId()),
                          _ => _timeZoneConverter.Convert(_.End, _abpSession.TenantId, _abpSession.GetUserId())
                          );
                      var a = sheet.Column(5);
                      a.Style.Numberformat.Format = "yyyy-mm-dd";
                      var b = sheet.Column(6);
                      b.Style.Numberformat.Format = "yyyy-mm-dd";
                      for (var i = 1; i <= 10; i++)
                      {
                          sheet.Column(i).AutoFit();
                      }
                  });
        }


        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportWarnByDevice(List<DeviceWarnDto> orders)
        {
            return CreateExcelPackage(
                    "故障统计设备.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("故障统计设备");
                        sheet.OutLineApplyStyle = true;
                        AddHeader(
                            sheet,
                           "机器编号",
                           "设备点位",
                           "故障类型",
                           "故障数量",
                           "解决数量",
                           "平均时间",
                           "运维人员",
                           "开始时间",
                           "结束时间"
                            );
                        AddObjects(
                            sheet, 2, orders,
                            _ => _.DeviceNum,
                            _ => _.DeviceName,
                            _ => _.WarnType,
                            _ => _.WarnCount,
                            _ => _.DealCount,
                            _ => _.PerTime,
                            _ => _.UserName,
                            _ => _timeZoneConverter.Convert(_.Start, _abpSession.TenantId, _abpSession.GetUserId()),
                            _ => _timeZoneConverter.Convert(_.End, _abpSession.TenantId, _abpSession.GetUserId())
                            );
                        var a = sheet.Column(8);
                        a.Style.Numberformat.Format = "yyyy-mm-dd";
                        var b = sheet.Column(9);
                        b.Style.Numberformat.Format = "yyyy-mm-dd";
                        for (var i = 1; i <= 10; i++)
                        {
                            sheet.Column(i).AutoFit();
                        }
                    });
        }

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportWarnByUser(List<DeviceWarnDto> orders)
        {
            return CreateExcelPackage(
                    "故障统计运维人员.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("故障统计运维人员");
                        sheet.OutLineApplyStyle = true;
                        AddHeader(
                            sheet,
                           "运维人员",
                           "故障数量",
                           "解决数量",
                           "平均时间",
                           "绩效考核",
                           "开始时间",
                           "结束时间"
                            );
                        AddObjects(
                            sheet, 2, orders,
                            _ => _.UserName,
                            _ => _.WarnCount,
                            _ => _.DealCount,
                            _ => _.PerTime,
                            _ => -180,
                            _ => _timeZoneConverter.Convert(_.Start, _abpSession.TenantId, _abpSession.GetUserId()),
                            _ => _timeZoneConverter.Convert(_.End, _abpSession.TenantId, _abpSession.GetUserId())
                            );
                        var a = sheet.Column(6);
                        a.Style.Numberformat.Format = "yyyy-mm-dd";
                        var b = sheet.Column(7);
                        b.Style.Numberformat.Format = "yyyy-mm-dd";
                        for (var i = 1; i <= 10; i++)
                        {
                            sheet.Column(i).AutoFit();
                        }
                    });
        }
        /// <summary>
        /// 导出签到统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public FileDto ExportSignsByUser(List<SignStaticialDto> input)
        {
            return CreateExcelPackage(
                   "签到统计.xlsx",
                   excelPackage =>
                   {
                       var sheet = excelPackage.Workbook.Worksheets.Add("签到统计");
                       sheet.OutLineApplyStyle = true;
                       AddHeader(
                           sheet,
                          "用户编号",
                          "用户名",
                          "签到次数",
                          "签到天数",
                          "开始时间",
                          "结束时间"
                           );
                       AddObjects(
                           sheet, 2, input,
                           _ => _.UserId,
                           _ => _.UserName,
                           _ => _.TimeSign,
                           _ => _.DaySign,
                           _ => _timeZoneConverter.Convert(_.Start, _abpSession.TenantId, _abpSession.GetUserId()),
                           _ => _timeZoneConverter.Convert(_.End, _abpSession.TenantId, _abpSession.GetUserId())
                           );
                       var a = sheet.Column(5);
                       a.Style.Numberformat.Format = "yyyy-mm-dd";
                       var b = sheet.Column(6);
                       b.Style.Numberformat.Format = "yyyy-mm-dd";
                       for (var i = 1; i <= 10; i++)
                       {
                           sheet.Column(i).AutoFit();
                       }
                   });
        }
        /// <summary>
        /// 导出签到详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public FileDto ExportSignsDetail(List<SignDetailDto> input)
        {
            return CreateExcelPackage(
                "签到详情统计.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add("签到详情统计");
                    sheet.OutLineApplyStyle = true;
                    AddHeader(
                        sheet,
                       "用户编号",
                       "用户名",
                       "是否签到",
                       "签到时间",
                       "签到地点",
                       "开始时间",
                       "结束时间"
                        );
                    AddObjects(
                        sheet, 2, input,
                        _ => _.UserId,
                        _ => _.UserName,
                        _ => _.IsSign ? "是" : "否",
                        _ => _timeZoneConverter.Convert(_.CreationTime, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.SignLocation,
                        _ => _timeZoneConverter.Convert(_.Start, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.End, _abpSession.TenantId, _abpSession.GetUserId())
                        );
                    var a = sheet.Column(4);
                    a.Style.Numberformat.Format = "yyyy-mm-dd";
                    var b = sheet.Column(6);
                    b.Style.Numberformat.Format = "yyyy-mm-dd";
                    var c = sheet.Column(7);
                    c.Style.Numberformat.Format = "yyyy-mm-dd";
                    for (var i = 1; i <= 10; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public FileDto ExportWarns(List<WarnDetailDto> input)
        {
            return CreateExcelPackage(
               "报警信息.xlsx",
               excelPackage =>
               {
                   var sheet = excelPackage.Workbook.Worksheets.Add("报警信息");
                   sheet.OutLineApplyStyle = true;
                   AddHeader(
                       sheet,
                      "设备编号",
                      "设备点位",
                      "是否解决",
                      "报警类型",
                      "报警时间",
                      "运维人员",
                      "处理时间",
                      "解决时间",
                      "解决时长"
                       );
                   AddObjects(
                       sheet, 2, input,
                       _ => _.DeviceNum,
                       _ => _.PointName,
                       _ => _.IsSolve ? "是" : "否",
                       _ => _.WarnType,
                       _ => _.WarnDate,
                       _ => _.UserName,
                       _ => _.SolveDate,
                       _ => _.SetTime,
                       _ => _.SolveTime
                       );
                   for (var i = 1; i <= 10; i++)
                   {
                       sheet.Column(i).AutoFit();
                   }
               });
        }

        private string ChangeType(string type)
        {
            switch (type)
            {
                case "cash":
                    return "现金";
                case "wx_pub":
                    return "微信支付";
                case "alipay":
                    return "支付宝支付";
                case "test":
                    return "测试";
                default:
                    return "";
            }
        }
    }
}
