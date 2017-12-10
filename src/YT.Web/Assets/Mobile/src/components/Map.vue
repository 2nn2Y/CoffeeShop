<template>
  <div style="height:100%;">
    <view-box ref="viewBox">
    <baidu-map :center="map.center" :zoom="map.zoom" class="bm-view">
      <bm-navigation anchor="BMAP_ANCHOR_TOP_RIGHT"></bm-navigation>
      <bm-marker :position="location" :dragging="true" animation="BMAP_ANIMATION_BOUNCE">
      </bm-marker>
      <bm-geolocation @locationSuccess="loadpoint" anchor="BMAP_ANCHOR_BOTTOM_RIGHT" :showAddressBar="true" :autoLocation="true"></bm-geolocation>
    </baidu-map>
{{request}}
<br/>
{{response}}

    <div style="margin: 10px;overflow: hidden;">
      <masker style="border-radius: 2px;">
        <div class="m-img" :style="{backgroundcolor: 'grey'}"></div>
        <div @click="callphoto" slot="content" class="m-title">
          拍照
          <br/>
        </div>
      </masker>
    </div>
    <div>
      <img class="previewer-demo-img" :key="index" v-for="(item, index) in list"
       :src="item.src" width="100" @click="show(index)">
      <div v-transfer-dom>
        <previewer v-if="list.length>0" :list="list" ref="previewer" :options="options"></previewer>
      </div>
    </div>
    <x-button :disabled="list.length<=0"
     @click.native="signup" :gradients="['#1D62F0', '#19D5FD']" type="warn">签到</x-button>
    </view-box>
  </div>
</template>

<script>
import { XCircle, Masker, Previewer, TransferDom, XButton, ViewBox } from "vux";
const Base64 = require("js-base64").Base64;
export default {
  name: "detail",
  data() {
    return {
      list: [],
      request: "",
      response: "",
      map: {
        center: "北京",
        zoom: 14
      },
      location: {
        lat: 0,
        lng: 0
      },

      options: {
        getThumbBoundsFn(index) {
          // find thumbnail element
          const thumbnail = document.querySelectorAll(".previewer-demo-img")[
            index
          ];
          // get window scroll Y
          const pageYScroll =
            window.pageYOffset || document.documentElement.scrollTop;
          // optionally get horizontal scroll
          // get position of element relative to viewport
          const rect = thumbnail.getBoundingClientRect();
          // w = width
          return { x: rect.left, y: rect.top + pageYScroll, w: rect.width };
          // Good guide on how to get element coordinates:
          // http://javascript.info/tutorial/coordinates
        }
      }
    };
  },
  directives: {
    TransferDom
  },
  computed: {
    site: function() {
      return this.change(this.location.lat, this.location.lng);
    }
  },
  created() {
    var _self = this;
    const url = document.location.href; // 当前url
    _self.wxConfig(url);
    _self.$wechat.ready(() => {
      // _self.$wechat.getLocation({
      //   type: "wgs84", // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
      //   success: function(res) {
      //     _self.location.lat = res.latitude;
      //     _self.location.lng = res.longitude;
      //   }
      // });
    });
  },
  components: {
    XCircle,
    Masker,
    Previewer,
    XButton,
    ViewBox
  },
  methods: {
    loadpoint(point) {
      alert(JSON.stringify(point));
      this.location = point;
    },
    show(index) {
      this.$refs.previewer.show(index);
    },
    change(x, y) {
      const ak = "pYjoSR2GThuatLt06MlaKzRgSWy4Zztq";
      const burl = `http://api.map.baidu.com/geoconv/v1/?coords=${x},${y}&from=1&to=5&ak=${ak}`;
      this.request = burl;
      this.$http
        .get(burl, {
          headers: {
            dataType: "jsonp"
          }
        })
        .then(r => {
          this.response = JSON.stringify(r);
          if (r.response) {
            this.location.lat = Base64.decode(r.response.result.x);
            this.location.lng = Base64.decode(r.response.result.y);
          }
        });
    },
    signup() {
      if (this.list.length <= 0) {
        this.showBox("未完成", "请先拍摄照片");
        return;
      }
      const url = "http://103.45.102.47:8888/api/services/app/mobile/Sign";
      const params = {
        userId: sessionStorage.getItem("userId"),
        pointId: this.$route.params.point,
        state: true,
        longitude: this.location.lng,
        dimension: this.location.lat,
        signProfiles: this.list
      };
      this.$http
        .post(url, params)
        .then(r => {
          if (r.data.success) {
            this.showBox("签到", "签到成功");
            this.$router.push({ path: "/sign" });
          }
        })
        .catch(e => {
          this.showBox("失败", e.response.data.error.message);
        });
    },
    callphoto() {
      var _self = this;
      _self.$wechat.chooseImage({
        count: 9, // 默认9
        sizeType: ["original", "compressed"], // 可以指定是原图还是压缩图，默认二者都有
        sourceType: ["camera"], // 可以指定来源是相册还是相机，默认二者都有
        success: function(res) {
          if (res.localIds) {
            res.localIds.forEach(element => {
              _self.$wechat.uploadImage({
                localId: element, // 需要上传的图片的本地ID，由chooseImage接口获得
                isShowProgressTips: 0, // 默认为1，显示进度提示
                success: function(res) {
                  _self.list.push({ src: element, medeaId: res.serverId });
                }
              });
            });
          }
        }
      });
    }
  }
};
</script>

<style scoped lang="less">
.bm-view {
  width: 100%;
  height: 300px;
}

.container {
  margin: 120px 70px;
  width: 200px;
  height: 200;
}
.m-img {
  padding-bottom: 33%;
  display: block;
  position: relative;
  max-width: 100%;
  background-size: cover;
  background-position: center center;
  cursor: pointer;
  border-radius: 2px;
}

.m-title {
  color: #fff;
  text-align: center;
  text-shadow: 0 0 2px rgba(0, 0, 0, 0.5);
  font-weight: 500;
  font-size: 16px;
  position: absolute;
  left: 0;
  right: 0;
  width: 100%;
  text-align: center;
  top: 50%;
  transform: translateY(-50%);
}

.m-time {
  font-size: 12px;
  padding-top: 4px;
  border-top: 1px solid #f0f0f0;
  display: inline-block;
  margin-top: 5px;
}
</style>
