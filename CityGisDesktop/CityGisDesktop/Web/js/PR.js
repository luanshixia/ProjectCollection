
(function () {
    if (!String.format) {
        String.format = function (format) {
            var args = Array.prototype.slice.call(arguments, 1);
            return format.replace(/{(\d+)}/g, function (match, number) {
                return typeof args[number] != 'undefined'
                ? args[number]
                : match;
            });
        };
    }
    if (!String.prototype.format) {
        String.prototype.format = function () {
            var args = arguments;
            return this.replace(/{(\d+)}/g, function (match, number) {
                return typeof args[number] != 'undefined'
                ? args[number]
                : match;
            });
        };
    }
})();

(function(global) {
    function ArrayList() {
        var args = [].slice.call(arguments), 
            length = args.length;

        this.length = length;

        for (var i = 0; i < length; i++) {
            this[i] = args[i];
        }
        return this;
    }

    ArrayList.prototype = new Array();

    ArrayList.prototype.add = function (item) {
        this.push(item);
    };

    ArrayList.prototype.clear = function () {
        this.length = 0;
    };

    ArrayList.prototype.removeAt = function (i) {
        this.splice(i, 1);
    };

    ArrayList.prototype.remove = function (item) {
        var i = this.indexOf(item);
        this.splice(i, 1);
    };

    ArrayList.prototype.size = function () {
        return this.length;
    };

    ArrayList.prototype.get = function (i) {
        return this[i];
    };

    ArrayList.prototype.set = function (i, item) {
        this[i] = item;
    };

    global.ArrayList = ArrayList;
})(this);

var PVector = (function () {
    
    function PVector(x, y, z) {
        if (arguments.length < 3) {
            z = 0;
            if (arguments.length < 2) {
                y = 0;
                if (arguments.length < 1) {
                    x = 0;
                }
            }
        }
        this.set(x, y, z);
    }

    PVector.prototype.set = function (x, y, z) {
        if (arguments.length < 3) {
            z = 0;
        }
        this.x = x;
        this.y = y;
        this.z = z;
    };

    PVector.prototype.get = function () {
        return new PVector(this.x, this.y, this.z);
    };

    PVector.prototype.mag = function () {
        return Math.sqrt(this.magSq());
    };

    PVector.prototype.magSq = function () {
        return this.x * this.x + this.y * this.y + this.z * this.z;
    };

    PVector.prototype.add = function (v) {
        return new PVector(this.x + v.x, this.y + v.y, this.z + v.z);
    };

    PVector.prototype.sub = function (v) {
        return new PVector(this.x - v.x, this.y - v.y, this.z - v.z);
    };

    PVector.prototype.mult = function (n) {
        return new PVector(this.x * n, this.y * n, this.z * n);
    };

    PVector.prototype.div = function (n) {
        return new PVector(this.x / n, this.y / n, this.z / n);
    };

    PVector.prototype.dist = function (v) {
        return this.sub(v).mag();
    };

    PVector.prototype.dot = function (v) {
        return this.x * v.x + this.y * v.y + this.z * v.z;
    };

    PVector.prototype.cross = function (v) {
        var x = this.y * v.z - this.z * v.y;
        var y = this.z * v.x - this.x * v.z;
        var z = this.x * v.y - this.y * v.x;
        return new PVector(x, y, z);
    };

    PVector.prototype.normalize = function () {
        return this.div(this.mag());
    };

    PVector.prototype.limit = function (max) {
        return this.mag() > max ? this.setMag(max) : this.get();
    };

    PVector.prototype.setMag = function (len) {
        return this.normalize().mult(len);
    };

    PVector.prototype.heading = function () {
        return Math.atan2(this.y, this.x);
    };

    PVector.prototype.rotate = function (theta) {
        // this is 2D only.
        var x = this.x * Math.cos(theta) + this.y * Math.sin(theta);
        var y = -this.x * Math.sin(theta) + this.y * Math.cos(theta);
        var z = this.z;
        return new PVector(x, y, z);
    };

    PVector.prototype.lerp = function (v, amt) {
        return this.add(v.sub(this).mult(amt));
    };

    PVector.prototype.array = function () {
        return [this.x, this.y, this.z];
    };

    PVector.random2D = function () {
        var theta = PR.random(0, Math.PI * 2);
        return PVector.fromAngle(theta);
    };

    PVector.random3D = function () {
        var theta = PR.random(0, Math.PI * 2);
        var phi = PR.random(-Math.PI/2, Math.PI/2);
        var unit = new PVector(1, 0);
        var xy = unit.rotate(theta);
        var z = unit.rotate(phi);
        return new PVector(xy.x, xy.y, z.y);
    };

    PVector.fromAngle = function (angle) {
        var xUnit = new PVector(1, 0);
        return xUnit.rotate(angle);
    };

    PVector.angleBetween = function (v1, v2) {
        return Math.acos(v1.dot(v2) / (v1.mag() * v2.mag()));
    };

    return PVector;
})();

