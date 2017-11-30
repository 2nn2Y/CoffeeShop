<template>
  <div>
    <panel :header="'我的订单'" 
    :footer="footer" :list="list" @on-click-item="detail" :type="'5'" @on-click-footer="next"></panel>
  </div>
</template>



<script>
import { Panel, Group, Radio } from "vux";

export default {
  components: {
    Panel,
    Group,
    Radio
  },
  created() {
    this.callTitle("订单");
  },
  methods: {
    next() {
      this.showBox("xia", "下一页");
    },
    detail(item) {
      this.showBox("详情", item.title);
    }
  },
  data() {
    return {
      type: "1",
      list: [
        {
          src: "../assets/icons/a.png",
          fallbackSrc: "http://placeholder.qiniudn.com/60x60/3cc51f/ffffff",
          title: "猫屎咖啡",
          desc: "口味一",
          url: "/order"
        },
        {
          src: "../assets/icons/b.png",
          title: "牛屎咖啡",
          desc: "口味二",
          url: {
            path: "/order",
            replace: false
          }
        }
      ],
      footer: {
        title: "显示更多"
      }
    };
  }
};
</script>