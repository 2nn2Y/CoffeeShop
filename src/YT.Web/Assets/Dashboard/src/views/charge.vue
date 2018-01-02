<template>
  <section class="content-wrap">
    <Row>
      <milk-table ref="list" :layout="[20,2,2]" :columns="cols" :search-api="searchApi" :params="params">
        <template slot="search">
          <Form ref="params" :model="params" inline :label-width="60">
            <FormItem label="用户昵称">
              <Input v-model="params.userName" style="width: 140px" placeholder="用户昵称"></Input>
            </FormItem>
            </FormItem>
            <FormItem label="开始时间">
              <DatePicker type="date" :editable="false" v-model="params.start" placeholder="开始时间" style="width: 140px"></DatePicker>
            </FormItem>
            <FormItem label="截至时间">
              <DatePicker type="date" :editable="false" v-model="params.end" placeholder="截至时间" style="width: 140px"></DatePicker>
            </FormItem>
          </Form>
        </template>
        <template slot="actions">
          <Button @click="exportData" type="primary">导出</Button>
        </template>
      </milk-table>
    </Row>
  </section>
</template>

<script>
import {
  getActivityAndChargeOrders,
  exportActivityAndChargeOrders
} from "api/statical";

export default {
  name: "store",
  data() {
    return {
      cols: [
        {
          title: "用户昵称",
          key: "nickName"
        },
        {
          title: "订单号",
          key: "orderNum"
        },
        {
          title: "支付类型",
          key: "payType",
          render: (h, params) => {
            return params.row.payType == 1
              ? "余额支付"
              : params.row.payType == 2
                ? "在线支付"
                : params.row.payType == 3 ? "充值" : "活动支付";
          }
        },
        {
          title: "订单状态",
          key: "orderState",
          render: (h, params) => {
            return params.row.orderState ? "已完成" : "未完成";
          }
        },
        {
          title: "金额",
          key: "price",
          render: (h, params) => {
            return params.row.price / 100;
          }
        },
        {
          title: "支付时间",
          key: "creationTime",
          render: (h, params) => {
            return this.$fmtTime(params.row.creationTime);
          }
        }
      ],
      searchApi: getActivityAndChargeOrders,
      params: {
        PayType: 3,
        userName: "",
        point: "",
        device: "",
        productName: "",
        start: null,
        end: null
      }
    };
  },
  components: {},
  created() {},
  methods: {
    exportData() {
      exportActivityAndChargeOrders(this.params)
        .then(r => {
          if (r.success) {
            this.$down(
              r.result.fileType,
              r.result.fileToken,
              r.result.fileName
            );
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

</style>
