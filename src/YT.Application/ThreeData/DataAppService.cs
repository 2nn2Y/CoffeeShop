
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Caching;
using YT.Dto;
using YT.Models;
using YT.ThreeData.Dtos;
using YT.ThreeData.Exporting;
using Abp.Linq.Extensions;
namespace YT.ThreeData
{
    /// <summary>
    /// 第三方数据接口
    /// </summary>
    public class DataAppService : YtAppServiceBase, IDataAppService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Point> _pointRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Warn> _warnRepository;
        private readonly IRepository<UserPoint> _userPointRepository;
        private readonly IRepository<SignRecord> _recordRepository;
        private readonly IDataExcelExporter _dataExcelExporter;

        private List<WarnType> Types => new List<WarnType>()
        {
            new WarnType("ECB_03", "	Tomuchcurrentincircuit3	", "		电路3电流过大	"),
            new WarnType("ECE_2", "	SlaveI/Oataddress#2doesnotrespond	", "		IO板2不响应	"),
            new WarnType("EC_2", "	Nomachineconnected	", "		没有IO板连接	"),
            new WarnType("EC_3", "	Systemnoready	", "		系统未就绪	"),
            new WarnType("EC_4", "	Nopaymentdevice	", "		无支付设备	"),
            new WarnType("EC_6", "	NOinternet	", "		请检查网络连接	"),
            new WarnType("EC_7", "	pleasecheckip?port?machinenumber	", "		网络设置错误检查IP,Port,机器号	"),
            new WarnType("EI_2", "	Communicationwithsevererror	", "		通讯设备通讯或协议错误	"),
            new WarnType("EJA_01", "	Doormotornotdetect.	", "		未检测到大门电机	"),
            new WarnType("EJA_02", "	Doormotorrunerror.	", "		大门电机故障	"),
            new WarnType("ERROR:0081", "mixermotor1opencircuit	", "		搅拌电机1开路	"),
            new WarnType("ERROR:0088", "mixermotor1blocked	", "		搅拌电机1堵转	"),
            new WarnType("ERROR:0181", "mixermotor2opencircuit	", "		搅拌电机2开路	"),
            new WarnType("ERROR:0188", "mixermotor2blocked	", "		搅拌电机2堵转	"),
            new WarnType("ERROR:0281", "mixermotor3opencircuit	", "		搅拌电机3开路	"),
            new WarnType("ERROR:0288", "mixermotor3blocked	", "		搅拌电机3堵转	"),
            new WarnType("ERROR:0381", "ERROR:0381	", "		搅拌电机4开路	"),
            new WarnType("ERROR:0388", "ERROR:0388	", "		搅拌电机4堵转	"),
            new WarnType("ERROR:0681", "ERROR:0681	", "		空气泵开路	"),
            new WarnType("ERROR:0688", "ERROR:0688	", "		空气泵堵转	"),
            new WarnType("ERROR:0881", "canistermotor1opencircuit	", "		料盒电机1开路	"),
            new WarnType("ERROR:0888", "canistermotor1blocked	", "		料盒电机1堵转	"),
            new WarnType("ERROR:0981", "canistermotor2opencircuit	", "		料盒电机2开路	"),
            new WarnType("ERROR:0988", "canistermotor2blocked	", "		料盒电机2堵转	"),
            new WarnType("ERROR:0A81", "canistermotor3opencircuit	", "		料盒电机3开路	"),
            new WarnType("ERROR:0A88", "canistermotor3blocked	", "		料盒电机3堵转	"),
            new WarnType("ERROR:0B81", "canistermotor4opencircuit	", "		料盒电机4开路	"),
            new WarnType("ERROR:0B88", "canistermotor4blocked	", "		料盒电机4堵转	"),
            new WarnType("ERROR:0C81", "canistermotor5opencircuit	", "		料盒电机5开路	"),
            new WarnType("ERROR:0C88", "canistermotor5blocked	", "		料盒电机5堵转	"),
            new WarnType("ERROR:0D81", "canistermotor6opencircuit	", "		料盒电机6开路	"),
            new WarnType("ERROR:0D88", "canistermotor6blocked	", "		料盒电机6堵转	"),
            new WarnType("ERROR:0E81", "ERROR:0E81	", "		料盒电机7开路	"),
            new WarnType("ERROR:0E88", "ERROR:0E88	", "		料盒电机7堵转	"),
            new WarnType("ERROR:0F81", "ERROR:0F81	", "		料盒电机8开路	"),
            new WarnType("ERROR:0F88", "ERROR:0F88	", "		料盒电机8堵转	"),
            new WarnType("ERROR:1081", "ESchambermotoropencircuit	", "		ES挤饼电机开路	"),
            new WarnType("ERROR:1088", "ESchambermotorblocked	", "		ES挤饼电机堵转	"),
            new WarnType("ERROR:1181", "ESsealingmotoropencircuit	", "		ES密封电机开路	"),
            new WarnType("ERROR:1188", "ESsealingmotorblocked	", "		ES密封电机堵转	"),
            new WarnType("ERROR:1281", "FBwipingmotoropencircuit	", "		FB刮片电机开路	"),
            new WarnType("ERROR:1288", "FBwipingmotorblocked	", "		FB刮片电机堵转	"),
            new WarnType("ERROR:1381", "FBsealingmotoropencircuit	", "		FB密封电机开路	"),
            new WarnType("ERROR:1388", "FBsealingmotorblocked	", "		FB密封电机堵转	"),
            new WarnType("ERROR:1681", "gearpumpopencircuit	", "		齿轮泵开路	"),
            new WarnType("ERROR:1688", "ERROR:1688	", "		齿轮泵堵转	"),
            new WarnType("ERROR:1881", "cupdispensemotoropencircuit	", "		分杯电机开路	"),
            new WarnType("ERROR:1888", "cupdispensemotorblocked	", "		分杯电机堵转	"),
            new WarnType("ERROR:1981", "cuptubemotoropencircuit	", "		杯桶电机开路	"),
            new WarnType("ERROR:1988", "cuptubemotorblocked	", "		杯桶电机堵转	"),
            new WarnType("ERROR:1A81", "cupcatchermotoropencircuit	", "		运杯电机开路	"),
            new WarnType("ERROR:1A88", "cupcatchermotorblocked	", "		运杯电机堵转	"),
            new WarnType("ERROR:1B81", "doormotoropencircuit	", "		大门电机开路	"),
            new WarnType("ERROR:1B88", "ERROR:1B88	", "		大门电机堵转	"),
            new WarnType("ERROR:1C81", "smalldoormotoropencircuit	", "		小门电机开路	"),
            new WarnType("ERROR:1C88", "ERROR:1C88	", "		小门电机堵转	"),
            new WarnType("ERROR:2081", "normaltemp.watervalveopencircuit	", "		常温进水阀开路	"),
            new WarnType("ERROR:2181", "coldwaterinletvalveopencircuit	", "		冷水进水阀开路	"),
            new WarnType("ERROR:2281", "ES2p3wvalveopencircuit	", "		ES二位三通阀开路	"),
            new WarnType("ERROR:2981", "2p2wvalve2opencircuit	", "		二位二通阀2开路	"),
            new WarnType("ERROR:2A81", "2p2wvalve3opencircuit	", "		二位二通阀3开路	"),
            new WarnType("ERROR:2B81", "2p2wvalve4opencircuit	", "		二位二通阀4开路	"),
            new WarnType("ERROR:2C81", "2p2wvalve5opencircuit	", "		二位二通阀5开路	"),
            new WarnType("ERROR:3581", "airbreakprobesensorstayon	", "		水盒探针传感器一直开	"),
            new WarnType("ERROR:3A81", "cupdispenserpositionsensorstayopen	", "		分杯马达位置传感器一直开	"),
            new WarnType("ERROR:3B00", "ERROR:3B00	", "		杯桶旋转传感器异常	"),
            new WarnType("ERROR:3B80", "cuprorationsensorstayoff	", "		杯桶旋转传感器一直关	"),
            new WarnType("ERROR:3B81", "cuprotationsensorstayon	", "		杯桶旋转传感器一直开	"),
            new WarnType("ERROR:3C81", "cupcatcherswitchmotorsensor1stayon	", "		运杯微动传感器1一直开	"),
            new WarnType("ERROR:3D81", "cupcatcherswitchmotorsensor2stayon	", "		运杯微动传感器2一直开	"),
            new WarnType("ERROR:4381", "FBwipingsensorstayon	", "		FB刮渣传感器一直开	"),
            new WarnType("ERROR:4480", "FBsealingsensorstayoff	", "		FB密封传感器一直	"),
            new WarnType("ERROR:4481", "FBsealingsensorstayopen	", "		FB密封传感器一直开	"),
            new WarnType("ERROR:5200", "ERROR:5200	", "电路板电流过大	"),
            new WarnType("ERROR:5201", "ERROR:5201	", "电路板电流过大	"),
            new WarnType("ERROR:5202", "ERROR:5202	", "电路板电流过大	"),
            new WarnType("ERROR:5206", "ERROR:5206	", "电路板电流过大	"),
            new WarnType("ERROR:5208", "ERROR:5208	", "电路板电流过大	"),
            new WarnType("ERROR:520B", "ERROR:520B	", "电路板电流过大	"),
            new WarnType("ERROR:520C", "ERROR:520C	", "电路板电流过大	"),
            new WarnType("ERROR:5210", "ERROR:5210	", "电路板电流过大	"),
            new WarnType("ERROR:5212", "ERROR:5212	", "电路板电流过大	"),
            new WarnType("ERROR:5214", "ERROR:5214	", "电路板电流过大	"),
            new WarnType("ERROR:5300", "nocups	", "		缺杯子	"),
            new WarnType("ERROR:5406", "ERROR:5406	", "		分杯失败多次	"),
            new WarnType("ERROR:5500", "cupholdercan'tmovetocorrectposition	", "		杯托移动不到位	"),
            new WarnType("ERROR:5600", "extracupsbeforemakingproducts	", "		做产品前有多余杯子	"),
            new WarnType("ERROR:5700", "nobeans	", "缺咖啡豆	"),
            new WarnType("ERROR:5901", "nowater1", "净水缺乏1	"),
            new WarnType("ERROR:5902", "nowater2", "净水缺乏2	"),
            new WarnType("ERROR:5A00", "wastewaterbinfull", "		废水桶满	"),
            new WarnType("ERROR:5B00", "driptreynotwell-installed	", "		接水托盘安装不到位	"),
            new WarnType("ERROR:5C00", "flowmetererror	", "流量计故障	"),
            new WarnType("ERROR:5C07", "ERROR:5C07	", "流量计故障	"),
            new WarnType("ERROR:5CFF", "ERROR:5CFF	", "流量计故障	"),
            new WarnType("ERROR:5D00", "boilertemp.sensorerror	", "锅炉温度传感器故障	"),
            new WarnType("ERROR:5DFF", "ERROR:5DFF	", "锅炉温度传感器故障	"),
            new WarnType("ERROR:6100", "ERROR:6100	", "锅炉温度过低	"),
            new WarnType("ERROR:6D00", "executivecomponenterror0whilemakingproduct", "做产品时执行部件异常0	"),
            new WarnType("ERROR:7000", "", "CUP板连接异常"),
            new WarnType("ERROR:7200", "airbreakfilledovertime	", "水箱填充超时"),
            new WarnType("ERROR:7300", "noboilerconnected", "锅炉未连接"),
            new WarnType("ERROR:7600", "ESbrewerboardconnecterror", "Brewer板连接错误"),
            new WarnType("ERROR:7700", "Esbrewermovementerror0", "咖啡酿造器动作错误0"),
            new WarnType("ERROR:7701", "ESbrewermovementerror1", "咖啡酿造器动作错误1"),
            new WarnType("ERROR:7702", "ESbrewermovementerror2", "咖啡酿造器动作错误2"),
            new WarnType("ERROR:7703", "ESbrewermovementerror3", "咖啡酿造器动作错误3"),
            new WarnType("ERROR:7704", "ESbrewermovementerror4", "咖啡酿造器动作错误4"),
            new WarnType("ERROR:7705", "ESbrewermovementerror5", "咖啡酿造器动作错误5"),
            new WarnType("ERROR:7707", "ESbrewermovementerror7", "咖啡酿造器动作错误7"),
            new WarnType("ERROR:7706", "ESbrewermovementerror6", "咖啡酿造器动作错误6"),
            new WarnType("ERROR:7708", "ESbrewermovementerror8", "咖啡酿造器动作错误8"),
            new WarnType("ERROR:7902", "FBbrewermovementerror2", "泡茶器动作错误2"),
            new WarnType("ERROR:7906", "FBbrewermovementerror6", "泡茶器动作错误6	"),
            new WarnType("ERROR:7A20", "ERROR:7A20	", "压力传感器异常"),
            new WarnType("ERROR:7AA5", "ERROR:7AA5	", "压力传感器异常"),
            new WarnType("ERROR:7AA7", "ERROR:7AA7	", "压力传感器异常"),
            new WarnType("ERROR:CUP_01", "Cupblocksthetray.	", "产品没被取走"),
            new WarnType("HOPPER_2", "HOPPER_2", "退币器故障"),
            new WarnType("INT_1", "	Cannotconnecttoserver.	", "请检查网络连接"),
            new WarnType("warning:01", "sugarisnotenough!	", "缺糖预警（尚余少量）"),
            new WarnType("warning:02", "ESwaterisnotenough!	", "缺水预警（尚余少量）"),
            new WarnType("warning:03", "Cupisnotenough!	", "缺杯预警（尚余少量）"),
            new WarnType("WARNING:0A", "Dooropened.	", "大门打开	")
        };
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="orderRepository"></param>
        /// <param name="pointRepository"></param>
        /// <param name="productRepository"></param>
        /// <param name="dataExcelExporter"></param>
        /// <param name="warnRepository"></param>
        /// <param name="userPointRepository"></param>
        /// <param name="recordRepository"></param>
        public DataAppService(ICacheManager cacheManager,
            IRepository<Order> orderRepository,
            IRepository<Point> pointRepository,
            IRepository<Product> productRepository,
            IDataExcelExporter dataExcelExporter,
            IRepository<Warn> warnRepository,
            IRepository<UserPoint> userPointRepository, IRepository<SignRecord> recordRepository)
        {
            _cacheManager = cacheManager;
            _orderRepository = orderRepository;
            _pointRepository = pointRepository;
            _productRepository = productRepository;
            _dataExcelExporter = dataExcelExporter;
            _warnRepository = warnRepository;
            _userPointRepository = userPointRepository;
            _recordRepository = recordRepository;
        }
        #region 统计报表
        /// <summary>
        /// 获取成交订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<OrderDetail>> GetOrderDetails(GetOrderInput input)
        {
            var query =await 
                _orderRepository.GetAll()
                    .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                    .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                    .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var orders = from c in query
                         join d in (await GetProductFromCache())
                         .WhereIf(!input.Product.IsNullOrWhiteSpace(), c => c.ProductName.Contains(input.Product)) on c.ProductNum equals d.ProductId
                         select new { c, d };
            var count = orders.Count();
            var result =
                orders.OrderByDescending(c => c.c.Date)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .Select(c => new OrderDetail()
                    {
                        Date = c.c.Date,
                        DeviceNum = c.c.DeviceNum,
                        PayType = c.c.PayType,
                        Price = c.c.Price,
                        ProductName = c.d.ProductName,
                        OrderNum = c.c.OrderNum
                    }).ToList();
            return new PagedResultDto<OrderDetail>(count, result);
        }

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProductSaleDto>> GetProductsSale(GetProductSaleInput input)
        {
            var query =
                _orderRepository.GetAll()
                    .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                    .WhereIf(input.End.HasValue, c => c.Date < input.End.Value);
            var temp = from c in query
                       group c by c.ProductNum
                into h
                       select new { h.Key, count = h.Count(), total = h.Sum(c => c.Price) };

            var orders = from c in (await GetProductFromCache())
                    .WhereIf(!input.Product.IsNullOrWhiteSpace(),
                        c => c.ProductName.Contains(input.Product))
                         join d in temp on c.ProductId equals d.Key
                         select new ProductSaleDto()
                         {
                             ProductName = c.ProductName,
                             Start = input.Start,
                             End = input.End,
                             Price = d.total,
                             Count = d.count
                         };
            var count = orders.Count();
            var result =
                orders.OrderByDescending(c => c.Count)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount).ToList();
            return new PagedResultDto<ProductSaleDto>(count, result);
        }

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProductSaleDto>> GetDeviceProductsSale(GetOrderInput input)
        {
            var query =await 
               _orderRepository.GetAll()
                   .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                   .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                   .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var temp = from c in query
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                       join e in await GetProductFromCache() on c.ProductNum equals e.ProductId
                       select
                       new
                       {
                           c.Date,
                           c.DeviceNum,
                           c.OrderNum,
                           c.PayType,
                           c.Price,
                           d.City,
                           d.DeviceType,
                           d.PointName,
                           d.SchoolName,
                           e.ProductName
                       };

            var orders = from c in temp
                         group c by new { c.DeviceNum, c.PointName, c.ProductName }
                into h
                         select new ProductSaleDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             DeviceName = h.Key.PointName,
                             DeviceNum = h.Key.DeviceNum,
                             ProductName = h.Key.ProductName,
                             Start = input.Start,
                             End = input.End
                         };
            var count = orders.Count();
            var result =
                orders.OrderByDescending(c => c.Count)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount).ToList();

            return new PagedResultDto<ProductSaleDto>(count, result);
        }

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProductSaleDto>> GetAreaProductsSale(GetSaleInput input)
        {
            var query =await   
               _orderRepository.GetAll()
                   .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                   .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                   .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var points = (await GetPointsFromCache())
                .WhereIf(!input.Area.IsNullOrWhiteSpace(), c => c.City.Contains(input.Area))
                .WhereIf(!input.City.IsNullOrWhiteSpace(), c => c.SchoolName.Contains(input.City));
            var products = (await GetProductFromCache()).WhereIf(!input.ProductName.IsNullOrWhiteSpace(),
                c => c.ProductName.Contains(input.ProductName));
            var temp = from c in query
                       join d in points on c.DeviceNum equals d.DeviceNum
                       join e in products on c.ProductNum equals e.ProductId
                       select
                       new
                       {
                           c.Date,
                           c.DeviceNum,
                           c.OrderNum,
                           c.PayType,
                           c.Price,
                           d.City,
                           d.DeviceType,
                           d.PointName,
                           d.SchoolName,
                           e.ProductName
                       };

            var orders = from c in temp
                         group c by new { c.SchoolName, c.PointName, c.ProductName }
                into h
                         select new ProductSaleDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             City = h.Key.PointName,
                             Area = h.Key.SchoolName,
                             ProductName = h.Key.ProductName,
                             Start = input.Start,
                             End = input.End
                         };
            var count = orders.Count();
            var result =
                orders.OrderByDescending(c => c.Count)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount).ToList();

            return new PagedResultDto<ProductSaleDto>(count, result);
        }

        /// <summary>
        /// 获取支付渠道销售
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProductSaleDto>> GetPayTypeSale(GetOrderInput input)
        {
            var query =await 
               _orderRepository.GetAll()
                   .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                   .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                   .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var temp = from c in query
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                       select
                       new
                       {
                           c.Date,
                           c.DeviceNum,
                           c.OrderNum,
                           c.PayType,
                           c.Price,
                           d.City,
                           d.DeviceType,
                           d.PointName,
                           d.SchoolName,
                       };

            var orders = from c in temp
                         group c by new { c.SchoolName, c.PayType }
                into h
                         select new ProductSaleDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             PayType = h.Key.PayType,
                             City = h.Key.SchoolName,
                             Start = input.Start,
                             End = input.End
                         };
            var count = orders.Count();
            var result =
                orders.OrderByDescending(c => c.Count)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount).ToList();

            return new PagedResultDto<ProductSaleDto>(count, result);
        }

        /// <summary>
        /// 获取时间区域统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<TimeAreaDto>> GetTimeAreaSale(GetOrderInput input)
        {
            var final = new List<TimeAreaDto>();
            var list = new List<TimeAreaDto>()
            {
               new TimeAreaDto("0~3","0,1,2,3"),
               new TimeAreaDto("4~6","4,5,6"),
               new TimeAreaDto("7~10","7,8,9,10"),
               new TimeAreaDto("11~14","11,12,13,14"),
               new TimeAreaDto("15~18","15,16,17,18"),
               new TimeAreaDto("19~22","19,20,21,22"),
               new TimeAreaDto("23~24","23,24"),
            };
            var query =await 
               _orderRepository.GetAll()
                   .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                   .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                   .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var temp = from c in query
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                       select
                       new
                       {
                           c.Date,
                           c.DeviceNum,
                           c.OrderNum,
                           c.PayType,
                           c.Price,
                           d.City,
                           d.DeviceType,
                           d.PointName,
                           d.SchoolName,
                       };

            var orders = from c in temp
                         group c by new { c.SchoolName, c.Date.Hour }
                into h
                         select new TimeAreaDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             Area = h.Key.Hour.ToString(),
                             SchoolName = h.Key.SchoolName,
                             Start = input.Start,
                             End = input.End
                         };
            var result = orders.ToList();

            foreach (var dto in list)
            {
                var t = result.Where(c => dto.Temp.Contains(c.Area));
                var d = from c in t
                        group c by c.SchoolName
                    into cc
                        select new { cc.Key, count = cc.Sum(c => c.Count), total = cc.Sum(c => c.Price) };
                if (d.Any())
                {
                    final.AddRange(d.Select(ee => new TimeAreaDto()
                    {
                        Area = dto.Area,
                        Count = ee.count,
                        Price = ee.total,
                        SchoolName = ee.Key,
                        Start = input.Start,
                        End = input.End
                    }));
                }
                else
                {
                    final.Add(dto);
                }
            }
            return new PagedResultDto<TimeAreaDto>(list.Count, final);
        }


        /// <summary>
        /// 根据设备统计警告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<DeviceWarnDto>> GetWarnByDevice(GetWarnInput input)
        {
            var warns =await _warnRepository.GetAll()
                .WhereIf(!input.Device.IsNullOrWhiteSpace(),c=>c.DeviceNum.Equals(input.Device))
                .WhereIf(!input.Type.IsNullOrWhiteSpace(),c=>c.WarnNum.Equals(input.Type))
                .WhereIf(input.Start.HasValue, c => c.WarnTime >= input.Start.Value)
                .WhereIf(input.End.HasValue, c => c.WarnTime < input.End.Value).ToListAsync();
            var temp = from c in warns
                       group c by new { c.DeviceNum, c.WarnNum }
                into h
                       select new
                       {
                           h.Key.DeviceNum,
                           h.Key.WarnNum,
                           total = h.Count(),
                           deal = h.Count(c => c.State),
                           time = h.Where(c => c.DealTime.HasValue).Sum(c => (c.DealTime.Value - c.WarnTime).Minutes)
                       };

            var users = await _userPointRepository.GetAll()
                .WhereIf(!input.User.IsNullOrWhiteSpace(), c => c.UserName.Equals(input.User)).ToListAsync();
            var points = await _pointRepository.GetAll()
                .WhereIf(!input.Point.IsNullOrWhiteSpace(), c => c.PointName.Contains(input.Point)).ToListAsync();
            var result = from c in points
                         join d in temp on c.DeviceNum equals d.DeviceNum
                         join e in users on c.DeviceNum equals e.PointId into b
                         from bb in b.DefaultIfEmpty()
                         select new DeviceWarnDto()
                         {
                             DeviceName = c.PointName,
                             DeviceNum = c.DeviceNum,
                             DealCount = d?.deal ?? 0,
                             End = input.End,
                             Id = c.Id,
                             PerTime = d?.time ?? 0,
                             Start = input.Start,
                             UserName = bb?.UserName,
                             WarnCount = d?.total ?? 0,
                             WarnType = d?.WarnNum
                         };

            var count = result.Count();
            var ttt =
                result.OrderByDescending(c => c.WarnCount).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            return new PagedResultDto<DeviceWarnDto>(count, ttt);
        }

        /// <summary>
        /// 根据设备统计警告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<DeviceWarnDto>> GetWarnByUser(GetOrderInput input)
        {
            var warns =await _warnRepository.GetAll().WhereIf(input.Start.HasValue, c => c.WarnTime >= input.Start.Value)
                .WhereIf(input.End.HasValue, c => c.WarnTime < input.End.Value).ToListAsync();

            var temp = from c in warns
                       group c by new { c.DeviceNum, c.WarnNum }
                into h
                       select new
                       {
                           h.Key.DeviceNum,
                           h.Key.WarnNum,
                           total = h.Count(),
                           deal = h.Count(c => c.State),
                           time = h.Where(c => c.DealTime.HasValue).Sum(c => (c.DealTime.Value - c.WarnTime).Minutes)
                       };
            var count = await _userPointRepository.CountAsync();
            var users = _userPointRepository.GetAll()
                .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.UserName.Contains(input.Device))
                .OrderByDescending(c => c.UserId).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            var result = from c in users
                         join d in temp on c.PointId equals d.DeviceNum into t
                         from tt in t.DefaultIfEmpty()
                         select new DeviceWarnDto()
                         {
                             DealCount = tt?.deal ?? 0,
                             End = input.End,
                             Id = c.Id,
                             PerTime = tt?.time ?? 0,
                             Start = input.Start,
                             UserName = c.UserName,
                             WarnCount = tt?.total ?? 0,
                         };


            return new PagedResultDto<DeviceWarnDto>(count, result.ToList());
        }

        /// <summary>
        /// 获取人员签到统计
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultDto<SignStaticialDto>> GetSignsByUser(GetOrderInput input)
        {
            var users = from c in
                _userPointRepository.GetAll().WhereIf(!input.Device.IsNullOrWhiteSpace()
                    , c => c.UserName.Contains(input.Device))
                        group c by new { c.UserId, c.UserName } into h
                        select new { h.Key.UserId, h.Key.UserName };
            var counts = users.Count();
            var signs = _recordRepository.GetAll()
                .WhereIf(input.Start.HasValue, c => c.CreationTime >= input.Start.Value)
                .WhereIf(input.End.HasValue, c => c.CreationTime < input.End.Value);

            var temp = from c in await users.ToListAsync()
                       join d in signs on c.UserId equals d.UserId
                       select new { c, d };
            var result = from t in temp
                         group t by new { t.c.UserId, t.c.UserName }
                into r
                         select
                         new SignStaticialDto()
                         {
                             
                             UserId = r.Key.UserId,
                             UserName = r.Key.UserName,
                             TimeSign = r.Count(),
                             Start = input.Start,
                             End = input.End,
                             DaySign = r.Count()
                         };
            var final =
                result.OrderByDescending(c => c.TimeSign).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            return new PagedResultDto<SignStaticialDto>(counts, final);
        }


        /// <summary>
        /// 获取统计明细
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultDto<SignDetailDto>> GetSignsDetail(GetSignInput input)
        {
            var users = from c in
              _userPointRepository.GetAll().WhereIf(!input.UserName.IsNullOrWhiteSpace()
                  , c => c.UserName.Contains(input.UserName))
                  .WhereIf(!input.UserNum.IsNullOrWhiteSpace()
                  , c => c.UserId.Contains(input.UserNum))
                        group c by new { c.UserId, c.UserName } into h
                        select new { h.Key.UserId, h.Key.UserName };
            var counts = users.Count();
            var signs =await _recordRepository.GetAll()
                .WhereIf(!input.Device.IsNullOrWhiteSpace(),c=>c.Point.DeviceNum.Contains(input.Device))
                .WhereIf(input.Start.HasValue, c => c.CreationTime >= input.Start.Value)
                .WhereIf(input.End.HasValue, c => c.CreationTime < input.End.Value).ToListAsync();

            var temp = from c in await users.ToListAsync()
                       join d in signs on c.UserId equals d.UserId 
                       select new SignDetailDto()
                       {
                           DeviceNum = d.Point.DeviceNum,
                           UserId = c.UserId,
                           UserName = c.UserName,
                           IsSign = true,
                           Start = input.Start,
                           End = input.End,
                           CreationTime =d.CreationTime,
                           Dimension =d.Dimension,
                           Longitude = d.Longitude,
                           SignLocation =d.Point.PointName,
                           Profiles =d.SignProfiles.Any()?
                           d.SignProfiles.Select(w => Host + w.Profile.Url).ToList() : null
                       };
            var final =
               temp.OrderByDescending(c => c.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            return new PagedResultDto<SignDetailDto>(counts, final);
        }

        /// <summary>
        /// 获取报警信息报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<WarnDetailDto>> GetWarns(GetWarnInfoInput input)
        {
            var query = _warnRepository.GetAll()
                .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                .WhereIf(!input.Type.IsNullOrWhiteSpace(), c => c.WarnNum.Contains(input.Type))
                .WhereIf(input.IsDeal.HasValue, c => c.State == input.IsDeal.Value);

            var users = (await GetUserPointsFromCache()).WhereIf(!input.User.IsNullOrWhiteSpace(),
                c => c.UserName.Contains(input.User));
            var temp = from c in await query.ToListAsync()
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                     join e in users on c.DeviceNum equals e.PointId
                       into h
                       from tt in h.DefaultIfEmpty()
                       select new WarnDetailDto()
                       {
                           Id = c.Id,
                           DeviceNum = c.DeviceNum,
                           IsSolve = c.State,
                           PointName = d.PointName,
                           SetTime = c.SetTime,
                           SolveDate = c.DealTime,
                           SolveTime = c.DealTime.HasValue ? (c.DealTime.Value - c.WarnTime).Minutes : 0,
                           UnSolveTime = !c.State?(DateTime.Now-c.WarnTime).TotalHours:0,
                           State = c.WarnNum,
                           WarnDate = c.WarnTime,
                           WarnType = c.WarnNum,
                           UserName= tt!=null?tt.UserName:""
                       };
            var count = temp.Count();
            temp = temp.WhereIf(input.Left.HasValue, c => c.SolveTime >= input.Left.Value)
                .WhereIf(input.Right.HasValue, c => c.SolveTime < input.Right.Value);
            var result = temp.OrderBy(c => c.IsSolve)
                .ThenByDescending(c=>c.UnSolveTime).Skip(input.SkipCount)
                .Take(input.MaxResultCount).ToList();
            foreach (var warnDetailDto in result)
            {
                warnDetailDto.WarnType = Types.FirstOrDefault(c => c.Type.Equals(warnDetailDto.WarnType))?.Chinese;
            }
            return new PagedResultDto<WarnDetailDto>(count, result);
        }
        #endregion



        #region cache
        private async Task<List<Product>> GetProductFromCache()
        {
            return await _cacheManager.GetCache(OrgCacheName.ProductCache)
                .GetAsync(OrgCacheName.ProductCache, async () => await GetProductFromDb());
        }
        /// <summary>
        /// db product
        /// </summary>
        /// <returns></returns>
        private async Task<List<Point>> GetPointsFromDb()
        {
            var result = await _pointRepository.GetAllListAsync();
            return result;
        }
        private async Task<List<Point>> GetPointsFromCache()
        {
            return await _cacheManager.GetCache(OrgCacheName.PointCache)
                .GetAsync(OrgCacheName.PointCache, async () => await GetPointsFromDb());
        }

        private async Task<List<UserPoint>> GetUserPointsFromCache()
        {
            return await _cacheManager.GetCache(OrgCacheName.UserPointCache)
                .GetAsync(OrgCacheName.UserPointCache, async () => await GetUserPointsFromDb());
        }
        /// <summary>
        /// db product
        /// </summary>
        /// <returns></returns>
        private async Task<List<UserPoint>> GetUserPointsFromDb()
        {
            var result = await _userPointRepository.GetAllListAsync();
            return result;
        }
        /// <summary>
        /// db product
        /// </summary>
        /// <returns></returns>
        private async Task<List<Product>> GetProductFromDb()
        {
            var result = await _productRepository.GetAllListAsync();
            return result;
        }
        #endregion

        #region 导出操作

        /// <summary>
        /// 获取成交订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportOrderDetails(GetOrderInput input)
        {
            var query =await 
                _orderRepository.GetAll()
                    .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                    .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                    .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var orders = from c in query
                         join d in (await GetProductFromCache())
                         .WhereIf(!input.Product.IsNullOrWhiteSpace(), c => c.ProductName.Contains(input.Product)) on c.ProductNum equals d.ProductId
                         select new { c, d };
            var result =
                orders.OrderByDescending(c => c.c.Date)
                    .Select(c => new OrderDetail()
                    {
                        Date = c.c.Date,
                        DeviceNum = c.c.DeviceNum,
                        PayType = c.c.PayType,
                        Price = c.c.Price,
                        ProductName = c.d.ProductName,
                        OrderNum = c.c.OrderNum
                    }).ToList();
            return _dataExcelExporter.ExportOrderDetails(result);
        }


        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportProductsSale(GetProductSaleInput input)
        {
            var query =
                _orderRepository.GetAll()
                    .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                    .WhereIf(input.End.HasValue, c => c.Date < input.End.Value);
            var temp = from c in await query.ToListAsync()
                       group c by c.ProductNum
                into h
                       select new { h.Key, count = h.Count(), total = h.Sum(c => c.Price) };

            var orders = from c in (await GetProductFromCache())
                    .WhereIf(!input.Product.IsNullOrWhiteSpace(),
                        c => c.ProductName.Contains(input.Product))
                         join d in temp on c.ProductId equals d.Key
                         select new ProductSaleDto()
                         {
                             ProductName = c.ProductName,
                             Start = input.Start,
                             End = input.End,
                             Price = d.total,
                             Count = d.count
                         };
            var result =
                orders.OrderByDescending(c => c.Count)
                   .ToList();
            return _dataExcelExporter.ExportProductsSale(result);
        }

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportDeviceProductsSale(GetOrderInput input)
        {
            var query =await 
                _orderRepository.GetAll()
                    .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                    .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                    .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var temp = from c in query
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                       join e in await GetProductFromCache() on c.ProductNum equals e.ProductId
                       select
                       new
                       {
                           c.Date,
                           c.DeviceNum,
                           c.OrderNum,
                           c.PayType,
                           c.Price,
                           d.City,
                           d.DeviceType,
                           d.PointName,
                           d.SchoolName,
                           e.ProductName
                       };

            var orders = from c in temp
                         group c by new { c.DeviceNum, c.PointName, c.ProductName }
                into h
                         select new ProductSaleDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             DeviceName = h.Key.PointName,
                             DeviceNum = h.Key.DeviceNum,
                             ProductName = h.Key.ProductName,
                             Start = input.Start,
                             End = input.End
                         };
            var result =
                orders.OrderByDescending(c => c.Count)
                   .ToList();

            return _dataExcelExporter.ExportDeviceProductsSale(result);
        }

        /// <summary>
        /// 获取产品销量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportAreaProductsSale(GetSaleInput input)
        {
            var query =await 
                 _orderRepository.GetAll()
                     .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                     .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                     .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var points = (await GetPointsFromCache())
                .WhereIf(!input.Area.IsNullOrWhiteSpace(), c => c.City.Contains(input.Area))
                .WhereIf(!input.City.IsNullOrWhiteSpace(), c => c.SchoolName.Contains(input.City));
            var products = (await GetProductFromCache()).WhereIf(!input.ProductName.IsNullOrWhiteSpace(),
                c => c.ProductName.Contains(input.ProductName));
            var temp = from c in query
                       join d in points on c.DeviceNum equals d.DeviceNum
                       join e in products on c.ProductNum equals e.ProductId
                       select
                       new
                       {
                           c.Date,
                           c.DeviceNum,
                           c.OrderNum,
                           c.PayType,
                           c.Price,
                           d.City,
                           d.DeviceType,
                           d.PointName,
                           d.SchoolName,
                           e.ProductName
                       };

            var orders = from c in temp
                         group c by new { c.City, c.SchoolName, c.ProductName }
                into h
                         select new ProductSaleDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             City = h.Key.SchoolName,
                             Area = h.Key.City,
                             ProductName = h.Key.ProductName,
                             Start = input.Start,
                             End = input.End
                         };
            var result =
                orders.OrderByDescending(c => c.Count)
                .ToList();

            return _dataExcelExporter.ExportAreaProductsSale(result);
        }

        /// <summary>
        /// 获取支付渠道销售
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportPayTypeSale(GetOrderInput input)
        {
            var query =
               _orderRepository.GetAll()
                   .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                   .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                   .WhereIf(input.End.HasValue, c => c.Date < input.End.Value);
            var temp = from c in await query.ToListAsync()
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                       select
                       new
                       {
                           c.Date,
                           c.DeviceNum,
                           c.OrderNum,
                           c.PayType,
                           c.Price,
                           d.City,
                           d.DeviceType,
                           d.PointName,
                           d.SchoolName,
                       };

            var orders = from c in temp
                         group c by new { c.SchoolName, c.PayType }
                into h
                         select new ProductSaleDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             PayType = h.Key.PayType,
                             City = h.Key.SchoolName,
                             Start = input.Start,
                             End = input.End
                         };
            var result =
                orders.OrderByDescending(c => c.Count)
                  .ToList();

            return _dataExcelExporter.ExportPayTypeSale(result);
        }

        /// <summary>
        /// 获取时间区域统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportTimeAreaSale(GetOrderInput input)
        {
            var final = new List<TimeAreaDto>();
            var list = new List<TimeAreaDto>()
            {
               new TimeAreaDto("0~3","0,1,2,3"),
               new TimeAreaDto("4~6","4,5,6"),
               new TimeAreaDto("7~10","7,8,9,10"),
               new TimeAreaDto("11~14","11,12,13,14"),
               new TimeAreaDto("15~18","15,16,17,18"),
               new TimeAreaDto("19~22","19,20,21,22"),
               new TimeAreaDto("23~24","23,24"),
            };
            var query =
               _orderRepository.GetAll()
                   .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                   .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                   .WhereIf(input.End.HasValue, c => c.Date < input.End.Value);
            var temp = from c in await query.ToListAsync()
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                       select
                       new
                       {
                           c.Date,
                           c.DeviceNum,
                           c.OrderNum,
                           c.PayType,
                           c.Price,
                           d.City,
                           d.DeviceType,
                           d.PointName,
                           d.SchoolName,
                       };

            var orders = from c in temp
                         group c by new { c.SchoolName, c.Date.Hour }
                into h
                         select new TimeAreaDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             Area = h.Key.Hour.ToString(),
                             SchoolName = h.Key.SchoolName,
                             Start = input.Start,
                             End = input.End
                         };
            var result = orders.ToList();

            foreach (var dto in list)
            {
                var t = result.Where(c => dto.Temp.Contains(c.Area));
                var d = from c in t
                        group c by c.SchoolName
                    into cc
                        select new { cc.Key, count = cc.Sum(c => c.Count), total = cc.Sum(c => c.Price) };
                if (d.Any())
                {
                    final.AddRange(d.Select(ee => new TimeAreaDto()
                    {
                        Area = dto.Area,
                        Count = ee.count,
                        Price = ee.total,
                        SchoolName = ee.Key,
                        Start = input.Start,
                        End = input.End
                    }));
                }
                else
                {
                    final.Add(dto);
                }
            }
            return _dataExcelExporter.ExportTimeAreaSale(final);
        }
        /// <summary>
        /// 根据设备统计警告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportWarnByDevice(GetWarnInput input)
        {
            var warns =await _warnRepository.GetAll()
                .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Equals(input.Device))
                .WhereIf(!input.Type.IsNullOrWhiteSpace(), c => c.WarnNum.Equals(input.Type))
                .WhereIf(input.Start.HasValue, c => c.WarnTime >= input.Start.Value)
                .WhereIf(input.End.HasValue, c => c.WarnTime < input.End.Value).ToListAsync();
            var temp = from c in warns
                       group c by new { c.DeviceNum, c.WarnNum }
                into h
                       select new
                       {
                           h.Key.DeviceNum,
                           h.Key.WarnNum,
                           total = h.Count(),
                           deal = h.Count(c => c.State),
                           time = h.Where(c => c.DealTime.HasValue).Sum(c => (c.DealTime.Value - c.WarnTime).Minutes)
                       };

            var users = await _userPointRepository.GetAll()
                .WhereIf(!input.User.IsNullOrWhiteSpace(), c => c.UserName.Equals(input.User)).ToListAsync();
            var points = await _pointRepository.GetAll()
                .WhereIf(!input.Point.IsNullOrWhiteSpace(), c => c.PointName.Contains(input.Point)).ToListAsync();
            var result = from c in points
                         join d in temp on c.DeviceNum equals d.DeviceNum
                         join e in users on c.DeviceNum equals e.PointId into b
                         from bb in b.DefaultIfEmpty()
                         select new DeviceWarnDto()
                         {
                             DeviceName = c.PointName,
                             DeviceNum = c.DeviceNum,
                             DealCount = d?.deal ?? 0,
                             End = input.End,
                             Id = c.Id,
                             PerTime = d?.time ?? 0,
                             Start = input.Start,
                             UserName = bb?.UserName,
                             WarnCount = d?.total ?? 0,
                             WarnType = d?.WarnNum
                         };

            return _dataExcelExporter.ExportWarnByDevice(result.ToList());
        }

        /// <summary>
        /// 根据设备统计警告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportWarnByUser(GetOrderInput input)
        {
            var warns = _warnRepository.GetAll().WhereIf(input.Start.HasValue, c => c.WarnTime >= input.Start.Value)
                .WhereIf(input.End.HasValue, c => c.WarnTime < input.End.Value);

            var temp = from c in warns
                       group c by new { c.DeviceNum }
                into h
                       select new
                       {
                           h.Key.DeviceNum,
                           total = h.Count(),
                           deal = h.Count(c => c.State),
                           time = h.Where(c => c.DealTime.HasValue).Sum(c => (c.DealTime.Value - c.WarnTime).Minutes)
                       };
            var users = _userPointRepository.GetAll()
                .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.UserName.Contains(input.Device)).ToList();
            var result = from c in users
                         join d in await temp.ToListAsync() on c.PointId equals d.DeviceNum into t
                         from tt in t.DefaultIfEmpty()
                         select new DeviceWarnDto()
                         {
                             DealCount = tt?.deal ?? 0,
                             End = input.End,
                             Id = c.Id,
                             PerTime = tt?.time ?? 0,
                             Start = input.Start,
                             UserName = c.UserName,
                             WarnCount = tt?.total ?? 0,
                         };


            return _dataExcelExporter.ExportWarnByUser(result.ToList());
        }

        /// <summary>
        /// 获取人员签到统计
        /// </summary>
        /// <returns></returns>
        public async Task<FileDto> ExportSignsByUser(GetOrderInput input)
        {
            var users = from c in
                _userPointRepository.GetAll().WhereIf(!input.Device.IsNullOrWhiteSpace()
                    , c => c.UserName.Contains(input.Device))
                        group c by new { c.UserId, c.UserName } into h
                        select new { h.Key.UserId, h.Key.UserName };
            var signs = _recordRepository.GetAll()
                .WhereIf(input.Start.HasValue, c => c.CreationTime >= input.Start.Value)
                .WhereIf(input.End.HasValue, c => c.CreationTime < input.End.Value);

            var temp = from c in await users.ToListAsync()
                       join d in await signs.ToListAsync() on c.UserId equals d.UserId
                       select new { c, d };
            var result = from t in temp
                         group t by new { t.c.UserId, t.c.UserName }
                into r
                         select
                         new SignStaticialDto()
                         {

                             UserId = r.Key.UserId,
                             UserName = r.Key.UserName,
                             TimeSign = r.Count(),
                             Start = input.Start,
                             End = input.End,
                             DaySign = r.Count()
                         };

            return _dataExcelExporter.ExportSignsByUser(result.ToList());
        }


        /// <summary>
        /// 获取统计明细
        /// </summary>
        /// <returns></returns>
        public async Task<FileDto> ExportSignsDetail(GetSignInput input)
        {
            var users = from c in
              _userPointRepository.GetAll().WhereIf(!input.UserName.IsNullOrWhiteSpace()
                  , c => c.UserName.Contains(input.UserName))
                  .WhereIf(!input.UserNum.IsNullOrWhiteSpace()
                  , c => c.UserId.Contains(input.UserNum))
                        group c by new { c.UserId, c.UserName } into h
                        select new { h.Key.UserId, h.Key.UserName };
            var signs = _recordRepository.GetAll()
                .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.Point.DeviceNum.Contains(input.Device))
                .WhereIf(input.Start.HasValue, c => c.CreationTime >= input.Start.Value)
                .WhereIf(input.End.HasValue, c => c.CreationTime < input.End.Value);
            var temp = from c in await users.ToListAsync()
                       join d in await signs.ToListAsync() on c.UserId equals d.UserId
                       select new SignDetailDto()
                       {
                           DeviceNum = d.Point.DeviceNum,
                           UserId = c.UserId,
                           UserName = c.UserName,
                           IsSign = true,
                           Start = input.Start,
                           End = input.End,
                           CreationTime = d.CreationTime,
                           Dimension = d.Dimension,
                           Longitude = d.Longitude,
                           SignLocation = d.Point.PointName,
                           Profiles = d.SignProfiles.Any() ?
                           d.SignProfiles.Select(w => Host + w.Profile.Url).ToList() : null
                       };
            return _dataExcelExporter.ExportSignsDetail(temp.ToList());
        }

        /// <summary>
        /// 获取报警信息报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportWarns(GetWarnInfoInput input)
        {
            var query = _warnRepository.GetAll()
                .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                .WhereIf(!input.Type.IsNullOrWhiteSpace(), c => c.WarnNum.Contains(input.Type))
                .WhereIf(input.IsDeal.HasValue, c => c.State == input.IsDeal.Value);

            var users = (await GetUserPointsFromCache()).WhereIf(!input.User.IsNullOrWhiteSpace(),
                c => c.UserName.Contains(input.User));
            var temp = from c in await query.ToListAsync()
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                       join e in users on c.DeviceNum equals e.PointId
                         into h
                       from tt in h.DefaultIfEmpty()
                       select new WarnDetailDto()
                       {
                           Id = c.Id,
                           DeviceNum = c.DeviceNum,
                           IsSolve = c.State,
                           PointName = d.PointName,
                           SetTime = c.SetTime,
                           SolveDate = c.DealTime,
                           SolveTime = c.DealTime.HasValue ? (c.DealTime.Value - c.WarnTime).Minutes : 0,
                           UnSolveTime = !c.State ? (DateTime.Now - c.WarnTime).TotalHours : 0,
                           State = c.WarnNum,
                           WarnDate = c.WarnTime,
                           WarnType = c.WarnNum,
                           UserName = tt != null ? tt.UserName : ""
                       };
            var result = temp.ToList();
            foreach (var warnDetailDto in result)
            {
                warnDetailDto.WarnType = Types.FirstOrDefault(c => c.Type.Equals(warnDetailDto.WarnType))?.Chinese;
            }
            return _dataExcelExporter.ExportWarns(result);
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
                case "free":
                    return "现金";
                case "fastcode":
                    return "提货码";
                default:
                    return "";
            }
        }
        #endregion

    }
    /// <summary>
    /// 警告类型
    /// </summary>
    public class WarnType 
    {
        /// <summary>
        /// 
        /// </summary>
        public WarnType() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="e"></param>
        /// <param name="c"></param>
        public WarnType(string t, string e, string c)
        {
            Type = t;
            English = e;
            Chinese = c;
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }   
        /// <summary>
        /// 英文
        /// </summary>
        public string English { get; set; }
        /// <summary>
        /// 中文
        /// </summary>
        public string Chinese { get; set; }
    }

}
