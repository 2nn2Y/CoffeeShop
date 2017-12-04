<template>
  <div>
    <card>
      <img slot="header" src="http://placeholder.qiniudn.com/640x300" style="width:100%;display:block;">
      <div slot="content" class="card-padding">
      </div>
    </card>
    <panel :header="'商品列表'" :footer="footer" :list="list" @on-click-item="detail" :type="'5'" @on-click-footer="next"></panel>
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
          desc: "由各种物质组成的巨型球状天体，叫做星球。星球有一定的形状，有自己的运行轨道。",
          url: "/detail/1"
        },
        {
          src: "../assets/icons/b.png",
          title: "牛屎咖啡",
          fallbackSrc: "http://placeholder.qiniudn.com/60x60/3cc51f/ffffff",
          desc: "由各种物质组成的巨型球状天体，叫做星球。星球有一定的形状，有自己的运行轨道。",
          url: {
            path: "/detail/2",
            replace: false
          },
          meta: {
            source: "来源信息",
            date: "时间",
            other: "其他信息"
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
