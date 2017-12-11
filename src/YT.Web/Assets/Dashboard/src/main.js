// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import iView from 'iview';
import dtime from 'time-formater';
// import 'iview/dist/styles/iview.css';
import '../src/my-theme/index.less';
import BaiduMap from 'vue-baidu-map'


import MilkTable from 'components/Table';
import TreeGrid from 'components/TreeGrid';
Vue.component('milk-table', MilkTable);
Vue.component('tree-table', TreeGrid);
Vue.use(iView);
Vue.config.productionTip = false
Vue.use(BaiduMap, {
  // ak 是在百度地图开发者平台申请的密钥 详见 http://lbsyun.baidu.com/apiconsole/key */
  ak: 'pYjoSR2GThuatLt06MlaKzRgSWy4Zztq'
})
/* 格式化日期*/
Vue.prototype.$fmtTime = (date, format) =>
  dtime(date).format(format || 'YYYY-MM-DD HH:mm:ss');
/* 列表格式转换成树格式
 * @param data 数组
 * @param parentId 父节点id
 * @param pidField 父节点字段名
 * @param grants 已授权信息
 */
const converToTreedata = (data, parentId, pidField, grants) => {
  const list = [];
  data.forEach(item => {
    if (item[pidField] === parentId) {
      item.children = converToTreedata(data, item.id, pidField, grants);
      item.title = item.displayName;
      if (grants) {
        const temp = grants.findIndex(key => key === item.name);
        if (temp > 0) {
          if (!item.children || item.children.length <= 0) {
            item.checked = true;
            item.expand = true;
          } else {
            item.checked = false;
            item.expand = true;
          }
        } else {
          item.checked = false;
          item.expand = true;
        }
      }
      list.push(item);
    }
  });
  return list;
};
Vue.prototype.$converToTreedata = converToTreedata;
Vue.prototype.$down = (type, token, name) => {
  let url =
    "http://services.youyinkeji.cn/api/File/Download?fileType=" +
    type +
    "&fileToken=" +
    token +
    "&fileName=" +
    name;
  window.open(url);
}

router.beforeEach((to, from, next) => {
  const token = sessionStorage.getItem("token");
  if (!token && to.path !== "/index") {
    next("/index")
    return false;
  } else {
    next();
  }

})
/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  data: {
    eventHub: new Vue()
  },
  components: { App }
})
