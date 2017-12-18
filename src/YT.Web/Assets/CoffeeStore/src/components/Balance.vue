<template>
  <div class="myBalance">
    <!-- <divider>充值金额</divider> -->
    <flexbox orient="vertical">
      <flexbox-item><div class="flex-demo"><img src="../assets/congzhi_a_xh.png"> 充值金额</div></flexbox-item>
    </flexbox>
    <box gap="15px 10px">
      <!-- <x-button mini @click.native="charge(1)">50元<br/><span>充50 得50</span></x-button>
      <x-button mini @click.native="charge(1)">100元</br><span>充100 得100</span></x-button>
      <x-button mini @click.native="charge(1)">150元</br><span>充150 得165</span></x-button>
      <x-button mini @click.native="charge(1)">200元</br><span>充200 得230</span></x-button>
      <x-button mini @click.native="charge(1)">300元</br><span>充300 得340</span></x-button> -->
      <span @click.native="changeMoney(1)">50元<br/><font>充50 得50</font></span>
      <span @click.native="changeMoney(1)">100元<br/><font>充100 得100</font></span>
      <span @click.native="changeMoney(1)">150元<br/><font>充150 得165</font></span>
      <span @click.native="changeMoney(1)">200元<br/><font>充200 得230</font></span>
      <span @click.native="changeMoney(1)">300元<br/><font>充300 得340</font></span>
    </box>
    <div class="immeCharge">
      <x-button mini class="myimme">立即充值</x-button>
    </div>
  </div>
</template>
<script>
import {
  XButton,
  Box,
  GroupTitle,
  Group,
  Divider,
  Flexbox,
  FlexboxItem
} from "vux";

export default {
  components: {
    XButton,
    Box,
    GroupTitle,
    Group,
    Divider,
    Flexbox,
    FlexboxItem
  },
  created() {
    this.wxConfig(window.location.href);
  },
  data() {
    return {
      money: 0
    };
  },
  computed: {
    openId() {
      return sessionStorage.getItem("openid");
    }
  },
  methods: {
    changeMoney(mo) {
      this.money = mo;
    },
    charge() {
      var _self = this;
      const service =
        "http://services.youyinkeji.cn/api/services/app/mobile/ChargePay";
      const params = {
        openId: _self.openId,
        price: this.money
      };
      _self.$http.post(service, params).then(r => {
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
              _self.showBox("支付成功", "稍后请到我的页面查看");
              _self.$router.push({ path: "/my" });
            },
            fail: function(res) {
              console.log(res);
              // 支付失败回调函数
              _self.showBox("支付失败", "请重试");
            }
          });
        }
      });
    }
  }
};
</script>

<style lang="less" scoped>
.myBalance {
  height: 100%;
  background: url("../assets/charge.png");
  background-size: 100% 100%;
  flexbox-item {
    width: 100%;
  }
  .flex-demo {
    text-align: center;
    color: #fff;
    padding: 8px 0;
    img {
      vertical-align: middle;
      width: 25px;
      height: 25px;
      margin-right: 5px;
    }
  }
  .immeCharge {
    width: 100%;
    text-align: center;
    position: absolute;
    bottom: 0;
    background: rgba(10, 7, 0, 0.4);
    padding: 0.6em 0;
    button {
      background: #f45c32;
      color: #fff;
      margin: 0;
    }
  }
  span {
    display: inline-block;
    background: none;
    border: 6px solid #cfd0d0;
    padding: 0;
    width: 29%;
    font-size: 16px;
    border-radius: 10px;
    margin-top: 10px;
    text-align: center;
    padding: 2px 0;
    background: #e8e9e9;
    &:hover {
      background: #ffede6;
      border: 6px solid #f9c2b4;
      color: #f45c32;
    }
  }
  .weui-btn:after {
    border: none;
  }
  font {
    font-size: 12px;
  }
  .myimme {
    color: #f45c32;
    width: auto;
    padding: 0.1em 5em;
    border-radius: 25px;
    margin-top: 15px;
    border: 1px solid #fc5c32;
    font-size: 15px;
  }
}
.custom-primary-red {
  border-radius: 99px !important;
  border-color: #ce3c39 !important;
  color: #ce3c39 !important;
  &:active {
    border-color: rgba(206, 60, 57, 0.6) !important;
    color: rgba(206, 60, 57, 0.6) !important;
    background-color: transparent;
  }
}
</style>
