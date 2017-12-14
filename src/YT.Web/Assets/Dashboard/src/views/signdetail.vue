<template>
  <section class="content-wrap">
    <Row>
      <milk-table ref="list" :layout="[20,2,2]" :columns="cols" :search-api="searchApi" :params="params">
        <template slot="search">
          <Form ref="params" :model="params" inline :label-width="60">
            <FormItem label="设备编号">
              <Input v-model="params.device" style="width: 140px" placeholder="设备编号"></Input>
            </FormItem>
             <FormItem label="运维人员">
              <Input v-model="params.userName" style="width: 140px" placeholder="运维人员"></Input>
            </FormItem>
             <FormItem label="人工编号">
              <Input v-model="params.userNum" style="width: 140px" placeholder="人员编号"></Input>
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
    <Modal v-model="modal.map" :width="900" title="地图展示" @on-ok="ok" @on-cancel="cancel">
      <baidu-map :center="position" :zoom="zoom" class="bm-view">
        <bm-scale anchor="BMAP_ANCHOR_TOP_RIGHT"></bm-scale>
        <bm-navigation anchor="BMAP_ANCHOR_TOP_RIGHT"></bm-navigation>
        <bm-marker :position="position" :dragging="false" animation="BMAP_ANIMATION_BOUNCE">
        </bm-marker>
      </baidu-map>
    </Modal>
    <Modal v-model="modal.image" title="图片展示" @on-ok="ok" @on-cancel="cancel">
      <img style="width:500px;height:500px;" v-for="(item,index) in images" :key="index" :src="item">
    </Modal>
  </section>
</template>
<script>
import { getSignStaticial, exportSignStaticial } from "api/statical";
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
          title: "设备编号",
          key: "deviceNum"
        },

        {
          title: "运维人员",
          key: "userName"
        },
        {
          title: "点位详情",
          key: "signLocation"
        },
        {
          title: "是否签到",
          key: "isSign",
          render: (h, params) => {
            return params.row.isSign ? "是" : "否";
          }
        },
        {
          title: "签到地点",
          key: "timeSign",
          render: (h, params) => {
            if (params.row.dimension && params.row.longitude) {
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
                        this.showMap(params.row);
                      }
                    }
                  },
                  "查看"
                )
              ]);
            }
          }
        },
        {
          title: "签到图片",
          key: "timeSign",
          render: (h, params) => {
            if (params.row.profiles) {
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
                        this.showImage(params.row.profiles);
                      }
                    }
                  },
                  "查看"
                )
              ]);
            }
          }
        },
        {
          title: "签到时间",
          key: "creationTime",
          render: (h, params) => {
            return params.row.creationTime
              ? this.$fmtTime(params.row.creationTime)
              : "";
          }
        }
      ],
      searchApi: getSignStaticial,
      params: {
        device: "",
        userName: "",
        userNum: "",
        start: null,
        end: null
      },
      position: {
        lng: 0,
        lat: 0
      },
      images: [],
      zoom: 14,
      modal: {
        map: false,
        image: false
      }
    };
  },
  components: {},
  created() {},
  methods: {
    exportData() {
      exportSignStaticial(this.params)
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
    showMap(row) {
      this.modal.map = true;
      this.position = { lng: row.longitude, lat: row.dimension };
    },
    showImage(images) {
      this.images = images;
      this.modal.image = true;
    },
    ok() {
      this.modal.map = false;
      this.modal.image = false;
    },
    cancel() {
      this.modal.map = false;
      this.modal.image = false;
    }
  }
};
</script>
<style scoped>
.bm-view {
  width: 100%;
  height: 400px;
}
</style>
