var gFreecell = null;

window.onload = function () {
  gFreecell = new FreecellGame("canvas", "./img/gnomangelo.png");
  gFreecell.onload = load;
};

function load() {
  gFreecell.init();
  showButton();
  //gFreecell.newGame();
}

function showButton() {
  showHide("bt_new");
  //showHide("bt_replay");
  //showHide("bt_other");
  //showHide("bt_undo");
  //showHide("bt_redo");
  //showHide("bt_hint");
}

function newGame() {
  gFreecell.newGame();
}

function FreecellGame(canvasId, cardsImageSource) {
  this.onload = null;
  
  //this.widthUse = 1.06;   // fWidthUse
  //this.noOverlap = .2;    // fNoOverlap
  //this.fit = 0;           // fFit
  //this.fitCell = null;    // cFit
  //this.FILL_BLUE = "rgba(0, 0, 200, 0.5)";
  //this.EMPTY_GREEN = "#578132";
  //this.srcCardWidth = 0;  // fSrcCardW
  //this.srcCardHeight = 0; // fSrcCardH
  //this.dstCardWidth = 0;  // fDstCardW
  //this.dstCardHeight = 0; // fDstCardH
  
  //this.xD = 0;
  //this.yD = 0;
  //this.bMouseDown = false;
  //this.bIE = false;
  //this.nClicks = 0;
  //this.bClickOnly = true; // tenzij blijkt dat onmousemove werkt
  
  this.deck = null;
  this.draw = new Draw(this);
  this.mouse = new Mouse(this);
  this.table = new Table(this);
  //this.rctDirty = new RctDirty();
  //this.movingCards = new MovingCards(this);
  
  this.body = $("#body")[0];
  this.canvas = $("#" + canvasId)[0];
  this.context = this.canvas.getContext("2d");
  
  this.cardsImage = new Image();
  this.cardsImage.onload = createDelegate(this, function () { this.onload(); });
  this.cardsImage.src = cardsImageSource;
}

FreecellGame.prototype.init = function () {
  this.draw.init();
  this.mouse.setCanvasMouseEvent(this.canvas);
  //this.canvas.onmousedown = createDelegate(this, this.mousedownHandler);
  //this.canvas.onmousemove = createDelegate(this, this.mouseovermap);
  //this.canvas.onmouseup = createDelegate(this, this.mouseupHandler);
  //this.canvas.onmouseout = createDelegate(this, this.mouseupHandler);
  //this.canvas.onclick = createDelegate(this, this.clickHandler);
};

FreecellGame.prototype.newGame = function () {
  //this.init();
  //this.resize();
  //this.deck.newDeck();
  this.deck = new Deck();
  this.table.newGame();
  this.draw.draw();
};

//FreecellGame.prototype.draw = function () {
//  //if (this.rctDirty.w === 0) {
//  //  this.canvas.width = this.canvas.width; // clear
//  //} else {
//  //  this.context.clearRect(this.rctDirty.x, this.rctDirty.y, this.rctDirty.w, this.rctDirty.h);
//  //}
//  this.context.clear();
//  this.table.draw(); // this.context, this.canvas, this.cardsImage
//  //this.rctDirty.clear();
//}; // FreecellGame.draw()

function Draw(game) {
  this.game = game;
  this.FILL_BLUE = "rgba(0, 0, 200, 0.5)";
  this.EMPTY_GREEN = "#578132";
  this.widthUse = 1.06;
  this.noOverlap = .2;
  this.srcCardWidth = 0;
  this.srcCardHeight = 0;
  this.dstCardWidth = 0;
  this.dstCardHeight = 0;
  this.cascadeMargH = 0;
  this.cascadeMargV = 0;
  this.topCellMargH = 0;
  this.topCellMargV = 0;
  this.margeIn = 0;
  this.foundationLeft = 0;
}

