var gIdCanvas = 1;

function draw() {
  drawJS(
   '\
    context.strokeStyle = "red";\n\
    context.lineWidth = 1;\n\
    context.strokeRect(1, 1, 4, 4);\n\
    ', 10, 10);
  drawJS(
   '\
    context.strokeStyle = "red";\n\
    context.lineWidth = 1;\n\
    context.strokeRect(1.5, 1.5, 4, 4);\n\
    ', 10, 10);
  drawJS(
   '\
    context.fillStyle = "red";\n\
    context.fillRect(1, 1, 4, 4);\n\
    ', 10, 10);
  drawJS(
   '\
    context.fillStyle = "red";\n\
    context.fillRect(1.5, 1.5, 4, 4);\n\
    ', 10, 10);
  drawJS(
   '\
    context.strokeStyle = "red";\n\
    context.lineWidth = 1;\n\
    context.pixelStrokeRect(1, 1, 4, 4);\n\
    // ==> context.strokeRect(1 + 0.5, 1 + 0.5, 4 - 1, 4 - 1);\n\
    ', 10, 10);
  drawJS(
   '\
    context.strokeStyle = "red";\n\
    context.lineWidth = 2;\n\
    context.strokeRect(2, 2, 5, 5);\n\
    ', 10, 10);
  drawJS(
   '\
    context.strokeStyle = "red";\n\
    context.lineWidth = 2;\n\
    context.pixelStrokeRect(1, 1, 7, 7);\n\
    // ==> context.strokeRect(2, 2, 5, 5);\n\
    ', 10, 10);
  drawJS(
   '\
    context.strokeStyle = "red";\n\
    context.lineWidth = 3;\n\
    context.strokeRect(2.5, 2.5, 7, 7);\n\
    ', 12, 12);
  drawJS(
   '\
    context.strokeStyle = "red";\n\
    context.lineWidth = 3;\n\
    context.pixelStrokeRect(1, 1, 10, 10);\n\
    // ==> context.strokeRect(2.5, 2.5, 7, 7);\n\
    ', 12, 12);
  drawJS(
   '\
    context.lineWidth = 1;\n\
    context.strokeStyle = "red";\n\
    context.strokeRect(1.5, 1.5, 4, 4);\n\
    context.strokeRect(5.5, 1.5, 4, 4);\n\
    context.strokeRect(1.5, 5.5, 4, 4);\n\
    context.strokeRect(5.5, 5.5, 4, 4);\n\
    ', 12, 12);
//  drawJS(
//   '\n\
//    ', 8, 8);
}

function drawJS(js, width, height) {
  var zoom_x = 20;
  var zoom_y = 20;
  var data = document.getElementById("data");
  var s = '\n\
   <div class="row"><div class="cell">javascript</div><div class="cell">draw</div><div class="cell">zoom</div></div>\n\
    <div class="row">\n\
      <div class="cell">\n\
        $javascript\n\
      </div>\n\
      <div class="cell"><canvas id="$idCanvas1" class="canvas" width="100" height="100"></canvas></div>\n\
      <div class="cell"><canvas id="$idCanvas2" class="canvas" width="$zoomCanvasWidth" height="$zoomCanvasHeight"></canvas></div>\n\
    </div>\n\
    <div class="row">&nbsp;</div>\n';
  //var js_txt = js.replace('\n', '<br>\n', 'g');
  var js_txt = js.replace(/\n/g, '<br>\n');
  s = s.replace('$javascript', js_txt);
  var canvas = 'canvas' + gIdCanvas++;
  var zoomCanvas = 'canvas' + gIdCanvas++;
  var zoomCanvasWidth = (width * zoom_x) + 40;
  var zoomCanvasHeight = (height * zoom_y) + 40;
  s = s.replace('$idCanvas1', canvas);
  s = s.replace('$idCanvas2', zoomCanvas);
  s = s.replace('$zoomCanvasWidth', zoomCanvasWidth);
  s = s.replace('$zoomCanvasHeight', zoomCanvasHeight);
  //console.log(s);
  //console.log(js);
  //console.log(js_txt);
  data.insertAdjacentHTML('beforeend', s);
  
  var context = initCanvas(canvas);
  eval(js);
  //drawZoom(zoomCanvas, context.getImageData(0, 0, width, height), zoom_x, zoom_y);
  var imageData = context.getImageData(0, 0, width, height);
  context = initCanvas(zoomCanvas);
  zoomImage(context, 20, 20, zoom_x, zoom_y, imageData);
  drawGrid(context, 20, 20, zoom_x, zoom_y, imageData.width, imageData.height);
}

//function drawZoom(zoomCanvas, imageData, zoom_x, zoom_y) {
//  var context = initCanvas(zoomCanvas);
////  var zoom_x = 20;
////  var zoom_y = 20;
//  zoomImage(context, 10.5, 10.5, zoom_x, zoom_y, imageData);
//  drawGrid(context, 10.5, 10.5, zoom_x, zoom_y, imageData.width, imageData.height);
//}

function initCanvas(canvasId) {
  var canvas = document.getElementById(canvasId);
  var context = canvas.getContext("2d");
  context.clear();
  return context;
}
