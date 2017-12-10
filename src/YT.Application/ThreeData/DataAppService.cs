
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
            var query = await
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
            var query = await
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
            var query = await
               _orderRepository.GetAll()
                   .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                   .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                   .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var points = (await GetPointsFromCache())
                .WhereIf(!input.Area.IsNullOrWhiteSpace(), c => c.City.Contains(input.Area))
                .WhereIf(!input.City.IsNullOrWhiteSpace(), c => c.PointName.Contains(input.City));
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
                         group c by new { c.City, c.PointName, c.SchoolName, c.ProductName }
                into h
                         select new ProductSaleDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             City = h.Key.PointName,
                             Area = h.Key.City,
                             DeviceName = h.Key.SchoolName,
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
            var query = await
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
            var query = await
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
            var warns = await _warnRepository.GetAll()
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
                .WhereIf(!input.User.IsNullOrWhiteSpace(), c => c.UserName.Contains(input.User)).ToListAsync();
            var points = await _pointRepository.GetAll()
                .WhereIf(!input.Point.IsNullOrWhiteSpace(), c => c.PointName.Contains(input.Point)).ToListAsync();
            var result = from c in points
                         join d in temp on c.DeviceNum equals d.DeviceNum
                         join e in users on c.DeviceNum equals e.PointId 
                         select new DeviceWarnDto()
                         {
                             DeviceName = c.PointName,
                             DeviceNum = c.DeviceNum,
                             DealCount = d?.deal ?? 0,
                             End = input.End,
                             Id = c.Id,
                             PerTime = d?.time ?? 0,
                             Start = input.Start,
                             UserName = e.UserName,
                             WarnCount = d?.total ?? 0,
                             WarnType = d?.WarnNum
                         };

            var count = result.Count();
            var ttt =
                result.OrderByDescending(c => c.WarnCount).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            foreach (var item in ttt)
            {
                item.WarnType = YtConsts.Types.FirstOrDefault(c => c.Type.Equals(item.WarnType))?.Chinese;
            }
            return new PagedResultDto<DeviceWarnDto>(count, ttt);
        }

        /// <summary>
        /// 根据设备统计警告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<DeviceWarnDto>> GetWarnByUser(GetOrderInput input)
        {
            var warns = await _warnRepository.GetAll().WhereIf(input.Start.HasValue, c => c.WarnTime >= input.Start.Value)
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
            var counts = result.Count();
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
            var signs = await _recordRepository.GetAll()
                .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.Point.DeviceNum.Contains(input.Device))
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
                           CreationTime = d.CreationTime,
                           Dimension = d.Dimension,
                           Longitude = d.Longitude,
                           SignLocation = d.Point.PointName,
                           Profiles = d.SignProfiles.Any() ?
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
                .WhereIf(input.IsDeal.HasValue, c => c.State == input.IsDeal.Value)
              .WhereIf(input.Start.HasValue, c => c.WarnTime>=input.Start.Value)
                .WhereIf(input.End.HasValue, c => c.WarnTime<input.End.Value);
            var users = (await GetUserPointsFromCache()).WhereIf(!input.User.IsNullOrWhiteSpace(),
                c => c.UserName.Contains(input.User));
            var temp = from c in await query.ToListAsync()
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                       join e in users on c.DeviceNum equals e.PointId
                       select new WarnDetailDto()
                       {
                           Id = c.Id,
                           DeviceNum = c.DeviceNum,
                           IsSolve = c.State,
                           PointName = d.PointName,
                           SetTime = c.SetTime,
                           SolveDate = c.DealTime,
                           SolveTime = c.DealTime.HasValue ? (c.DealTime.Value - c.WarnTime).Minutes : 0,
                           UnSolveTime = !c.State ? (DateTime.Now - c.WarnTime).TotalMinutes : 0,
                           State = c.WarnNum,
                           WarnDate = c.WarnTime,
                           WarnType = c.WarnNum,
                           UserName =e.UserName
                       };
          
            temp = temp.WhereIf(input.Left.HasValue, c => c.SolveTime >= input.Left.Value)
                .WhereIf(input.Right.HasValue, c => c.SolveTime < input.Right.Value);
            var count = temp.Count();
            var result = temp.OrderBy(c => c.IsSolve)
                .ThenByDescending(c => c.UnSolveTime).Skip(input.SkipCount)
                .Take(input.MaxResultCount).ToList();
            foreach (var warnDetailDto in result)
            {
                warnDetailDto.WarnType = YtConsts.Types.FirstOrDefault(c => c.Type.Equals(warnDetailDto.WarnType))?.Chinese;
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
            var query = await
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
            var query = await
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
            var query = await
              _orderRepository.GetAll()
                  .WhereIf(!input.Device.IsNullOrWhiteSpace(), c => c.DeviceNum.Contains(input.Device))
                  .WhereIf(input.Start.HasValue, c => c.Date >= input.Start.Value)
                  .WhereIf(input.End.HasValue, c => c.Date < input.End.Value).ToListAsync();
            var points = (await GetPointsFromCache())
                .WhereIf(!input.Area.IsNullOrWhiteSpace(), c => c.City.Contains(input.Area))
                .WhereIf(!input.City.IsNullOrWhiteSpace(), c => c.PointName.Contains(input.City));
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
                         group c by new { c.City, c.PointName, c.SchoolName, c.ProductName }
                into h
                         select new ProductSaleDto()
                         {
                             Count = h.Count(),
                             Price = h.Sum(c => c.Price),
                             City = h.Key.PointName,
                             Area = h.Key.City,
                             DeviceName = h.Key.SchoolName,
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
            var warns = await _warnRepository.GetAll()
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
                .WhereIf(!input.User.IsNullOrWhiteSpace(), c => c.UserName.Contains(input.User)).ToListAsync();
            var points = await _pointRepository.GetAll()
                .WhereIf(!input.Point.IsNullOrWhiteSpace(), c => c.PointName.Contains(input.Point)).ToListAsync();
            var result = from c in points
                         join d in temp on c.DeviceNum equals d.DeviceNum
                         join e in users on c.DeviceNum equals e.PointId 
                         select new DeviceWarnDto()
                         {
                             DeviceName = c.PointName,
                             DeviceNum = c.DeviceNum,
                             DealCount = d?.deal ?? 0,
                             End = input.End,
                             Id = c.Id,
                             PerTime = d?.time ?? 0,
                             Start = input.Start,
                             UserName = e.UserName,
                             WarnCount = d?.total ?? 0,
                             WarnType = d?.WarnNum
                         };
            foreach (var item in result)
            {
                item.WarnType = YtConsts.Types.FirstOrDefault(c => c.Type.Equals(item.WarnType))?.Chinese;
            }
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
                  .WhereIf(input.IsDeal.HasValue, c => c.State == input.IsDeal.Value)
                .WhereIf(input.Start.HasValue, c => c.WarnTime >= input.Start.Value)
                  .WhereIf(input.End.HasValue, c => c.WarnTime < input.End.Value);

            var users = (await GetUserPointsFromCache()).WhereIf(!input.User.IsNullOrWhiteSpace(),
                c => c.UserName.Contains(input.User));
            var temp = from c in await query.ToListAsync()
                       join d in await GetPointsFromCache() on c.DeviceNum equals d.DeviceNum
                       join e in users on c.DeviceNum equals e.PointId
                       select new WarnDetailDto()
                       {
                           Id = c.Id,
                           DeviceNum = c.DeviceNum,
                           IsSolve = c.State,
                           PointName = d.PointName,
                           SetTime = c.SetTime,
                           SolveDate = c.DealTime,
                           SolveTime = c.DealTime.HasValue ? (c.DealTime.Value - c.WarnTime).Minutes : 0,
                           UnSolveTime = !c.State ? (DateTime.Now - c.WarnTime).TotalMinutes : 0,
                           State = c.WarnNum,
                           WarnDate = c.WarnTime,
                           WarnType = c.WarnNum,
                           UserName = e.UserName
                       };

            var result = temp.ToList();
            foreach (var warnDetailDto in result)
            {
                warnDetailDto.WarnType = YtConsts.Types.FirstOrDefault(c => c.Type.Equals(warnDetailDto.WarnType))?.Chinese;
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


}
