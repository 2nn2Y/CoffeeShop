<template>
  <div>
    <card>
      <img slot="header" src="http://placeholder.qiniudn.com/640x300" style="width:100%;display:block;">
      <div slot="content" class="card-padding">
      </div>
    </card>
     <x-button type="primary" @click.native="buyCard">购买</x-button>
  </div>
</template>

<script>
import { Panel, Group, Radio, Card } from "vux";
export default {
  components: {
    Panel,
    Group,
    Radio,
    Card
  },
  methods: {
    buyCard() {
      console.log(1);
    }
  },
  data() {
    return {};
  }
};
</script>
