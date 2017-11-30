<template>  
  <div>  
    <!-- <app-header></app-header>   -->
    <router-view></router-view>
    <app-footer></app-footer>  
  </div>  
</template>  

<script>
import { Tab, TabItem } from "vux";
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
    AppFooter
  }
};
</script>