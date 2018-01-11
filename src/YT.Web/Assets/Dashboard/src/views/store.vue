<template>
  <section class="content-wrap">
    <Row>
      <milk-table ref="list" :layout="[20,2,2]" :columns="cols" :search-api="searchApi" :params="params">
        <template slot="search">
          <Form ref="params" :model="params" inline :label-width="60">
            <FormItem label="会员昵称">
              <Input v-model="params.userName" style="width: 140px" placeholder="会员昵称"></Input>
            </FormItem>
            <FormItem label="商品名称">
              <Input v-model="params.productName" style="width: 140px" placeholder="产品名"></Input>
            </FormItem>
            <FormItem label="设备名称">
              <Input v-model="params.point" style="width: 140px" placeholder="点位名"></Input>
            </FormItem>
            <FormItem label="设备编号">
              <Input v-model="params.device" style="width: 140px" placeholder="设备编号"></Input>
            </FormItem>
            <FormItem label="订单类型">
              <Select v-model="params.orderType" style="width:140px">
                <Option value="">全部</Option>
                <Option value="2" >冰山</Option>
                <Option value="1">技诺</Option>
              </Select>
            </FormItem>
            <FormItem label="订单状态">
              <Select v-model="params.state" style="width:140px">
                <Option value="">全部</Option>
                <Option value="true" >已支付</Option>
                <Option value="false">未支付</Option>
              </Select>
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
import { getUserOrders, exportUserOrders } from "api/statical";

export default {
  name: "store",
  data() {
    return {
      cols: [
        {
          title: "会员昵称",
          key: "nickName"
        },
        {
          title: "设备名称",
          key: "pointName"
        },
        {
          title: "设备编号",
          key: "deviceNum"
        },
        {
          title: "商品名称",
          key: "productName"
        },
        {
          title: "订单编号",
          key: "orderNum"
        },
        {
          title: "订单类型",
          key: "orderType",
          render: (h, params) => {
            return params.row.orderType == 1 ? "技诺支付" : "冰山支付";
          }
        },
        {
          title: "支付金额",
          key: "price",
          render: (h, params) => {
            return params.row.price / 100;
          }
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
            return params.row.payState ? "已支付" : "未支付";
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
      searchApi: getUserOrders,
      params: {
        payType: 1,
        orderType: null,
        userName: "",
        point: "",
        device: "",
        state: null,
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
      exportUserOrders(this.params)
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