Draw.prototype.init = function () {
  this.srcCardWidth = this.game.cardsImage.width / 13;
  this.srcCardHeight = this.game.cardsImage.height / 5;
  this.dstCardWidth = this.game.canvas.width / (8 + 7 * (this.widthUse - 1));
  this.dstCardHeight = this.srcCardHeight * this.dstCardWidth / this.srcCardWidth;
  
  //var fMarg = (canvas.width - 8 * dstCardWidth) / 7;
  this.cascadeMargH = (this.game.canvas.width - 8 * this.dstCardWidth) / 7;
  //var fVMarg = dstCardHeight * 1.1;
  this.cascadeMargV = this.dstCardHeight * 1.1;
  
  //var fMarg = (canvas.width - 8 * dstCardWidth) / 7 / 2;
  //var fMarg = (canvas.width - 8 * dstCardWidth) / 7 / 2;
  this.topCellMargH = this.cascadeMargH / 2;
  //var fVMarg = 2 * fMarg; // TODO: de juiste waarde hier
  //var fVMarg = 2 * fMarg;
  this.topCellMargV = this.cascadeMargH;
  
  //var fIn = dstCardWidth * .02;
  //var fIn = dstCardWidth * .02;
  //var fIn = dstCardWidth * .02;
  this.margeIn = this.dstCardWidth * .02;
  
  ///var fLeft= canvas.width - 4 * dstCardWidth - 3 * fMarg;
  this.foundationLeft = this.game.canvas.width - 4 * this.dstCardWidth - 3 * this.topCellMargH;
};

Draw.prototype.draw = function () {
  //if (this.rctDirty.w === 0) {
  //  this.canvas.width = this.canvas.width; // clear
  //} else {
  //  this.context.clearRect(this.rctDirty.x, this.rctDirty.y, this.rctDirty.w, this.rctDirty.h);
  //}
  this.game.context.clear();
  //this.table.draw(); // this.context, this.canvas, this.cardsImage
  var table = this.game.table;
  for(var i = 0; i < 4; i++) {
    //this.freeCells[i].draw();
    this.drawFreeCell(table.freeCells[i]);
  }  
  for(var i = 0; i < 4; i++) {
    //this.foundationCells[i].draw();
    this.drawFoundationCell(table.foundationCells[i]);
  }  
  for(var i = 0; i < 8; i++) {
    //this.cascadeCells[i].draw();
    this.drawCascadeCell(table.cascadeCells[i]);
  }  
  
  //this.rctDirty.clear();
}; // Draw.draw()

Draw.prototype.drawFreeCell = function (cell) {
  var context = this.game.context;
  //var srcCardWidth = this.game.srcCardWidth;
  //var srcCardHeight = this.game.srcCardHeight;
  //var dstCardWidth = this.game.dstCardWidth;
  //var dstCardHeight = this.game.dstCardHeight;
  //var fMarg = (this.game.canvas.width - 8 * this.dstCardWidth) / 7 / 2;
  //var fVMarg = 2 * fMarg; // TODO: de juiste waarde hier
  //var fIn = this.dstCardWidth * .02;
  if (cell.card === null || this.game.mouse.movingCards.cardsOwner === cell) {
    var x = 0 + cell.pos * this.dstCardWidth + cell.pos * this.topCellMargH + this.margeIn; // this.x
    var y = this.topCellMargV + this.margeIn; // this.y
    var w = this.dstCardWidth - 2 * this.margeIn; // this.w
    var h = this.dstCardHeight - 2 * this.margeIn; // this.h
    context.fillStyle = this.EMPTY_GREEN;
    roundRect(context, x, y, w, h);
    //if (this === this.game.fitCell) {
    //  context.fillStyle = this.FILL_BLUE;
    //  roundRect(context, this.x, this.y, this.w, this.h);
    //}
  } else {
    var iSrcX = cell.card.num * this.srcCardWidth;
    var iSrcY = cell.card.color * this.srcCardHeight;
    var x = cell.card.x1 = 0 + cell.pos * this.dstCardWidth + cell.pos * this.topCellMargH;
    var y = cell.card.y1 = this.topCellMargV;
    cell.card.x2 = x + this.dstCardWidth;
    cell.card.y2 = y + this.dstCardHeight;
    context.drawImage(this.game.cardsImage, iSrcX, iSrcY, this.srcCardWidth, this.srcCardHeight, x, y, this.dstCardWidth, this.dstCardHeight);
  }
};

