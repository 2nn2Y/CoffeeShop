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
    this.init();
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
        if (r.data && r.data.result && r.data.result.items) {
          r.data.result.items.forEach(element => {
            const code = element.fastCode.split("");
            this.list.push({
              src: element.imageUrl,
              fallbackSrc: "http://placeholder.qiniudn.com/60x60/3cc51f/ffffff",
              title: element.productName,
              desc: `浓度${code[0]} 奶度${code[1]} 糖度${code[2]}`
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
      list: [],
      footer: {
        title: "显示更多"
      }
    };
  }
};
</script>
