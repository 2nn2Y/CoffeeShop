using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using Abp.UI;
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
        private readonly IRepository<Warn> _warnRepository;
        private readonly IRepository<SignRecord> _signRepository;
        private readonly IRepository<UserPoint> _userpointRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Point> _pointRepository;
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
        public MobileAppService(IRepository<Warn> warnRepository,
            ICacheManager cacheManager, IRepository<Product> productRepository, IRepository<Point> pointRepository, IRepository<SignRecord> signRepository, IRepository<UserPoint> userpointRepository)
        {
            _warnRepository = warnRepository;
            _cacheManager = cacheManager;
            _productRepository = productRepository;
            _pointRepository = pointRepository;
            _signRepository = signRepository;
            _userpointRepository = userpointRepository;
        }

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
                .GetAllListAsync(c=>c.CreationTime>=today&&c.CreationTime<next);
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
    }

}
