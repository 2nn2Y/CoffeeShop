using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using YT.Navigations;

namespace YT.Authorizations.PermissionDefault
{
    public abstract class BasePermissionProvider : PermissionProvider
    {

    }
    public class AdminPermissionProvider : BasePermissionProvider
    {
        public override IEnumerable<PermissionDefinition> GetPermissionDefinitions(PermissionDefinitionProviderContext context)
        {
            return new List<PermissionDefinition>()
            {

                new PermissionDefinition(StaticPermissionsName.Page, "页面", "鼻祖权限")
                {
                    Childs = new List<PermissionDefinition>()
                    {


                        new PermissionDefinition(StaticPermissionsName.Page_System, "权限管理", "权限管理",
                            PermissionType.Control)
                        {
                            Childs = new List<PermissionDefinition>()
                            {
                                new PermissionDefinition(StaticPermissionsName.Page_System_Role, "角色管理", "角色管理",
                                    PermissionType.Control)
                                {
                                    Childs = new List<PermissionDefinition>()
                                    {
                                        new PermissionDefinition(StaticPermissionsName.Page_System_Role_Create, "创建角色",
                                            "", PermissionType.Control),
                                        new PermissionDefinition(StaticPermissionsName.Page_System_Role_Edit, "编辑角色", "",
                                            PermissionType.Control),
                                        new PermissionDefinition(StaticPermissionsName.Page_System_Role_Delete, "删除角色",
                                            "", PermissionType.Control),
                                    }
                                },
                                new PermissionDefinition(StaticPermissionsName.Page_System_User, "账户管理", "账户管理",
                                    PermissionType.Control)
                                {
                                    Childs = new List<PermissionDefinition>()
                                    {
                                        new PermissionDefinition(StaticPermissionsName.Page_System_User_Create, "创建账户",
                                            "", PermissionType.Control),
                                        new PermissionDefinition(StaticPermissionsName.Page_System_User_Delete, "编辑账户",
                                            "", PermissionType.Control),
                                        new PermissionDefinition(StaticPermissionsName.Page_System_User_Edit, "删除账户", "",
                                            PermissionType.Control),
                                    }
                                }
                            }
                        },
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_Sign, "签到统计", "签到统计",
                            PermissionType.Control),
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_SignDetail, "签到明细", "签到明细",
                            PermissionType.Control),
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_WarnDevice, "故障统计-设备", "故障统计-设备",
                            PermissionType.Control),
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_Warn, "报警信息", "报警信息",
                            PermissionType.Control),
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_Order, "成交订单", "成交订单",
                            PermissionType.Control),
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_Productsale, "产品销量", "产品销量",
                            PermissionType.Control),
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_DeviceSale, "设备销量", "设备销量",
                            PermissionType.Control),
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_AreaSale, "区域销量", "区域销量",
                            PermissionType.Control),
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_PayType, "支付渠道", "支付渠道",
                            PermissionType.Control),
                        new PermissionDefinition(StaticPermissionsName.Page_Staticical_TimeArea, "时段销量", "时段销量",
                            PermissionType.Control),

                    }
                }
            };
        }
    }
    /// <summary>
    /// 静态权限名
    /// </summary>
    public class StaticPermissionsName
    {
        //默认首页权限
        public const string Page = "page";

        public const string Page_Dashboard = "page.dashboard";

       


        /// <summary>
        /// xitong
        /// </summary>
        public const string Page_System = "page.system";
        public const string Page_System_Role = "page.system.role";
        public const string Page_System_Role_Create = "page.system.role.create";
        public const string Page_System_Role_Edit = "page.system.role.edit";
        public const string Page_System_Role_Delete = "page.system.role.delete";
        public const string Page_System_User = "page.system.user";
        public const string Page_System_User_Create = "page.system.user.create";
        public const string Page_System_User_Edit = "page.system.user.edit";
        public const string Page_System_User_Delete = "page.system.user.delete";


        public const string Page_Staticical_Sign = "page.staticical.sign";
        public const string Page_Staticical_SignDetail = "page.staticical.signdetail";
        public const string Page_Staticical_WarnDevice = "page.staticical.warndevice";
        public const string Page_Staticical_Warn = "page.staticical.warn";
        public const string Page_Staticical_Order= "page.staticical.order";
        public const string Page_Staticical_Productsale = "page.staticical.productsale";
        public const string Page_Staticical_DeviceSale = "page.staticical.devicesale";
        public const string Page_Staticical_AreaSale = "page.staticical.areasale";
        public const string Page_Staticical_PayType = "page.staticical.paytype";
        public const string Page_Staticical_TimeArea = "page.staticical.timearea";


    }
}
