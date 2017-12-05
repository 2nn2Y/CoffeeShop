<template>
  <div>
    <panel :header="'商品详情'" :list="list" :type="'5'"></panel>
    <divider>咖啡配置</divider>
    <group>
      <x-number title="浓度" aria-readonly="true" :min="0" :max="5" v-model="call.o"></x-number>
      <x-number title="奶度" aria-readonly="true" :min="0" :max="5" v-model="call.m"></x-number>
      <x-number title="糖度" aria-readonly="true" :min="0" :max="5" v-model="call.s"></x-number>
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
    this.init();
  },
  computed: {
    fastcode() {
      return this.call.o + "" + this.call.m + "" + this.call.s;
    },
    openId() {
      return sessionStorage.getItem("openid");
    }
  },
  data() {
    return {
      call: {
        o: 0,
        m: 0,
        s: 0
      },
      order: "",
      type: null,
      device: "",
      selectCard: null,
      model: {},
      cards: [],
      list: []
    };
  },
  methods: {
    init() {
      const params = this.$route.params.id.split("-");
      const fastcode = params[2].split("");
      this.call.o = fastcode[0];
      this.call.m = fastcode[1];
      this.call.s = fastcode[2];
      this.order = params[3];
      this.device = params[1];
      this.type = params[4];
      // pid-did-fastcode-order-type
      // 102-10152-222-2017120509563310152abcde-1
      const url =
        "http://103.45.102.47:8888/api/services/app/mobile/GetProduct";

      this.$http
        .post(url, {
          userId: this.openId,
          productId: params[0]
        })
        .then(r => {
          if (r.data && r.data.result) {
            const element = r.data.result;
            this.model = element.product;
            this.cards = element.cards;
            this.list.push({
              src: this.model.imageUrl,
              fallbackSrc: "http://placeholder.qiniudn.com/60x60/3cc51f/ffffff",
              title: this.model.productName,
              desc: this.model.description,
              url: "/detail/" + this.model.id
            });
          }
        });
    },
    balancepay() {
      const url =
        "http://103.45.102.47:8888/api/services/app/mobile/BalancePay";
      const params = {
        openId: this.openId,
        price: this.model.price,
        productId: this.model.id,
        order: this.order,
        fastCode: this.fastcode,
        orderType: this.type,
        key: this.selectCard,
        device: this.device
      };
      this.$http
        .post(url, params)
        .then(r => {
          if (r.data && r.data.success) {
            this.$router.push({ path: "/my" });
          }
        })
        .catch(error => {
          console.log(error);
          this.showBox("错误", error.error.message);
        });
    },
    linepay() {
      const url = "http://103.45.102.47:8888/api/services/app/mobile/LinePay";
      const params = {
        openId: this.openId,
        price: this.model.price,
        productId: this.model.id,
        order: this.order,
        fastCode: this.fastcode,
        orderType: this.type,
        key: this.selectCard,
        device: this.device
      };
      this.$http
        .post(url, params)
        .then(r => {
          if (r.data && r.data.success) {
            const params = JSON.parse(r.data.result);
            this.$wechat.chooseWXPay({
              timestamp: params.timeStamp, // 支付签名时间戳，注意微信jssdk中的所有使用timestamp字段均为小写。但最新版的支付后台生成签名使用的timeStamp字段名需大写其中的S字符
              nonceStr: params.nonceStr, // 支付签名随机串，不长于 32 位
              package: params.package, // 统一支付接口返回的prepay_id参数值，提交格式如：prepay_id=***）
              signType: params.signType, // 签名方式，默认为'SHA1'，使用新版支付需传入'MD5'
              paySign: params.paySign, // 支付签名
              success: function(res) {
                console.log(res);
                this.$router.push({ path: "/my" });
              }
            });
          }
        })
        .catch(error => {
          console.log(error);
          this.showBox("错误", error.error.message);
        });
    }
  }
};
</script>
