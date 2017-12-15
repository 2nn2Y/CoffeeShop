<template>
  <div class="myBalance">
    <!-- <divider>充值金额</divider> -->
    <flexbox orient="vertical">
      <flexbox-item><div class="flex-demo"><img src="../assets/congzhi_a_xh.png"> 充值金额</div></flexbox-item>
    </flexbox>
    <box gap="10px 10px">
      <x-button mini @click.native="charge(1)">50元<br/><span>充50 得50</span></x-button>
      <x-button mini @click.native="charge(1)">100元</br><span>充100 得100</span></x-button>
      <x-button mini @click.native="charge(1)">150元</br><span>充150 得165</span></x-button>
      <x-button mini @click.native="charge(1)">200元</br><span>充200 得230</span></x-button>
      <x-button mini @click.native="charge(1)">300元</br><span>充300 得340</span></x-button>
    </box>
    <div style="width:100%; text-align:center">
      <x-button mini class="myimme">立即充值</x-button>
    </div>
  </div>
</template>
<script>
import { XButton, Box, GroupTitle, Group, Divider, Flexbox, FlexboxItem } from "vux";

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
        "http://services.youyinkeji.cn/api/services/app/mobile/ChargePay";
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

<style lang="less" scoped>
.myBalance{
  background:#fff;
  height:100%;
  flexbox-item{
    width:100%;
  }
  .flex-demo {
    text-align: center;
    color: #000;
    border-bottom: 1px solid #ddd;
    padding: 8px 0;
    img{
      vertical-align: middle;
      width:25px;
      height:25px;
    }
  }
  button{
    background: none;
    border: 1px solid #ddd;
    padding: 0;
    width: 32.2%;
    font-size: 14px;
    &:hover{
      background:#ffede6;
      border: 1px solid #ffede6;
      color:#f45c32
    }
  }
  .weui-btn:after{
    border: none;
  }
  span{
    font-size: 10px
  }
  .myimme{
    color: #f45c32;
    width: auto;
    padding: 0 3em;
    border-radius: 25px;
    margin-top: 15px;
    border:1px solid #fc5c32
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
