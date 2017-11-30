'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () {
  function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i];
      descriptor.enumerable = descriptor.enumerable || false;
      descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true;
      Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var Stars = function () {
  function Stars(ctx, width, height, amount) {
    _classCallCheck(this, Stars);

    this.ctx = ctx;
    this.width = width;
    this.height = height;
    this.stars = this.getStars(amount);
  }

  //返回一堆的小星星


  _createClass(Stars, [{
    key: 'getStars',
    value: function getStars(amount) {
      var stars = [];
      while (amount--) {
        // console.log(amount)
        // 创建一个星星的坐标及大小数据描述JSON对象，添加到stars数组
        stars.push({
          x: Math.random() * this.width,
          y: Math.random() * this.height,
          r: Math.random() + 0.2
        });
      }
      return stars;
    }

    //画满天星

  }, {
    key: 'draw',
    value: function draw() {
      var ctx = this.ctx;
      ctx.save(); //保存canvas当前绘制，一般新绘制前调用
      ctx.fillStyle = 'white';
      //star 是从stars里的一颗star 这里是function的参数
      this.stars.forEach(function (star) {
        ctx.beginPath(); //开始绘制
        ctx.arc(star.x, star.y, star.r, 0, 2 * Math.PI); //画椭圆
        ctx.fill(); //填充色
      });
      ctx.restore(); //再次保存
    }

    //星星没隔10帧闪一下

  }, {
    key: 'blink',
    value: function blink() {
      //map方法 找到合适的星星放进新数组 匹配
      this.stars = this.stars.map(function (star) {
        var sign = Math.random() > 0.5 ? 1 : -1;
        star.r += sign * 0.2;
        if (star.r < 0) {
          star.r = -star.r;
        } else if (star.r > 1) {
          star.r -= 0.2;
          // console.log(star.r)
        }
        return star;
      });
    }
  }]);

  return Stars;
}();

exports.default = Stars;