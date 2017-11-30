import fetch from 'utils/fetch';
// 获取订单统计
export function getWarnByDevice(data) {
    return fetch({
        url: '/api/services/app/data/GetWarnByDevice',
        method: 'post',
        data
    });
}
export function getWarnByUser(data) {
    return fetch({
        url: '/api/services/app/data/GetWarnByUser',
        method: 'post',
        data
    });
}
// 获取订单统计
export function exportWarnByDevice(data) {
    return fetch({
        url: '/api/services/app/data/ExportWarnByDevice',
        method: 'post',
        data
    });
}
export function exportWarnByUser(data) {
    return fetch({
        url: '/api/services/app/data/ExportWarnByUser',
        method: 'post',
        data
    });
}