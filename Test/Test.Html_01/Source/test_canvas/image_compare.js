var gimgcomp = new ImageCompare();

window.onload = function () {
  paramsToValues();
  gimgcomp.init();
  gimgcomp.readVars();
  gimgcomp.image1.onload = function () {
    if (gimgcomp.image2.imageLoaded)
      gimgcomp.viewImageDiff();
    var colors = getImageColors(gimgcomp.image1.imageData);
    printImageColors($("#colors1")[0], colors);
  };
  gimgcomp.image2.onload = function () {
    if (gimgcomp.image1.imageLoaded)
      gimgcomp.viewImageDiff();
  };
  gimgcomp.image1.load(gimgcomp.zoom_x, gimgcomp.zoom_y);
  gimgcomp.image2.load(gimgcomp.zoom_x, gimgcomp.zoom_y);
};

function paramsToValues() {
  prm = getUrlParams();
  if (prm['file1'] !== undefined)
    //document.getElementById("file1").value = prm['file1'];
    $("#file1").val(prm['file1']);
  if (prm['dir1'] !== undefined)
    //document.getElementById("directory1").value = prm['dir1'];
    $("#directory1").val(prm['dir1']);
  if (prm['file2'] !== undefined)
    //document.getElementById("file2").value = prm['file2'];
    $("#file2").val(prm['file2']);
  if (prm['dir2'] !== undefined)
    //document.getElementById("directory2").value = prm['dir2'];
    $("#directory2").val(prm['dir2']);
  if (prm['zoom_x'] !== undefined)
    //document.getElementById("zoom_x").value = prm['zoom_x'];
    $("#zoom_x").val(prm['zoom_x']);
  if (prm['zoom_y'] !== undefined)
    //document.getElementById("zoom_y").value = prm['zoom_y'];
    $("#zoom_y").val(prm['zoom_y']);
}

function DrawImage() {
  this.imageCompare = null;
  this.file = null;
  this.canvas = null;
  this.context = null;
  this.image = null;
  this.imageLoaded = false;
  this.imageData = null;
  this.onload = null;
  this.imageX = null;
  this.imageY = null;
  this.zoom_x = 1;
  this.zoom_y = 1;
  this.zoomImageX = null;
  this.zoomImageY = null;
  this.pixelZoomImageX = null;
  this.pixelZoomImageY = null;
  this.pixelZoomImageColor = null;
}

DrawImage.prototype.init = function (canvasId) {
  //this.canvas = document.getElementById(canvasId);
  this.canvas = $("#" + canvasId)[0];
  this.context = this.canvas.getContext("2d");
  //this.canvas.onmousemove = createDelegate(this, this.mousemove);
};

DrawImage.prototype.mousemove = function (e) {
  //document.getElementById("object").value = this.file;
  $("#object").val(this.file);
  var offsetX = e.getOffsetX();
  var offsetY = e.getOffsetY();
  this.getPixelZoomXY(offsetX, offsetY);
  this.getPixelColor(this.pixelZoomImageX, this.pixelZoomImageY);
  //document.getElementById("offsetX").value = offsetX;
  //document.getElementById("offsetY").value = offsetY;
  $("#offsetX").val(offsetX);
  $("#offsetY").val(offsetY);
  $("#pixelX").val(this.pixelZoomImageX);
  $("#pixelY").val(this.pixelZoomImageY);
  $("#color1text").val(this.pixelZoomImageColor);
  //$("#color1input").val(this.pixelZoomImageColor);
  $("#color1div")[0].style.backgroundColor = this.pixelZoomImageColor;
};

DrawImage.prototype.getPixelZoomXY = function (offsetX, offsetY) {
  this.pixelZoomImageX = Math.floor((offsetX - this.zoomImageX) / this.zoom_x);
  this.pixelZoomImageY = Math.floor((offsetY - this.zoomImageY) / this.zoom_y);
};

DrawImage.prototype.getPixelColor = function (x, y) {
  var i = (x + (y * this.image.width)) * 4;
  var r = this.imageData.data[i];
  var g = this.imageData.data[i + 1];
  var b = this.imageData.data[i + 2];
  var a = this.imageData.data[i + 3];
  //this.pixelZoomImageColor = "rgba(" + r + ", " + g + ", " + b + ", " + a + ")";
  this.pixelZoomImageColor = "#" + toHex(r, 2) + toHex(g, 2) + toHex(b, 2);
  return this.pixelZoomImageColor;
};

