<template>
  <div class="myAuthor">
    <img src="../assets/home.png" style="width:100%; height:100%;">
  </div>
</template>
<script>
export default {
  created() {
    const code = sessionStorage.getItem("code");
    if (code != null) {
      this.login();
    } else {
      const userId = sessionStorage.getItem("openid");
      if (!userId) {
        const url = encodeURI("http://card.youyinkeji.cn");
        // 跳转到微信授权页面
        window.location.href = `https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx9065b59568dcf5a8&redirect_uri=${url}&response_type=code&scope=snsapi_userinfo&state=666#wechat_redirect`;
      } else {
        this.$router.push({
          path: "/my"
        });
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
      const code = sessionStorage.getItem("code");
      const url =
        "http://services.youyinkeji.cn/api/Wechat/GetOpenIdByCode?code=" + code;
      // 通过cookie中保存的token 获取用户信息
      this.$http
        .get(url)
        .then(response => {
          //  this.showBox("", JSON.stringify(response));
          if (response) {
            if (response.data) {
              sessionStorage.setItem("openid", response.data);
              sessionStorage.setItem("code", "");
              const beforeUrl = sessionStorage.getItem("beforeUrl");
              if (beforeUrl) {
                window.location.href =
                  "http://card.youyinkeji.cn/#" + beforeUrl;
              } else {
                window.location.href = "http://card.youyinkeji.cn/#/my";
              }
            }
          }
        })
        .catch(e => {
          console.log(e);
          sessionStorage.clear();
          window.location.href = "http://card.youyinkeji.cn/#/author";
        });
    }
  }
};
</script>
<style lang="less" scoped>
.myAuthor {
  height: 100%;
  overflow: hidden;
}
</style>
