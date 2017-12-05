<template>
  <div>
    <x-header :right-options="{showMore: true}" @on-click-more="showMenus = true">{{title}}</x-header>
    <div v-transfer-dom>
      <actionsheet :on-click-menu="share" :menus="menus" v-model="showMenus" show-cancel></actionsheet>
    </div>
  </div>
</template>

<script>
  import {
    XHeader,
    Actionsheet,
    TransferDom
  } from "vux";
  import {
    mapState
  } from "vuex";
  export default {
    directives: {
      TransferDom
    },
    components: {
      XHeader,
      Actionsheet
    },
    computed: {
      ...mapState({
        title: state => state.title
      })
    },
    data() {
      return {
        menus: {
          menu1: "分享到朋友圈",
          menu2: "分享给微信好友"
        },
        showMenus: false
      };
    },
    methods: {
      share(key, item) {
        if (key)
          const link = "http://card.youyinkeji.cn";
        this.$wechat.onMenuShareTimeline({
          title: '分享test', // 分享标题
          link: link, // 分享链接，该链接域名或路径必须与当前页面对应的公众号JS安全域名一致
          imgUrl: '', // 分享图标
          success: function() {
            // 用户确认分享后执行的回调函数
            this.showMenus = false;
          },
          cancel: function() {
            // 用户取消分享后执行的回调函数
          }
        });
      }
    }
  };
</script>

<style>
  .overwrite-title-demo {
    margin-top: 5px;
  }
</style>