using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using Abp.UI;
using YT.Configuration;
using YT.Models;
using YT.ThreeData.Dtos;

namespace YT.ThreeData
{
    /// <summary>
    /// 手机端接口
    /// </summary>
    [AbpAllowAnonymous]

    public class MobileAppService : ApplicationService, IMobileAppService
    {
        /// <summary>
        /// 产品列集合
        /// </summary>
        private static List<CoffeeProduct> List => new List<CoffeeProduct>()
        {
            new CoffeeProduct(1,"猫屎咖啡","上流人士必备咖啡,搭配香菜大蒜是不可多得的美味",
                @"http://103.45.102.47:8888\Files\Images\a.jpg",4730),
              new CoffeeProduct(2,"墨菲","上流人士必备咖啡,搭配香菜大蒜是不可多得的美味",
                @"http://103.45.102.47:8888\Files\Images\a.jpg",1730),
               new CoffeeProduct(3,"拿铁A","上流人士必备咖啡,搭配香菜大蒜是不可多得的美味",
                @"http://103.45.102.47:8888\Files\Images\a.jpg",2730),
                new CoffeeProduct(4,"猫屎咖啡B","上流人士必备咖啡,搭配香菜大蒜是不可多得的美味",
                @"http://103.45.102.47:8888\Files\Images\a.jpg",4730),
              new CoffeeProduct(5,"墨菲C","上流人士必备咖啡,搭配香菜大蒜是不可多得的美味",
                @"http://103.45.102.47:8888\Files\Images\a.jpg",1730),
               new CoffeeProduct(6,"拿铁D","上流人士必备咖啡,搭配香菜大蒜是不可多得的美味",
                @"http://103.45.102.47:8888\Files\Images\a.jpg",2730)
        };
        private readonly IRepository<Warn> _warnRepository;
        private readonly IRepository<StoreUser> _userRepository;
        private readonly IRepository<SignRecord> _signRepository;
        private readonly IRepository<UserPoint> _userpointRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Point> _pointRepository;
        private readonly IRepository<StoreOrder> _storeRepository;
        private readonly ICacheManager _cacheManager;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="warnRepository"></param>
        /// <param name="cacheManager"></param>
        /// <param name="productRepository"></param>
        /// <param name="pointRepository"></param>
        /// <param name="signRepository"></param>
        /// <param name="userpointRepository"></param>
        /// <param name="storeRepository"></param>
        /// <param name="userRepository"></param>
        public MobileAppService(IRepository<Warn> warnRepository,
            ICacheManager cacheManager, IRepository<Product> productRepository, IRepository<Point> pointRepository, IRepository<SignRecord> signRepository, IRepository<UserPoint> userpointRepository, IRepository<StoreOrder> storeRepository, IRepository<StoreUser> userRepository)
        {
            _warnRepository = warnRepository;
            _cacheManager = cacheManager;
            _productRepository = productRepository;
            _pointRepository = pointRepository;
            _signRepository = signRepository;
            _userpointRepository = userpointRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
        }
        #region H5商城
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        public List<CoffeeProduct> GetProducts()
        {
            return List;
        }
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        public CoffeeProduct GetProduct(EntityDto input)
        {
            return List.First(c => c.Id == input.Id);
        }
        /// <summary>
        /// 余额支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task BalancePay(InsertOrderInput input)
        {
            var user = await _userRepository.FirstOrDefaultAsync(c => c.OpenId.Equals(input.OpenId));
            if (user == null) throw new UserFriendlyException("用户信息不存在");
            if (user.Balance < input.Price) throw new UserFriendlyException("用户余额不足");
            var order = new StoreOrder()
            {
                Code = GenderCode(),
                FastCode = input.FastCode,
                OpenId = input.OpenId,
                OrderState = null,
                PayType = PayType.BalancePay,
                PayState = null,
                Price = input.Price,
                ProductId = input.ProductId
            };
            await _storeRepository.InsertAsync(order);
            user.Balance -= input.Price;
            order.PayState = true;
        }
        /// <summary>
        /// 在线支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> LinePay(InsertOrderInput input)
        {
            var p = List.First(c => c.Id == input.ProductId);
            var order = new StoreOrder()
            {
                Code = GenderCode(),
                FastCode = input.FastCode,
                OpenId = input.OpenId,
                PayType = PayType.LinePay,
                WechatOrder = Guid.NewGuid().ToString("N"),
                OrderState = null,
                PayState = null,
                Price = input.Price,
                ProductId = input.ProductId
            };
            await _storeRepository.InsertAsync(order);
            JsApiPay jsApiPay = new JsApiPay
            {
                Openid = input.OpenId,
                TotalFee = input.Price
            };
            jsApiPay.GetUnifiedOrderResult(order.WechatOrder, p.ProductName, p.Description);
            var param = jsApiPay.GetJsApiParameters();
            return param;
        }
        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> ChargePay(InsertOrderInput input)
        {
            var order = new StoreOrder()
            {
                OpenId = input.OpenId,
                PayType = PayType.PayCharge,
                WechatOrder = Guid.NewGuid().ToString("N"),
                OrderState = null,
                PayState = null,
                Price = input.Price,
                ProductId = 0
            };
            await _storeRepository.InsertAsync(order);
            JsApiPay jsApiPay = new JsApiPay
            {
                Openid = input.OpenId,
                TotalFee = input.Price
            };
            jsApiPay.GetUnifiedOrderResult(order.WechatOrder, "用户充值", "用户充值");
            var param = jsApiPay.GetJsApiParameters();
            return param;
        }
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<StoreOrderDto>> GetOrders(GetOrderInput input)
        {
            var orders = await _storeRepository.GetAll()
                .Where(c => c.OpenId.Equals(input.Device)
                            && c.PayState.HasValue && c.PayState.Value)
                .Where(c => c.PayType == PayType.BalancePay || c.PayType == PayType.LinePay).ToListAsync();
            var count = orders.Count;
            var temp = orders.OrderByDescending(c => c.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount);

            var result = from c in temp
                         join d in List on c.ProductId equals d.Id
                         select new StoreOrderDto()
                         {
                             Description = d.Description,
                             FastCode = c.FastCode,
                             Id = c.Id,
                             ImageUrl = d.ImageUrl,
                             ProductName = d.ProductName,
                             OpenId = c.OpenId
                         };
            return new PagedResultDto<StoreOrderDto>(count, result.ToList());
        }
        /// <summary>
        /// 提货
        /// </summary>
        /// <returns></returns>
        public async Task PickOrder(PickOrderInput input)
        {
            var order = await _storeRepository.FirstOrDefaultAsync(input.OrderId);
            if (order == null) throw new UserFriendlyException("该订单不存在");
            if (!order.PayState.HasValue || !order.PayState.Value) throw new UserFriendlyException("该订单未支付或无效");
            order.OrderNum = DateTime.Now.ToString("yyyyMMddhhmmss") + input.DeviceNum +
                             new Random().Next(10000, 99999);
            order.DeviceNum = input.DeviceNum;
            var result = await PickProduct(order);
            if (result.status == "success")
            {
                order.OrderState = true;
            }
        }
        /// <summary>
        /// 取货
        /// </summary>
        /// <returns></returns>
        private async Task<dynamic> PickProduct(StoreOrder order)
        {
            var url = "http://103.231.67.143:8079/FASTCODE";
            var temp = $@"ID={order.OrderNum}&UserName={"shuoyibuer"}&Password={"coffee888"}&Vmc={
                order.DeviceNum}&Ptype=Fastcode&Pid={order.ProductId}&Fastcode={order.FastCode}";
            var mac = ToMd5(temp);

