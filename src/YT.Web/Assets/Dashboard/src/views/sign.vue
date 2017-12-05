<template>
  <section class="content-wrap">
    <Row>
      <milk-table ref="list" :layout="[20,2,2]" :columns="cols" :search-api="searchApi" :params="params">
        <template slot="search">
          <Form ref="params" :model="params" inline :label-width="60">
            <FormItem label="运维人员名">
              <Input v-model="params.device" style="width: 140px" placeholder="运维人员"></Input>
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
import { getSignByuser, exportSignByuser } from "api/statical";
export default {
  name: "user",
  data() {
    return {
      cols: [
        {
          title: "人员工号",
          key: "userId"
        },
        {
          title: "运维人员",
          key: "userName"
        },
        {
          title: "签到日",
          key: "daySign"
        },
        {
          title: "签到次数",
          key: "timeSign"
        }
      ],
      searchApi: getSignByuser,
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
      exportSignByuser(this.params)
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