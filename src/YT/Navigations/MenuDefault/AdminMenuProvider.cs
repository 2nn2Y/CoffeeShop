using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using YT.Authorizations.PermissionDefault;

namespace YT.Navigations.MenuDefault
{
    public abstract class BaseMenuProvider : MenuProvider
    {

    }

    public class AdminMenuProvider : BaseMenuProvider
    {
        public override IEnumerable<MenuDefinition> GetMenuDefinitions(MenuDefinitionProviderContext context)
        {
            return new List<MenuDefinition>()
           {
                  new MenuDefinition("权限管理","","settings",true,StaticPermissionsName.Page_System)
                {
                    Childs = new List<MenuDefinition>()
                    {
                        new MenuDefinition("用户管理","/users","",true,StaticPermissionsName.Page_System_User),
                        new MenuDefinition("角色管理","/roles","",true,StaticPermissionsName.Page_System_Role),
                        new MenuDefinition("签到统计","/sign","",true,StaticPermissionsName.Page_Staticical_Sign),
                        new MenuDefinition("签到明细","/signdetail","",true,StaticPermissionsName.Page_Staticical_SignDetail),
                        new MenuDefinition("故障统计-设备","/warndevice","",true,StaticPermissionsName.Page_Staticical_WarnDevice),
                        new MenuDefinition("报警信息","/warn","",true,StaticPermissionsName.Page_Staticical_Warn),
                        new MenuDefinition("成交订单","/order","",true,StaticPermissionsName.Page_Staticical_Order),
                        new MenuDefinition("产品销量","/productsale","",true,StaticPermissionsName.Page_Staticical_Productsale),
                        new MenuDefinition("设备销量","/devicesale","",true,StaticPermissionsName.Page_Staticical_DeviceSale),
                        new MenuDefinition("区域销量","/areasale","",true,StaticPermissionsName.Page_Staticical_AreaSale),
                        new MenuDefinition("支付渠道","/paytype","",true,StaticPermissionsName.Page_Staticical_PayType),
                        new MenuDefinition("时段销量","/timearea","",true,StaticPermissionsName.Page_Staticical_TimeArea),
                    }
                },
           };
        }
    }
}