            var param = $@"ID={order.OrderNum}&Ptype=Fastcode&Pid={order.ProductId}&Vmc={
                order.DeviceNum}&Fastcode={order.FastCode}&Username={"shuoyibuer"}&Mac={mac}";
            url = url + "?" + param;
            var result = await HttpHandler.PostAsync<dynamic>(url);
            return result;
        }

        #endregion
        #region 手机端
        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>

        public async Task Sign(SignDto input)
        {
            var sign = new SignRecord()
            {
                Dimension = input.Dimension,
                Longitude = input.Longitude,
                PointId = input.PointId,
                State = true,
                UserId = input.UserId,
                SignProfiles = input.SignProfiles.Select(c => new SignProfile()
                {
                    MediaId = c.MedeaId
                }).ToList()
            };
            await _signRepository.InsertAsync(sign);
        }
        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WarnDto> GetWarnsByUser(FilterDto input)
        {
            var points = await GetPointsFromCache();
            var users = await _userpointRepository.GetAllListAsync(c => c.UserId.Equals(input.UserId));
            var warns = await _warnRepository.GetAllListAsync();
            var result = from c in users
                         join warn in warns on c.PointId equals warn.DeviceNum
                         join e in points on warn.DeviceNum equals e.DeviceNum
                         select new MobileWarnDto()
                         {
                             Content = warn.WarnNum,
                             Description = warn.WarnContent,
                             DeviceId = warn.DeviceNum,
                             Id = warn.Id,
                             State = warn.State,
                             DeviceName = e.PointName
                         };
            return new WarnDto()
            {
                Anomaly = result.Where(c => !c.State).ToList(),
                Normal = result.Where(c => c.State).ToList()
            };
        }
        /// <summary>
        /// 处理报警信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DealWarn(EntityDto<int> input)
        {
            var warn = await _warnRepository.FirstOrDefaultAsync(input.Id);
            if (warn == null) throw new UserFriendlyException("该报警信息不存在");
            if (warn.State) throw new UserFriendlyException("该报警信息已解决");
            warn.State = true;
        }

        /// <summary>
        /// 获取报警信息详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<MobileWarnDto> GetWarnByUser(EntityDto<int> input)
        {
            var points = await GetPointsFromCache();
            var warn = await _warnRepository.FirstOrDefaultAsync(input.Id);
            if (warn != null)
            {
                var result = new MobileWarnDto()
                {
                    Content = warn.WarnNum,
                    Description = warn.WarnContent,
                    DeviceId = warn.DeviceNum,
                    Id = warn.Id,
                    State = warn.State
                };
                return result;
            }
            throw new UserFriendlyException("该报警信息不存在");
        }

        /// <summary>
        /// 获取签到列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<SignPointDto>> GetSignList(FilterDto input)
        {
            var points = await GetPointsFromCache();
            var users = await _userpointRepository.GetAllListAsync(c => c.UserId.Equals(input.UserId));
            var temp = users.Select(c => c.PointId).ToList();
            var today = DateTime.Now.Date;
            var next = today.AddDays(1);
            var signs = await _signRepository
                .GetAllListAsync(c => c.CreationTime >= today && c.CreationTime < next);
            var result = from c in points.Where(c => temp.Contains(c.DeviceNum))
                         join d in signs on c.Id equals d.PointId into j
                         from jj in j.DefaultIfEmpty()
                         select new SignPointDto()
                         {
                             Id = c.Id,
                             PointId = c.DeviceNum,
                             Point = c.PointName,
                             State = jj != null
                         };
            return result.ToList();
        }
        #endregion
        #region cache
        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <returns>加密后的字符串</returns>
        private static string ToMd5(string str)
        {
            byte[] sor = Encoding.UTF8.GetBytes(str);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }
        private string GenderCode()
        {
            string result = string.Empty;
            string[] arrString = { "A", "B", "C", "D", "E", "F", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", };
            Random rd = new Random();
            for (int i = 0; i < 8; i++)
            {
                result += arrString[rd.Next(arrString.Length - 1)];
            }
            return result;
        }
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
    }

    /// <summary>
    /// 手机端接口
    /// </summary>
    public interface IMobileAppService : IApplicationService
    {
        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        Task Sign(SignDto input);

        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<WarnDto> GetWarnsByUser(FilterDto input);

        /// <summary>
        /// 获取报警信息详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MobileWarnDto> GetWarnByUser(EntityDto<int> input);

        /// <summary>
        /// 处理报警信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DealWarn(EntityDto<int> input);

        /// <summary>
        /// 获取签到列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<SignPointDto>> GetSignList(FilterDto input);

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        List<CoffeeProduct> GetProducts();

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<StoreOrderDto>> GetOrders(GetOrderInput input);

        /// <summary>
        /// 提货
        /// </summary>
        /// <returns></returns>
        Task PickOrder(PickOrderInput input);

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        CoffeeProduct GetProduct(EntityDto input);

        /// <summary>
        /// 余额支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task BalancePay(InsertOrderInput input);

        /// <summary>
        /// 在线支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> LinePay(InsertOrderInput input);
    }
}
