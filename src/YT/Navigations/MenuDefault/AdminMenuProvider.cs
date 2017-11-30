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
                    }
                },
           };
        }
    }
}
