//Espaces de noms http://falola.developpez.com/tutoriels/javascript/namespace/
var gFreecell = null;

window.onload = function () {
  gFreecell = new FreecellGame("canvas", "./img/gnomangelo.png");
  gFreecell.onload = load;
};

function load() {
  showButton();
  gFreecell.newGame();
}

function showButton() {
  showHide("bt_new");
  showHide("bt_replay");
  showHide("bt_other");
  showHide("bt_undo");
  showHide("bt_redo");
  showHide("bt_hint");
}

function newGame() {
  //gFreecell.newGame();
  window.location.href = window.location.href;
}

function replayGame() {
  gFreecell.replayGame();
}

function other() {
  alert('Waarom zou je een ander spel willen spelen? Dit is toch veel leuker!');
}

function undo() {
  gFreecell.undo();
}

function redo() {
  gFreecell.redo();
}

function hint() {
  //var router = Router(table);
}

function FreecellGame(canvasId, cardsImageSource) {
  this.onload = null;
  
  this.widthUse = 1.06;   // fWidthUse
  this.noOverlap = .2;    // fNoOverlap
  this.fit = 0;           // fFit
  this.fitCell = null;    // cFit
  this.FILL_BLUE = "rgba(0, 0, 200, 0.5)";
  this.EMPTY_GREEN = "#578132";
  this.srcCardWidth = 0;  // fSrcCardW
  this.srcCardHeight = 0; // fSrcCardH
  this.dstCardWidth = 0;  // fDstCardW
  this.dstCardHeight = 0; // fDstCardH
  //this.canvasHeight = 0;  // fCanvasH
  
  this.xD = 0;
  this.yD = 0;
  this.bMouseDown = false;
  this.bIE = false;
  this.nClicks = 0;
  this.bClickOnly = true; // tenzij blijkt dat onmousemove werkt
  
  this.deck = new Deck();
  this.table = new Table(this);
  this.rctDirty = new RctDirty();
  this.movingCards = new MovingCards(this);
  
  this.body = $("#body")[0];
  this.canvas = $("#" + canvasId)[0];
  this.context = this.canvas.getContext("2d");
  
  this.cardsImage = new Image();  // imgDeck
  this.cardsImage.onload = createDelegate(this, function () { this.onload(); });
  this.cardsImage.src = cardsImageSource;
}

FreecellGame.prototype.newGame = function () {
  this.init();
  this.resize();
};

FreecellGame.prototype.replayGame = function () {
  this.table.reset();
  this.reDraw();
};

FreecellGame.prototype.undo = function () {
  this.table.undo();
  this.reDraw();
};

FreecellGame.prototype.redo = function () {
  this.table.redo();
  this.reDraw();
};

FreecellGame.prototype.init = function () {
  this.deck.init();
  this.table.init();
  this.canvas.onmousedown = createDelegate(this, this.mousedownHandler);
  this.canvas.onmousemove = createDelegate(this, this.mouseovermap);
  this.canvas.onmouseup = createDelegate(this, this.mouseupHandler);
  this.canvas.onmouseout = createDelegate(this, this.mouseupHandler);
  this.canvas.onclick = createDelegate(this, this.clickHandler);
};

//FreecellGame.prototype.draw = function () {
//  this.context.drawImage(this.cardsImage, 0, 0);
//};

FreecellGame.prototype.resize = function () {
  // width 1000 = 1013 - 8 - 5
  // height 500 = 641 - 125 - 16
  this.canvas.width = this.body.scrollWidth - canvas.offsetLeft - 5;
  this.canvas.height = this.body.scrollHeight - canvas.offsetTop - 20; // 16
  this.updateCardDim();
  this.reDraw();
};

FreecellGame.prototype.updateCardDim = function () {
  //this.canvasHeight = this.canvas.height;
  if (this.srcCardWidth === 0) {
    this.srcCardWidth = this.cardsImage.width / 13;
    this.srcCardHeight = this.cardsImage.height / 5;
  }
  this.dstCardWidth = this.canvas.width / (8 + 7 * (this.widthUse - 1));
  this.dstCardHeight = this.srcCardHeight * this.dstCardWidth / this.srcCardWidth;
};

FreecellGame.prototype.reDraw = function () {
  if (this.rctDirty.w === 0) {
    this.canvas.width = this.canvas.width; // clear
  } else {
    this.context.clearRect(this.rctDirty.x, this.rctDirty.y, this.rctDirty.w, this.rctDirty.h);
  }
  this.table.draw(); // this.context, this.canvas, this.cardsImage
  this.rctDirty.clear();
}; // reDraw()

