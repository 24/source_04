var gIdCanvas = 1;
var gImage = null;

//onload="draw();"
window.onload = go;

function go() {
  gImage = new Image();
  gImage.onload = draw;
  gImage.src = "img/gnomangelo.png";
}

function draw() {
  logHtml("test canvas 09<br>");
  // drawImage : srcX 1215.2307692307693 srcY 0 srcW 202.53846153846155 srcH 315 dstX 0 dstY 273.88833307923466 dstW 160.09501187648456 dstH 248.9893937083951
  // 14pt Comic Sans MS
//  drawJS(
//   '\
//    context.font = "10pt sans-serif";\n\
//    context.fillText("abcdef", 20, 20);\n\
//    ', 50, 50);
  drawJS(
   '\
    context.drawImage(gImage, 1215.2307692307693, 0, 202.53846153846155, 315, 1, 1, 160.09501187648456, 248.9893937083951);\n\
    ', 170, 260);
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
      <div class="cell"><canvas id="$idCanvas1" class="canvas" width="$canvasWidth" height="$canvasHeight"></canvas></div>\n\
      <div class="cell"><canvas id="$idCanvas2" class="canvas" width="$zoomCanvasWidth" height="$zoomCanvasHeight"></canvas></div>\n\
    </div>\n\
    <div class="row">&nbsp;</div>\n';
  var js_txt = js.replace('\n', '<br>\n', 'g');
  s = s.replace('$javascript', js_txt);
  var canvas = 'canvas' + gIdCanvas++;
  var zoomCanvas = 'canvas' + gIdCanvas++;
  var zoomCanvasWidth = (width * zoom_x) + 60;
  var zoomCanvasHeight = (height * zoom_y) + 40;
  s = s.replace('$idCanvas1', canvas);
  s = s.replace('$idCanvas2', zoomCanvas);
  s = s.replace('$canvasWidth', width);
  s = s.replace('$canvasHeight', height);
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
  zoomImage(context, 30, 20, zoom_x, zoom_y, imageData);
  drawGrid(context, 30, 20, zoom_x, zoom_y, imageData.width, imageData.height);
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