Draw.prototype.drawFoundationCell = function (cell) {
  // voor de marge tussen kaarten houden we de helft aan van wat
  // in de 'waterval' gebruikt wordt:
  //var canvas = this.game.canvas;
  var context = this.game.context;
  //var srcCardWidth = this.game.srcCardWidth;
  //var srcCardHeight = this.game.srcCardHeight;
  //var dstCardWidth = this.game.dstCardWidth;
  //var dstCardHeight = this.game.dstCardHeight;
  //var fMarg = (canvas.width - 8 * dstCardWidth) / 7 / 2;
  //var fVMarg = 2 * fMarg; // TODO: de juiste waarde hier
  //var fIn= dstCardWidth * .02;
  //var fLeft= canvas.width - 4 * dstCardWidth - 3 * fMarg;
  if (cell.topcard === null) {
    var x = this.foundationLeft + cell.pos * this.dstCardWidth + cell.pos * this.topCellMargH + this.margeIn; // this.x
    var y = this.topCellMargV + this.margeIn; // this.y
    var w = this.dstCardWidth - 2 * this.margeIn; // this.w
    var h = this.dstCardHeight - 2 * this.margeIn; // this.h
    context.fillStyle = this.EMPTY_GREEN;
    roundRect(context, x, y, w, h);
    //if (this === this.game.fitCell) { // cFit
    //  context.fillStyle = this.FILL_BLUE;
    //  roundRect(context, x, y, w, h);
    //}
  } else {
    var x = this.foundationLeft + cell.pos * this.dstCardWidth + cell.pos * this.topCellMargH; // this.x
    var y = this.topCellMargV; // this.y
    var w = this.dstCardWidth; // this.w
    var h = this.dstCardHeight; // this.h
    var iSrcX = cell.topcard.num * this.srcCardWidth;
    var iSrcY = cell.topcard.color * this.srcCardHeight;
    //this.topcard.x = this.x;
    //this.topcard.y = this.y;
    context.drawImage(this.game.cardsImage, iSrcX, iSrcY, this.srcCardWidth, this.srcCardHeight, x, y, this.dstCardWidth, this.dstCardHeight);
    //if (this === this.game.fitCell) { // cFit
    //  context.fillStyle = this.FILL_BLUE;
    //  roundRect(context, this.x, this.y, this.w, this.h);
    //}
  }
}; // Draw.drawFoundationCell()