FreecellGame.prototype.clickHandler = function (e) {
  if (!this.bClickOnly) {
    return false;
  }
  if (this.nClicks === 0) {
    this.mousedownHandler(e);
    this.nClicks++;
  } else {
    this.mouseovermap(e);
    this.mouseovermap(e);
    this.mouseupHandler(e);
    this.nClicks = 0;
  }
};

FreecellGame.prototype.mousedownHandler = function (e) {
  this.bMouseDown = true;
  this.bClickOnly = false;
  if (this.bIE) {
    e = event; // event for internet explorer
  }
  this.xD = e.clientX;
  this.yD = e.clientY;
  var x = e.clientX - this.canvas.offsetLeft;
  var y = e.clientY - this.canvas.offsetTop;
  for(var i = 0; i < 8; i++) {
    if (this.table.cascadeCells[i].isOver(x, y)) {
      // de singleton movingCards heeft nu een waarde
      break;
    }
    // ook van de vrije cellen kan een kaart gepakt worden:
    if (i < 4 && this.table.freeCells[i].isOver(x,y)) {
      break;
    }
  }
  return false;
};

FreecellGame.prototype.mouseovermap = function (e) {
  if (!this.bMouseDown) {
    return true;
  }
  if (this.bIE) {
    e = event; // event for internet explorer
  }
  this.fitCell = null;
  this.fit = 0;
  if (this.movingCards.cards.length > 0) {
    for(var i = 0; i < 8; i++) {
      this.table.cascadeCells[i].testReceiveMovingCards();
    }
    for(var i = 0; i < 4; i++) {
      this.table.freeCells[i].testReceiveMovingCards();
    }
    for(var i = 0; i < 4; i++) {
      this.table.foundationCells[i].testReceiveMovingCards();
    }
  }    
  this.reDraw();
  if (this.movingCards.cards.length === 0) {
    return false;
  }
  //var g = canvas.getContext("2d");
  this.movingCards.dx = e.clientX - this.xD;
  this.movingCards.dy = e.clientY - this.yD;
  this.movingCards.draw();
  //rctDirty.add(movingCards.x, movingCards.y);
  //rctDirty.add(movingCards.x + fDstCardW, movingCards.y + fDstCardH);
  // TODO: bovenstaande inschakelen, maar de admin. dan ook compleet maken
  return false;
};

FreecellGame.prototype.mouseupHandler = function (e) {
  this.reDraw();
  if (!this.bMouseDown) {
    return true;
  }
  if (this.bIE) {
    e = event;
  }
  this.bMouseDown = false;
  var bToBackup = false;
  if (this.movingCards.cards.length > 0) {
    // kijk welke cel de zwevende kaart accepteert
    for(var i = 0; i < 8; i++) {
      if (this.table.cascadeCells[i].receiveMovingCards()) {
        this.movingCards.cardsOwner.releaseMovingCards();
        bToBackup = true;
        break;
      }
      if (i >= 4) {
        continue;
      }
      if (this.table.freeCells[i].receiveMovingCards()) {
        this.movingCards.cardsOwner.releaseMovingCards();
        bToBackup = true;
        break;
      }
      if (this.table.foundationCells[i].receiveMovingCards()) {
        this.movingCards.cardsOwner.releaseMovingCards();
        bToBackup = true;
        break;
      }
    }
    this.fitCell = null;
    this.movingCards.drop();
    if (bToBackup) {
      this.table.backup();
    }
    this.reDraw();
    //var canvas= document.getElementById("canvas");
    //var g= canvas.getContext("2d");
    // canvas, imgDeck
    this.table.autoPlay();
  }    
  return false;
};

function Table(game) {
  this.game = game;
  this.freeCells = new Array();        // aFreeCell    4 emplacements libre
  this.foundationCells = new Array();  // aStackCell   1 emplacement par couleur
  this.cascadeCells = new Array();     // aCascadeCell 8 colonnes de cartes
  this.backupData = new Array(); // aBackup
  this.undoNb = 0; // nUndo
  this.won = false;
}

Table.prototype.init = function () {
  // aFreeCell
  for (var i = 0; i < 4; i++) {
    this.freeCells[i] = new FreeCell(this.game, i);
  }
  // aStackCell
  for (var i = 0; i < 4; i++) {
    this.foundationCells[i] = new FoundationCell(this.game, i);
  }
  // aCascadeCell
  for (var i = 0; i < 8; i++) {
    this.cascadeCells[i] = new CascadeCell(this.game, i);
  }
  for(var i = 0; i < 4 * 13; i++) {
    var x = i % 8;
    this.cascadeCells[x].add(this.game.deck.next());
  }
};

//draw: function(g, canvas, imgDeck){
Table.prototype.draw = function () {
  for(var i = 0; i < 4; i++) {
    this.freeCells[i].draw(); // aFreeCell  canvas, context, this.cardsImage
  }  
  for(var i = 0; i < 4; i++) {
    this.foundationCells[i].draw(); // aStackCell   canvas, context, this.cardsImage
  }  
  for(var i = 0; i < 8; i++) {
    this.cascadeCells[i].draw(); // aCascadeCell       canvas, context, this.cardsImage
  }  
};