DrawImage.prototype.load = function (zoom_x, zoom_y) {
  if (this.file !== null) {
    this.image = new Image();
    this.image.drawImage = this;
    this.image.onload = function () {
        this.drawImage.draw(zoom_x, zoom_y);
    };
    this.image.src = this.file;
  }
};

DrawImage.prototype.draw = function (zoom_x, zoom_y) {
  this.context.clear();
  this.imageLoaded = true;
  this.canvas.width = this.image.width * zoom_x + 20;
  this.canvas.height = this.image.height * (zoom_y + 1) + 30;
  this.imageX = 10;
  this.imageY = 10;
  this.zoomImageX = this.imageX;
  this.zoomImageY = (this.imageY * 2) + this.image.height;
  this.context.drawImage(this.image, this.imageX, this.imageY, this.image.width, this.image.height);
  this.imageData = this.context.getImageData(this.imageX, this.imageY, this.image.width, this.image.height);
  this.drawZoom(zoom_x, zoom_y);
  this.onload();
};

DrawImage.prototype.drawZoom = function (zoom_x, zoom_y) {
  zoomImage(this.context, this.zoomImageX, this.zoomImageY, zoom_x, zoom_y, this.imageData);
  drawGrid(this.context, this.zoomImageX, this.zoomImageY, zoom_x, zoom_y, this.image.width, this.image.height);
};

function ImageCompare() {
  this.image1 = new DrawImage();
  this.image1.imageCompare = this;
  this.image2 = new DrawImage();
  this.image2.imageCompare = this;
  this.imageDiff = null;
  this.zoom_x = 1;
  this.zoom_y = 1;
}

ImageCompare.prototype.init = function () {
//  this.image1.canvas = document.getElementById("canvas1");
//  this.image1.context = this.image1.canvas.getContext("2d");
//  this.image2.canvas = document.getElementById("canvas2");
//  this.image2.context = this.image2.canvas.getContext("2d");
//  this.image1.canvas.onmousemove = createDelegate(this.image1, this.image1.mousemove);
//  this.image2.canvas.onmousemove = createDelegate(this.image2, this.image2.mousemove);
  this.image1.init("canvas1");
  this.image2.init("canvas2");
  this.image1.canvas.onmousemove = createDelegate(this, this.mousemove);
  this.image2.canvas.onmousemove = createDelegate(this, this.mousemove);
};

ImageCompare.prototype.mousemove = function (e) {
  var image, image_no;
  if (e.target === this.image1.canvas) {
    image = this.image1;
    image_no = 1;
  } else if (e.target === this.image2.canvas) {
    image = this.image2;
    image_no = 2;
  } else
    return;
  $("#object").val(image.file);
  var offsetX = e.getOffsetX();
  var offsetY = e.getOffsetY();
  image.getPixelZoomXY(offsetX, offsetY);
  //image.getPixelColor(this.pixelZoomImageX, this.pixelZoomImageY);
  var color1 = this.image1.getPixelColor(image.pixelZoomImageX, image.pixelZoomImageY);
  var color2 = this.image2.getPixelColor(image.pixelZoomImageX, image.pixelZoomImageY);
  //document.getElementById("offsetX").value = offsetX;
  //document.getElementById("offsetY").value = offsetY;
  $("#offsetX").val(offsetX);
  $("#offsetY").val(offsetY);
  $("#pixelX").val(image.pixelZoomImageX);
  $("#pixelY").val(image.pixelZoomImageY);
  $("#color1text").val(color1);
  //$("#color1input").val(this.pixelZoomImageColor);
  $("#color1div")[0].style.backgroundColor = color1;
  $("#color2text").val(color2);
  //$("#color1input").val(this.pixelZoomImageColor);
  $("#color2div")[0].style.backgroundColor = color2;
};

