<template>
  <div id="menu">
    <canvas id="canvas" class="canvas"></canvas>
    <transition name="fade-out">
      <div class="logo-bg"></div>
    </transition>
    <transition name="fade-in">
      <div class="login-box">
        <Form ref="form" :model="account" >
          <div class="ipunt-wrap">
            <label for="username" class="icon-user"></label>
            <input type="text"  placeholder="用户名" v-model="account.userName">
          </div>
          <div class="ipunt-wrap">
            <label for="password" class="icon-password"></label>
            <input type="password" id="password" placeholder="密码" v-model="account.password">
          </div>
          <div class="button">
            <a class="gv" href="javascript:;" @click="login">登录</a>
          </div>
          <div class="toregist">
            还没有账号？
            <router-link to="/regist">
              <a href="javascript:;">去注册</a>
            </router-link>
          </div>
        </Form>
      </div>
    </transition>
    <div class="city"></div>
    <div class="moon"></div>
  </div>
</template>

<script>
import Stars from "../../static/js/Star";
import Moon from "../../static/js/Moon";
import Meteor from "../../static/js/Meteor";
import { Authenticate } from "api/login";
export default {
  data() {
    return {
      account: {
        userName: "admin",
        password: "123456"
      }
    };
  },
  created() {
    if (sessionStorage.token) {
      this.$router.push("/users");
    }
  },
  mounted() {
    let canvas = document.getElementById("canvas"),
      ctx = canvas.getContext("2d"),
      width = window.innerWidth,
      height = window.innerHeight,
      //实例化月亮和星星。流星是随机时间生成，所以只初始化数组
      moon = new Moon(ctx, width, height),
      stars = new Stars(ctx, width, height, 200),
      meteors = [],
      count = 0;

    canvas.width = width;
    canvas.height = height;

    const meteorGenerator = () => {
      //x位置偏移，以免经过月亮
      let x = Math.random() * width + 800;
      meteors.push(new Meteor(ctx, x, height));

      //每隔随机时间，生成新流星
      setTimeout(() => {
        meteorGenerator();
      }, Math.random() * 2000);
    };

    const frame = () => {
      count++;
      count % 10 == 0 && stars.blink();
      moon.draw();
      stars.draw();

      meteors.forEach((meteor, index, arr) => {
        //如果流星离开视野之内，销毁流星实例，回收内存
        if (meteor.flow()) {
          meteor.draw();
        } else {
          arr.splice(index, 1);
        }
      });
      requestAnimationFrame(frame);
    };
    meteorGenerator();
    frame();
  },
  methods: {
    login() {
      Authenticate(this.account.userName, this.account.password)
        .then(r => {
          if (r.result) {
            sessionStorage.setItem("token", r.result);
            this.$router.push({ path: "/users" });
          }
        })
        .catch(e => {
          this.$Message.error(e.message);
        });
    }
  }
};
</script>

<style scoped>
#menu {
  height: 100%;
  overflow: hidden;
  position: relative;
}

.canvas {
  position: fixed;
  z-index: -1;
}

.logo-bg {
  width: 800px;
  height: 200px;
  position: absolute;
  z-index: 10000;
  top: 80px;
  left: 50%;
  transform: translateX(-50%);
  background: url("../assets/images/top_logo.png") no-repeat;
}

.nav {
  width: 200px;
  height: auto;
  position: absolute;
  z-index: 10000;
  top: 350px;
  left: 50%;
  margin-left: -100px;
}

.gv {
  text-decoration: none;
  background: url("../assets/images/nav_gv.png") repeat 0px 0px;
  width: 130px;
  height: 43px;
  display: block;
  text-align: center;
  line-height: 43px;
  cursor: pointer;
  float: left;
  margin: 10px 2px 10px 2px;
  font: 18px/43px "microsoft yahei";
  color: #066197;
}

a.gv:hover {
  background: url("../assets/images/nav_gv.png") repeat 0px -43px;
  color: #1d7eb8;
  -webkit-box-shadow: 0 0 6px #1d7eb8;
  transition-duration: 0.5s;
}

.city {
  width: 100%;
  height: 170px;
  position: fixed;
  bottom: 0px;
  z-index: 100;
  background: url("../assets/images/city.png") no-repeat;
  background-size: cover;
}

.moon {
  width: 100px;
  height: 100px;
  position: absolute;
  left: 100px;
  top: 100px;
  background: url("../assets/images/moon.png") no-repeat;
  background-size: cover;
}

.fade-out-enter-active,
.fade-out-leave-active {
  transition: all 0.5s;
}

.fade-out-enter,
.fade-out-leave-active {
  opacity: 0;
  transform: translateX(-400px);
}

.fade-in-enter-active,
.fade-in-leave-active {
  transition: all 0.5s;
}

.fade-in-enter,
.fade-in-leave-active {
  opacity: 0;
  transform: translateX(400px);
}

.login-box {
  width: 600px;
  padding: 50px;
  margin: 200px auto;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  z-index: 100001;
}

.ipunt-wrap label {
  display: inline-block;
  width: 25px;
  height: 25px;
  vertical-align: middle;
  margin-right: 10px;
  background-size: cover;
}

.icon-id {
  background: url("../assets/images/id.png") no-repeat;
}

.icon-user {
  background: url("../assets/images/user.png") no-repeat;
}

.icon-password {
  background: url("../assets/images/password.png") no-repeat;
}

.ipunt-wrap input {
  border: none;
  outline: none;
  background: none;
  border-bottom: 1px solid #fff;
  margin-top: 30px;
  width: 200px;
  height: 30px;
  line-height: 30px;
  /* text-align: center; */
  color: #fff;
  font-size: 14px;
  padding: 0 5px;
}

.button {
  margin-top: 30px;
  margin-left: 60px;
}

.toregist {
  font-size: 12px;
  float: right;
  padding-top: 20px;
  color: #fff;
}

.toregist a {
  color: #066197;
  text-decoration: none;
}
</style>