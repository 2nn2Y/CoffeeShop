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
     /// �����û�������Ϣ
     /// </summary>
     /// <param name="userListDtos"></param>
     /// <returns></returns>
        FileDto ExportToFile(List<UserListDto> userListDtos);

   
      
        /// <summary>
        /// ��ȡ�ɽ�����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportOrderDetails(List<OrderDetail> orders);
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        FileDto ExportStoreOrders(List<StoreOrderListDto> orders);

        /// <summary>
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportProductsSale(List<ProductSaleDto> orders);
     

        /// <summary>
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportDeviceProductsSale(List<ProductSaleDto> orders);
        

        /// <summary>
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportAreaProductsSale(List<ProductSaleDto> orders);
      

        /// <summary>
        /// ��ȡ֧����������
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportPayTypeSale(List<ProductSaleDto> orders);
     

        /// <summary>
        /// ��ȡʱ������ͳ��
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
         FileDto ExportTimeAreaSale(List<TimeAreaDto> input);

        /// <summary>
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportWarnByDevice(List<DeviceWarnDto> orders);

        /// <summary>
        /// ��ȡ��Ʒ����
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
         FileDto ExportWarnByUser(List<DeviceWarnDto> orders);

        /// <summary>
        /// ����ǩ������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        FileDto ExportSignsByUser(List<SignStaticialDto> input);

        /// <summary>
        /// ����ǩ������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        FileDto ExportSignsDetail(List<SignDetailDto> input);
        /// <summary>
        /// ����������Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        FileDto ExportWarns(List<WarnDetailDto> input);
    }
}