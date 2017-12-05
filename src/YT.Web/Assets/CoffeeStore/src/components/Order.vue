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
  methods: {
    next() {
      this.current++;
      this.init();
    },
    init() {
      const url = "http://103.45.102.47:8888/api/services/app/mobile/GetOrders";
      const params = {
        device: sessionStorage.getItem("openid"),
        skipCount: (this.current - 1) * 10,
        maxResultCount: 10
      };
      this.$http.post(url, params).then(r => {
        if (r.data && r.data.result) {
          r.data.result.forEach(element => {
            this.list.push({
              src: element.imageUrl,
              fallbackSrc: "http://placeholder.qiniudn.com/60x60/3cc51f/ffffff",
              title: element.productName,
              desc: element.description
            });
          });
        }
      });
    }
  },
  data() {
    return {
      type: "1",
      current: 1,
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