Draw.prototype.drawCascadeCell = function (cells) {
  var canvas = this.game.canvas;
  var context = this.game.context;
  //var rctDirty = this.game.rctDirty;
  //var srcCardWidth = this.game.srcCardWidth;
  //var srcCardHeight = this.game.srcCardHeight;
  //var dstCardWidth = this.game.dstCardWidth;
  //var dstCardHeight = this.game.dstCardHeight;
  //var fMarg = (canvas.width - 8 * dstCardWidth) / 7;
  //var fVMarg = dstCardHeight * 1.1;
  //var fIn = dstCardWidth * .02;
//this.cascadeMargH
//this.cascadeMargV
//this.margeIn
  var left = cells.pos * this.dstCardWidth + cells.pos * this.cascadeMargH;
  //if (rctDirty.w > 0) {
  //  if (rctDirty.x > left + this.dstCardWidth || rctDirty.x + rctDirty.w < left) {
  //    return;
  //  }
  //}
  var x = left; // this.x
  var y = this.cascadeMargV; // this.y
  if (cells.nbCards === 0) { // || this.game.movingCards.has(cells.cards[0])
    context.fillStyle = this.EMPTY_GREEN;
    roundRect(context, left + this.margeIn, this.cascadeMargV + this.margeIn, this.dstCardWidth - 2 * this.margeIn, this.dstCardHeight - 2 * this.margeIn);
    //if (this === this.game.fitCell) {
    //  context.fillStyle = this.FILL_BLUE;
    //  roundRect(context, left + this.margeIn, this.cascadeMargV + this.margeIn, this.dstCardWidth - 2 * this.margeIn, this.dstCardHeight - 2 * this.margeIn);
    //}
  } else {
    var overlapH = (canvas.height - this.cascadeMargV - this.dstCardHeight) / (cells.nbCards === 1 ? 1 : cells.nbCards - 1);
    if (overlapH > this.dstCardHeight * this.noOverlap) {
      overlapH = this.dstCardHeight * this.noOverlap;
    }
    for (var i = 0 ; i < cells.nbCards; i++) {
      var card = cells.cards[i];
      if (this.game.mouse.movingCards.has(card)) {
        break;
      }
      var iSrcX = card.num * this.srcCardWidth;
      var iSrcY = card.color * this.srcCardHeight;
      x = card.x1 = left;
      y = card.y1 = this.cascadeMargV + i * overlapH;
      card.x2 = x + this.dstCardWidth;
      card.y2 = y + this.dstCardHeight;
      context.drawImage(this.game.cardsImage, iSrcX, iSrcY, this.srcCardWidth, this.srcCardHeight, x, y, this.dstCardWidth, this.dstCardHeight);
    }
    //if (this === this.game.fitCell) {
    //  context.fillStyle= this.FILL_BLUE;
    //  roundRect(context, left, this.cascadeMargV + (cells.nbCards - 1) * overlapH, this.dstCardWidth, this.dstCardHeight);
    //}
  }
}; // Draw.drawCascadeCell()

Draw.prototype.eraseCards = function (movingCards) {
  
};

Draw.prototype.drawMovingCards = function (movingCards) {
  if (movingCards.cards.length === 0) {
    return;
  }
  //log("drawMovingCards");
  var x = movingCards.cards[0].x1 + movingCards.dx; // movingCards.x
  var y = movingCards.cards[0].y1 + movingCards.dy; // movingCards.y
  for (var i = 0; i < movingCards.cards.length; i++) {
    var card = movingCards.cards[i];
    //this.game.context.drawImage(imgDeck, card.num * fSrcCardW, card.color * fSrcCardH, fSrcCardW, fSrcCardH,
    //  movingCards.x, movingCards.y + i * fNoOverlap * fDstCardH, fDstCardW, fDstCardH);
    this.game.context.drawImage(this.game.cardsImage, card.num * this.srcCardWidth, card.color * this.srcCardHeight, this.srcCardWidth,
      this.srcCardHeight, x, y + i * this.noOverlap * this.dstCardHeight, this.dstCardWidth, this.dstCardHeight);
  }
}; // Draw.drawMovingCards()

Draw.prototype.eraseCards = function (movingCards) {
  // movingCards.cards
  var cards = movingCards.cards;
  for (var i = 0; i < cards.length; i++) {
    var card = cards[i];
    this.game.context.clearRect(card.x1 - 1, card.y1, card.x2 - card.x1 + 2, card.y2 - card.y1);
  }
  
};

function Mouse(game) {
  this.game = game;
  //this.x = 0; // xD
  //this.y = 0; // yD
  this.mouseDown = false;
  this.ie = false;
  //this.nClicks = 0;
  //this.clickOnly = true;
  this.movingCards = new MovingCards();
}

