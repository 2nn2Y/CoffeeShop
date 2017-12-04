<template>
  <div>
    <card>
      <img slot="header" src="http://placeholder.qiniudn.com/640x300" style="width:100%;display:block;">
      <div slot="content" class="card-padding">
      </div>
    </card>
    <panel :header="'商品列表'"  :list="list"  :type="'5'" @on-click-footer="next"></panel>
  </div>
</template>



<script>
import { Panel, Group, Radio, Card } from "vux";
export default {
  components: {
    Panel,
    Group,
    Radio,
    Card
  },
  created() {
    this.callTitle("咖啡店");
    this.init();
  },
  methods: {
    init() {
      const url =
        "http://103.45.102.47:8888/api/services/app/mobile/GetProducts";
      this.$http.post(url, {}).then(r => {
        if (r.data && r.data.result) {
          r.data.result.forEach(element => {
            this.list.push({
              src: element.imageUrl,
              fallbackSrc: "http://placeholder.qiniudn.com/60x60/3cc51f/ffffff",
              title: element.productName,
              desc: element.description,
              url: "/detail/" + element.id
            });
          });
        }
      });
    },
    next() {
      this.showBox("xia", "下一页");
    }
  },

  data() {
    return {
      type: "1",
      list: [],
      footer: {
        title: "显示更多"
      }
    };
  }
};
</script>
