<template>
  <div class="myDetail">
    <flexbox orient="vertical">
      <flexbox-item><div class="flex-demo"><img src="../assets/xiangping_a_xh.png"> 商品详情</div></flexbox-item>
    </flexbox>
    <panel :list="list" :type="'5'"></panel>
    <group>
      <flexbox orient="vertical">
        <flexbox-item><div class="flex-demo"><img src="../assets/peizhi_a_xh.png"> 咖啡配置</div></flexbox-item>
      </flexbox>
      <cell v-if="type==1" title="浓度" :value="call.o"></cell>
      <cell v-if="type==1" title="奶度" :value="call.m"></cell>
      <cell v-if="type==1" title="糖度" :value="call.s"></cell>
      <!-- <x-number title="浓度" aria-readonly="true" :min="0" :max="5" v-model="call.o"></x-number>
      <x-number title="奶度" aria-readonly="true" :min="0" :max="5" v-model="call.m"></x-number>
      <x-number title="糖度" aria-readonly="true" :min="0" :max="5" v-model="call.s"></x-number> -->
      <selector ref="card" title="选择代金券" @on-change="change"
       :value-map="['id', 'value']"
         :options="cards" v-model="selectCard">
      </selector>
      <!-- <cell title="总金额" v-model="totalprice"></cell> -->
      <div class="myTotal">
        <span>总金额：{{ totalprice }}</span>
        <font v-if="this.cards"></font>
      </div>
    </group>
    <grid>
      <grid-item>
        <box gap="0px 13px">
          <x-button plain type="primary"  @click.native="balancepay" class="custom-primary-green">余额支付</x-button>
        </box>
      </grid-item>
      <grid-item style="border-left: 1px dashed #ddd">
        <box gap="0px 13px">
           <x-button plain type="default"  @click.native="linepay" class="custom-primary-blue">在线支付</x-button>
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
  Box,
  Flexbox,
  FlexboxItem
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
    Box,
    Flexbox,
    FlexboxItem
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
        return temp < 0 ? 0 : temp * 1.0 / 100;
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
      const params = this.$route.params.id.split("^");
      var fastcode =
        params[2].length === 1
          ? "00" + params[2]
          : params[2].length === 2 ? "0" + params[2] : params[2];
      fastcode = fastcode.split("");
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
      var price =
        _self.card != null
          ? _self.model.price - _self.card[0].cost
          : _self.model.price;
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
<style lang="less">
.myDetail {
  .weui-panel__hd {
    text-align: center;
    color: #000;
    font-size: 18px;
  }
  .weui-panel__hd:after {
    left: 0;
  }
  .vux-selector.weui-cell_select-after {
    padding: 8px 15px;
  }
  flexbox-item {
    width: 100%;
  }
  .flex-demo {
    text-align: center;
    color: #000;
    border-bottom: 1px solid #ddd;
    padding-bottom: 9px;
    background: #fff;
    padding-top: 10px;
    font-size: 17px;
    img {
      vertical-align: middle;
      width: 23px;
      height: 23px;
      margin-right: 5px;
    }
  }
  .weui-grids {
    position: absolute;
    bottom: 0;
    background: #fff;
    width: 100%;
    height: 45px;
    padding: 8px 0;
    button {
      font-size: 15px;
      padding: 4px 0;
    }
    button.weui-btn_plain-primary,
    input.weui-btn_plain-primary {
      background: #22ac38;
    }
    button.weui-btn_plain-default,
    input.weui-btn_plain-default {
      background: #1b77f2;
      border: 1px solid #1b77f2;
    }
  }
  .weui-grid {
    padding: 0 !important;
  }
  .weui-panel {
    margin-top: 0;
  }

  .weui-grid:before {
    border-right: 1px dashed #d9d9d9;
  }
  .weui-grid:after {
    border-bottom: none;
  }
  .myTotal {
    text-align: center;
    padding: 21px 0;
    border-top: 1px solid #ddd;
    width: 94%;
    margin-left: 3%;
    position: relative;
    span {
      color: red;
    }
    font {
      position: absolute;
      top: -2em;
      right: 1em;
      padding: 3px;
      background: #f00;
      border-radius: 50%;
    }
  }
}
.custom-primary-blue {
  border-radius: 120px !important;
  // border-color: #CE3C39!important;
  color: #ffffff !important;
  background-color: #007eff;
  &:active {
    border-color: rgba(206, 60, 57, 0.6) !important;
    color: rgba(206, 60, 57, 0.6) !important;
    background-color: transparent;
  }
}
.custom-primary-green {
  border-radius: 120px !important;
  color: #ffffff !important;
  background-color: #21c321;
  &:active {
    border-color: rgba(206, 60, 57, 0.6) !important;
    color: rgba(206, 60, 57, 0.6) !important;
    background-color: transparent;
  }
}
</style>
