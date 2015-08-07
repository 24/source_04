function draw() {
    draw1();
    //draw2();
    //draw3();
    //draw4();
}
function draw4() {
    var canvas1 = document.getElementById("canvas1");
    var context1 = canvas1.getContext("2d");
    context1.clear();
    
    context1.lineWidth = 1;
    context1.strokeStyle = "rgb(200, 0, 0)";
    context1.strokeRect(1, 1, 4, 4);
    var imageData = context1.getImageData(0, 0, 10, 10);
    
    var canvas2 = document.getElementById("canvas2");
    var context2 = canvas2.getContext("2d");
    context2.clear();
    var zoom_x = 20;
    var zoom_y = 20;
    zoomImage(context2, 10.5, 10.5, zoom_x, zoom_y, imageData);
    drawGrid(context2, 10.5, 10.5, zoom_x, zoom_y, imageData.width, imageData.height);
}

function draw1() {
    var canvas = document.getElementById("canvas");
    var context = canvas.getContext("2d");
    context.clear();

    context.fillStyle = "rgb(200,0,0)";
    context.fillRect (10.5, 10.5, 55, 50);

    context.fillStyle = "rgba(0, 0, 200, 0.5)";
    context.fillRect (30.5, 30.5, 55, 50);
    
    context.lineWidth = 2;
    context.strokeStyle = "rgb(0, 0, 0)";
    context.strokeRect(110.5, 10.5, 55, 50);
    context.strokeStyle = "rgb(200, 0, 0)";
    context.strokeRect(130.5, 30.5, 55, 50);
    
    context.beginPath();
    context.lineWidth = 2;
    context.strokeStyle = "rgb(0, 0, 0)";
    context.rect(210.5, 10.5, 55, 50);
    context.closePath();
    context.stroke();
    
    context.beginPath();
    context.lineWidth = 2;
    context.strokeStyle = "rgb(200, 0, 0)";
    context.rect(230.5, 30.5, 55, 50);
    context.closePath();
    context.stroke();
    
    context.lineWidth = 1;
    context.strokeStyle = "rgb(0, 0, 0)";
    context.strokeRect(310.5, 10.5, 55, 50);
    context.strokeRect(410, 10, 55, 50);
    
    context.lineWidth = 1;
    context.strokeStyle = "rgb(0, 0, 0)";
    context.strokeRect(510.5, 10.5, 50, 50);
    context.strokeRect(560.5, 10.5, 50, 50);
    context.strokeRect(510.5, 60.5, 50, 50);
    context.strokeRect(560.5, 60.5, 50, 50);
    
    context.lineWidth = 1;
    context.strokeStyle = "rgb(0, 0, 0)";
    context.strokeRect(710.5, 10.5, 51, 51);
    context.strokeRect(760.5, 10.5, 51, 51);
    context.strokeRect(710.5, 60.5, 51, 51);
    context.strokeRect(760.5, 60.5, 51, 51);
}

function draw3() {
    var canvas = document.getElementById("canvas");
    var context = canvas.getContext("2d");
    context.clear();

    var x = 200;
    var y = 10;
    var dx = 40;
    var dy = 40;
    var m1 = 10;
    var m2 = 5;
    
    context.lineWidth = 2;
    //context.strokeStyle="red";
    context.strokeStyle = "rgb(0, 0, 0)";
    //context.strokeStyle = "rgb(200, 0, 0)";
    //context.rect(x + m2, y + m2, dx - (2 * m2), dy - (2 * m2));
    context.strokeRect(x + m2, y + m2, dx - (2 * m2), dy - (2 * m2));
    //context.stroke();
    
    x += dx;
    //context.strokeStyle="black";
    //context.strokeStyle = "rgb(0, 0, 0)";
    context.strokeStyle = "rgb(200, 0, 0)";
    context.strokeRect(x + m2, y + m2, dx - (2 * m2), dy - (2 * m2));
    //context.rect(x + m2, y + m2, dx - (2 * m2), dy - (2 * m2));
    //context.stroke();
    
    drawImage(context, 600, 10, 40, 40, 4, 4, 2, 1);
}

function draw2() {
    var canvas = document.getElementById("canvas");
    var context = canvas.getContext("2d");
    context.clear();

    var x = 200;
    var y = 10;
    var dx = 40;
    var dy = 40;
    var m1 = 10;
    var m2 = 5;
    context.fillStyle = "rgb(200, 200, 200)";
    context.fillRect(x + m1, y + m1, dx - (2 * m1), dy - (2 * m1));
    //context.strokeStyle="black";
    context.strokeStyle="red";
    context.rect(x + m2, y + m2, dx - (2 * m2), dy - (2 * m2));
    context.stroke();
    x += dx;
    context.fillStyle = "rgb(200, 200, 200)";
    context.fillRect(x + m1, y + m1, dx - (2 * m1), dy - (2 * m1));
    context.strokeStyle="black";
    //context.strokeStyle="red";
    context.rect(x + m2, y + m2, dx - (2 * m2), dy - (2 * m2));
    context.stroke();
    
    //drawImage(context, 600, 10, 40, 40, 4, 4);
}

function drawImage(context, x, y, zoom_x, zoom_y, width, height, m1, m2) {
    //var imgData = context.getImageData(x, y, width, height);
    //var width = imageData1.width;
    //var height = imageData1.height;
    //y += height + 10;
    //context.putImageData(imgData, x, y + dy + 10);
    context.lineWidth = 2;
    //var m1 = 1; // 10
    //var m2 = 1; // 5
    var select = new Array();
    var i = 0;
    var y3 = y;
    var rectBlack = false;
    for (var y2 = 0; y2 < height; y2++) {
        var x3 = x;
        for (var x2 = 0; x2 < width; x2++, i += 4) {
//            var r = imageData1.data[i];
//            var g = imageData1.data[i + 1];
//            var b = imageData1.data[i + 2];
//            var a = imageData1.data[i + 3];
            //context.fillStyle = "rgb(" + r + "," + g + "," + b + ")";
            context.fillStyle = "rgb(200, 200, 200)";
            context.fillRect(x3 + m1, y3 + m1, zoom_x - (2 * m1), zoom_y - (2 * m1));
            //if (r !== imageData2.data[i] || b !== imageData2.data[i + 2] || a !== imageData2.data[i + 3] || a !== imageData2.data[i + 3])
            if (rectBlack)
                context.strokeStyle="black";
            else
                context.strokeStyle="red";
            rectBlack = !rectBlack;
            //context.rect(x3 + m2, y3 + m2, zoom_x - (2 * m2), zoom_y - (2 * m2));
            //context.stroke();
            context.strokeRect(x3 + m2, y3 + m2, zoom_x - (2 * m2), zoom_y - (2 * m2));
            x3 += zoom_x - 1;
        }
        y3 += zoom_y - 1;
    }
}
