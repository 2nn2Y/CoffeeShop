<template>
  <div>
    <panel :header="'商品详情'" :list="list" :type="'5'"></panel>
    <divider>咖啡配置</divider>
    <group>
      <cell title="浓度" :value="call.o"></cell>
      <cell title="奶度" :value="call.m"></cell>
      <cell title="糖度" :value="call.s"></cell>
      <!-- <x-number title="浓度" aria-readonly="true" :min="0" :max="5" v-model="call.o"></x-number>
      <x-number title="奶度" aria-readonly="true" :min="0" :max="5" v-model="call.m"></x-number>
      <x-number title="糖度" aria-readonly="true" :min="0" :max="5" v-model="call.s"></x-number> -->
      <selector ref="card" title="选择代金券" @on-change="change"  :value-map="['id', 'value']"  :options="cards" v-model="selectCard"></selector>
      <cell title="总金额" v-model="totalprice"></cell>
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
    this.wxConfig(window.location.href);
    this.init();
  },
  computed: {
    fastcode() {
      return this.call.o + "" + this.call.m + "" + this.call.s;
    },
    openId() {
      return sessionStorage.getItem("openid");
    },
    totalprice() {
      if (this.card) {
        var temp = this.model.price - this.card[0].cost;
        return temp * 1.0 / 100;
      } else {
        return this.model.price * 1.0 / 100;
      }
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
      card: null,
      model: {},
      cards: [],
      list: []
    };
  },
  methods: {
    change(value) {
      var _self = this,
        t;
      clearTimeout(t);
      var set = function() {
        var m = _self.$refs.card.getFullValue();
        console.log(m);
        _self.card = m;
      };
      t = setTimeout(set, 1000);
      this.selectCard = value;
    },
    init() {
      const params = this.$route.params.id.split("-");
      const fastcode = params[2].split("");
      this.call.o = fastcode[0];
      this.call.m = fastcode[1];
      this.call.s = fastcode[2];
      this.order = params[4];
      this.device = params[1];
      this.type = params[5];
      // 102-10152-222-990-2017120509563310152abcde-1
      // pid-did-fastcode-order-type
      // 102-10152-222-2017120509563310152abcde-1
      const url =
        "http://services.youyinkeji.cn/api/services/app/mobile/GetProduct";
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
      var _self = this;
      var price =
        _self.card != null
          ? _self.model.price - _self.card[0].cost
          : _self.model.price;
      const url =
        "http://services.youyinkeji.cn/api/services/app/mobile/BalancePay";
      const params = {
        openId: _self.openId,
        price: price,
        productId: _self.model.id,
        order: _self.order,
        fastCode: _self.fastcode,
        orderType: _self.type,
        key: _self.selectCard,
        device: _self.device
      };
      _self.$http
        .post(url, params)
        .then(r => {
          if (r.data && r.data.success) {
            _self.showBox("支付成功", "稍后为您出货");
            _self.$router.push({ path: "/my" });
          }
        })
        .catch(error => {
          console.log(error);
          // 支付失败回调函数
          _self.showBox("支付失败", error.response.data.error.message);
        });
    },
    linepay() {
      var _self = this;
      const url =
        "http://services.youyinkeji.cn/api/services/app/mobile/LinePay";
      const params = {
        openId: _self.openId,
        price: _self.totalprice,
        productId: _self.model.id,
        order: _self.order,
        fastCode: _self.fastcode,
        orderType: _self.type,
        key: _self.selectCard,
        device: _self.device
      };
      _self.$http
        .post(url, params)
        .then(r => {
          if (r.data && r.data.success) {
            const params = JSON.parse(r.data.result);
            _self.$wechat.chooseWXPay({
              timestamp: params.timeStamp, // 支付签名时间戳，注意微信jssdk中的所有使用timestamp字段均为小写。但最新版的支付后台生成签名使用的timeStamp字段名需大写其中的S字符
              nonceStr: params.nonceStr, // 支付签名随机串，不长于 32 位
              package: params.package, // 统一支付接口返回的prepay_id参数值，提交格式如：prepay_id=***）
              signType: params.signType, // 签名方式，默认为'SHA1'，使用新版支付需传入'MD5'
              paySign: params.paySign, // 支付签名
              success: function(res) {
                console.log(res);
                _self.showBox("支付成功", "稍后为您出货");
                _self.$router.push({ path: "/my" });
              },
              fail: function(res) {
                console.log(res);
                // 支付失败回调函数
                _self.showBox("支付失败", "请重试");
              }
            });
          }
        })
        .catch(error => {
          _self.showBox("支付失败", error.response.data.error.message);
        });
    }
  }
};
</script>