Table.prototype.getMoveCapacity = function () {
  var nF = 0;
  for (var i = 0; i < 4; i++) {
    if (this.freeCells[i].card === null) {
      nF++;
    }
  }
  var nC = 0;
  for (var i = 0; i < 8; i++) {
    if (this.cascadeCells[i].nbCards === 0) {
      nC++;
    }
  }
  return Math.max((nF + 1) * nC + 1, (nF + 1 + nC));
}; // Table.getMoveCapacity()

Table.prototype.getMoveCapacityMin1 = function () {
  var nF = 0;
  for (var i = 0; i < 4; i++) {
    if (this.freeCells[i].card === null) {
      nF++;
    }
  }
  var nC = 0;
  for (var i = 0; i < 8; i++) {
    if (this.cascadeCells[i].nbCards === 0) {
      nC++;
    }
  }
  return Math.max((nF + 1) * (nC - 1) + 1, (nF + 1 + (nC - 1)));
}; // Table.getMoveCapacityMin1()

Table.prototype.autoPlay = function () {
  //this.canvas= canvas;
  //this.imgDeck= imgDeck;
  setTimeout("gFreecell.table.autoPlayOnTimer()", 100);
};

Table.prototype.autoPlayOnTimer = function () {
  //var canvas= this.canvas;
  //var imgDeck= this.imgDeck;
  //var g= canvas.getContext("2d");
  for (;;) {
    var bAgain= false;
    // loop de stapels af en zoek kaarten die erop kunnen
    for(var i = 0; i < 4; i++) {
      for(var j = 0; j < 8; j++) {
        var card = this.cascadeCells[j].getTopCard();
        if (card === null) {
          continue;
        }
        if (card.num === 0 && this.foundationCells[i].topcard === null) {
          // aas wegleggen
          this.foundationCells[i].topcard = card;
          this.cascadeCells[j].nbCards--;
          bAgain= true;
          break;
        } else if (this.foundationCells[i].topcard !== null
              && this.foundationCells[i].topcard.color === card.color
              && this.foundationCells[i].topcard.num + 1 === card.num
              && this.shouldStack(card)) {
          // stapel verhogen
          this.foundationCells[i].topcard = card;
          this.cascadeCells[j].nbCards--;
          bAgain = true;
          break;
        }
      } // kaartkolommen (onderste helft) proberen af te stapelen
      if (bAgain) {
        break;
      }
      for(var j = 0; j < 4; j++) {
        var card = this.freeCells[j].card;
        if (card === null) {
          continue;
        }
        if (card.num === 0 && this.foundationCells[i].topcard === null) {
          // een aas kan altijd
          this.foundationCells[i].topcard = card;
          this.freeCells[j].card = null;
          bAgain= true;
          break;
        }
        if (this.foundationCells[i].topcard !== null
              && this.foundationCells[i].topcard.color === card.color
              && this.foundationCells[i].topcard.num + 1 === card.num
              && this.shouldStack(card)) {
          this.foundationCells[i].topcard = card;
          this.freeCells[j].card = null;
          bAgain = true;
          break;
        }
      } // vrije cellen (linksboven) proberen leeg te maken
    } // alle 4 kaartstapels rechts
    if (!bAgain) {
      if (!this.won) {
        if (this.isAllStacked()) {
          this.won= true;
          if (confirm(congrats() + "\n\nKies  OK  voor een nieuw spel.")) {
            window.location.href = window.location.href;
          }
        }
      }
      break;
    }
    this.game.canvas.width = this.game.canvas.width; // clear
    this.draw();
    setTimeout("gFreecell.table.autoPlayOnTimer()",100);
    break;
  }
}; // Table.autoPlay()

Table.prototype.shouldStack = function (card) {
  // een kaart hoger dan een 2 kan alleen automatisch worden opgestapeld
  // als de stapels van de andere kleur (rood vs zwart) de lagere kaarten
  // bevatten, anders gezegd, een kaart bevatten die minstens zo hoog is.
  if (card.num < 2) {
    return true;
  }
  var score = 0; // score van 2 nodig
  for (var i = 0; i < 4; i++) {
    var card2 = this.foundationCells[i].topcard;
    if (card2 === null) {
      continue; // lege stapel
    }
    if (card2.blackRedEquals(card)) {
      continue;
    }
    if (card2.num >= card.num - 1) {
      score++;
    }
  }
  return score === 2;
}; // Table.shouldStack(card)

