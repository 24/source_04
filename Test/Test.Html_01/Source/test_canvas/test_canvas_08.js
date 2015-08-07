window.onload = function () {
  resize();
};

function resize() {
  var body = $("#body")[0];
  $("#bodyClientWidth")[0].innerText = body.clientWidth;
  $("#bodyClientHeight")[0].innerText = body.clientHeight;
  $("#bodyClientLeft")[0].innerText = body.clientLeft;
  $("#bodyClientTop")[0].innerText = body.clientTop;
  $("#bodyOffsetWidth")[0].innerText = body.offsetWidth;
  $("#bodyOffsetHeight")[0].innerText = body.offsetHeight;
  $("#bodyOffsetLeft")[0].innerText = body.offsetLeft;
  $("#bodyOffsetTop")[0].innerText = body.offsetTop;
  $("#bodyScrollWidth")[0].innerText = body.scrollWidth;
  $("#bodyScrollHeight")[0].innerText = body.scrollHeight;
  $("#bodyScrollLeft")[0].innerText = body.scrollLeft;
  $("#bodyScrollTop")[0].innerText = body.scrollTop;
  var canvas = $("#canvas")[0];
  canvas.width = body.scrollWidth - canvas.offsetLeft - 5;
  canvas.height = body.scrollHeight - canvas.offsetTop - 20; // 16
  $("#canvasWidth")[0].innerText = canvas.width;
  $("#canvasHeight")[0].innerText = canvas.height;
  $("#canvasClientWidth")[0].innerText = canvas.clientWidth;
  $("#canvasClientHeight")[0].innerText = canvas.clientHeight;
  $("#canvasClientLeft")[0].innerText = canvas.clientLeft;
  $("#canvasClientTop")[0].innerText = canvas.clientTop;
  $("#canvasOffsetWidth")[0].innerText = canvas.offsetWidth;
  $("#canvasOffsetHeight")[0].innerText = canvas.offsetHeight;
  $("#canvasOffsetLeft")[0].innerText = canvas.offsetLeft;
  $("#canvasOffsetTop")[0].innerText = canvas.offsetTop;
  $("#canvasScrollWidth")[0].innerText = canvas.scrollWidth;
  $("#canvasScrollHeight")[0].innerText = canvas.scrollHeight;
  $("#canvasScrollLeft")[0].innerText = canvas.scrollLeft;
  $("#canvasScrollTop")[0].innerText = canvas.scrollTop;
}
