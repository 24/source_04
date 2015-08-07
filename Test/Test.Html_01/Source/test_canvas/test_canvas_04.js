window.onload = function(){
    paramsToValues();
    drawCanvas();
};

function paramsToValues() {
    prm = getUrlParams();
    if (prm['file'] !== undefined)
        document.getElementById("file").value = prm['file'];
    if (prm['dir'] !== undefined)
        document.getElementById("directory").value = prm['dir'];
    if (prm['zoom_x'] !== undefined)
        document.getElementById("zoom_x").value = prm['zoom_x'];
    if (prm['zoom_y'] !== undefined)
        document.getElementById("zoom_y").value = prm['zoom_y'];
}

function drawCanvas() {
    var canvas = document.getElementById("canvas");
    var context = canvas.getContext("2d");
    context.clear();
    var file = getFile();
    if (file === "") return;
    var image = new Image();
    image.onload = function(){
        context.drawImage(this, 10, 10, this.width, this.height);
        drawImage(context, 10, 10, this.width, this.height);
    };
    image.src = file;
}

function drawImage(context, x, y, width, height) {
    var imgData = context.getImageData(x, y, width, height);
    y += height + 10;
    //context.putImageData(imgData, x, y + dy + 10);
    var zoom_x = Math.floor(document.getElementById("zoom_x").value / 100);
    var zoom_y = Math.floor(document.getElementById("zoom_y").value / 100);
    var i = 0;
    var y3 = y;
    for (var y2 = 0; y2 < height; y2++) {
        var x3 = x;
        for (var x2 = 0; x2 < width; x2++) {
            var r = imgData.data[i++];
            var g = imgData.data[i++];
            var b = imgData.data[i++];
            var a = imgData.data[i++];
            context.fillStyle = "rgb(" + r + "," + g + "," + b + ")";
            context.fillRect(x3, y3, zoom_x, zoom_y);
            context.rect(x3, y3, zoom_x, zoom_y);
            //context.strokeStyle="red";
            context.stroke();            
            x3 += zoom_x;
        }
        y3 += zoom_y;
    }
}

function getFile() {
    if (document.getElementById("directory").value === "" || document.getElementById("file").value === "") return "";
    return document.getElementById("directory").value + "/" + document.getElementById("file").value;
}

function setFile() {
    document.getElementById("file").value = document.getElementById("hidden_file").files[0].fileName;
}