Table.prototype.isAllStacked = function () {
  var n = 0;
  for (var i = 0; i < 4; i++) {
    var card = this.foundationCells[i].topcard;
    if (card !== null) {
      n += (card.num + 1);
    }
  }
  return (n === 4 * 13);
}; // Table.isAllStacked()

Table.prototype.backup = function () {
  this.backupData[this.undoNb++] = new TableBackup(this.freeCells, this.foundationCells, this.cascadeCells);
}; // Table.backup()

Table.prototype.undo = function () {
  var iLast = this.undoNb - 2;
  if (iLast < 0) {
    iLast= 0;
  }
  var bak = this.backupData[iLast];
  for (var i = 0; i < 4; i++) {
    this.freeCells[i].card = bak.freeCards[i];
  }
  for (var i = 0; i < 4; i++) {
    this.foundationCells[i].topcard = bak.foundationCards[i];
  }
  for (var i = 0; i < 8; i++) {
    this.cascadeCells[i].cards = new Array();
    for (var j = 0; j < bak.cascadeCards[i].length; j++) {
      this.cascadeCells[i].cards[j] = bak.cascadeCards[i][j];
    }
    this.cascadeCells[i].nbCards = this.cascadeCells[i].cards.length; 
  }
  if (this.undoNb > 1) { // de eerste (beginsituatie) laten we staan
    this.undoNb--; 
  }
}; // Table.undo()

Table.prototype.redo = function () {
  if (this.backupData[this.undoNb] !== null) {
    this.undoNb += 2;
    this.undo();
  }
}; // Table.redo()

Table.prototype.reset = function () {
  var bak0 = this.backupData[0];
  this.backupData = new Array();
  this.backupData[0] = bak0;
  this.undoNb = 1;
  this.undo();
}; // Table.reset()

function TableBackup(freeCells, foundationCells, cascadeCells) {
  this.freeCards = new Array();
  this.foundationCards = new Array();
  this.cascadeCards= new Array();
  for (var i = 0; i < 4; i++) {
	  this.freeCards[i] = freeCells[i].card;
  }
  for (var i = 0; i < 4; i++) {
    this.foundationCards[i] = foundationCells[i].topcard;
  }
  for (var i = 0; i < 8; i++) {
    this.cascadeCards[i] = new Array();
    for (var j = 0; j < cascadeCells[i].nbCards; j++) {
      this.cascadeCards[i][j] = cascadeCells[i].cards[j];
    }
  }
}

function Deck() {
  this.cardsLeft =  0;
  this.cards = new Array();
  this.cardColors = new Array("C", "D", "H", "S"); // C=trèfle club, D=carreau diamond, H=coeur heart, S=pique spade
  this.cardColorsTitle = new Array("club", "diamond", "heart", "spade");
  this.cardNums = new Array("A","2","3","4","5","6","7","8","9","10","J","Q","K");
  this.cardNumsTitle = new Array("ace", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "jack", "queen", "king");
}

Deck.prototype.init = function () {
  this.cards = new Array();
  for(var i = 0; i < 13; i++) {
    for(var j = 0; j < 4; j++) {
      this.cards[this.cards.length] = new Card(this, j, i);
    }
  }
  this.cardsLeft = this.cards.length;
};

Deck.prototype.next = function () {
  var i = Math.floor(Math.random() * this.cardsLeft);
  ret = this.cards[i];
  this.cards[i] = this.cards[this.cardsLeft - 1];
  this.cardsLeft--;
  return ret;
};

function RctDirty() {
  this.x = 0;
  this.y = 0;
  this.w = 0;
  this.h = 0;
}

RctDirty.prototype.clear = function () {
  this.x = 0; this.y = 0; this.w = 0; this.h = 0;
};

RctDirty.prototype.add = function (x, y) {
  if (this.w === 0) {
    this.x = x;
    this.y = y;
    this.w = 1;
    this.h = 1;
  } else {
    if (x < this.x) {
      this.w += (this.x - x);
      this.x = x;
    }
    if (x > this.x + this.w) {
      this.w += (x - (this.x + this.w));
    }
    if (y < this.y) {
      this.h += (this.y - y);
      this.y = y;
    }
    if (y > this.x + this.h){
      this.h += (y - (this.y + this.h));
    }
  }
};

function MovingCards(game) {
  this.game = game;
  this.x = 0;
  this.y = 0;
  this.dx = 0;
  this.dy = 0;
  this.cardsOwner = null;
  this.cards = new Array(); // aCard
}

MovingCards.prototype.has = function (card) {
  if (this.cards.length < 1) {
    return false;
  }
  return (this.cards[0] === card);
};

MovingCards.prototype.drop = function () {
  this.cards = new Array();
  this.cardsOwner = null;
};

