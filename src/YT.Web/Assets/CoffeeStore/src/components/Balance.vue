<template>
  <div>
    <divider>充值金额</divider>
    <box gap="10px 10px">
      <x-button @click.native="charge(5000)" :gradients="['#1D62F0', '#19D5FD']">充50得50</x-button>
      <x-button @click.native="charge(10000)" :gradients="['#A644FF', '#FC5BC4']">充100得110</x-button>
      <x-button @click.native="charge(15000)" :gradients="['#FF2719', '#FF61AD']">充150得165</x-button>
      <x-button @click.native="charge(20000)" :gradients="['#6F1BFE', '#9479DF']">充200得230</x-button>
      <x-button @click.native="charge(30000)" :gradients="['#FF5E3A', '#FF9500']">充300得340</x-button>
    </box>
  </div>
</template>
<script>
import { XButton, Box, GroupTitle, Group, Divider } from "vux";

export default {
  components: {
    XButton,
    Box,
    GroupTitle,
    Group,
    Divider
  },
  created() {
    this.wxConfig(window.location.href);
  },
  data() {
    return {};
  },
  computed: {
    openId() {
      return sessionStorage.getItem("openid");
    }
  },
  methods: {
    charge(money) {
      var _self = this;
      const service =
        "http://103.45.102.47:8888/api/services/app/mobile/ChargePay";
      const params = {
        openId: _self.openId,
        price: money
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

<style lang="less">
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