var PColor = (function () {

    function PColor(r, g, b, a) {
        this.red = r;
        this.green = g;
        this.blue = b;
        this.alpha = a;
    }

    return PColor;
})();

var PImage = (function () {

    function PImage(width, height, pixels) {
        this.imageData = null;
        this.width = width;
        this.height = height;
        this.pixels = pixels;
    }

    PImage.prototype.loadPixels = function () {

    };

    PImage.prototype.updatePixels = function () {

    };

    PImage.prototype.resize = function (w, h) {

    };

    PImage.prototype.get = function (x, y, w, h) {

    };

    PImage.prototype.set = function (x, y, c) {

    };

    PImage.prototype.mask = function (img) {

    };

    PImage.prototype.filter = function () {

    };

    PImage.prototype.copy = function () {

    };

    PImage.prototype.blend = function () {

    };

    PImage.prototype.save = function () {

    };

    return PImage;
})();

var PR = {
    context: null,
    frameRate: 30,
    frameCount: 0,
    mouseX: 0,
    mouseY: 0,
    pmouseX: 0,
    pmouseY: 0,
    key: null,
    _translateX: 0,
    _translateY: 0,
    _rotateAngle: 0,
    _scaleX: 1,
    _scaleY: 1,
    _vertices: [],
    size: function (w, h) {
        this.context.width = w;
        this.context.height = h;
    },
    noStroke: function () {
        this.context.strokeStyle = "rgba(0,0,0,0)";
    },
    noFill: function () {
        this.context.fillStyle = "rgba(0,0,0,0)";
    },
    smooth: function () {
    },
    radians: function (angle) {
        return angle / 180 * Math.PI;
    },
    random: function (min, max) {
        var rand = Math.random();
        return min + rand * (max - min);
    },
    background: function (r, g, b) {
        if (arguments.length === 1) {
            g = r; b = r;
        }
        this.translate(0, 0);
        this.rotate(0);
        this.scale(1, 1);

        this.context.save();
        this.context.fillStyle = this.Utils.rgb(r, g, b);
        this.context.fillRect(0, 0, this.context.width, this.context.height);
        this.context.restore();
    },
    fill: function (r, g, b, a) {
        if (arguments.length === 3) {
            a = 1;
        } else if (arguments.length === 2) {
            a = g; g = r; b = r;
        } else if (arguments.length === 1) {
            a = 1; g = r; b = r;
        }
        this.context.fillStyle = this.Utils.rgba(r, g, b, a);
    },
    stroke: function (r, g, b, a) {
        if (arguments.length === 3) {
            a = 1;
        } else if (arguments.length === 2) {
            a = g; g = r; b = r;
        } else if (arguments.length === 1) {
            a = 1; g = r; b = r;
        }
        this.context.strokeStyle = this.Utils.rgba(r, g, b, a);
    },
    strokeWeight: function (w) {
        this.context.lineWidth = w;
    },
    circle: function (x, y, r) {
        this.context.beginPath();
        this.context.arc(x, y, r, 0, 2 * Math.PI, false);
        this.context.closePath();
        this.context.fill();
        this.context.stroke();
    },
    rect: function (x, y, w, h) {
        this.context.fillRect(x, y, w, h);
        this.context.strokeRect(x, y, w, h);
    },
    line: function (x1, y1, x2, y2) {
        this.context.beginPath();
        this.context.moveTo(x1, y1);
        this.context.lineTo(x2, y2);
        this.context.stroke();
    },
    lines: function (vs, close) {
        this.context.beginPath();
        this.context.moveTo(vs[0].x, vs[0].y);
        for (var i = 1; i < vs.length; i++) {
            this.context.lineTo(vs[i].x, vs[i].y);
        }
        close && this.context.closePath();
        this.context.fill();
        this.context.stroke();
    },
    text: function (str, x, y) {
        this.context.fillText(str, x, y);
        this.context.strokeText(str, x, y);
    },
    textFont: function (which, size) {
        this.context.font = String.format("{0}px {1}", size, which);
    },
    translate: function (x, y) {
        this.context.translate(x - this._translateX, y - this._translateY);
        this._translateX = x;
        this._translateY = y;
    },
    rotate: function (angle) {
        this.context.rotate(angle - this._rotateAngle);
        this._rotateAngle = angle;
    },
    scale: function (x, y) {
        this.context.scale(x / this._scaleX, y / this._scaleY);
        this._scaleX = x;
        this._scaleY = y;
    },
    beginShape: function () {
        this._vertices = [];
    },
    vertex: function (x, y) {
        this._vertices.push(new PVector(x, y));
    },
    endShape: function () {
        this.lines(this._vertices, true);
    },
    loadImage: function (filename) {

    },
    image: function (img, x, y, w, h) {

    },
    get: function(x, y, w, h) {

    },
    set: function(x, y, c) {

    },
    launch: function (canvasId, proj) {
        var canvas = document.getElementById(canvasId);
        this.context = canvas.getContext("2d");
        var self = this;

        // register
        canvas.addEventListener("mousemove", function (e) {
            self.pmouseX = self.mouseX;
            self.pmouseY = self.mouseY;
            if (e.offsetX) {
                self.mouseX = e.offsetX;
                self.mouseY = e.offsetY;
            } else if (e.layerX) {
                self.mouseX = e.layerX;
                self.mouseY = e.layerY;
            }
        });
        canvas.addEventListener("keypress", function (e) {
            self.key = e.keyCode;
        });
        proj.mousePressed && canvas.addEventListener("mousedown", proj.mousePressed);
        proj.mouseReleased && canvas.addEventListener("mouseup", proj.mouseReleased);
        proj.keyPressed && canvas.addEventListener("keypress", proj.keyPressed);

        // loop
        proj.setup();
        var interval = 1000 / this.frameRate;
        function render() {
            proj.draw();
            this.frameCount++;
            setTimeout(render, interval);
        };
        render();
    },
    draw: function (canvasId, func) {
        var canvas = document.getElementById(canvasId);
        this.context = canvas.getContext("2d");
        var self = this;
        func();
    },
    Utils: {
        _hex: function (c) {
            var hex = c.toString(16);
            return hex.length == 1 ? "0" + hex : hex;
        },

        rgbaToHex: function (r, g, b, a) {
            if (a > 0 && a <= 1) {
                a = parseInt((a * 255).toFixed(0));
            }
            return "#" + this._hex(a) + this._hex(r) + this._hex(g) + this._hex(b);
        },

        rgbToHex: function (r, g, b) {
            return "#" + this._hex(r) + this._hex(g) + this._hex(b);
        },

        rgba: function (r, g, b, a) {
            if (a > 1) {
                a = a / 255;
            }
            return String.format("rgba({0},{1},{2},{3})", r, g, b, a);
        },

        rgb: function (r, g, b) {
            return String.format("rgb({0},{1},{2})", r, g, b);
        }
    }
};