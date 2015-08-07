function draw() {
  //test();
  draw01("canvas01", "canvas02");
  draw02("canvas03", "canvas04");
  draw03("canvas05", "canvas06");
  draw04("canvas07", "canvas08");
  draw05("canvas09", "canvas10");
  draw06("canvas11", "canvas12");
  draw10("canvas19", "canvas20");
}

function test() {
  var context = initCanvas("canvas01");
  console.log("lineWidth : " + context.lineWidth);
  context.lineWidth = 1;
  console.log("lineWidth : " + context.lineWidth);
  context.lineWidth = 1.5;
  console.log("lineWidth : " + context.lineWidth);
  context.lineWidth = 2;
  console.log("lineWidth : " + context.lineWidth);
  context.lineWidth = 2.5;
  console.log("lineWidth : " + context.lineWidth);
}

function draw01(canvas, zoomCanvas) {
  var context = initCanvas(canvas);
  context.lineWidth = 1;
  context.strokeStyle = "red";
  context.strokeRect(1, 1, 4, 4);
  drawZoom(zoomCanvas, context.getImageData(0, 0, 8, 8));
}

function draw02(canvas, zoomCanvas) {
  var context = initCanvas(canvas);
  context.lineWidth = 1;
  context.strokeStyle = "red";
  context.strokeRect(1.5, 1.5, 4, 4);
  drawZoom(zoomCanvas, context.getImageData(0, 0, 8, 8));
}

function draw03(canvas, zoomCanvas) {
  var context = initCanvas(canvas);
  context.fillStyle = "red";
  context.fillRect(1, 1, 4, 4);
  drawZoom(zoomCanvas, context.getImageData(0, 0, 8, 8));
}

function draw04(canvas, zoomCanvas) {
  var context = initCanvas(canvas);
  context.fillStyle = "red";
  context.fillRect(1.5, 1.5, 4, 4);
  drawZoom(zoomCanvas, context.getImageData(0, 0, 8, 8));
}

function draw05(canvas, zoomCanvas) {
  var context = initCanvas(canvas);
  context.lineWidth = 1;
  context.strokeStyle = "red";
  context.pixelStrokeRect(1, 1, 4, 4);
  drawZoom(zoomCanvas, context.getImageData(0, 0, 8, 8));
}

function draw06(canvas, zoomCanvas) {
  var context = initCanvas(canvas);
  context.strokeStyle = "red";
  context.lineWidth = 2;
  context.strokeRect(2, 2, 5, 5);
  drawZoom(zoomCanvas, context.getImageData(0, 0, 14, 14));
}

function draw10(canvas, zoomCanvas) {
  var context = initCanvas(canvas);
  context.lineWidth = 1;
  context.strokeStyle = "red";
  context.strokeRect(1.5, 1.5, 4, 4);
  context.strokeRect(5.5, 1.5, 4, 4);
  context.strokeRect(1.5, 5.5, 4, 4);
  context.strokeRect(5.5, 5.5, 4, 4);
  drawZoom(zoomCanvas, context.getImageData(0, 0, 12, 12));
}

function drawZoom(zoomCanvas, imageData) {
  var context = initCanvas(zoomCanvas);
  var zoom_x = 20;
  var zoom_y = 20;
  zoomImage(context, 10.5, 10.5, zoom_x, zoom_y, imageData);
  drawGrid(context, 10.5, 10.5, zoom_x, zoom_y, imageData.width, imageData.height);
}

function initCanvas(canvasId) {
  var canvas = document.getElementById(canvasId);
  var context = canvas.getContext("2d");
  context.clear();
  return context;
}
