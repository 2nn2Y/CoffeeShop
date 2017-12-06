<template>
  <div id="sidebar">
    <ul>
      <div :key="item.url" v-for="(item,index) in Sidebar">
        <li v-if="!item.children" class="list-item">
          <router-link :to="item.url">
            <Icon v-if="item.icon" style="margin-right:5px;" :type="item.icon"></Icon>
            {{item.content}}</router-link>
        </li>
        <li class="list-else" v-else >
          <a id="icona" @click.stop="extend(item)" v-if="!item.state"><Icon  style="margin-right:5px;"  type="chevron-up"></Icon>{{item.content}}</a>
          <a id="icona" @click.stop="extend(item)" v-if="item.state"><Icon  style="margin-right:5px;"  type="chevron-down"></Icon>{{item.content}}</a>

          <ul v-if="item.state">
            <li :key="b.url" v-for="(b,i) in item.children" class="list-item">
              <router-link @click.stop :to="b.url">{{b.content}}</router-link>
            </li>
          </ul>
        </li>
      </div>
    </ul>
  </div>
</template>

<script>
import { userMenus } from "api/menu";
export default {
  data() {
    return {
      temp: [],
      list: [],
      Sidebar: [
        { content: "用户管理", url: "/users", icon: "person" },
        { content: "角色管理", url: "/roles", icon: "person-stalker" },
        {
          content: "数据统计",
          url: "",
          icon: "levels",
          state: false,
          children: [
            { content: "签到统计", url: "/sign" },
            { content: "签到明细", url: "/signdetail" },
            { content: "故障统计-设备", url: "/warndevice" },
            { content: "报警信息", url: "/warn" },
            { content: "成交订单", url: "/order" },
            { content: "产品销量", url: "/productsale" },
            { content: "设备销量", url: "/devicesale" },
            { content: "区域销量", url: "/areasale" },
            { content: "支付渠道", url: "/paytype" },
            { content: "时段销量", url: "/timearea" }
          ]
        }
      ]
    };
  },
  created() {
    this.init();
  },
  methods: {
    extend(item) {
      item.state = !item.state;
    },
    init() {
      userMenus().then(r => {
        if (r.success) {
          this.temp = r.result;
          this.genderMenus();
        }
      });
    },
    genderMenus() {
      this.temp.forEach(element => {
        if (!element.parentId) {
          const model = {
            content: element.displayName,
            url: element.url,
            icon: element.icon
          };
          this.temp.forEach(c => {
            if (c.parentId == element.id) {
              model.children.push({
                content: c.displayName,
                url: c.url,
                icon: c.icon
              });
            }
          });
          this.list.push(model);
        }
      });
    }
  }
};
</script>

<style scoped>
#sidebar {
  position: fixed;
  overflow: auto;
  height: 600px;
  width: 180px;
  z-index: 10;
  border-right: 1px solid #fff;
}
#icona {
  display: block;
}
.list-else {
  height: 40px;
  line-height: 50px;
  text-align: center;
  font-family: "微软雅黑";
}

.list-item {
  height: 50px;
  line-height: 50px;
  margin-left: 10px;
  text-align: center;
  font-family: "微软雅黑";
}

.list-item a.active {
  display: block;
  background: rgba(7, 17, 27, 1);
}

.list-item a.hover {
  display: block;
  background: burlywood;
}
</style>
