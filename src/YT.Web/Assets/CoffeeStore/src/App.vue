<template>
  <div id="app">
    <loading v-model="loading"></loading>
    <router-view></router-view>
  </div>
</template>
<script>
import { Loading } from "vux";
import { mapState } from "vuex";
export default {
  name: "app",
  components: {
    Loading
  },
  computed: {
    ...mapState({
      loading: state => state.loading
    })
  }
};
</script>

<style lang="less">
@import "~vux/src/styles/reset.less";

body {
  background-color: #fbf9fe;
}
</style>
