<template>  
  <tabbar style="position: fixed">  
    <tabbar-item  link="/accident">  
      <img slot="icon" :selected="select"  src="../assets/icons/a.png">  
      <span slot="label">故障处理</span>  
    </tabbar-item>  
    <tabbar-item :selected="select" link="/sign">  
      <img slot="icon"  src="../assets/icons/b.png">  
      <span slot="label">签到</span>  
    </tabbar-item>  
  </tabbar>  
</template>  
<style>
.add {
  height: 40px;
  width: 40px;
}
</style>  
<script>
import { Tabbar, TabbarItem } from "vux";
export default {
  name: "AppFooter",
  components: {
    Tabbar,
    TabbarItem
  },
  data() {
    return {
      select: false,
      msg: ""
    };
  }
};
</script> 