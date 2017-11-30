<template>
  <div>
    <divider>充值金额</divider>
    <box gap="10px 10px">
      <x-button :gradients="['#1D62F0', '#19D5FD']">充50得50</x-button>
      <x-button :gradients="['#A644FF', '#FC5BC4']">充100得110</x-button>
      <x-button :gradients="['#FF2719', '#FF61AD']">充150得165</x-button>
      <x-button :gradients="['#6F1BFE', '#9479DF']">充200得230</x-button>
      <x-button :gradients="['#FF5E3A', '#FF9500']">充300得340</x-button>
    </box>
  </div>
</template>
<script>
import { XButton, Box, GroupTitle, Group, Divider } from "vux";

export default {
  components: {
    XButton,
    Box,
    GroupTitle,
    Group,
    Divider
  },
  created() {
    this.callTitle("充值");
  },
  methods: {
    change(value) {
      console.log("change:", value);
    },
    processButton001() {
      this.submit001 = "processing";
      this.disable001 = true;
    }
  },
  data() {
    return {
      submit001: "click me",
      disable001: false
    };
  }
};
</script>

<style lang="less">
.custom-primary-red {
  border-radius: 99px !important;
  border-color: #ce3c39 !important;
  color: #ce3c39 !important;
  &:active {
    border-color: rgba(206, 60, 57, 0.6) !important;
    color: rgba(206, 60, 57, 0.6) !important;
    background-color: transparent;
  }
}
</style>