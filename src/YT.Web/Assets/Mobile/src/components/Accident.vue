<template>
  <div style="height:100%;">
    <view-box ref="viewBox">
     <card :header="{title: '未处理'}">
      <div slot="content" >
      <x-button @click.native="detail(a)"
       v-for="(a,i) in undeal"
       :key="i" :gradients="['#FF2719', '#FF61AD']" type="warn">{{a.deviceName}}--{{a.content}}</x-button>
      </div>
    </card>
      <card :header="{title: '已处理'}">
      <div slot="content" >
      <x-button  @click.native="detail(b)"
       v-for="(b,i) in deal"
       :key="i" type="info" :gradients="['#1D62F0', '#19D5FD']" >{{b.deviceName}}--{{b.content}}</x-button>
      </div>
    </card>
    </view-box>
  </div>
</template>
<script>
import { Group, Cell, Card, XButton, ViewBox } from "vux";
export default {
  name: "detail",
  data() {
    return {
      undeal: [],
      deal: []
    };
  },
  created() {
    this.init();
  },
  components: {
    Group,
    Cell,
    Card,
    XButton,
    ViewBox
  },
  methods: {
    detail(item) {
      this.$router.push({ path: "/detail", query: { id: item.id } });
    },
    init() {
      const url =
        "http://services.youyinkeji.cn/api/services/app/mobile/GetWarnsByUser";
      const params = {
        userId: sessionStorage.getItem("userId")
      };
      this.$http.post(url, params).then(r => {
        if (r.data) {
          this.undeal = r.data.result.anomaly;
          this.deal = r.data.result.normal;
        }
      });
    }
  }
};
</script>

<style scoped lang="less">
@import "~vux/src/styles/1px.less";

.card-demo-flex {
  display: flex;
}
.card-demo-content01 {
  padding: 10px 0;
}
.card-padding {
  padding: 15px;
}
.card-demo-flex > div {
  flex: 1;
  text-align: center;
  font-size: 12px;
}
.card-demo-flex span {
  color: #f74c31;
}
</style>
