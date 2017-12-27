<template>
  <div >
     <!-- <img src="../assets/timg.jpg" style="width:375px;height:620px;">  -->
  </div>
</template>
<script >
export default {
  created() {
    if (window.location.href.indexOf("code") >= 0) {
      this.login();
    } else {
      const userId = sessionStorage.getItem("userId");
      if (!userId) {
        const url = encodeURI("http://wx.youyinkeji.cn/#/author");
        // const test = `https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid=ww480b7545345a38f7&agentid=1000002&redirect_uri=${url}&state=web_login@gyoss9`;
        // window.location.href = test;
        // 跳转到微信授权页面
        window.location.href = `https://open.weixin.qq.com/connect/oauth2/authorize?appid=ww480b7545345a38f7&redirect_uri=${url}&response_type=code&scope=snsapi_userinfo&agentid=1000002&state=STATE#wechat_redirect`;
      } else {
        this.$router.push({ path: "/sign" });
      }
    }
  },
  data() {
    return {
      response: null,
      url: null,
      err: null,
      code: window.location.href.split("=")[1]
    };
  },
  methods: {
    login() {
      const url =
        "http://services.youyinkeji.cn/api/Sign/GetInfoByCode?code=" +
        window.location.href.split("=")[1];
      this.url = window.location.href;
      // 通过cookie中保存的token 获取用户信息
      this.$http
        .get(url)
        .then(response => {
          this.response = JSON.stringify(response);
          if (response && response.data) {
            if (response.data.userid) {
              sessionStorage.setItem("userId", response.data.userid);
              this.$router.push({ path: "/sign" });
            }
            //  window.location.href = "http://wx.youyinkeji.cn/#/sign";
          } else {
            if (sessionStorage.getItem("userId")) {
              const ww = encodeURI("http://wx.youyinkeji.cn/#/author");
              //  const test = `https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid=ww480b7545345a38f7&agentid=1000002&redirect_uri=${ww}&state=web_login@gyoss9`;
              // window.location.href = test;
              // 跳转到微信授权页面
              window.location.href = `https://open.weixin.qq.com/connect/oauth2/authorize?appid=ww480b7545345a38f7&redirect_uri=${ww}&response_type=code&scope=snsapi_userinfo&agentid=1000002&state=STATE#wechat_redirect`;
            }
          }
        })
        .catch(e => {
          this.err = JSON.stringify(e);
          this.showBox("错误", e.response.data.error);
        });
    }
  }
};
</script>
<style scoped>

</style>
