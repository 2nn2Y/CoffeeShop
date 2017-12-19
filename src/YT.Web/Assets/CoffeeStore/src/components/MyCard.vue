<template>
  <div class="myCard">
    <div style="margin: 10px;overflow: hidden;" :key="item" v-for="item in list">
      <masker  style="border-radius: 8px;">
        <div class="m-img" v-bind:style="{ 'background-image': 'url(' + item.image + ')','background-repeat':'no-repeat','background-size':'cover'}">
        </div>
        <div slot="content" class="m-title">
          <span class="m-time">{{item.id}}</span>
          <br/>
          {{item.name}}
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
  created() {
    this.init();
  },
  data() {
    return {
      list: []
    };
  },
  methods: {
    init() {
      const url =
        "http://services.youyinkeji.cn/api/services/app/mobile/GetUsercards";
      this.$http
        .post(url, {
          id: sessionStorage.getItem("openid")
        })
        .then(r => {
          if (r.data && r.data.result) {
            this.list = r.data.result;
          }
        });
    }
  }
};
</script>

<style lang="less" scoped>
.myCard{
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
    color: #f45c32
  }

  .m-time {
    font-size: 12px;
    padding: 3px 0;
    border-bottom: 1px solid #f0f0f0;
    display: inline-block;
    margin-top: 5px;
    color: #fff
  }
}
</style>