Mouse.prototype.setCanvasMouseEvent = function (canvas) {
  canvas.onmousedown = createDelegate(this, this.mousedown);
  canvas.onmousemove = createDelegate(this, this.mousemove); //mouseovermap
  canvas.onmouseup = createDelegate(this, this.mouseup);
  canvas.onmouseout = createDelegate(this, this.mouseout); // mouseupHandler
  canvas.onclick = createDelegate(this, this.click);
};

Mouse.prototype.mousedown = function (e) {
  this.mouseDown = true;
  //this.clickOnly = false;
  if (this.ie) {
    e = event; // event for internet explorer
  }
  var table = this.game.table;
  this.x = e.clientX;
  this.y = e.clientY;
  var x = e.clientX - this.game.canvas.offsetLeft;
  var y = e.clientY - this.game.canvas.offsetTop;
  var found = false;
  for(var i = 0; i < 8; i++) {
    if (this.isOverCascadeCell(table.cascadeCells[i], x, y)) {
      //log("mousedown : found cascade cell");
      found = true;
      break;
    }
    if (i < 4 && this.isOverFreeCell(table.freeCells[i], x, y)) {
      //log("mousedown : found free cell");
      found = true;
      break;
    }
  }
  //if (!found)
  //  log("mousedown : no cell found");
  if (found) {
    this.movingCards.clientX = e.clientX;
    this.movingCards.clientY = e.clientY;
    this.movingCards.dx = 0;
    this.movingCards.dy = 0;
    this.game.draw.eraseCards(this.movingCards);
    //this.game.draw.drawMovingCards(this.movingCards);
    }
  return false;
};

Mouse.prototype.mousemove = function (e) {
  if (!this.mouseDown) {
    return true;
  }
  if (this.ie) {
    e = event; // event for internet explorer
  }
  this.fitCell = null;
  this.fit = 0;
//  if (this.movingCards.cards.length > 0) {
//    var table = this.game.table;
//    for(var i = 0; i < 8; i++) {
//      table.cascadeCells[i].testReceiveMovingCards();
//    }
//    for(var i = 0; i < 4; i++) {
//      table.freeCells[i].testReceiveMovingCards();
//    }
//    for(var i = 0; i < 4; i++) {
//      table.foundationCells[i].testReceiveMovingCards();
//    }
//  }    
  //this.reDraw();
  ///this.game.draw.draw();
  if (this.movingCards.cards.length === 0) {
    //log("mousemove : no card in movingCards");
    return false;
  }
  this.movingCards.dx = e.clientX - this.movingCards.clientX;
  this.movingCards.dy = e.clientY - this.movingCards.clientY;
  //this.movingCards.draw();
  ////this.game.draw.drawMovingCards(this.movingCards);
  //rctDirty.add(movingCards.x, movingCards.y);
  //rctDirty.add(movingCards.x + fDstCardW, movingCards.y + fDstCardH);
  // TODO: bovenstaande inschakelen, maar de admin. dan ook compleet maken
  return false;
};

Mouse.prototype.mouseup = function (e) {
  this.mouseDown = false;
  this.movingCards.cardsOwner = null;
  this.movingCards.cards = new Array();
  this.movingCards.x = 0;
  this.movingCards.y = 0;
};

Mouse.prototype.mouseout = function (e) {
};

Mouse.prototype.click = function (e) {
};

Mouse.prototype.isOverFreeCell = function (cell, x, y) {
  if (cell.card === null) {
    return false;
  }
  //if (x < cell.card.x || x > cell.card.x + this.draw.dstCardWidth || y < cell.card.y || y > cell.card.y + this.game.dstCardHeight) {
  if (x < cell.card.x1 || x > cell.card.x2 || y < cell.card.y1 || y > cell.card.y2) {
    return false; 
  }
  this.movingCards.cardsOwner = cell;
  this.movingCards.cards[0] = cell.card;
  this.movingCards.x = cell.card.x1;
  this.movingCards.y = cell.card.y1;
  return true;
};

