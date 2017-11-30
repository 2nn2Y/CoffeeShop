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

Vue.prototype.$down = (type, token, name) => {
    let url =
        "http://103.45.102.47:8888/api/File/Download?fileType=" +
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
    components: { App }
})