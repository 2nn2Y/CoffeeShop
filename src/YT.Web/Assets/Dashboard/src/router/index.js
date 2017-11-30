import Vue from 'vue'
import Router from 'vue-router'
import Container from '@/components/Index'
import Home from '@/components/Home'


Vue.use(Router)

export default new Router({
  linkActiveClass: 'active',
  routes: [{
    path: '/',
    component: Container
  },
  {
    path: '/index',
    component: Container,
    children: [{
      path: '/regist',
      // 懒加载
      component: resolve => require(['@/components/Regist.vue'], resolve)
    }]
  },
  {
    path: '/home',
    component: Home,
    children: [
      { path: '/users', component: resolve => require(['views/user'], resolve) },
      { path: '/roles', component: resolve => require(['views/role'], resolve) },
      { path: '/order', component: resolve => require(['views/order'], resolve) },
      { path: '/productsale', component: resolve => require(['views/productsale'], resolve) },
      { path: '/devicesale', component: resolve => require(['views/devicesale'], resolve) },
      { path: '/areasale', component: resolve => require(['views/areasale'], resolve) },
      { path: '/paytype', component: resolve => require(['views/paytype'], resolve) },
      { path: '/timearea', component: resolve => require(['views/timearea'], resolve) },
      { path: '/sign', component: resolve => require(['views/sign'], resolve) },
      { path: '/signdetail', component: resolve => require(['views/signdetail'], resolve) },
      { path: '/warndevice', component: resolve => require(['views/warndevice'], resolve) },
      { path: '/warn', component: resolve => require(['views/warn'], resolve) }
    ]
  }]
})