Mouse.prototype.isOverCascadeCell = function (cells, x, y) {
  if (cells.nbCards < 1) {
    //log("isOverCascadeCell : false, no card on this cascade (" + cells.pos + ")");
    return false;
  }
  var cardN = cells.cards[cells.nbCards - 1];
  //if (x > cardN.x && x < cardN.x + this.game.dstCardWidth && y > cardN.y && y < cardN.y + this.game.dstCardHeight) {
  if (x >= cardN.x1 && x < cardN.x2 && y >= cardN.y1 && y < cardN.y2) {
    this.movingCards.cardsOwner = cells;
    this.movingCards.cards[0] = cardN;
    this.movingCards.x = cardN.x1;
    this.movingCards.y = cardN.y1;
    //log("isOverCascadeCell : true, found 1 card on this cascade (" + cells.pos + ", " + (cells.nbCards - 1) + ")");
    return true;
  }
  var cBelow = cardN;
  //var nMax = this.game.table.getMoveCapacity();
  var nMax = 10;
  for(var i = cells.nbCards - 2; i >= 0; i--) {
    var card = cells.cards[i];
    //if (card.blackRedEquals(cBelow) || card.num !== cBelow.num + 1) {
    if (!card.isCardSuiteNext(cBelow)) {
      //log("isOverCascadeCell : false, card no " + i + " is not part of a serie (" + cells.pos + ")");
      return false;
    }
    var nToPark = cells.nbCards - i; 
    if (nToPark > nMax) {
      //log("isOverCascadeCell : false, too many cards, max is " + nMax + " (" + cells.pos + ", " + i + ")");
      return false;
    }
    //if (x > card.x && x < card.x + this.game.dstCardWidth && y > card.y && y < card.y + this.game.dstCardHeight) {
    if (x >= card.x1 && x < card.x2 && y > card.y1 && y < card.y2) {
      this.movingCards.cardsOwner = cells;
      this.movingCards.x = cardN.x1;
      this.movingCards.y = cardN.y1;
      for(var j = i; j < cells.nbCards; j++) {
        this.movingCards.cards[j - i] = cells.cards[j];
      }
      //log("isOverCascadeCell : true, found " + nToPark + " cards (" + cells.pos + ", " + i + ")");
      return true;
    }
    cBelow = card;
  }
  //log("isOverCascadeCell : false, no card found on this cascade (" + cells.pos + ")");
  return false;
};

function MovingCards() {
  //this.game = game;
  //this.x = 0;
  //this.y = 0;
  this.clientX = 0;
  this.clientY = 0;
  this.dx = 0;
  this.dy = 0;
  this.cardsOwner = null;
  this.cards = new Array();
}

MovingCards.prototype.has = function (card) {
  if (this.cards.length < 1) {
    return false;
  }
  return (this.cards[0] === card);
};

function Table(game) {
  this.game = game;
  this.freeCells = null;        // 4 emplacements libre
  this.foundationCells = null;  // 1 emplacement par couleur
  this.cascadeCells = null;     // 8 colonnes de cartes
  this.won = false;
}

Table.prototype.newGame = function () {
  this.freeCells = new Array();        // 4 emplacements libre
  this.foundationCells = new Array();  // 1 emplacement par couleur
  this.cascadeCells = new Array();     // 8 colonnes de cartes
  this.won = false;
  for (var i = 0; i < 4; i++) {
    this.freeCells[i] = new FreeCell(this.game, i);
  }
  for (var i = 0; i < 4; i++) {
    this.foundationCells[i] = new FoundationCell(this.game, i);
  }
  for (var i = 0; i < 8; i++) {
    this.cascadeCells[i] = new CascadeCell(this.game, i);
  }
  for(var i = 0; i < 4 * 13; i++) {
    var x = i % 8;
    this.cascadeCells[x].add(this.game.deck.next());
  }
};

