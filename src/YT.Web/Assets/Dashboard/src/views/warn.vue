<template>
  <section class="content-wrap">
    <Row>
      <milk-table ref="list" :formatter-row="format" :layout="[20,2,2]" :columns="cols" :search-api="searchApi" :params="params">
        <template slot="search">
          <Form ref="params" :model="params" inline :label-width="60">
            <FormItem label="设备编号">
              <Input v-model="params.device" style="width: 140px" placeholder="设备编号"></Input>
            </FormItem>
             <FormItem label="点位名称">
              <Input v-model="params.point" style="width: 140px" placeholder="点位名称"></Input>
            </FormItem>
            <FormItem label="故障类型">
              <Input v-model="params.type" style="width: 140px" placeholder="故障类型"></Input>
            </FormItem>
            <FormItem label="是否解决">
              <Select v-model="params.isDeal" style="width:140px">
                <Option value="">请选择</Option>
                <Option value="true">是</Option>
                <Option value="false">否</Option>
              </Select>
            </FormItem>
            <FormItem label="运维人员">
              <Input v-model="params.user" style="width: 140px" placeholder="运维人员"></Input>
            </FormItem>
            <FormItem label="开始时长">
                <Input v-model="params.left" placeholder="开始时长" style="width: 140px"></Input>
            </FormItem>
            <FormItem label="截至时长">
                 <Input v-model="params.right" placeholder="截至时长" style="width: 140px"></Input>
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
import { getWarns, exportWarns } from "api/statical";
export default {
  name: "user",
  data() {
    return {
      cols: [
        {
          title: "设备编号",
          key: "deviceNum"
        },
        {
          title: "点位详情",
          key: "pointName"
        },
        {
          title: "故障类型",
          key: "warnType"
        },
        {
          title: "故障发生时间",
          key: "warnDate",
          render: (h, params) => {
            return params.row.warnDate
              ? this.$fmtTime(params.row.warnDate)
              : "";
          }
        },
        {
          title: "是否解决",
          key: "isSolve",
          render: (h, params) => {
            return params.row.isSolve ? "是" : "否";
          }
        },
        {
          title: "处理完成时间",
          key: "setTime",
          render: (h, params) => {
            return params.row.setTime ? this.$fmtTime(params.row.setTime) : "";
          }
        },
        {
          title: "故障解决时间",
          key: "solveDate",
          render: (h, params) => {
            return params.row.solveDate
              ? this.$fmtTime(params.row.solveDate)
              : "";
          }
        },
        {
          title: "未解决时长",
          key: "unSolveTime",
          render: (h, params) => {
            return params.row.unSolveTime > 0
              ? params.row.unSolveTime.toFixed(2)
              : 0;
          }
        },
        {
          title: "解决时长",
          key: "solveTime"
        },
        {
          title: "运维人员",
          key: "userName"
        }
      ],
      searchApi: getWarns,
      format: this.formatter,
      params: {
        device: "",
        isDeal: "",
        left: null,
        right: null,
        type: "",
        user: "",
        start: null,
        end: null,
        point: ""
      }
    };
  },
  components: {},
  created() {},
  methods: {
    exportData() {
      exportWarns(this.params)
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
    },
    formatter(row, index) {
      if (row.unSolveTime > 120) {
        return "demo-table-error-row";
      } else if (row.unSolveTime > 60) {
        return "demo-table-warn-row";
      } else if (row.unSolveTime <= 60 && row.unSolveTime > 0) {
        return "demo-table-info-row";
      }
      return "";
    }
  }
};
</script>

<style>
.ivu-table .demo-table-info-row td {
  background-color: green;
  color: #fff;
}
.ivu-table .demo-table-warn-row td {
  background-color: #7f3203;
  color: #fff;
}

.ivu-table .demo-table-error-row td {
  background-color: red;
  color: #fff;
}
</style>
