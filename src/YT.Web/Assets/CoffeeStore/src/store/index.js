/**
 * 设置vuex
 */
import Vue from "vue"
import Vuex from "vuex"
Vue.use(Vuex);
const store = new Vuex.Store({})
store.registerModule("vux", {
    state: {
        loading: false,
        showBack: true,
        title: ""
    },
    mutations: {
        updateLoading(state, loading) {
            state.loading = loading
        },
        updateShowBack(state, showBack) {
            state.showBack = showBack
        },
        updateTitle(state, title) {
            state.title = title
        }
    }
})