ImageCompare.prototype.readVars = function () {
  this.image1.file = getFile(1);
  this.image2.file = getFile(2);
//  this.zoom_x = Math.floor(document.getElementById("zoom_x").value / 100);
//  this.zoom_y = Math.floor(document.getElementById("zoom_y").value / 100);
  this.zoom_x = Math.floor($("#zoom_x").val() / 100);
  this.zoom_y = Math.floor($("#zoom_y").val() / 100);
  this.image1.zoom_x = this.zoom_x;
  this.image1.zoom_y = this.zoom_y;
  this.image2.zoom_x = this.zoom_x;
  this.image2.zoom_y = this.zoom_y;
};

ImageCompare.prototype.viewImageDiff = function () {
    this.imageDiff = getImageDiff(this.image1.imageData, this.image2.imageData);
    drawDiff(this.imageDiff, this.image1.context, 10, this.image1.image.height + 20, this.zoom_x, this.zoom_y);
    drawDiff(this.imageDiff, this.image2.context, 10, this.image2.image.height + 20, this.zoom_x, this.zoom_y);
};

/*********************************************************************************************
//function drawCanvas() {
//    var canvas1 = document.getElementById("canvas1");
//    var canvas2 = document.getElementById("canvas2");
//    var context1 = canvas1.getContext("2d");
//    var context2 = canvas2.getContext("2d");
//    context1.clear();
//    context2.clear();
//    var file1 = getFile(1);
//    var file2 = getFile(2);
//    var image1Loaded = false;
//    var image2Loaded = false;
//    var zoom_x = Math.floor(document.getElementById("zoom_x").value / 100);
//    var zoom_y = Math.floor(document.getElementById("zoom_y").value / 100);
//    var image1 = null;
//    var imageData1 = null;
//    //var image1Width, image1Height;
//    var image1Height, image2Height;
//    if (file1 !== "") {
//        image1 = new Image();
//        image1.onload = function(){
//            image1Loaded = true;
//            //image1Width = this.width;
//            image1Height = this.height;
//            canvas1.width = this.width * zoom_x + 20;
//            canvas1.height = image1Height * (zoom_y + 1) + 30;
//            context1.drawImage(this, 10, 10, this.width, image1Height);
//            imageData1 = context1.getImageData(10, 10, this.width, image1Height);
//            zoomImage(context1, 10, image1Height + 20, zoom_x, zoom_y, imageData1);
//            drawGrid(context1, 10, image1Height + 20, zoom_x, zoom_y, this.width, image1Height);
//            if (image2Loaded) {
//              var diff = getImageDiff(imageData1, imageData2);
//              drawDiff(diff, context1, 10, image1Height + 20, zoom_x, zoom_y);
//              drawDiff(diff, context2, 10, image2Height + 20, zoom_x, zoom_y);
//            }
//        };
//        image1.src = file1;
//    }
//    var image2 = null;
//    var imageData2 = null;
//    //var image2Width, image2Height;
//    if (file2 !== "") {
//        image2 = new Image();
//        image2.onload = function(){
//            image2Loaded = true;
//            //image2Width = this.width;
//            image2Height = this.height;
//            canvas2.width = this.width * zoom_x + 20;
//            canvas2.height = image2Height * (zoom_y + 1) + 30;
//            context2.drawImage(this, 10, 10, this.width, image2Height);
//            imageData2 = context2.getImageData(10, 10, this.width, image2Height);
//            zoomImage(context2, 10, image2Height + 20, zoom_x, zoom_y, imageData2);
//            drawGrid(context2, 10, image2Height + 20, zoom_x, zoom_y, this.width, image2Height);
//            if (image1Loaded) {
//              var diff = getImageDiff(imageData1, imageData2);
//              drawDiff(diff, context1, 10, image1Height + 20, zoom_x, zoom_y);
//              drawDiff(diff, context2, 10, image2Height + 20, zoom_x, zoom_y);
//            }
//        };
//        image2.src = file2;
//    }
//}
*********************************************************************************************/

function drawDiff(diff, context, x, y, width, height) {
  context.strokeStyle = "red";
  context.lineWidth = 1;
  var width2 = width + context.lineWidth;
  var height2 = height + context.lineWidth;
  for (var i in diff) {
    var d = diff[i];
    var x2 = x + (d[0] * width);
    var y2 = y + (d[1] * height);
    context.pixelStrokeRect(x2, y2, width2, height2);
  }
}

