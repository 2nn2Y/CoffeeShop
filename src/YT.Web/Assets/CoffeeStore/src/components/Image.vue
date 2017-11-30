<template>
  <div v-cloak v-show="show">
        <div v-show="mailbox">
          <div class="deanMailbox" >
            <div class="deanMailDiv">
              <div>
                <ul class="uplodeUl">
                  <li :key="item" v-for="(item,index) in images" class="uplodeLi">
                    <img :src="item.imgSrc" class="uplodeImg"/>
                    <div @click="remove_img(index)" class="imgA"></div>
                  </li>
                  <li>
                    <div class="uplodeDean" v-if="images.length != 10" @click="chooseImg">
                      <img src="../assets/img/uolode.png" width="100%" height="100%"/>
                    </div>
                    <span class="uplodeTxt font16" v-if="images.length<2">上传问题照片</span>
                  </li>
                </ul>
              </div>
            </div>
          </div>
          <div class="squadButton agencyBut" v-on:click="submit">提交</div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: "sommoProblem",
  data() {
    return {
      show: false,
      feedback: [
        {
          cat_list: []
        }
      ],
      sommoProblem: true,
      mailbox: false,
      feedbacks: false,
      feed_index: 0,
      sommoProblemOne: true,
      sommoProblemTwo: false,
      sommeoIndex: 0,
      back: [],
      selectShows: true,
      myback: [],
      selectProblem: "",
      textAreaProblem: "",
      images: [],
      imageArray: [],
      myFeedBackOnClick: false,
      infoList: {},
      styleObject: {
        width: "",
        height: ""
      },
      avatar: "",
      userNames: "",
      img: [],
      style: {
        width: ""
      }
    };
  },
  methods: {
    verify: function() {
      let thisObj = this;
      this.$chaos.setTitle("帮助与反馈");
      this.$chaos.verify(function() {
        thisObj.$chaos.ajax({
          //需要调用微信jdk接口，后台已经打包好了，前端直接调用的接口，想要在vux中使用微信jdk必须在引用vux给的东西，这个可以在 https://vux.li/#/?id=%E4%BD%BF%E7%94%A8%E5%BE%AE%E4%BF%A1-jssdk 网站中看到
          data: {
            url: location.href.split("#")[0]
          },
          slient: true,
          userinfo: true,
          url: "Weixin/get_jssdk_config",
          callback: function(type, res) {
            if (type !== "success") {
              return;
            }
            if (res.status) {
              thisObj.$wechat.config(res.info);
            }
          }
        });
      });
    },
    getLocalImgData: function(localIds) {
      let localId = localIds.pop();
      let thisObj = this;
      thisObj.$wechat.getLocalImgData({
        //vux中把wx换成 thisObj.$wechat
        localId: localId,
        success: function(res) {
          let localData = res.localData;
          thisObj.images.push({ imgSrc: localData, localId: localId });
          if (localIds.length > 0) {
            thisObj.getLocalImgData(localIds);
          }
        },
        fail: function() {
          thisObj.$vux.toast.show({
            text: "图片有误",
            type: "text",
            width: "180px",
            position: "bottom"
          });
        }
      });
    },
    chooseImg: function() {
      let thisObj = this;
      thisObj.$wechat.chooseImage({
        count: 9, // 默认9
        sizeType: ["original", "compressed"],
        sourceType: ["album", "camera"],
        success: function(res) {
          let localIds = res.localIds;
          if (window.__wxjs_is_wkwebview) {
            //判断是否是WKWebview内核，也就是苹果内核
            thisObj.getLocalImgData(localIds);
          } else {
            for (let i = 0; i < localIds.length; i++) {
              thisObj.images.push({
                imgSrc: localIds[i],
                localId: localIds[i]
              });
            }
          }
          setTimeout(function() {
            $(".uplodeImg").each(function() {
              if ($(this).height() > $(this).width()) {
                $(this).css({ width: "100%", height: "auto" });
              } else {
                $(this).css({ width: "auto", height: "100%" });
              }
            });
          }, 600);
        }
      });
    },
    remove_img: function(index) {
      let thisObj = this;
      thisObj.$vux.confirm.show({
        title: "系统提示",
        content: "确认要删除吗?",
        onConfirm() {
          thisObj.images.splice(index, 1);
        }
      });
    },
    submit: function() {
      let thisObj = this;
      thisObj.imageArray = [];
      thisObj.wx_upload_img(thisObj.images.length);
    },
    wx_upload_img: function(index) {
      let thisObj = this;
      if (index === 0) {
        if (thisObj.images.length >= 9) {
          thisObj.$vux.toast.show({
            text: "图片最多上传9张",
            type: "text",
            width: "180px",
            position: "bottom"
          });
          return;
        }
        thisObj.$vux.confirm.show({
          title: "系统提醒",
          content: "确认要提交吗?",
          onConfirm() {
            thisObj.$vux.loading.show({
              text: "Loading"
            });
            thisObj.$chaos.ajax({
              data: {
                file: thisObj.imageArray
              },
              slient: true,
              userinfo: true,
              url: "User/user_leave_msg",
              callback: function(type, res) {
                if (type !== "success") {
                  return;
                }
                if (res.status) {
                  thisObj.$vux.loading.hide();
                } else {
                  thisObj.$vux.toast.show({
                    text: res.msg,
                    type: "text",
                    width: "180px",
                    position: "bottom"
                  });
                }
              }
            });
          }
        });
        return;
      }
      let thisSrc = thisObj.images[index - 1]["localId"];
      thisObj.$wechat.uploadImage({
        localId: thisSrc, // 需要上传的图片的本地ID，由chooseImage接口获得
        isShowProgressTips: 0, // 默认为1，显示进度提示
        success: function(res) {
          let serverId = res.serverId; // 返回图片的服务器端ID
          thisObj.imageArray.push(serverId);
          thisObj.wx_upload_img(index - 1);
        }
      });
    }
  },
  created: function() {
    this.verify();
  },
  watch: {
    $route: "verify"
  }
};
</script>
<style scoped>
.divmyFeed {
  margin-bottom: 30px;
}
#feedUl li {
  float: left;
  margin-right: 5px;
  overflow: hidden;
  margin-bottom: 5px;
}
#feedUl li:nth-child(3n) {
  margin-right: 0px;
}
#feedUl:after {
  content: "";
  display: block;
  clear: both;
}
#feedUl {
  margin-top: 10px;
}
[v-cloak] {
  display: none;
}
.noMessageColor {
  color: #bbb;
  margin-bottom: 10px;
}
.messageImg {
  text-align: center;
  margin-top: 180px;
}
.myFeedOn {
  margin: 10px 30px 10px 38px;
  background: #ffffff;
  box-shadow: 0 1px 5px #ccc;
  /*padding:10px 15px;*/
}
.myFeedTitle {
  padding: 15px 20px;
  position: relative;
}
.feedCon {
  padding-bottom: 10px;
  border-bottom: 1px solid #bebebe;
}
.feedQues {
  padding: 8px 0px;
  word-break: break-all;
  text-align: justify;
}
.feedTitle {
  padding: 14px 0px 14px 32px;
  overflow: hidden;
}
.boderB {
  border-bottom: 1px solid #dfdfdf;
  border-top: 1px solid #dfdfdf;
}
.feedP2 {
  color: #393939;
  font-size: 16px;
  line-height: 1.9;
  text-align: justify;
}
.feedP1 {
  font-size: 14px;
  color: #6c6c6c;
  line-height: 1.5;
}
.feedImage {
  position: absolute;
  left: -24px;
  top: 22px;
  width: 60px;
  height: 60px;
  border-radius: 50%;
  overflow: hidden;
}
.feedBottom {
  padding: 30px 0px 6px;
  margin-bottom: 20px;
}
.feedBottomYellow {
  font-size: 14px;
  text-align: center;
  line-height: 1.3;
  color: #ffa200;
}
.feedBottomP {
  font-size: 14px;
  text-align: center;
  line-height: 3;
  color: #bababa;
}
.imgA {
  position: absolute;
  top: 0px;
  right: 0px;
  width: 20px;
  height: 20px;
  background-size: 100% 100%;
  background-image: url(../assets/img/del.png);
}
.uplodeLi {
  width: 22%;
  float: left;
  margin-right: 4%;
  overflow: hidden;
  box-sizing: border-box;
  height: 73px;
  margin-bottom: 7px;
  position: relative;
}
.uplodeLi:nth-child(4n) {
  margin-right: 0%;
}
.fileInput {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  width: 100%;
  opacity: 0;
}
.selectDiv {
  position: relative;
}
.placeProble {
  position: absolute;
  left: -47px;
  top: 10px;
  background: transparent;
  z-index: -8;
}
.selectDiv {
  display: inline-block;
}
.sommoProblemTxt li {
  border-bottom: 1px solid #e2e2e2;
}
.uplodeUl:after {
  content: "";
  display: block;
  clear: both;
}
.sommoProblemTxtP {
  margin: 0 30px;
  background: url(../assets/img/circle.png) no-repeat;
  text-indent: 20px;
  background-size: 2.5%;
  background-position: 0 6px;
}
.personalUl a,
.opinionA a {
  color: #363636;
}
.sommoProblemHead {
  border-bottom: 1px solid #fbac36;
  padding: 4px 0;
}
.sommoProblemHead li {
  width: 33.333%;
  float: left;
  text-align: center;
  line-height: 2.5;
  position: relative;
}
.opinionA li {
  padding: 20px 30px;
}
.sommoProblemBor:after {
  content: "";
  display: block;
  width: 45%;
  margin: 0 auto;
  height: 2px;
  background: #fbac36;
}
.sommoProblemHead li {
  width: 33.333%;
  float: left;
  text-align: center;
  line-height: 2.5;
  position: relative;
}
.sommoProblemHead li {
  width: 33.333%;
  float: left;
  text-align: center;
  line-height: 2.5;
  position: relative;
}
.sommoProblemHead ul:after {
  content: "";
  display: block;
  clear: both;
}
.deanMailbox {
  border-bottom: 1px solid #e2e2e2;
}
.deanMailSel {
  width: 100%;
  text-align: center;
  border-bottom: 1px solid #e2e2e2;
}
.deanMailboxSelect {
  -webkit-appearance: none;
  background: transparent;
  border: 0;
  font-size: 16px;
  line-height: 2.8;
  z-index: 999;
  width: 100%;
  padding-right: 20px;
  height: 45px;
  width: 90px;
  outline: none;
}
.placeProble {
  width: 120px;
}
.deanMailboxSelect:active {
  background: transparent;
}

.deanMailDiv {
  margin: 15px 20px;
}
.deanMailboxTextarea {
  resize: none;
  width: 100%;
  border: 0;
  font-size: 16px;
}
.uplodeDean {
  width: 22%;
  height: 73px;
  display: inline-block;
  position: relative;
}
.uplodeTxt {
  padding: 0px 10px;
}
.font16 {
  font-size: 16px;
}
.myFeedback {
  margin: 0 20px;
}
.myFeedback li {
  background: #f6f6f6;
  padding: 20px 20px 11px;
  margin: 20px 0;
  box-shadow: 0 1px 4px #ccc;
}
.myFeedbackDiv {
  margin-bottom: 5px;
}
.fl {
  float: left;
}
.myFeed {
  margin-left: 70px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  word-break: break-all;
}
.shz {
  color: #d30012;
}
.myFeedXz {
  width: 30px;
  height: 12px;
  margin: 16px auto 0;
  background: url(../assets/img/xiala.png) no-repeat;
  background-size: 65%;
  background-position: center center;
}
</style>