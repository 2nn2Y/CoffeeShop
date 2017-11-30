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
                        new PermissionDefinition(StaticPermissionsName.Page_Dashboard, "控制台", "整体概述",
                            PermissionType.Control),

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
                        }
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

    }
}