function getImageDiff(imageData1, imageData2) {
  var diff = new Array();
  var width1 = imageData1.width;
  var height1 = imageData1.height;
  var width2 = imageData2.width;
  var height2 = imageData2.height;
  var i1 = 0;
  var i2 = 0;
  for (var y = 0; y < height1 && y < height2; y++) {
    for (var x = 0; x < width1 && x < width2; x++, i1 += 4, i2 += 4) {
      if (imageData1.data[i1] !== imageData2.data[i2]
        || imageData1.data[i1 + 1] !== imageData2.data[i2 + 1]
        || imageData1.data[i1 + 2] !== imageData2.data[i2 + 2]
        || imageData1.data[i1 + 3] !== imageData2.data[i2 + 3]) {
        diff.push([x,y]);
      }
    }
    if (x < width1) i1 += 4 * (width1 - x);
    if (x < width2) i2 += 4 * (width2 - x);
  }
  return diff;
}

function getImageColors(imageData) {
  var colors = new Array();
  var width = imageData.width;
  var height = imageData.height;
  var i = 0;
  for (var y = 0; y < height; y++) {
    for (var x = 0; x < width; x++, i += 4) {
      var r = imageData.data[i];
      var g = imageData.data[i + 1];
      var b = imageData.data[i + 2];
      var a = imageData.data[i + 3];
      var co = rgb(r, g, b);
      colors[co] = { 'r': r, 'g': g, 'b': b };
    }
  }
  return colors;
}

function printImageColors(elm, colors) {
  var coWhite = { 'r': 255, 'g': 255, 'b': 255 };
  var coBlack = { 'r': 0, 'g': 0, 'b': 0 };
  for (var i in colors) {
    var co = colors[i];
    var html = "\
<div class='row'>\n\
<div class='cell' style='width: 40px; height: 20px; background-color: rgb(" + co.r + ", " + co.g + ", " + co.b + ");'></div>\n\
<div class='cell'>r " + toHex(co.r, 2) + "</div>\n\
<div class='cell'>g " + toHex(co.g, 2) + "</div>\n\
<div class='cell'>b " + toHex(co.b, 2) + "</div>\n\
<div class='cell'>d1 " + rgbDistance(co, coWhite) + "</div>\n\
<div class='cell'>d2 " + rgbDistance(co, coBlack) + "</div>\n\
</div>\n\
";
    elm.insertAdjacentHTML('beforeend', html);
  }
}

function getFile(i) {
  //if (document.getElementById("directory" + i).value === "" || document.getElementById("file" + i).value === "") return null;
  //return document.getElementById("directory" + i).value + "/" + document.getElementById("file" + i).value;
  if ($("#directory" + i).val() === "" || $("#file" + i).val() === "") return null;
  return $("#directory" + i).val() + "/" + $("#file" + i).val();
  
}

function setFile(i) {
  //document.getElementById("file" + i).value = document.getElementById("hidden_file" + i).files[0].fileName;
  $("#file" + i).val() = $("#hidden_file" + i).files[0].fileName;
}

function testFile() {
//    var file = new ActiveXObject("Scripting.FileSystemObject");
//    var a = file.CreateTextFile("c:\\testfile.txt", true);
//    a.WriteLine("Salut cppFrance !");
//    a.Close();
    //var fw = new FileWriter();
    // chrome FileWriter
    console.log("toto");
    
    // Writing to local file system in Chrome extension http://stackoverflow.com/questions/5429513/writing-to-local-file-system-in-chrome-extension
    //window.requestFileSystem(window.PERSISTENT, 5 * 1024 * 1024, initFs);
}

/**************************************************************************************************************
//function initFs(fs) {
//    fs.root.getFile('log.txt', { create: true, exclusive: true }, function (fileEntry) {
//        fileEntry.isFile = true;
//        fileEntry.name = 'log.txt';
//        fileEntry.fullPath = '/log.txt';
//        fileEntry.createWriter(function (fileWriter) {
//            fileWriter.seek(fileWriter.length);
//            var bb = new BlobBuilder();
//            //bb.append("\n<TimeStamp>" + getTimestamp() + "</TimeStamp><Browser>Chrome</Browser><URL>" + tabURL + "</URL>\n");
//            bb.append("tata");
//            fileWriter.write(bb.getBlob('text/plain'));
//        });
//    });
//}
**************************************************************************************************************/
