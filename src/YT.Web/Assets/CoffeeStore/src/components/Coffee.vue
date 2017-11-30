<template>
  <div>
    <panel :header="'商品列表'" 
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
          title: "猫屎咖啡",
          desc: "由各种物质组成的巨型球状天体，叫做星球。星球有一定的形状，有自己的运行轨道。",
          url: "/order"
        },
        {
          src: "../assets/icons/b.png",
          title: "牛屎咖啡",
          desc: "由各种物质组成的巨型球状天体，叫做星球。星球有一定的形状，有自己的运行轨道。",
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