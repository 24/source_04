function _drawImage(g, image, srcX, srcY, srcW, srcH, dstX, dstY, dstW, dstH) {
  log('drawImage : srcX ' + srcX + ' srcY ' + srcY + ' srcW ' + srcW + ' srcH ' + srcH + ' dstX ' + dstX + ' dstY ' + dstY + ' dstW ' + dstW + ' dstH ' + dstH);
  g.drawImage(image, srcX, srcY, srcW, srcH, dstX, dstY, dstW, dstH);
}

function _roundRect(g, x, y, w, h) {
  log('roundRect : fillStyle ' + g.fillStyle + ' x ' + x + ' y ' + y + ' w ' + w + ' h ' + h);
  roundRect(g, x, y, w, h);
}
