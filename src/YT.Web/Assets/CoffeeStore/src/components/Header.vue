<template>
  <div>
    <x-header :right-options="{showMore: true}" @on-click-more="showMenus = true">{{title}}</x-header>
    <div v-transfer-dom>
      <actionsheet :menus="menus" v-model="showMenus" show-cancel></actionsheet>
    </div>
  </div>
</template>

<script>
import { XHeader, Actionsheet, TransferDom } from "vux";
import { mapState } from "vuex";

export default {
  directives: {
    TransferDom
  },
  components: {
    XHeader,
    Actionsheet
  },
  computed: {
    ...mapState({
      title: state => state.title
    })
  },
  data() {
    return {
      menus: {
        menu1: "分享到朋友圈",
        menu2: "分享给微信好友"
      },
      showMenus: false
    };
  }
};
</script>

<style>
.overwrite-title-demo {
  margin-top: 5px;
}
</style>