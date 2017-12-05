<template>
  <div>
    <panel :header="'商品详情'" :list="list" :type="'5'"></panel>
    <divider>咖啡配置</divider>
    <group>
      <x-number title="浓度" :min="0" :max="5" v-model="call.o"></x-number>
      <x-number title="奶度" :min="0" :max="5" v-model="call.m"></x-number>
      <x-number title="糖度" :min="0" :max="5" v-model="call.t"></x-number>
      <selector title="选择代金券" :options="cards" v-model="selectCard"></selector>
      <cell title="总金额" v-model="model.price"></cell>
    </group>
    <grid>
      <grid-item>
        <box gap="10px 10px">
          <x-button type="primary" @click.native="balancepay">余额支付</x-button>
        </box>
      </grid-item>
      <grid-item>
        <box gap="10px 10px">
          <x-button type="primary" @click.native="linepay">在线支付</x-button>
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
      this.init();
      this.getCard();
    },
    computed: {
      fastcode() {
        return this.call.o + "" + this.call.m + "" + this.call.s;
      }
    },
    data() {
      return {
        call: {
          o: 0,
          m: 0,
          s: 0
        },
        selectCard: null,
        model: {},
        cards: [],
        list: []
      };
    },
    methods: {
      init() {
        debugger;
        const params = this.$route.params.id.split("-");
        const fastcode=params[2].split("");
        this.call.o=fastcode[0];
        this.call.m=fastcode[1];
        this.call.s=fastcode[2];
        // pid-did-fastcode-order-type
        // 102-10152-222-2017120509563310152abcde-1
        const url =
          "http://103.45.102.47:8888/api/services/app/mobile/GetProduct";
        this.$http.post(url, {
          id: this.$route.params.id
        }).then(r => {
          if (r.data && r.data.result) {
            const element = r.data.result;
            this.model = element;
            this.list.push({
              src: element.imageUrl,
              fallbackSrc: "http://placeholder.qiniudn.com/60x60/3cc51f/ffffff",
              title: element.productName,
              desc: element.description,
              url: "/detail/" + element.id
            });
          }
        });
      },
      balancepay() {
        console.log(1);
      },
      getCard() {
        const url =
          "http://103.45.102.47:8888/api/Wechat/GetUserCards?openId=" +
          sessionStorage.getItem("openid");
        this.$http.get(url).then(r => {
          if (r.data && r.data.has_share_card) {
            this.cards = r.data.card_list;
          }
        });
      },
      linepay() {
        console.log(1);
      }
    }

  };

</script>