MovingCards.prototype.draw = function () {
  if (this.cards.length === 0) {
    return;
  }
  this.x = this.cards[0].x + this.dx;
  this.y = this.cards[0].y + this.dy;
  for (var i = 0; i < this.cards.length; i++) {
    var card = this.cards[i];
    //this.game.context.drawImage(imgDeck, card.num * fSrcCardW, card.color * fSrcCardH, fSrcCardW, fSrcCardH,
    //  movingCards.x, movingCards.y + i * fNoOverlap * fDstCardH, fDstCardW, fDstCardH);
    this.game.context.drawImage(this.game.cardsImage, card.num * this.game.srcCardWidth, card.color * this.game.srcCardHeight, this.game.srcCardWidth,
      this.game.srcCardHeight, this.x, this.y + i * this.game.noOverlap * this.game.dstCardHeight, this.game.dstCardWidth, this.game.dstCardHeight);
  }
}; // MovingCards.draw()

// Card
function Card(deck, color, num) {
  this.deck = deck;
  this.color = color; // iColor 0=trèfle club, 1=carreau diamond, 2=coeur heart, 3=pique spade
  this.num = num; // iNum
  this.x = 0;
  this.y = 0;
}

Card.prototype.getCode = function () {
    return this.deck.cardColors[this.color] + this.deck.cardNums[this.num];
  };

Card.prototype.getTitle = function () {
    return this.deck.cardColorsTitle[this.color] + " " + this.deck.cardNumsTitle[this.num];
  };
  
Card.prototype.blackRedEquals = function (card) {
    if (this.color === card.color) {
      return true;
    } else if (this.color + card.color === 3) {
      return true;
    } else {
      return false;
    }
  };

// FreeCell
function FreeCell(game, pos) {
  this.game = game;
  this.pos = pos;
  this.card = null;
  this.x = 0;
  this.y = 0;
  this.w = 0;
  this.h = 0;
}

FreeCell.prototype.draw = function () {
  // voor de marge tussen kaarten houden we de helft aan van wat
  // in de 'waterval' gebruikt wordt:
  var context = this.game.context;
  var srcCardWidth = this.game.srcCardWidth;
  var srcCardHeight = this.game.srcCardHeight;
  var dstCardWidth = this.game.dstCardWidth;
  var dstCardHeight = this.game.dstCardHeight;
  var fMarg = (this.game.canvas.width - 8 * dstCardWidth) / 7 / 2;
  var fVMarg = 2 * fMarg; // TODO: de juiste waarde hier
  var fIn = dstCardWidth * .02;
  if (this.card === null || this.game.movingCards.cardsOwner === this) {
    this.x = 0 + this.pos * dstCardWidth + this.pos * fMarg + fIn;
    this.y = fVMarg + fIn;
    this.w = dstCardWidth - 2 * fIn;     
    this.h = dstCardHeight - 2 * fIn;
    context.fillStyle = this.game.EMPTY_GREEN;
    roundRect(context, this.x, this.y, this.w, this.h);
    if (this === this.game.fitCell) {
      context.fillStyle = this.game.FILL_BLUE;
      roundRect(context, this.x, this.y, this.w, this.h);
    }
  } else {
    var iSrcX = this.card.num * srcCardWidth;
    var iSrcY = this.card.color * srcCardHeight;
    this.card.x = 0 + this.pos * dstCardWidth + this.pos * fMarg;
    this.card.y = fVMarg;
    context.drawImage(this.game.cardsImage, iSrcX, iSrcY, srcCardWidth, srcCardHeight, this.card.x, this.card.y, dstCardWidth, dstCardHeight);
  }
}; // FreeCell.draw()

FreeCell.prototype.isOver = function (x, y) {
  if (this.card === null) {
    return false;
  }
  if (x < this.card.x || x > this.card.x + this.game.dstCardWidth || y < this.card.y || y > this.card.y + this.game.dstCardHeight) {
    return false; 
  }
  this.game.movingCards.cardsOwner = this;
  this.game.movingCards.cards[0] = this.card;
  this.game.movingCards.x = this.card.x;
  this.game.movingCards.y = this.card.y;
  return true;
}; // FreeCell.isOver(x, y)

FreeCell.prototype.testReceiveMovingCards = function () {
  // onderzoek of de bovenste van de movingCards hier hangen
  // en erop zou kunnen (daar zijn nogal wat voorwaarden aan)
  // De globale variabele cFit krijgt de waarde van dit object
  // als de movingCards deze beter dan de voorgaande overlapt
  if (this.card !== null) {
    return false;
  }
  if (this.game.movingCards.cards.length !== 1) {
    return false;
  }
  var fOver = getOverlap(this.x, this.y, this.w, this.h, this.game.movingCards.x, this.game.movingCards.y, this.game.dstCardWidth, this.game.dstCardHeight);
  if (fOver > this.game.fit) {
    this.game.fit = fOver;
    this.game.fitCell = this;
  }
  return fOver > 0;
}; // FreeCell.testReceiveMovingCards()

