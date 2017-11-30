import Vue from "vue";
import Vuex from "vuex";
Vue.use(Vuex);
// 需要维护的状态
const state = {
    title: "唯一咖啡店",
    loading: false
};

const mutations = {
    // 初始化 state
    calltitle(state, title) {
        state.title = title
    },
    updateLoading(state, value) {
        state.loading = value
    }
};

export default new Vuex.Store({
    state,
    mutations
});