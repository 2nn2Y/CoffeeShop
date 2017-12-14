<template>
  <div class="myMain">
    <blur :blur-amount="40" :url="url">
      <p class="center">
        <img :src="url">
      </p>
      <p class="center">{{nickname}}</p>
    </blur>
    <group>
      <cell :title="money" link="/balance" value="充值" is-link></cell>
      <cell :title="card" link="/mycard" value="" is-link></cell>
      <cell title="我的订单" link="/order" value="" is-link></cell>
    </group>
  </div>
</template>

<script>
import { Flexbox, FlexboxItem, Blur, CellBox, Group, Cell } from "vux";

export default {
  components: {
    Blur,
    Flexbox,
    FlexboxItem,
    CellBox,
    Group,
    Cell
  },
  created() {
    this.init();
  },
  data() {
    return {
      url: sessionStorage.getItem("headimgurl"),
      nickname: sessionStorage.getItem("nickname"),
      balance: 0,
      cards: 0
    };
  },
  computed: {
    money() {
      return `咖啡券(${this.balance * 1.0 / 100})`;
    },
    card() {
      return `我的卡券(${this.cards})`;
    }
  },
  methods: {
    init() {
      const url =
        "http://services.youyinkeji.cn/api/Wechat/GetUserBalance?openId=" +
        sessionStorage.getItem("openid");
      this.$http.get(url).then(r => {
        if (r.data) {
          this.balance = r.data.balance;
          this.cards = r.data.cards;
        }
      });
    }
  }
};
</script>

<<style lang="less">
.center {
  text-align: center;
  padding-top: 20px;
  color: #fff;
  font-size: 18px;
}

.center img {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  border: 4px solid #ececec;
}

.myMain{
  .weui-cells{
    margin-top: 0
  }
  .vux-no-group-title{
    margin-top: 0.5em
  }
  .weui-cell__ft{
    font-size: 12px;
    color: red
  }
}
  
</style>
