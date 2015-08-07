var gIdCanvas = 1;

function draw() {
//  drawJS(
//   '\
//    context.strokeStyle = "red";\n\
//    context.lineWidth = 1;\n\
//    context.strokeRect(1, 1, 4, 4);\n\
//    ', 8, 8);
  drawJS(
   '\
    context.fillStyle = "red";\n\
    context.fillRect(1, 1, 1, 1);\n\
    ', 3, 3, false);
  drawJS(
   '\
    context.fillStyle = "red";\n\
    context.fillRect(1, 1, 1, 1);\n\
    ', 3, 3, true);

}

function drawJS(js, width, height, grid) {
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
      <div class="cell"><canvas id="$idCanvas3" class="canvas" width="$zoomZoomCanvasWidth" height="$zoomZoomCanvasHeight"></canvas></div>\n\
    </div>\n\
    <div class="row">&nbsp;</div>\n';
  var js_txt = js.replace('\n', '<br>\n', 'g');
  s = s.replace('$javascript', js_txt);
  var canvas = 'canvas' + gIdCanvas++;
  var zoomCanvas = 'canvas' + gIdCanvas++;
  var zoomZoomCanvas = 'canvas' + gIdCanvas++;
  var zoomCanvasWidth = (width * zoom_x) + 20;    // 300
  var zoomCanvasHeight = (height * zoom_y) + 20;  // 200
  var zoomZoomCanvasWidth = (zoomCanvasWidth * zoom_x) + 20;    // 300
  var zoomZoomCanvasHeight = (zoomCanvasHeight * zoom_y) + 20;  // 200
  s = s.replace('$idCanvas1', canvas);
  s = s.replace('$idCanvas2', zoomCanvas);
  s = s.replace('$idCanvas3', zoomZoomCanvas);
  s = s.replace('$zoomCanvasWidth', zoomCanvasWidth);
  s = s.replace('$zoomCanvasHeight', zoomCanvasHeight);
  s = s.replace('$zoomZoomCanvasWidth', zoomZoomCanvasWidth);
  s = s.replace('$zoomZoomCanvasHeight', zoomZoomCanvasHeight);
  //console.log(s);
  //console.log(js);
  //console.log(js_txt);
  data.insertAdjacentHTML('beforeend', s);
  
  var context = initCanvas(canvas);
  eval(js);
  //drawZoom(zoomCanvas, context.getImageData(0, 0, width, height), zoom_x, zoom_y);
  var imageData = context.getImageData(0, 0, width, height);
  context = initCanvas(zoomCanvas);
  zoomImage(context, 2, 2, zoom_x, zoom_y, imageData);
//  console.log("*************************************************************************************************");
//  console.log("drawGrid 1 **************************************************************************************");
//  console.log("*************************************************************************************************");
  if (grid)
    drawGrid(context, 2, 2, zoom_x, zoom_y, imageData.width, imageData.height);
  
  imageData = context.getImageData(0, 0, zoomCanvasWidth, zoomCanvasHeight);
  context = initCanvas(zoomZoomCanvas);
  zoomImage(context, 2, 2, zoom_x, zoom_y, imageData);
//  console.log("*************************************************************************************************");
//  console.log("drawGrid 2 **************************************************************************************");
//  console.log("*************************************************************************************************");
  drawGrid(context, 2, 2, zoom_x, zoom_y, imageData.width, imageData.height);
}

function initCanvas(canvasId) {
  var canvas = document.getElementById(canvasId);
  var context = canvas.getContext("2d");
  context.clear();
  return context;
}
