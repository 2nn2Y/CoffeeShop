import fetch from 'utils/fetch';
// 获取订单统计
export function getOrderDetails(data) {
  return fetch({
    url: '/api/services/app/data/GetOrderDetails',
    method: 'post',
    data
  });
}
export function exportOrderDetails(data) {
  return fetch({
    url: '/api/services/app/data/ExportOrderDetails',
    method: 'post',
    data
  });
}
// 获取商品统计
export function getProductsSale(data) {
  return fetch({
    url: '/api/services/app/data/GetProductsSale',
    method: 'post',
    data
  });
}
// 获取商品统计
export function exportProductsSale(data) {
  return fetch({
    url: '/api/services/app/data/ExportProductsSale',
    method: 'post',
    data
  });
}
// 获取个点位商品统计
export function getDeviceProductsSale(data) {
  return fetch({
    url: '/api/services/app/data/GetDeviceProductsSale',
    method: 'post',
    data
  });
}
export function exportDeviceProductsSale(data) {
  return fetch({
    url: '/api/services/app/data/ExportDeviceProductsSale',
    method: 'post',
    data
  });
}
// 获取区域商品统计
export function getAreaProductsSale(data) {
  return fetch({
    url: '/api/services/app/data/GetAreaProductsSale',
    method: 'post',
    data
  });
}
// 获取区域商品统计
export function exportAreaProductsSale(data) {
  return fetch({
    url: '/api/services/app/data/ExportAreaProductsSale',
    method: 'post',
    data
  });
}
// 支付类型统计统计
export function getPayTypeSale(data) {
  return fetch({
    url: '/api/services/app/data/GetPayTypeSale',
    method: 'post',
    data
  });
}
// 支付类型统计统计
export function exportPayTypeSale(data) {
  return fetch({
    url: '/api/services/app/data/ExportPayTypeSale',
    method: 'post',
    data
  });
}
// 时间轴统计
export function getTimeAreaSale(data) {
  return fetch({
    url: '/api/services/app/data/GetTimeAreaSale',
    method: 'post',
    data
  });
}
export function exportTimeAreaSale(data) {
  return fetch({
    url: '/api/services/app/data/ExportTimeAreaSale',
    method: 'post',
    data
  });
}
// 时间轴统计
export function getSignByuser(data) {
  return fetch({
    url: '/api/services/app/data/GetSignsByUser',
    method: 'post',
    data
  });
}
export function exportSignByuser(data) {
  return fetch({
    url: '/api/services/app/data/ExportSignsByUser',
    method: 'post',
    data
  });
}
// 时间轴统计
export function getSignStaticial(data) {
  return fetch({
    url: '/api/services/app/data/GetSignsDetail',
    method: 'post',
    data
  });
}
export function exportSignStaticial(data) {
  return fetch({
    url: '/api/services/app/data/ExportSignsDetail',
    method: 'post',
    data
  });
}
export function getWarns(data) {
  return fetch({
    url: '/api/services/app/data/GetWarns',
    method: 'post',
    data
  });
}
export function exportWarns(data) {
  return fetch({
    url: '/api/services/app/data/ExportWarns',
    method: 'post',
    data
  });
}
export function getStoreOrders(data) {
  return fetch({
    url: '/api/services/app/data/GetStoreOrders',
    method: 'post',
    data
  });
}
export function exportStoreOrders(data) {
  return fetch({
    url: '/api/services/app/data/ExportStoreOrders',
    method: 'post',
    data
  });
}

export function getUserOrders(data) {
  return fetch({
    url: '/api/services/app/data/GetUserOrdersAsync',
    method: 'post',
    data
  });
}
export function exportUserOrders(data) {
  return fetch({
    url: '/api/services/app/data/ExportUserOrdersAsync',
    method: 'post',
    data
  });
}
export function getActivityAndChargeOrders(data) {
  return fetch({
    url: '/api/services/app/data/GetChargeAndActivityOrdersAsync',
    method: 'post',
    data
  });
}
export function exportActivityAndChargeOrders(data) {
  return fetch({
    url: '/api/services/app/data/ExportChargeAndActivityOrdersAsync',
    method: 'post',
    data
  });
}
