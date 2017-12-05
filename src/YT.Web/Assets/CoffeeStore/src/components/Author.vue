<template>
  <div >
    <img src="../assets/timg.jpg" style="width:375px;height:620px;">
  </div>
</template>
<script >
export default {
  created() {
    if (window.location.href.indexOf("code") >= 0) {
      this.login();
    } else {
      const userId = sessionStorage.getItem("openid");
      if (!userId) {
        const url = encodeURI("http://card.youyinkeji.cn");
        // 跳转到微信授权页面
        window.location.href = `https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx734728844b17a945&redirect_uri=${url}&response_type=code&scope=snsapi_userinfo&state=333#wechat_redirect`;
      } else {
        this.$router.push({ path: "/my" });
      }
    }
  },
  data() {
    return {
      width: document.documentElement.clientWidth,
      height: document.documentElement.clientHeight,
      code: window.location.href.split("=")[1]
    };
  },
  methods: {
    login() {
      const url =
        "http://103.45.102.47:8888/api/wechat/GetInfoByCode?code=" +
        window.location.href.split("=")[1];
      // 通过cookie中保存的token 获取用户信息
      this.$http
        .get(url)
        .then(response => {
          if (response && response.data) {
            if (response.data.openid) {
              sessionStorage.setItem("openid", response.data.openid);
              sessionStorage.setItem("nickname", response.data.nickname);
              sessionStorage.setItem("headimgurl", response.data.headimgurl);
              const beforeUrl = sessionStorage.getItem("beforeUrl");
              if (beforeUrl) {
                window.location.href =
                  "http://card.youyinkeji.cn/#" + beforeUrl;
              } else {
                window.location.href = "http://card.youyinkeji.cn/#/my";
              }
            }
          } else {
            if (sessionStorage.getItem("userId")) {
              const ww = encodeURI("http://card.youyinkeji.cn");
              // 跳转到微信授权页面
              window.location.href = `https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx734728844b17a945&redirect_uri=${ww}&response_type=code&scope=snsapi_userinfo&state=333#wechat_redirect`;
            }
          }
        })
        .catch(e => {
          this.showBox("错误", e.response.data.error);
        });
    }
  }
};
</script>
<style scoped>

</style>
