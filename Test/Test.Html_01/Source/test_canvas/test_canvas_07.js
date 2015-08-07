function draw() {
  //draw01();
  draw02();
}

var run = true;

function startStop() {
  run = !run;
}

function draw02() {
  var canvas = $('#canvas')[0];
  var context = canvas.getContext('2d');
  var x = 1;
  var y = 1;
  var w = 10;
  var h = 10;
  var dx = 5;
  var dy = 5;
  var xmin = 1;
  var ymin = 1;
  var xmax = canvas.width - w - 1;
  var ymax = canvas.height - h - 1;
  var xmove = dx;
  var ymove = 0;
  var i = 0;
  context.fillStyle = "red";
  context.fillRect(x, y, w, h);
  function animate() {
    if (!run) return;
    if (xmove === dx) {
      if (x >= xmax) {
        xmove = 0;
        ymove = dy;
      }
    }
    else if (ymove === dy) {
      if (y >= ymax) {
        xmove = -dx;
        ymove = 0;
      }
    }
    else if (xmove === -dx) {
      if (x <= xmin) {
        xmove = 0;
        ymove = -dy;
      }
    }
    else if (ymove === -dy) {
      if (y <= ymin) {
        xmove = dx;
        ymove = 0;
      }
    }
    context.clearRect(x, y, w, h);
    x += xmove;
    y += ymove;
    context.fillRect(x, y, w, h);
    if (++i > 100000) clearInterval(interval);
  }
  var interval = setInterval(animate, 10);
}

function draw01() {
  var canvas = $('#canvas')[0];
  var context = canvas.getContext('2d');
  context.lineWidth = 2;
  context.fillStyle = "rgba(206,0,0,255)";
  // Positionnement au centre
  context.translate(canvas.width / 2, canvas.height / 2);
  var i = 0;
  function dessiner() {
    context.translate(4, 1);
    context.rotate(0.2);
    context.fillRect(i, 0, 20, 20);
    i++;
    if (i > 400) clearInterval(inter);
    context.fillStyle = "rgba(206,0," + i + ",255)";
  }
  var inter = setInterval(dessiner, 10);
}
