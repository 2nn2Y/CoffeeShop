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
        /// �����û���Ϣ
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
        /// ��ȡ�ɽ�����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportOrderDetails(List<OrderDetail> orders)
        {
            return CreateExcelPackage(
                  "��������.xlsx",
                  excelPackage =>
                  {
                      var sheet = excelPackage.Workbook.Worksheets.Add("��������");
                      sheet.OutLineApplyStyle = true;
                      AddHeader(
                          sheet,
                         "������",
                         "������",
                         "��Ʒ��",
                         "�۸�",
                         "֧������",
                         "�ɽ�ʱ��"
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
        /// ��������
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportStoreOrders(List<StoreOrderListDto> orders)
        {
            return CreateExcelPackage(
                   "��������.xlsx",
                   excelPackage =>
                   {
                       var sheet = excelPackage.Workbook.Worksheets.Add("��������");
                       sheet.OutLineApplyStyle = true;
                       AddHeader(
                           sheet,
                          "�û�id",
                          "��Ʒ��",
                          "������",
                          "�۸�",
                          "֧������",
                          "֧��״̬",
                          "����״̬",
                          "�ɽ�ʱ��"
                           );
                       AddObjects(
                           sheet, 2, orders,
                           _ => _.UserName,
                           _ => _.ProductName,
                           _ => _.OrderNum,
                           _ => _.Price*1.0/100,
                           _ =>
                               _.PayType == PayType.BalancePay
                                   ? "���֧��"
                                   : (_.PayType == PayType.ActivityPay
                                       ? "�֧��"
                                       : (_.PayType == PayType.LinePay ? "����֧��" : "��ֵ")),
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
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportProductsSale(List<ProductSaleDto> orders)
        {
            return CreateExcelPackage(
                   "��Ʒ����.xlsx",
                   excelPackage =>
                   {
                       var sheet = excelPackage.Workbook.Worksheets.Add("��Ʒ����");
                       sheet.OutLineApplyStyle = true;
                       AddHeader(
                           sheet,
                          "��Ʒ����",
                          "����",
                          "�ܼ�",
                          "��ʼʱ��",
                          "����ʱ��"
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
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportDeviceProductsSale(List<ProductSaleDto> orders)
        {
            return CreateExcelPackage(
                    "�豸����.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("�豸����");
                        sheet.OutLineApplyStyle = true;
                        AddHeader(
                            sheet,
                           "�������",
                           "�豸��λ",
                           "��Ʒ����",
                           "����",
                           "�ܼ�",
                           "��ʼʱ��",
                           "����ʱ��"
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
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportAreaProductsSale(List<ProductSaleDto> orders)
        {
            return CreateExcelPackage(
                    "��������.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("��������");
                        sheet.OutLineApplyStyle = true;
                        AddHeader(
                            sheet,
                           "����",
                           "�豸����",
                           "��Ʒ����",
                           "����",
                           "�ܼ�",
                           "��ʼʱ��",
                           "����ʱ��"
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
        /// ��ȡ֧����������
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportPayTypeSale(List<ProductSaleDto> orders)
        {
            return CreateExcelPackage(
                   "֧������.xlsx",
                   excelPackage =>
                   {
                       var sheet = excelPackage.Workbook.Worksheets.Add("֧������");
                       sheet.OutLineApplyStyle = true;
                       AddHeader(
                           sheet,
                          "֧������",
                          "����",
                          "����",
                          "�ܼ�",
                          "��ʼʱ��",
                          "����ʱ��"
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
        /// ��ȡʱ������ͳ��
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public FileDto ExportTimeAreaSale(List<TimeAreaDto> input)
        {
            return CreateExcelPackage(
                  "ʱ������.xlsx",
                  excelPackage =>
                  {
                      var sheet = excelPackage.Workbook.Worksheets.Add("ʱ������");
                      sheet.OutLineApplyStyle = true;
                      AddHeader(
                          sheet,
                         "ʱ��",
                         "����",
                         "����",
                         "�ܼ�",
                         "��ʼʱ��",
                         "����ʱ��"
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
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportWarnByDevice(List<DeviceWarnDto> orders)
        {
            return CreateExcelPackage(
                    "����ͳ���豸.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("����ͳ���豸");
                        sheet.OutLineApplyStyle = true;
                        AddHeader(
                            sheet,
                           "�������",
                           "�豸��λ",
                           "��������",
                           "��������",
                           "�������",
                           "ƽ��ʱ��",
                           "��ά��Ա",
                           "��ʼʱ��",
                           "����ʱ��"
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
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public FileDto ExportWarnByUser(List<DeviceWarnDto> orders)
        {
            return CreateExcelPackage(
                    "����ͳ����ά��Ա.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("����ͳ����ά��Ա");
                        sheet.OutLineApplyStyle = true;
                        AddHeader(
                            sheet,
                           "��ά��Ա",
                           "��������",
                           "�������",
                           "ƽ��ʱ��",
                           "��Ч����",
                           "��ʼʱ��",
                           "����ʱ��"
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
        /// ����ǩ��ͳ��
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public FileDto ExportSignsByUser(List<SignStaticialDto> input)
        {
            return CreateExcelPackage(
                   "ǩ��ͳ��.xlsx",
                   excelPackage =>
                   {
                       var sheet = excelPackage.Workbook.Worksheets.Add("ǩ��ͳ��");
                       sheet.OutLineApplyStyle = true;
                       AddHeader(
                           sheet,
                          "�û����",
                          "�û���",
                          "ǩ������",
                          "ǩ������",
                          "��ʼʱ��",
                          "����ʱ��"
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
        /// ����ǩ������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public FileDto ExportSignsDetail(List<SignDetailDto> input)
        {
            return CreateExcelPackage(
                "ǩ������ͳ��.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add("ǩ������ͳ��");
                    sheet.OutLineApplyStyle = true;
                    AddHeader(
                        sheet,
                       "�û����",
                       "�û���",
                       "�Ƿ�ǩ��",
                       "ǩ��ʱ��",
                       "ǩ���ص�",
                       "��ʼʱ��",
                       "����ʱ��"
                        );
                    AddObjects(
                        sheet, 2, input,
                        _ => _.UserId,
                        _ => _.UserName,
                        _ => _.IsSign ? "��" : "��",
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
        /// ����
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public FileDto ExportWarns(List<WarnDetailDto> input)
        {
            return CreateExcelPackage(
               "������Ϣ.xlsx",
               excelPackage =>
               {
                   var sheet = excelPackage.Workbook.Worksheets.Add("������Ϣ");
                   sheet.OutLineApplyStyle = true;
                   AddHeader(
                       sheet,
                      "�豸���",
                      "�豸��λ",
                      "�Ƿ���",
                      "��������",
                      "����ʱ��",
                      "��ά��Ա",
                      "����ʱ��",
                      "���ʱ��",
                      "���ʱ��"
                       );
                   AddObjects(
                       sheet, 2, input,
                       _ => _.DeviceNum,
                       _ => _.PointName,
                       _ => _.IsSolve ? "��" : "��",
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
                    return "�ֽ�";
                case "wx_pub":
                    return "΢��֧��";
                case "alipay":
                    return "֧����֧��";
                case "test":
                    return "����";
                default:
                    return "";
            }
        }
    }
}
