<template>
  <div>
    <div style="margin: 10px;overflow: hidden;" :key="item" v-for="item in list">
      <masker style="border-radius: 2px;">
        <div class="m-img" :style="{backgroundImage: 'url(' + item.img + ')'}"></div>
        <div slot="content" class="m-title">
          {{item.title}}
          <br/>
          <span class="m-time">有效期:{{item.time}}</span>
        </div>
      </masker>
    </div>
  </div>
</template>

<script>
import { Masker } from "vux";

export default {
  components: {
    Masker
  },
  data() {
    return {
      list: [
        {
          title: "抵扣券",
          time: "2017-03-10",
          img:
            "https://cdn.xiaotaojiang.com/uploads/82/1572ec37969ee263735262dc017975/_.jpg"
        },
        {
          title: "满减券",
          time: "2017-04-10",
          img:
            "https://cdn.xiaotaojiang.com/uploads/59/b22e0e62363a4a652f28630b3233b9/_.jpg"
        },
        {
          title: "打折券",
          time: "2017-05-10",
          img:
            "https://cdn.xiaotaojiang.com/uploads/56/4b3601364b86fdfd234ef11d8712ad/_.jpg"
        }
      ]
    };
  }
};
</script>

<style lang="less">
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

.m-time {
  font-size: 12px;
  padding-top: 4px;
  border-top: 1px solid #f0f0f0;
  display: inline-block;
  margin-top: 5px;
}
</style>