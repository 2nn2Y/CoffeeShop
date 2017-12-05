<template>
  <div>
    <blur :blur-amount="40" :url="url">
      <p class="center">
        <img :src="url">
      </p>
      <p class="center">{{nickname}}</p>
    </blur>
    <group>
      <cell :title="title" link="/balance" value="充值" is-link></cell>
      <cell title="我的卡券" link="/mycard" value="" is-link></cell>
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
    this.callTitle("我的");
    this.init();
  },
  data() {
    return {
      url: sessionStorage.getItem("headimgurl"),
      nickname: sessionStorage.getItem("nickname"),
      balance: 0
    };
  },
  computed: {
    title() {
      return `咖啡券(${this.balance})`;
    }
  },
  methods: {
    init() {
      const url =
        "http://103.45.102.47:8888/api/Wechat/GetUserBalance?openId=" +
        sessionStorage.getItem("openid");
      this.$http.get(url).then(r => {
        if (r.data) {
          this.balance = r.data.balance;
        }
      });
    }
  }
};
</script>

<style scoped>
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
</style>
