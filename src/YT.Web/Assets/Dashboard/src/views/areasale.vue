<template>
  <section class="content-wrap">
    <Row>
      <milk-table ref="list" :layout="[20,2,2]" :columns="cols" :search-api="searchApi" :params="params">
        <template slot="search">
          <Form ref="params" :model="params" inline :label-width="60">
             <FormItem label="地区">
              <Input v-model="params.area" style="width: 140px" placeholder="地区"></Input>
            </FormItem>
             <FormItem label="区域">
              <Input v-model="params.city" style="width: 140px" placeholder="区域"></Input>
            </FormItem>
             <FormItem label="设备编号">
              <Input v-model="params.deviceNum" style="width: 140px" placeholder="设备编号"></Input>
            </FormItem>
              <FormItem label="设备名称">
              <Input v-model="params.deviceName" style="width: 140px" placeholder="设备名称"></Input>
            </FormItem>
             <FormItem label="产品名称">
              <Input v-model="params.productName" style="width: 140px" placeholder="产品名称"></Input>
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
import { getAreaProductsSale, exportAreaProductsSale } from "api/statical";
export default {
  name: "user",
  data() {
    return {
      cols: [
        {
          title: "地区",
          key: "area"
        },
        {
          title: "区域",
          key: "city"
        },
        {
          title: "设备名称",
          key: "deviceName"
        },
  {
          title: "设备编号",
          key: "deviceNum"
        },
        {
          title: "产品名称",
          key: "productName"
        },
        {
          title: "销量",
          key: "count"
        },
        {
          title: "总价",
          key: "price",
          render: (h, params) => {
            return params.row.price / 100;
          }
        }
      ],
      searchApi: getAreaProductsSale,
      params: {
        deviceNum: "",
        deviceName: "",
        area: "",
        city: "",
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
      exportAreaProductsSale(this.params)
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
