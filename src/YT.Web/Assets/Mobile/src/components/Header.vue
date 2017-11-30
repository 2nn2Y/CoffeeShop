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

export default {
  directives: {
    TransferDom
  },
  components: {
    XHeader,
    Actionsheet
  },
  data() {
    return {
      title: "故障处理",
      menus: {
        menu1: "a",
        menu2: "b"
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