FreeCell.prototype.receiveMovingCards = function () {
  if (!(this === this.game.fitCell)) {
    return false;
  }
  this.card = this.game.movingCards.cards[0];
  return true;
};

FreeCell.prototype.releaseMovingCards = function () {
  if (this.card === this.game.movingCards.cards[0]) {
    this.card = null;
  }
}; // FreeCell.releaseMovingCards()

// StackCell
function FoundationCell(game, pos) {
  this.game = game;
  this.pos = pos;
  this.topcard = null;
  this.x = 0;
  this.y = 0;
  this.w = 0;
  this.h = 0;
}

FoundationCell.prototype.draw = function () {
  // voor de marge tussen kaarten houden we de helft aan van wat
  // in de 'waterval' gebruikt wordt:
  var canvas = this.game.canvas;
  var context = this.game.context;
  var srcCardWidth = this.game.srcCardWidth;
  var srcCardHeight = this.game.srcCardHeight;
  var dstCardWidth = this.game.dstCardWidth;
  var dstCardHeight = this.game.dstCardHeight;
  var fMarg = (canvas.width - 8 * dstCardWidth) / 7 / 2;
  var fVMarg = 2 * fMarg; // TODO: de juiste waarde hier
  var fIn= dstCardWidth * .02;
  var fLeft= canvas.width - 4 * dstCardWidth - 3 * fMarg;
  if (this.topcard === null) {
    this.x = fLeft + this.pos * dstCardWidth + this.pos * fMarg + fIn;
    this.y = fVMarg + fIn;
    this.w = dstCardWidth - 2 * fIn;
    this.h = dstCardHeight - 2 * fIn;
    context.fillStyle = this.game.EMPTY_GREEN;
    roundRect(context, this.x, this.y, this.w, this.h);
    if (this === this.game.fitCell) { // cFit
      context.fillStyle = this.game.FILL_BLUE;
      roundRect(context, this.x, this.y, this.w, this.h);
    }
  } else {
    this.x = fLeft + this.pos * dstCardWidth + this.pos * fMarg;
    this.y = fVMarg;
    this.w = dstCardWidth;
    this.h = dstCardHeight;
    var iSrcX = this.topcard.num * srcCardWidth;
    var iSrcY = this.topcard.color * srcCardHeight;
    this.topcard.x = this.x;
    this.topcard.y = this.y;
    context.drawImage(this.game.cardsImage, iSrcX, iSrcY, srcCardWidth, srcCardHeight, this.x, this.y, dstCardWidth, dstCardHeight);
    if (this === this.game.fitCell) { // cFit
      context.fillStyle = this.game.FILL_BLUE;
      roundRect(context, this.x, this.y, this.w, this.h);
    }
  }
}; // FoundationCell.draw()

FoundationCell.prototype.testReceiveMovingCards = function () {
  // (voor beschrijving: zie boven onder de klasse FreeCell)
  if (this.game.movingCards.cards.length !== 1) {
    return false; // je kunt maar Ã©Ã©n kaart tegelijk optassen
  }
  var mcard = this.game.movingCards.cards[0];
  if (this.topcard === null) {
    if (mcard.num > 0) {
      return false;
    }
  } else {
    if (mcard.color !== this.topcard.color) {
      return false;
    }
    if (mcard.num !== this.topcard.num + 1) {
      return false;
    }
  }
  var fOver = getOverlap(this.x, this.y, this.w, this.h, this.game.movingCards.x, this.game.movingCards.y, this.game.dstCardWidth, this.game.dstCardHeight);
  if (fOver > this.game.fit) {
    this.game.fit = fOver;
    this.game.fitCell = this;
  }
  return fOver > 0;
}; // FoundationCell.testReceiveMovingCards()

FoundationCell.prototype.receiveMovingCards = function () {
  if (!(this === this.game.fitCell)) {
    return false;
  }
  this.topcard = this.game.movingCards.cards[0];
  return true;
};

// CascadeCell
function CascadeCell(game, pos) {
  this.game = game;
  this.pos = pos;
  this.cards = new Array();
  this.nbCards = 0;
  this.x = 0;
  this.y = 0;
}

CascadeCell.prototype.add = function (card) {
  this.cards[this.nbCards] = card;
  this.nbCards++;
};

CascadeCell.prototype.getTopCard = function () {
  if (this.nbCards === 0) {
    return null;
  }
  return this.cards[this.nbCards - 1];
};

