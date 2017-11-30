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
            //�ʼ����͹���
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "454258314@qq.com");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "����");
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
                new Product(102, "�ȿ�����ŵ"),
                new Product(933, "��������ŵ"),
                new Product(246, "������"),
                new Product(934, "������"),
                new Product(103, "����ʽŨ������"),
                new Product(930, "����ʽ����"),
                new Product(105, "����ʽ����"),
                new Product(283, "��Ħ��"),
                new Product(938, "����ʽĨ������"),
                new Product(245, "����ʽĨ������"),
                new Product(701, "�ȸ�ʽ˿���̲�"),
                new Product(802, "�ȿɿ�ţ��"),
                new Product(240, "�������Ŷ�"),
                new Product(104, "�ȸ�ʽԧ��"),
                new Product(810, "���㴼ţ��"),
                new Product(800, "����Ũ�ɿ���"),
                new Product(204, "�ȳ�����"),
                new Product(700, "���������")
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
