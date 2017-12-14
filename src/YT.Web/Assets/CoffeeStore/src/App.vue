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

html,body{
  height: 100%;
}
body{
  background-color: #f3f3f3;
}
#app{
  height: 100%
}
#vux_view_box_body{
  padding: 0
}
</style>
