<template>
  <div>
    <panel :header="'商品详情'" :list="list" :type="'5'"></panel>
    <divider>咖啡配置</divider>
    <group>
      <x-number title="浓度" :min="0" :max="5" v-model="value"></x-number>
      <x-number title="奶度" :min="0" :max="5" v-model="value"></x-number>
      <x-number title="糖度" :min="0" :max="5" v-model="value"></x-number>
      <selector title="选择代金券" :options="cards" v-model="value"></selector>
      <cell title="总金额" v-model="value"></cell>
    </group>
    <grid>
      <grid-item>
        <box gap="10px 10px">
          <x-button type="primary">余额支付</x-button>
        </box>
      </grid-item>
      <grid-item>
        <box gap="10px 10px">
          <x-button type="primary">在线支付</x-button>
        </box>
      </grid-item>
    </grid>
  </div>
</template>

<script>
import {
  Panel,
  Group,
  Radio,
  XNumber,
  Divider,
  Selector,
  Cell,
  Grid,
  GridItem,
  XButton,
  Box
} from "vux";
export default {
  components: {
    Panel,
    Group,
    Radio,
    XNumber,
    Divider,
    Selector,
    Cell,
    Grid,
    GridItem,
    XButton,
    Box
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
      cards: [
        {
          key: 1,
          value: "打折券"
        },
        {
          key: 2,
          value: "满减券"
        }
      ],
      type: "1",
      list: [
        {
          src: "../assets/icons/a.png",
          fallbackSrc: "http://placeholder.qiniudn.com/60x60/3cc51f/ffffff",
          title: "猫屎咖啡",
          desc: "由各种物质组成的巨型球状天体，叫做星球。星球有一定的形状，有自己的运行轨道。"
        }
      ],
      footer: {
        title: "显示更多"
      }
    };
  }
};
</script>
