<template>
  
</template>

<script>
import Stomp from "stompjs";
export default {
  // npm install stompjs
  // npm install --save net
  name: "stomp",
  data() {
    return {
      client: Stomp.client("ws://118.26.73.70:25674/ws")
    };
  },
  created() {
    // 创建连接
    this.connect();
  },
  methods: {
    // 连接成功回调
    onConnected(frame) {
      console.log("Connected: " + frame);
      var topic = "/topic/environments";
      this.client.subscribe(topic, this.responseCallback, this.onFailed);
    },
    // 失败回调
    onFailed(frame) {
      console.log("Failed: " + frame);
    },
    // 接受消息回调
    responseCallback(frame) {
      console.log("responseCallback msg=>" + frame.body);
    },
    // 连接
    connect() {
      var clientid = "aaaaaaaa";
      var headers = {
        login: "admin",
        passcode: "admin",
        "client-id": clientid
        // additional header
      };
      this.client.connect(headers, this.onConnected, this.onFailed);
    }
  }
};
</script>