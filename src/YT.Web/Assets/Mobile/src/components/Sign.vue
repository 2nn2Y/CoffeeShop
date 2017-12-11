<template>
  <div class="vux-circle">
     <view-box ref="viewBox">
     <card :header="{title: '签到列表'}">
      <div slot="content" >
       <x-button @click.native="sign(a)"
        v-for="(a,i) in things"
         :key="i" :gradients="a.state?signcolor:unsigncolor"
           :type="a.state">{{a.point}}({{a.pointId}})--{{a.state?'已签':'未签'}}</x-button>
      </div>
    </card>
     </view-box >
  </div>
</template>
<script>
import { Group, Cell, Card, XButton, Divider, Icon, ViewBox } from "vux";

export default {
  name: "sign",
  data() {
    return {
      images: [],
      unsigncolor: ["#FF2719", "#FF61AD"],
      signcolor: ["#1D62F0", "#19D5FD"],
      things: []
    };
  },
  created() {
    var _self = this;
    const url = document.location.href; // 当前url
    _self.wxConfig(url);
    this.getsigns();
  },
  components: {
    Group,
    Cell,
    Card,
    XButton,
    Divider,
    Icon,
    ViewBox
  },
  methods: {
    sign(item) {
      if (item.state) {
        this.showBox("已签到", "不可重复签到");
        return;
      } else {
        this.$router.push({ name: "map", params: { point: item.id } });
      }
    },
    getsigns() {
      const url =
        "http://services.youyinkeji.cn/api/services/app/mobile/GetSignList";
      const params = {
        userId: sessionStorage.getItem("userId")
      };
      this.$http.post(url, params).then(r => {
        if (r.data) {
          this.things = r.data.result;
        }
      });
    }
  }
};
</script>

<style scoped lang="less">
@import "~vux/src/styles/1px.less";
.vux-circle {
  text-align: center;
}

.vux-circle-demo > div {
  margin: 0 auto;
}
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