CascadeCell.prototype.draw = function () {
  var canvas = this.game.canvas;
  var context = this.game.context;
  var rctDirty = this.game.rctDirty;
  var srcCardWidth = this.game.srcCardWidth;
  var srcCardHeight = this.game.srcCardHeight;
  var dstCardWidth = this.game.dstCardWidth;
  var dstCardHeight = this.game.dstCardHeight;
  var fMarg = (canvas.width - 8 * dstCardWidth) / 7;
  var fVMarg = dstCardHeight * 1.1;
  var fIn = dstCardWidth * .02;
  var fLeft = this.pos * dstCardWidth + this.pos * fMarg;
  if (rctDirty.w > 0) {
    if (rctDirty.x > fLeft + dstCardWidth || rctDirty.x + rctDirty.w < fLeft) {
      return;
    }
  }
  this.x = fLeft;
  this.y = fVMarg;
  if (this.nbCards === 0 || this.game.movingCards.has(this.cards[0])) {
    context.fillStyle = this.game.EMPTY_GREEN;
    roundRect(context, fLeft + fIn, fVMarg + fIn, dstCardWidth - 2 * fIn, dstCardHeight - 2 * fIn);
    if (this === this.game.fitCell) {
      context.fillStyle = this.game.FILL_BLUE;
      roundRect(context, fLeft + fIn, fVMarg + fIn, dstCardWidth - 2 * fIn, dstCardHeight - 2 * fIn);
    }
  } else {
    var fOverlapH = (canvas.height - fVMarg - dstCardHeight) / (this.nbCards === 1 ? 1 : this.nbCards - 1);
    if (fOverlapH > dstCardHeight * this.game.noOverlap) {
      fOverlapH = dstCardHeight * this.game.noOverlap;
    }
    for (var i = 0 ; i < this.nbCards; i++){
      var card = this.cards[i];
      if (this.game.movingCards.has(card)) {
        break;
      }
      var iSrcX = card.num * srcCardWidth;
      var iSrcY = card.color * srcCardHeight;
      card.x = fLeft;
      card.y = fVMarg + i * fOverlapH;
      context.drawImage(this.game.cardsImage, iSrcX, iSrcY, srcCardWidth, srcCardHeight, card.x, card.y, dstCardWidth, dstCardHeight);
    }
    if (this === this.game.fitCell) {
      context.fillStyle= this.game.FILL_BLUE;
      roundRect(context, fLeft, fVMarg + (this.nbCards - 1) * fOverlapH, dstCardWidth, dstCardHeight);
    }
  }
}; // CascadeCell.draw()

CascadeCell.prototype.isOver = function (x, y) {
  //log("CascadeCell.isOver(x = " + x + ", y = " + y + ")");
  if (this.nbCards < 1) {
    //log("CascadeCell.isOver(x = " + x + ", y = " + y + ") : false, nbCards < 1 (" + this.nbCards + ")");
    return false;
  }
  // nu we weten hoe groot de kaarten zijn,
  // kunnen we bepalen of de (x, y) op de onderste ligt
  var cardN = this.cards[this.nbCards - 1];
  if (x > cardN.x && x < cardN.x + this.game.dstCardWidth && y > cardN.y && y < cardN.y + this.game.dstCardHeight) {
    this.game.movingCards.cardsOwner = this;
    this.game.movingCards.cards[0] = cardN;
    this.game.movingCards.x = cardN.x;
    this.game.movingCards.y = cardN.y;
    //log("CascadeCell.isOver(x = " + x + ", y = " + y + ") : true, take last card");
    return true;
  }
  var cBelow = cardN;
  // de onderste kaart is dan wel niet aangeklikt, maar een hogere?
  var nMax = this.game.table.getMoveCapacity();
  //log("CascadeCell.isOver(x = " + x + ", y = " + y + ") : nMax " + nMax);
  for(var i = this.nbCards - 2; i >= 0; i--) {
    var card = this.cards[i];
    if (card.blackRedEquals(cBelow) || card.num !== cBelow.num + 1) {
      //log("CascadeCell.isOver(x = " + x + ", y = " + y + ") : false, card " + i + " is not part of suite");
      return false; // kaart past niet bij die daaronder
    }
    // voordat ik ga kijken of op deze kaart is geklikt:
    // zou ik dit aantal ineens kunnen verslepen?
    var nToPark = this.nbCards - i; 
    if (nToPark > nMax) {
      //log("CascadeCell.isOver(x = " + x + ", y = " + y + ") : false, card is over nMax (" + nMax + ") (" + nToPark + ")");
      return false;
    }
    // nu controleren of de muis boven deze kaart zit
    if (x > card.x && x < card.x + this.game.dstCardWidth && y > card.y && y < card.y + this.game.dstCardHeight) {
      // idd, nu het hele stapeltje afgeven
      this.game.movingCards.cardsOwner = this;
      this.game.movingCards.x = cardN.x;
      this.game.movingCards.y = cardN.y;
      for(var j = i; j < this.nbCards; j++) {
        this.game.movingCards.cards[j - i] = this.cards[j];
      }
      //log("CascadeCell.isOver(x = " + x + ", y = " + y + ") : true, take cards from " + i + " to " + this.nbCards - 1);
      return true;
    }
    cBelow = card;
  }
  return false;
}; // CascadeCell.isOver(x, y)

