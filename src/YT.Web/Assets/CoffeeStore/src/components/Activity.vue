<template>
  <div class="myActivity">
    <div style="margin: 10px;overflow: hidden;border-radius:8px" :key="item.id" v-for="item in list">
      <masker @click.native="choose(item)"  style="border-radius: 2px;">
        <div class="m-img" :style="{ backgroundImage: 'url(' + item.imageUrl + ')'}" ></div>
        <div slot="content" class="m-title">
            <div class="titleImg">
              <span class="m-time">{{item.description}}</span>
              <br/>
              {{item.productName}}
            </div>
            <div>
              <span v-if="item.id==selected" class="m-selected"></span>
            </div>
        </div>
      </masker>
    </div>
     <x-button class="m-buy" type="primary" @click.native="buyCard">购买</x-button>
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
      count: 1,
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
      const url =
        "http://services.youyinkeji.cn/api/services/app/mobile/CardPay";
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
                _self.showBox("支付失败", JSON.stringify(res));
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
      const url =
        "http://services.youyinkeji.cn/api/services/app/mobile/GetCards";
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
.myActivity{
  background:#fff;
  padding:5px 0;
  margin-top: 10px
}
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
  color: #f45c32;
  text-align: center;
  text-shadow: 0 0 2px rgba(0, 0, 0, 0.5);
  font-weight: 500;
  font-size: 16px;
  position: absolute;
  left: 0;
  right: 0;
  width: 100%;
  text-align: center;
  top: 17%;
  /* transform: translateY(-50%); */
  padding-bottom: 5px;
  height: 74%;
  .titleImg{
      background: url('../assets/juanbeijing_a_xxh.png');
      width: 93%;
      position: absolute;
      left: 3.5%;
      padding-bottom: 6px
  }
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
  position: absolute;
  bottom: 0;
  left: 0;
  /* border-radius: 10px */
}
.m-time {
  font-size: 12px;
  padding: 8px 0;
  border-bottom: 1px dashed #f0f0f0;
  display: inline-block;
  margin-top: 5px;
  color: #efefef;
  margin-bottom: 5px
}
.m-buy{
  position: absolute;
  bottom: 0;
  height:60px;
  background: #fff;
  border-radius: 0;
  color: #4c4c4c;
  border-top: 1px solid #ddd;
}
.weui-btn:after{
  border:none
}
</style>
