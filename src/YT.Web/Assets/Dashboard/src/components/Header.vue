<template>
  <div id="header">
    <div class="logo">
      咖啡机后台
    </div>
    <div class="user" >
      <img class="avatar" src="../assets/images/avatar.jpg" alt="">
       <Dropdown @on-click="action">
        <a href="javascript:void(0)">
            {{account.name}}
        </a>
        <DropdownMenu slot="list">
            <DropdownItem name="modify">修改资料</DropdownItem>
            <DropdownItem name="logout">退出</DropdownItem>
        </DropdownMenu>
    </Dropdown>
    </div>
  </div>
</template>
<script>
import { getInfo } from "api/login";
export default {
  data() {
    return {
      account: {}
    };
  },
  created() {
    this.init();
  },
  methods: {
    init() {
      getInfo().then(r => {
        if (r.result) {
          this.account = r.result.user;
        }
      });
    },
    action(item) {
      if (item === "modify") {
      } else {
        this.logout();
      }
    },
    logout() {
      sessionStorage.clear();
      this.$router.push("/index");
    }
  }
};
</script>

<style scoped>
#header {
  position: relative;
  height: 80px;
  line-height: 80px;
  display: flex;
  padding: 0 20px;
  justify-content: space-between;
  z-index: 12;
}

.logo {
  font-size: 26px;
  font-weight: 600;
}

.logo span {
  font-size: 16px;
  margin-left: 15px;
}

.user {
  font-size: 16px;
}

.user .avatar {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  vertical-align: middle;
  margin-right: 10px;
}

.user .icon {
  width: 17px;
  height: 17px;
  margin-left: 5px;
  vertical-align: middle;
}

.user-x {
  position: absolute;
  right: 20px;
  top: 70px;
  width: 100px;
  height: 100px;
  z-index: 1000;
}

.user-x ul li {
  list-style: none;
  width: 100px;
  height: 50px;
  line-height: 50px;
  text-align: center;
  cursor: pointer;
}

.user-x ul li a {
  display: block;
}

.user-x ul li:hover {
  background: rgba(7, 17, 27, 0.5);
}
</style>
