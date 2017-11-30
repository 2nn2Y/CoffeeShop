using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using Abp.Net.Mail;
using YT.Editions;
using YT.EntityFramework;
using YT.Managers.MultiTenancy;
using YT.Models;

namespace YT.Migrations.Seed
{
    public class DefaultTenantBuilder
    {
        private readonly MilkDbContext _context;

        public DefaultTenantBuilder(MilkDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultTenant();
            CreateDefaultProduct();
           // CreateDefaultPoint();
            CreateDefaultUserPoint();
        }

        private void CreateDefaultTenant()
        {

            //Default tenant

            var defaultTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                defaultTenant = new Tenant(Tenant.DefaultTenantName,
                    Tenant.DefaultTenantName);

                var defaultEdition = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                if (defaultEdition != null)
                {
                    defaultTenant.EditionId = defaultEdition.Id;
                }

                _context.Tenants.Add(defaultTenant);
                _context.SaveChanges();
            }
            //邮件发送管理
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "454258314@qq.com");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "华夏");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Port, "587");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Host, "smtp.qq.com");
            AddSettingIfNotExists(EmailSettingNames.Smtp.UserName, "454258314@qq.com");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Password, "vvkqelenlrambhgj");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Domain, "");
            AddSettingIfNotExists(EmailSettingNames.Smtp.EnableSsl, "true");
            AddSettingIfNotExists(EmailSettingNames.Smtp.UseDefaultCredentials, "false");
        }


     
        private void CreateDefaultProduct()
        {
            var plist = new List<Product>()
            {
                new Product(102, "热卡布奇诺"),
                new Product(933, "冰卡布奇诺"),
                new Product(246, "热拿铁"),
                new Product(934, "冰拿铁"),
                new Product(103, "热意式浓缩咖啡"),
                new Product(930, "冰美式咖啡"),
                new Product(105, "热美式咖啡"),
                new Product(283, "热摩卡"),
                new Product(938, "冰日式抹茶拿铁"),
                new Product(245, "热日式抹茶拿铁"),
                new Product(701, "热港式丝袜奶茶"),
                new Product(802, "热可可牛奶"),
                new Product(240, "热玛琪雅朵"),
                new Product(104, "热港式鸳鸯"),
                new Product(810, "热香醇牛奶"),
                new Product(800, "热香浓巧克力"),
                new Product(204, "热长咖啡"),
                new Product(700, "热锡兰红茶")
            };
            foreach (var product in plist)
            {
                AddProductsIfNotExists(product);
            }
        }
        private void CreateDefaultUserPoint()
        {

        }
        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
        private void AddProductsIfNotExists(Product product)
        {
            if (_context.Products.Any(s => s.ProductId.Equals(product.ProductId)))
            {
                return;
            }
            _context.Products.Add(product);
            _context.SaveChanges();
        }
    }
}
