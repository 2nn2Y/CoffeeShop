<template>
  <section class="content-wrap">
    <Row>
      <milk-table ref="list" :layout="[20,2,2]" :columns="cols" :search-api="searchApi" :params="params">
        <template slot="search">
          <Form ref="params" :model="params" inline :label-width="60">
            <FormItem label="产品名">
              <Input v-model="params.product" style="width: 140px" placeholder="请输入商品名"></Input>
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
import { getWarnByDevice, exportWarnByDevice } from "api/warn";
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
          title: "点位地址",
          key: "deviceName"
        },
        {
          title: "故障类型",
          key: "warnType"
        },
        {
          title: "故障数量",
          key: "warnCount"
        },
        {
          title: "解决数量",
          key: "dealCount"
        },
        {
          title: "解决平均时间",
          key: "perTime"
        },
        {
          title: "运维人员",
          key: "userName"
        }
      ],
      searchApi: getWarnByDevice,
      params: {
        device: "",
        product: "",
        start: null,
        end: null
      }
    };
  },
  components: {},
  created() {},
  methods: {
    exportData() {
      exportWarnByDevice(this.params)
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