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

var Meteor = function () {
  function Meteor(ctx, x, h) {
    _classCallCheck(this, Meteor);

    this.ctx = ctx;
    this.x = x;
    this.y = 0;
    this.h = h;
    this.vx = -(4 + Math.random() * 4);
    this.vy = -this.vx;
    this.len = Math.random() * 300 + 500;
  }

  _createClass(Meteor, [{
    key: 'flow',
    value: function flow() {
      //判定流星出界
      if (this.x < -this.len || this.y > this.h + this.len) {
        return false;
      }
      this.x += this.vx;
      this.y += this.vy;
      return true;
    }
  }, {
    key: 'draw',
    value: function draw() {
      var ctx = this.ctx,

        //径向渐变，从流星头尾圆心，半径越大，透明度越高
        gra = ctx.createRadialGradient(this.x, this.y, 0, this.x, this.y, this.len);

      var PI = Math.PI;
      gra.addColorStop(0, 'rgba(255,255,255,1)');
      gra.addColorStop(1, 'rgba(0,0,0,0)');
      ctx.save();
      ctx.fillStyle = gra;
      ctx.beginPath();
      //流星头，二分之一圆
      ctx.arc(this.x, this.y, 1, PI / 4, 5 * PI / 4);
      //绘制流星尾，三角形
      ctx.lineTo(this.x + this.len, this.y - this.len);
      ctx.closePath();
      ctx.fill();
      ctx.restore();
    }
  }]);

  return Meteor;
}();

exports.default = Meteor;