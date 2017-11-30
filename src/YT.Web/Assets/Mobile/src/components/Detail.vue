<template>
  <div>
    <group label-width="5em" title="故障详情">
      <cell primary="content" :value="warn.description"></cell>
      <cell primary="content">
        <x-button v-if="!warn.state" @click.native="reslove(a)" :gradients="['#1D62F0', '#19D5FD']" type="warn">解决</x-button>
      </cell>
    </group>
    <divider>地图</divider>
    <baidu-map :center="map.center" :zoom="map.zoom" @ready="handler" class="bm-view">
      <bm-navigation anchor="BMAP_ANCHOR_TOP_RIGHT"></bm-navigation>
      <bm-marker :position="model.position" :dragging="false" animation="BMAP_ANIMATION_BOUNCE">
      </bm-marker>
    </baidu-map>
  </div>
</template>

<script>
import { Group, Cell, Card, XButton, CellBox, Divider } from "vux";
export default {
  name: "detail",
  data() {
    return {
      map: {
        center: "北京",
        zoom: 13
      },
      warn: {},
      model: {
        content: "漏水漏电漏风",
        position: {
          lng: 116.404,
          lat: 39.915
        }
      }
    };
  },
  created() {
    this.init();
  },
  components: {
    Group,
    Cell,
    Card,
    XButton,
    CellBox,
    Divider
  },
  methods: {
    reslove() {
      const url = "http://103.45.102.47:8888/api/services/app/mobile/DealWarn";
      const params = {
        id: this.warn.id
      };
      this.$http.post(url, params).then(r => {
        if (r.data.success) {
          this.showBox("成功", "解决问题成功");
          this.$router.push({
            path: "/accident"
          });
        }
      });
    },
    init() {
      const url =
        "http://103.45.102.47:8888/api/services/app/mobile/GetWarnByUser";
      const params = {
        id: this.$route.query.id
      };
      this.$http.post(url, params).then(r => {
        if (r.data) {
          this.warn = r.data.result;
        }
      });
    },
    handler({ BMap, map }) {
      console.log(BMap, map);
      this.zoom = 15;
    }
  }
};
</script>

<style scoped lang="less">
@import "~vux/src/styles/1px.less";
.bm-view {
  width: 100%;
  height: 350px;
}

.card-demo-flex {
  display: flex;
}

.card-demo-content01 {
  padding: 10px 0;
}

.card-padding {
  padding: 15px;
}

.card-demo-flex > div {
  flex: 1;
  text-align: center;
  font-size: 12px;
}

.card-demo-flex span {
  color: #f74c31;
}
</style>
