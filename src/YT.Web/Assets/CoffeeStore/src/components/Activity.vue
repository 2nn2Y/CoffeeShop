<template>
  <div>
    <div style="margin: 10px;overflow: hidden;" :key="item.id" v-for="item in list">
      <masker @click.native="choose(item)"  style="border-radius: 2px;">
        <div class="m-img" :style="{ backgroundImage: 'url(' + item.imageUrl + ')'}" ></div>
        <div slot="content" class="m-title">
          {{item.productName}}
          <br/>
          <span class="m-time">{{item.description}}</span>
          <div>
          <span v-if="item.id==selected" class="m-selected"></span>
          </div>
        </div>
      </masker>
    </div>
     <x-button type="primary" @click.native="buyCard">购买</x-button>
  </div>
</template>

<script>
import { Panel, Group, Radio, Card, XButton, Masker } from "vux";
export default {
  components: {
    Panel,
    Group,
    Radio,
    Card,
    XButton,
    Masker
  },
  computed: {
    openId() {
      return sessionStorage.getItem("openid");
    }
  },
  data() {
    return {
      list: [],
      current: null,
      selected: null
    };
  },
  created() {
    this.wxConfig(window.location.href);
    this.init();
  },
  methods: {
    buyCard() {
      var _self = this;
      if (_self.current == null) {
        _self.showBox("请选择", "请先选择卡券购买");
        return;
      }
      const url = "http://103.45.102.47:8888/api/services/app/mobile/CardPay";
      const params = {
        openId: _self.openId,
        price: _self.current.price,
        productId: _self.current.id,
        order: "",
        fastCode: "",
        orderType: 4,
        device: ""
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
                _self.showBox("支付成功", "请到我的页面查看");
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
    },
    choose(item) {
      this.selected = item.id;
      this.current = item;
    },
    init() {
      const url = "http://103.45.102.47:8888/api/services/app/mobile/GetCards";
      this.$http.post(url, {}).then(r => {
        if (r.data && r.data.result) {
          this.list = r.data.result;
        }
      });
    }
  }
};
</script>
<style lang="less" scoped>
.m-img {
  padding-bottom: 33%;
  display: block;
  position: relative;
  max-width: 100%;
  background-size: cover;
  background-position: center center;
  cursor: pointer;
  border-radius: 2px;
}

.m-title {
  color: #fff;
  text-align: center;
  text-shadow: 0 0 2px rgba(0, 0, 0, 0.5);
  font-weight: 500;
  font-size: 16px;
  position: absolute;
  left: 0;
  right: 0;
  width: 100%;
  text-align: center;
  top: 50%;
  transform: translateY(-50%);
}
.m-selected {
  font-size: 12px;
  width: 100%;
  height: 5px;
  background-color: #1aad19;
  padding-top: 4px;
  border-top: 1px solid #f0f0f0;
  display: inline-block;
  margin-top: 5px;
}
.m-time {
  font-size: 12px;
  padding-top: 4px;
  border-top: 1px solid #f0f0f0;
  display: inline-block;
  margin-top: 5px;
}
</style>
