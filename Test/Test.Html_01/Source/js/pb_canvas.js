CanvasRenderingContext2D.prototype.clear =
  CanvasRenderingContext2D.prototype.clear || function (preserveTransform) {
    // How to clear the canvas for redrawing http://stackoverflow.com/questions/2142535/how-to-clear-the-canvas-for-redrawing
    // The problem with using context.clearRect(0,0,canvas.width,canvas.height) is that if you have modified the transformation matrix
    // you likely will not be clearing the canvas properly.
    if (preserveTransform) {
      // Store the current transformation matrix
      this.save();
      // Use the identity matrix while clearing the canvas
      this.setTransform(1, 0, 0, 1, 0, 0);
    }
    this.clearRect(0, 0, this.canvas.width, this.canvas.height);
    //If you are drawing lines, make sure you don't forget
    this.beginPath();
    if (preserveTransform) {
      // Restore the transform
      this.restore();
    }
};

CanvasRenderingContext2D.prototype.pixelStrokeRect =
  CanvasRenderingContext2D.prototype.pixelStrokeRect || function (x, y, width, height) {
    //this.strokeRect(x + 0.5, y + 0.5, width - 1, height - 1);
    this.strokeRect(x + (this.lineWidth / 2), y + (this.lineWidth / 2), width - this.lineWidth, height - this.lineWidth);
};

CanvasRenderingContext2D.prototype.pixelMoveTo =
  CanvasRenderingContext2D.prototype.pixelMoveTo || function (x, y) {
    //this.strokeRect(x + 0.5, y + 0.5, width - 1, height - 1);
    this.moveTo(x + (this.lineWidth / 2), y + (this.lineWidth / 2));
};

CanvasRenderingContext2D.prototype.pixelLineTo =
  CanvasRenderingContext2D.prototype.pixelLineTo || function (x, y) {
    //this.strokeRect(x + 0.5, y + 0.5, width - 1, height - 1);
    this.lineTo(x + (this.lineWidth / 2), y + (this.lineWidth / 2));
};

function zoomImage(context, x, y, zoom_x, zoom_y, imageData) {
  var width = imageData.width;
  var height = imageData.height;
  var i = 0;
  var y3 = y;
  for (var y2 = 0; y2 < height; y2++) {
    var x3 = x;
    for (var x2 = 0; x2 < width; x2++, i += 4) {
      var r = imageData.data[i];
      var g = imageData.data[i + 1];
      var b = imageData.data[i + 2];
      var a = imageData.data[i + 3];
      //context.fillStyle = "rgb(" + r + "," + g + "," + b + ")";
      context.fillStyle = "rgba(" + r + "," + g + "," + b + "," + a / 255 + ")";
      context.fillRect(x3, y3, zoom_x, zoom_y);
      x3 += zoom_x;
    }
    y3 += zoom_y;
  }
}

function drawGrid(context, x, y, width, height, nbCol, nbLine) {
  //context.strokeStyle = "black";
  context.strokeStyle = "rgba(0, 0, 0, 255)";
  context.lineWidth = 1;
  var width2 = width + context.lineWidth;
  var height2 = height + context.lineWidth;
  var y2 = y;
  for (var line = 0; line < nbLine; line++) {
    var x2 = x;
    for (var col = 0; col < nbCol; col++) {
      //context.strokeRect(x2, y2, width, height);
      context.pixelStrokeRect(x2, y2, width2, height2);
      //console.log("pixelStrokeRect(" + x2 + ", " + y2 + ", " + width2 + ", " + height2 + ")");
      x2 += width;
    }
    y2 += height;
  }
  context.fillStyle = "rgba(0, 0, 0, 255)";
  context.font = "10pt sans-serif";
  context.textAlign = "center";  // start (défaut), end, left, center, right
  context.textBaseLine = "bottom";  // top, hanging, middle, alphabetic (défaut), ideographic, bottom
  // legend x
  var x2 = x;
  var y2 = y - 5;
  var e = 5;
  for (var col = 0; col <= nbCol; col += e, x2 += e * width) {
    context.beginPath();
    context.pixelMoveTo(x2, y2);
    context.pixelLineTo(x2, y2 + 5);
    context.stroke();
    context.closePath();
    context.fillText(col, x2, y2);
  }
  // legend y
  context.textAlign = "right";  // start (défaut), end, left, center, right
  context.textBaseLine = "bottom";  // top, hanging, middle, alphabetic (défaut), ideographic, bottom
  var x2 = x - 5;
  var y2 = y;
  var e = 5;
  for (var line = 0; line <= nbLine; line += e, y2 += e * height) {
    context.beginPath();
    context.pixelMoveTo(x2, y2);
    context.pixelLineTo(x2 + 5, y2);
    context.stroke();
    context.closePath();
    context.fillText(line, x2, y2 + 5);
  }
}
