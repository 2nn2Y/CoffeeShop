<template>
  <section class="content-wrap">
    <Row>
      <milk-table ref="list" :layout="[17,4,3]" :columns="cols" :search-api="searchApi" :params="params">
        <template slot="search">
          <Form ref="params" :model="params" inline :label-width="60">
            <FormItem label="用户姓名">
              <Input v-model="params.name" placeholder="请输入用户姓名"></Input>
            </FormItem>
            <FormItem label="手机号码">
              <Input v-model="params.phone" placeholder="请输入手机号码"></Input>
            </FormItem>
            <FormItem label="权限角色">
              <Select v-model="params.role">
                <Option v-for="c in roles" :value="c.id" :key="c.id">{{c.displayName}}</Option>
              </Select>
            </FormItem>
          </Form>
        </template>
        <template slot="actions">
          <Button @click="add" type="primary">添加</Button>
        </template>
      </milk-table>
    </Row>
  </section>
</template>

<script>
import { getUsers, getRoles, getUserForEdit, deleteUser } from "api/manage";

export default {
  name: "user",
  data() {
    return {
      addInfoShow: false,
      cols: [
        {
          type: "selection",
          align: "center",
          width: "70px"
        },
        {
          title: "账户",
          key: "userName"
        },

        {
          title: "用户姓名",
          key: "name"
        },
        {
          title: "手机号",
          key: "phoneNumber"
        },
        {
          title: "角色",
          key: "roles",
          render: (h, params) => {
            let names = "";
            if (params.row.roles) {
              params.row.roles.forEach(c => {
                names += c.roleName + ",";
              });
            }
            return names;
          }
        },
        {
          title: "状态",
          key: "isActive",
          render: (h, params) => {
            return params.row.isActive ? "启用" : "禁用";
          }
        },
        {
          title: "创建时间",
          key: "creationTime",
          render: (h, params) => {
            return this.$fmtTime(params.row.creationTime);
          }
        },
        {
          title: "操作",
          key: "action",
          align: "center",
          render: (h, params) => {
            return h("div", [
              h(
                "Button",
                {
                  props: {
                    type: "primary",
                    size: "small"
                  },
                  style: {
                    marginRight: "5px"
                  },
                  on: {
                    click: () => {
                      this.edit(params.row);
                    }
                  }
                },
                "编辑"
              ),
              h(
                "Button",
                {
                  props: {
                    type: "error",
                    size: "small"
                  },
                  on: {
                    click: () => {
                      this.delete(params.row);
                    }
                  }
                },
                "删除"
              )
            ]);
          }
        }
      ],
      searchApi: getUsers,
      params: { name: "", phone: "", role: null },
      modal: {
        isEdit: false,
        title: "添加",
        current: null
      },
      roles: []
    };
  },
  components: {},
  created() {},
  methods: {
    // 删除
    delete(model) {
      var table = this.$refs.list;
      this.$Modal.confirm({
        title: "删除提示",
        content: "确定要删除当前用户么?",
        onOk: () => {
          const parms = { id: model.id };
          deleteUser(parms).then(c => {
            if (c.data.success) {
              table.initData();
            }
          });
        }
      });
    },
    add() {
      this.modal.isEdit = true;
      this.modal.title = "添加用户";
    },
    edit(row) {
      this.modal.current = row.id;
      this.modal.isEdit = true;
      this.modal.title = "编辑用户:" + row.name;
    },
    save() {
      this.$refs.account.commit();
    },
    cancel() {
      this.modal.isEdit = false;
      this.modal.title = "添加用户";
      this.modal.current = null;
      this.$refs.list.initData();
    },
    initRoles() {
      getRoles().then(c => {
        if (c.data.success) {
          this.roles = c.data.result.items;
        }
      });
    },
    mounted() {}
  }
};
</script>

<style scoped>
section.content-wrap {
  position: relative;
}

section.content-wrap .button {
  position: absolute;
  top: 20px;
  left: 60%;
  width: 120px;
  height: 30px;
  line-height: 30px;
  text-align: center;
  padding: 8px 20px;
  /* margin: 30px 0; */
  font-size: 20px;
  border-top: 1px solid #fff;
  border-bottom: 1px solid #fff;
  cursor: pointer;
}

.page-wrap {
  position: absolute;
  top: 450px;
  left: 50%;
  transform: translateX(-50%);
}

.page-wrap ul li {
  list-style: none;
  float: left;
  width: 40px;
  height: 40px;
  line-height: 40px;
  text-align: center;
  border: 1px solid rgba(255, 255, 255, 0.2);
  margin-right: 5px;
}

.page-wrap ul li:hover {
  background: #fff;
  color: rgba(7, 17, 27, 0.96);
}

.addInfo {
  position: fixed;
  top: 0;
  left: 0;
  /* z-index: 1000; */
  width: 100%;
  height: 100%;
  overflow: auto;
  backdrop-filter: blur(10px);
  transition: all 0.5s;
  background: rgba(7, 17, 27, 0.9);
}

.addInfo.fade-enter-active,
.addInfo.fade-leave-active {
  opacity: 1;
}

.addInfo.fade-enter,
.addInfo.fade-leave-active {
  opacity: 0;
}

.addInfo-wrapper {
  min-height: 100%;
  width: 100%;
}

.addInfo-main {
  /* margin-top: 44px; */
  padding-bottom: 150px;
}

.addInfo-close {
  position: relative;
  width: 32px;
  height: 32px;
  line-height: 32px;
  text-align: center;
  border-radius: 50%;
  border: 1px solid #fff;
  margin: 0px auto 0 auto;
  clear: both;
  font-size: 16px;
  cursor: pointer;
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

.from-wrap {
  padding-left: 450px;
  margin-top: 100px;
}

.ipunt-wrap label {
  width: 100px;
  text-align: center;
  display: inline-block;
}

.ipunt-wrap input {
  border: none;
  outline: none;
  background: none;
  border-bottom: 1px solid #fff;
  margin-bottom: 30px;
  width: 300px;
  height: 30px;
  line-height: 30px;
  color: #fff;
  font-size: 18px;
  padding: 0 5px;
}

.ipunt-button {
  margin-left: 150px;
}
</style>