CascadeCell.prototype.testReceiveMovingCards = function () {
  if (this.game.movingCards.cardsOwner === this) {
    return false;
  }
  var iy = this.nbCards === 0 ? 0 : this.nbCards - 1;
  var dy = iy * (this.game.dstCardHeight * this.game.noOverlap);
  var fOver = getOverlap(this.x, this.y + dy, this.game.dstCardWidth, this.game.dstCardHeight, this.game.movingCards.x, this.game.movingCards.y,
    this.game.dstCardWidth, this.game.dstCardHeight);
  if (fOver <= this.game.fit) {
    return false; // we hoeven niet eens te onderzoeken of het mag
  }
  var mcard = this.game.movingCards.cards[0]; // il manquait var, mcard était global, cf freecell.js 544
  if (this.nbCards > 0) {
    // liggen er nog kaarten, dat moet het nieuwe setje passen
    var cardN = this.cards[this.nbCards - 1];
    if (mcard.blackRedEquals(cardN)) {
      return false; // dezelfde kleur mag zeker niet
    }
    if (mcard.num !== cardN.num - 1) {
      return false; // nummer is niet aflopend
    }
  } else {
    // als het een vrije plek is, moeten we nog even
    // het totaal aantal vrije plekken tellen
    if (this.game.table.getMoveCapacityMin1() < this.game.movingCards.cards.length) {
      return false;
    }
  }
  this.game.fit = fOver;
  this.game.fitCell = this;
  return fOver > 0;
}; // CascadeCell.testReceiveMovingCards()

CascadeCell.prototype.receiveMovingCards = function () {
  if (!(this === this.game.fitCell)) {
    return false;
  }
  for (var i = 0; i < this.game.movingCards.cards.length; i++) {
    this.cards[this.nbCards++] = this.game.movingCards.cards[i];
  }
  return true;
}; // CascadeCell.receiveMovingCards()

CascadeCell.prototype.releaseMovingCards = function () {
  for (var i = 0; i < this.nbCards; i++) {
    if (this.cards[i] === this.game.movingCards.cards[0]) {
      this.nbCards= i;
      break;
    }
  }
}; // CascadeCell.releaseMovingCards()

/**
 * Draws a rounded rectangle using the current state of the canvas. 
 * @param {CanvasRenderingContext2D} ctx
 * @param {Number} x The top left x coordinate
 * @param {Number} y The top left y coordinate 
 * @param {Number} width The width of the rectangle 
 * @param {Number} height The height of the rectangle
 */
function roundRect(ctx, x, y, width, height) {
  var radius = width / 15;
  ctx.beginPath();
  ctx.moveTo(x + radius, y);
  ctx.lineTo(x + width - radius, y);
  ctx.quadraticCurveTo(x + width, y, x + width, y + radius);
  ctx.lineTo(x + width, y + height - radius);
  ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
  ctx.lineTo(x + radius, y + height);
  ctx.quadraticCurveTo(x, y + height, x, y + height - radius);
  ctx.lineTo(x, y + radius);
  ctx.quadraticCurveTo(x, y, x + radius, y);
  ctx.closePath();
  ctx.fill();
}

function getOverlap(x1, y1, w1, h1, x2, y2, w2, h2) {
  if (x1 > x2 + w2) {
    return 0;
  }
  if (x2 > x1 + w1) {
    return 0;
  }
  if (y1 > y2 + h2) {
    return 0;
  }
  if (y2 > y1 + h1) {
    return 0;
  }
  var a1 = w1 * h1;
  var a2 = w2 * h2;
  var aM = a1 < a2 ? a1 : a2; // kleinste bepaalt maximale overlap
  var dx = x2 > x1 ? x1 + w1 - x2 : x2 + w2 - x1;
  var dy = y2 > y1 ? y1 + h1 - y2 : y2 + h2 - y1;
  return dx * dy / aM;
}

function congrats() {
  var msgs = [
    "Hoera, gewonnen!",
    "Leuk, alweer gescoord.",
    "Nou, dat kan wel wat sneller hoor...",
    "Geweldig!",
    /* overige: */
    "Gefeliciteerd! Ga zo door.",             
    "Van harte!",
    "Er zijn nog bijna 52! andere puzzels, dus...",
    "Heel goed.",
    "Zo moet dat dus.",
    "Goed hoor."
  ];
  return msgs[Math.floor(Math.random() * msgs.length)];
}
