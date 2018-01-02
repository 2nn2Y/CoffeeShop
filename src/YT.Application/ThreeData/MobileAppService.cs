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
using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using Newtonsoft.Json;
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
        private readonly IRepository<UserCard> _cardRepository;
        /// <summary>
        /// 产品列集合
        /// </summary>
        private readonly IRepository<Warn> _warnRepository;
        private readonly IRepository<StoreUser> _userRepository;
        private readonly IRepository<SignRecord> _signRepository;
        private readonly IRepository<UserPoint> _userpointRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Point> _pointRepository;
        private readonly IRepository<StoreOrder> _storeRepository;
        private readonly IRepository<ChargeType> _chargeTypeRepository;
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
       /// <param name="cardRepository"></param>
       /// <param name="chargeTypeRepository"></param>
        public MobileAppService(IRepository<Warn> warnRepository,
            ICacheManager cacheManager, IRepository<Product> productRepository, IRepository<Point> pointRepository, IRepository<SignRecord> signRepository, IRepository<UserPoint> userpointRepository, IRepository<StoreOrder> storeRepository, IRepository<StoreUser> userRepository, IRepository<UserCard> cardRepository, IRepository<ChargeType> chargeTypeRepository)
        {
            _warnRepository = warnRepository;
            _cacheManager = cacheManager;
            _productRepository = productRepository;
            _pointRepository = pointRepository;
            _signRepository = signRepository;
            _userpointRepository = userpointRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _chargeTypeRepository = chargeTypeRepository;
        }
        #region H5商城
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> GetProducts()
        {
            var query = (await GetProductFromCache()).Where(c => !c.IsCard);
            return query.ToList();
        }
        /// <summary>
        /// 获取卡圈列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> GetCards()
        {
            var query = (await GetProductFromCache()).Where(c => c.IsCard);
            return query.ToList();
        }
        /// <summary>
        /// 获取我的列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<CardInfo>> GetUserCards(EntityDto<string> input)
        {

            var cards = await _cardRepository.GetAllListAsync(c => c.OpenId.Equals(input.Id) && !c.State);
            return cards.Select(c => new CardInfo()
            {
                Cost = c.Cost,
                Id = c.Key,
                Image = c.Image,
                Name = c.ProductName,
                Description = c.Description
            }).ToList();
        }
        /// <summary>
        /// 获取产品详情
        /// </summary>
        /// <returns></returns>
        public async Task<ProductAndCardDto> GetProduct(FilterDto input)
        {
            var cards = await _cardRepository.GetAllListAsync(c => c.OpenId.Equals(input.UserId));
            var model = new ProductAndCardDto()
            {
                Product = await _productRepository.FirstOrDefaultAsync(c => c.ProductId == input.ProductId),
            };
            if (cards.Any())
            {
                model.Cards = cards.Select(c => new CardInfo()
                {
                    Id = c.Key,
                    Cost = c.Cost,
                    Value = c.ProductName
                }).ToList();
            }
            return model;
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

            var p = await _productRepository.FirstOrDefaultAsync(c => c.Id == input.ProductId);
            if (p == null) throw new UserFriendlyException("该商品不存在");
            var o = await _storeRepository.FirstOrDefaultAsync(c =>
            c.OrderNum.Equals(input.Order));
            if (o != null && o.PayState.HasValue && o.PayState.Value) throw new UserFriendlyException("该订单已存在,请重新下单");
            o = new StoreOrder()
            {
                FastCode = input.FastCode,
                OpenId = input.OpenId,
                PayType = PayType.BalancePay,
                OrderType = input.OrderType,
                OrderState = null,
                OrderNum = input.Order,
                DeviceNum = input.Device,
                PayState = null,
                Price = p.Price,
                ProductId = p.ProductId
            };
            await _storeRepository.InsertOrUpdateAsync(o);
            await CurrentUnitOfWork.SaveChangesAsync();
            //使用优惠券
            if (input.Key.HasValue)
            {
                var card = await _cardRepository.FirstOrDefaultAsync(c => c.Key == input.Key.Value);
                if (card != null)
                {
                    var price = p.Price - card.Cost;
                    price = price < 0 ? 0 : price;
                    user.Balance -= price;
                    card.State = true;
                }
            }
            else
            {
                user.Balance -= input.Price;
            }
            o.PayState = true;
            //发货
            if (o.OrderType == OrderType.Ice)
            {
                var result = await PickProductIce(o);
                Logger.Warn($"出货通知日志:{o.OrderNum}----{result}");
            }
            else
            {
                var temp = await PickProductJack(o);
                Logger.Warn($"出货通知日志:{o.OrderNum}----{temp}");
                if (temp.status == "success")
                {
                    o.OrderState = true;
                }
            }
        }
        /// <summary>
        /// 获取所有充值类型
        /// </summary>
        /// <returns></returns>
        public async Task<List<ChargeType>> GetAllCharges()
        {
            var list = await _chargeTypeRepository.GetAllListAsync();
            return list;
        }

        /// <summary>
        /// 在线支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> LinePay(InsertOrderInput input)
        {
            var p = await _productRepository.FirstOrDefaultAsync(c => c.Id == input.ProductId);
            if (p == null) throw new UserFriendlyException("该商品不存在");
            var order = await _storeRepository.FirstOrDefaultAsync(c => c.OrderNum.Equals(input.Order));
            if (order == null)
            {
                order = new StoreOrder()
                {
                    FastCode = input.FastCode,
                    OpenId = input.OpenId,
                    PayType = PayType.LinePay,
                    OrderType = input.OrderType,
                    OrderState = null,
                    OrderNum = input.Order,
                    DeviceNum = input.Device,
                    PayState = null,
                    Price = p.Price,
                    ProductId = p.ProductId
                };
            }
            else
            {
                if (order.PayState.HasValue && order.PayState.Value)
                {
                    throw new UserFriendlyException("该订单已支付,不可重复付款");
                }
                order.OpenId = input.OpenId;
                order.Price = p.Price;
                order.FastCode = input.FastCode;
                order.OrderType = input.OrderType;
                order.PayType = PayType.LinePay;
            }
            JsApiPay jsApiPay = new JsApiPay
            {
                Openid = input.OpenId,
                TotalFee = p.Price
            };
            var card = await _cardRepository.FirstOrDefaultAsync(c => c.Key == input.Key.Value);
            //使用优惠券
            if (card != null)
            {
                order.UseCard = input.Key;
                var t = p.Price - card.Cost;
                jsApiPay.TotalFee = t <= 0 ? 0 : t;

            }
            await _storeRepository.InsertOrUpdateAsync(order);
            jsApiPay.GetUnifiedOrderResult(order.OrderNum, p.ProductName, p.Description);
            var param = jsApiPay.GetJsApiParameters();
            return param;
        }
        /// <summary>
        /// 卡券支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> CardPay(InsertOrderInput input)
        {
            var p = await _productRepository.FirstOrDefaultAsync(c => c.Id == input.ProductId);
            if (p == null) throw new UserFriendlyException("该商品不存在");
            var order = new StoreOrder()
            {
                OpenId = input.OpenId,
                PayType = PayType.ActivityPay,
                OrderNum = Guid.NewGuid().ToString("N"),
                OrderState = null,
                PayState = null,
                Price = p.Price,
                ProductId = p.ProductId
            };
            await _storeRepository.InsertAsync(order);
            JsApiPay jsApiPay = new JsApiPay
            {
                Openid = input.OpenId,
                TotalFee = p.Price
            };
            jsApiPay.GetUnifiedOrderResult(order.OrderNum, p.ProductName, p.Description);
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
                OrderNum = Guid.NewGuid().ToString("N"),
                OrderState = null,
                PayState = null,
                Price = input.Price,
                ProductId = input.ProductId
            };
            var type = await _chargeTypeRepository.FirstOrDefaultAsync(input.ProductId);
            if (type == null) throw new UserFriendlyException("充值类型不存在");
            await _storeRepository.InsertAsync(order);
            JsApiPay jsApiPay = new JsApiPay
            {
                Openid = input.OpenId,
                TotalFee = type.Cost
            };
            jsApiPay.GetUnifiedOrderResult(order.OrderNum, "用户充值", "用户充值");
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
            var products = await _productRepository.GetAllListAsync();
            var orders = await _storeRepository.GetAll()
                .Where(c => c.OpenId.Equals(input.Device)
                            && c.PayState.HasValue && c.PayState.Value)
                .Where(c => c.PayType == PayType.BalancePay || c.PayType == PayType.LinePay)
                .Where(c => c.OrderState.HasValue && c.OrderState.Value)
                .ToListAsync();
            var count = orders.Count;
            var temp = orders.OrderByDescending(c => c.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount);

            var result = from c in temp
                         join d in products on c.ProductId equals d.ProductId
                         select new StoreOrderDto()
                         {
                             Description = "",
                             FastCode = c.FastCode,
                             Id = c.Id,
                             ImageUrl = d.ImageUrl,
                             ProductName = d.ProductName,
                             OpenId = c.OpenId
                         };
            return new PagedResultDto<StoreOrderDto>(count, result.ToList());
        }

        /// <summary>
        /// 取货
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> PickProductJack(StoreOrder order)
        {
            var url = "http://103.231.67.143:8079/FASTCODE";
            var temp = $@"ID={order.OrderNum}&USERNAME={"shuoyibuer"}&PASSWORD={"coffee888"}&VMC={
                order.DeviceNum}&PTYPE=FASTCODE&PID={order.ProductId}&FASTCODE={order.FastCode}";
            var mac = ToMd5(temp);

            var param = $@"ID={order.OrderNum}&PTYPE=FASTCODE&PID={order.ProductId}&VMC={
                order.DeviceNum}&FASTCODE={order.FastCode}&USERNAME={"shuoyibuer"}&MAC={mac}";
            url = url + "?" + param;
            var result = await HttpHandler.PostAsync<dynamic>(url);
            return result;
        }
        /// <summary>
        /// 取货
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> PickProductIce(StoreOrder order)
        {
            var url = order.NoticeUrl;
            if (url.IsNullOrWhiteSpace() || order.Key.IsNullOrWhiteSpace())
                throw new UserFriendlyException("该订单无效,没有回调地址");

            var result = HttpHandler.PostMoths(url, JsonConvert.SerializeObject(new
            {
                payStatus = "0",
                key = order.Key
            }));
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
                             Description = YtConsts.Types.FirstOrDefault(w => w.Type.Equals(warn.WarnNum))?.Chinese,
                             DeviceId = warn.DeviceNum,
                             Id = warn.Id,
                             State = warn.State,
                             DeviceName = e.SchoolName,WarnTime = warn.WarnTime
                         };
            var now = DateTime.Now.Date;
            return new WarnDto()
            {
                Anomaly = result.Where(c => !c.State).ToList(),
                Normal = result.Where(c => c.State).Where(c=>c.WarnTime.Date==now).ToList()
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
            warn.DealTime = DateTime.Now;
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
                result.Description =
                  YtConsts.Types.FirstOrDefault(c => c.Type.Equals(result.Content))?.Chinese;
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

        #region 报警相关
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

        #endregion

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        Task<List<Product>> GetProducts();

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> ChargePay(InsertOrderInput input);
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<StoreOrderDto>> GetOrders(GetOrderInput input);
        /// <summary>
        /// 获取产品详情
        /// </summary>
        /// <returns></returns>
        Task<ProductAndCardDto> GetProduct(FilterDto input);
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

        /// <summary>
        /// 获取卡圈列表
        /// </summary>
        /// <returns></returns>
        Task<List<Product>> GetCards();

        /// <summary>
        /// 获取我的列表
        /// </summary>
        /// <returns></returns>
        Task<List<CardInfo>> GetUserCards(EntityDto<string> input);

        /// <summary>
        /// 卡券支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> CardPay(InsertOrderInput input);

        /// <summary>
        /// 取货
        /// </summary>
        /// <returns></returns>
        Task<dynamic> PickProductJack(StoreOrder order);

        /// <summary>
        /// 取货
        /// </summary>
        /// <returns></returns>
        Task<dynamic> PickProductIce(StoreOrder order);
        /// <summary>
        /// 获取所有充值类型
        /// </summary>
        /// <returns></returns>
        Task<List<ChargeType>> GetAllCharges();
    }
}