<template>
  <div>
    <blur :blur-amount="40" :url="url">
      <p class="center">
        <img :src="url">
      </p>
      <p class="center">ddd</p>
    </blur>
    <group>
      <cell title="余额(150)" link="/balance" value="充值" is-link></cell>
     
      <cell title="我的卡券" link="/mycard" value="" is-link></cell>
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
  data() {
    return {
      url: "https://o3e85j0cv.qnssl.com/tulips-1083572__340.jpg"
    };
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
