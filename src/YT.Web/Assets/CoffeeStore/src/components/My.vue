<template>
  <div class="myMain">
    <blur :blur-amount="40" :url="info.imageUrl">
      <p class="center">
        <img :src="info.imageUrl">
      </p>
      <p class="center">{{info.nickName}}</p>
    </blur>
    <group>
      <cell :title="money" link="/balance" value="充值" is-link>
        <img slot="icon" width="20" style="display:block;margin-right:5px;" src="../assets/kafeijuan_a_xh.png">
      </cell>
      <cell :title="card" link="/mycard" value="" is-link>
        <img slot="icon" width="20" style="display:block;margin-right:5px;" src="../assets/kajuan_a_xh.png">
      </cell>
      <cell title="我的订单" link="/order" value="" is-link>
        <img slot="icon" width="20" style="display:block;margin-right:5px;" src="../assets/dingdan_a_xh.png">
      </cell>
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
      cards: 0,
      info: { balance: 0 }
    };
  },
  computed: {
    money() {
      return `咖啡券(${this.info.balance * 1.0 / 100})`;
    },
    card() {
      return `我的卡券(${this.cards})`;
    }
  },
  methods: {
    init() {
      const url =
        "http://services.youyinkeji.cn/api/Wechat/GetInfoByOpenId?openId=" +
        sessionStorage.getItem("openid");
      this.$http.get(url).then(r => {
        console.log(r);
        if (r && r.data) {
          this.info = r.data.info;
          this.cards = r.data.cards;
        }
      });
    }
  }
};
</script>

<style lang="less">
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

.myMain {
  .weui-cell {
    padding: 16px 15px
  }
  .weui-cells {
    margin-top: 0
  }
  .vux-no-group-title {
    margin-top: 0.5em
  }
  .weui-cell__ft {
    font-size: 12px;
    color: red
  }
}

.myMain>div:nth-child(1) {
  height: 250px !important;
  padding-top: 50px
}
</style>
