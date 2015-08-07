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
        var zoom_x = document.getElementById("zoom_x").value / 100;
        var zoom_y = document.getElementById("zoom_y").value / 100;
        context.drawImage(this, 10, 10, this.width * zoom_x, this.height * zoom_y);
    };
    image.src = file;
}

function getFile() {
    if (document.getElementById("directory").value === "" || document.getElementById("file").value === "") return "";
    return document.getElementById("directory").value + "/" + document.getElementById("file").value;
}

function setFile() {
    document.getElementById("file").value = document.getElementById("hidden_file").files[0].fileName;
}
