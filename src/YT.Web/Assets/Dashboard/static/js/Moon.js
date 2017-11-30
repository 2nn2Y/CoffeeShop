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

var Moon = function () {
  function Moon(ctx, width, height) {
    _classCallCheck(this, Moon);

    this.ctx = ctx;
    this.width = width;
    this.height = height;
  }

  _createClass(Moon, [{
    key: 'draw',
    value: function draw() {
      var ctx = this.ctx,
        gradient = ctx.createRadialGradient(100, 100, 60, 200, 200, 600);
      //月亮
      // gradient.addColorStop(0, 'rgba(255,255,255,0.5)')
      // gradient.addColorStop(0.01, 'rgb(70,70,80)')
      // gradient.addColorStop(0.2, 'rgb(40,40,50)')
      gradient.addColorStop(0.4, 'rgb(20,20,30)');
      gradient.addColorStop(1, 'rgb(0,0,10)');
      ctx.save();
      ctx.fillStyle = gradient;
      ctx.fillRect(0, 0, this.width, this.height);
      ctx.restore();
    }
  }]);

  return Moon;
}();

exports.default = Moon;