Table.prototype.draw = function () {
  for(var i = 0; i < 4; i++) {
    //this.freeCells[i].draw();
    this.draw.drawFreeCell(this.freeCells[i]);
  }  
  for(var i = 0; i < 4; i++) {
    //this.foundationCells[i].draw();
    this.draw.drawFoundationCell(this.foundationCells[i]);
  }  
  for(var i = 0; i < 8; i++) {
    //this.cascadeCells[i].draw();
    this.draw.drawCascadeCell(this.cascadeCells[i]);
  }  
};

function Deck() {
  this.cardsLeft =  0;
  this.cards = null;
  //this.cardColors = new Array("C", "D", "H", "S"); // C=trèfle club, D=carreau diamond, H=coeur heart, S=pique spade
  //this.cardColorsTitle = new Array("club", "diamond", "heart", "spade");
  //this.cardNums = new Array("A","2","3","4","5","6","7","8","9","10","J","Q","K");
  //this.cardNumsTitle = new Array("ace", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "jack", "queen", "king");
  this.cards = new Array();
  for(var i = 0; i < 13; i++) {
    for(var j = 0; j < 4; j++) {
      this.cards[this.cards.length] = new Card(this, j, i);
    }
  }
  this.cardsLeft = this.cards.length;
}

//Deck.prototype.newDeck = function () {
//  this.cards = new Array();
//  for(var i = 0; i < 13; i++) {
//    for(var j = 0; j < 4; j++) {
//      this.cards[this.cards.length] = new Card(this, j, i);
//    }
//  }
//  this.cardsLeft = this.cards.length;
//};

Deck.prototype.next = function () {
  var i = Math.floor(Math.random() * this.cardsLeft);
  ret = this.cards[i];
  this.cards[i] = this.cards[this.cardsLeft - 1];
  this.cardsLeft--;
  return ret;
};

function FreeCell(game, pos) {
  this.game = game;
  this.pos = pos;
  this.card = null;
  //this.x = 0;
  //this.y = 0;
  //this.w = 0;
  //this.h = 0;
}

function FoundationCell(game, pos) {
  this.game = game;
  this.pos = pos;
  this.topcard = null;
  //this.x = 0;
  //this.y = 0;
  //this.w = 0;
  //this.h = 0;
}

function CascadeCell(game, pos) {
  this.game = game;
  this.pos = pos;
  this.cards = new Array();
  this.nbCards = 0;
  //this.x = 0;
  //this.y = 0;
}

CascadeCell.prototype.add = function (card) {
  this.cards[this.nbCards] = card;
  this.nbCards++;
};

function Card(deck, color, num) {
  this.deck = deck;
  this.color = color; // 0=trèfle club, 1=carreau diamond, 2=coeur heart, 3=pique spade
  this.num = num;
  this.x1 = 0;
  this.y1 = 0;
  this.x2 = 0;
  this.y2 = 0;
}

Card.prototype.blackRedEquals = function (card) {
    if (this.color === card.color) {
      return true;
    } else if (this.color + card.color === 3) {
      return true;
    } else {
      return false;
    }
  };

Card.prototype.isCardSuiteNext = function (card) {
    if (this.color === card.color || this.color + card.color === 3 || this.num !== card.num + 1)
      return false;
    else
      return true;
  };

function roundRect(context, x, y, width, height) {
  var radius = width / 15;
  context.beginPath();
  context.moveTo(x + radius, y);
  context.lineTo(x + width - radius, y);
  context.quadraticCurveTo(x + width, y, x + width, y + radius);
  context.lineTo(x + width, y + height - radius);
  context.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
  context.lineTo(x + radius, y + height);
  context.quadraticCurveTo(x, y + height, x, y + height - radius);
  context.lineTo(x, y + radius);
  context.quadraticCurveTo(x, y, x + radius, y);
  context.closePath();
  context.fill();
}
