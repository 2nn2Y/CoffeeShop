<template>
  <div style="height:100%;">
    <view-box ref="viewBox">
      <app-header></app-header>
      <router-view></router-view>
      <app-footer></app-footer>
    </view-box>
  </div>
</template>

<script>
import { Tab, TabItem, ViewBox } from "vux";
import AppHeader from "./Header";
import AppFooter from "./Footer";

export default {
  data() {
    return {};
  },
  components: {
    Tab,
    TabItem,
    AppHeader,
    AppFooter,
    ViewBox
  }
};
</script>


