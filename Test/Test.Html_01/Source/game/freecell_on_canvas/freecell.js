/** door K, januari 2012, voor C */
/*
Freecell Voor Canvas is vrije software; herdistributie en/of het aanbrengen 
van wijzigingen is toegestaan onder de voorwaarden vermeld in de GNU 
General Public License die gepubliceerd is door de Free Software Foundation; 
ofwel versie 3 van de licentie of, (zo u wilt) elke latere versie.

Freecell Voor Canvas wordt gedistribueerd met de hoop dat het bruikbaar zal 
zijn, maar zonder enige garantie; zelfs niet een ge√Ømpliceerde garantie van 
verkoopbaarheid of geschiktheid voor een bepaald doel. Zie de GNU General 
Public License voor meer informatie.

You should have received a copy of the GNU General Public License along with 
this program.  If not, see <http://www.gnu.org/licenses/>.
*/

/** ruimte in de breedte voor een kaart in de 'waterval' */
var fWidthUse= 1.06;
/** het deel dat vertikaal niet overlapt, rond 20% */
var fNoOverlap= .2;
/** de mate waarin de movingCards boven een cel hangen (0...1) */
var fFit= 0;
/** de cel waar de movingCards het dichtst bij zijn */
var cFit= null;
/** kleur die aangeeft dat je een kaart kunt plaatsen */
var FILL_BLUE= "rgba(0, 0, 200, 0.5)";
/** kleur van een lege cel */
var EMPTY_GREEN= "#578132";
/** breedte van een kaart in de grote bitmap met alle kaarten */
var fSrcCardW= 0;
/** hoogte van een kaart in de grote bitmap met alle kaarten */
var fSrcCardH= 0;
/** breedte van een kaart in beeld */
var fDstCardW= 0;
/** hoogte van een kaart in beeld */
var fDstCardH= 0;
/** beschikbare hoogte in beeld */
var fCanvasH= 0;

function updateCardDim(canvas, imgDeck){
  fCanvasH= canvas.height;
  if (fSrcCardW==0){
    fSrcCardW= imgDeck.width / 13;
    fSrcCardH= imgDeck.height / 5;
  }
  fDstCardW= canvas.width / (8 + 7 * (fWidthUse - 1));
  fDstCardH= fSrcCardH * fDstCardW / fSrcCardW;
}

function congrats(){
  var aMsg= [
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
  return aMsg[Math.floor(Math.random() * aMsg.length)];
}

/** de rechthoek die hertekend moet worden */ // TODO: in gebruik nemen
var rctDirty={
  x: 0,
  y: 0,
  w: 0,
  h: 0,
  clear: function(){
    x= 0; y= 0; w= 0; h= 0;
  },
  add: function(x, y){
    if (this.w==0){
      this.x= x;
      this.y= y;
      this.w= 1;
      this.h= 1;
    } else{
      if (x < this.x){
        this.w += (this.x - x);
        this.x= x;
      }
      if (x > this.x + this.w){
        this.w += (x - (this.x + this.w));
      }
      if (y < this.y){
        this.h += (this.y - y);
        this.y= y;
      }
      if (y > this.x + this.h){
        this.h += (y - (this.y + this.h));
      }
    }
  } // add(x, y)
}; // var rctDirty


function Card(iColor, iNum){
  this.iColor= iColor; // 0=klaver, 1=ruiten, 2=harten, 3=schoppen
  this.iNum= iNum;
}
var asCardColor= new Array("K","R","H","S");
var asCardColorTitle= new Array("klaver","ruiten","harten","schoppen");
var asCardNum= new Array(
    "A","2","3","4","5","6","7","8","9","10","B","V","H");
var asCardNumTitle= new Array(
    "aas","twee","drie","vier","vijf","zes","zeven","acht","negen","tien",
    "boer","vrouw","heer");
Card.prototype= {
  x: 0, // ingevuld na plaatsing in beeld
  y: 0, // ingevuld na plaatsing in beeld
  /**
   * @return kleurnaam|nummernaam, dus bijvoorbeeld KA voor klaver-aas.
   */
  getCode: function(){
    return asCardColor[this.iColor] + asCardNum[this.iNum];
  },
  getTitle: function(){
    return asCardColorTitle[this.iColor] + " " + asCardNumTitle[this.iNum];
  }, // getTitle: function()
  /** @return true bij 0+0, 1+1, 2+2, 3+3, 0+3, 1+2 */
  blackRedEquals: function(card){
    if (this.iColor==card.iColor){
      return true;
    } else if (this.iColor + card.iColor == 3){
      return true;
    } else{
      return false;
    }
  }
}; // Card.prototype

/**
 * geef de mate van overlap tussen twee rechthoeken
 * @return een getal tussen 0 en 1
 */
function getOverlap(x1, y1, w1, h1, x2, y2, w2, h2){
  if (x1 > x2 + w2){
    return 0;
  }
  if (x2 > x1 + w1){
    return 0;
  }
  if (y1 > y2 + h2){
    return 0;
  }
  if (y2 > y1 + h1){
    return 0;
  }
  var a1= w1 * h1;
  var a2= w2 * h2;
  var aM= a1 < a2 ? a1 : a2; // kleinste bepaalt maximale overlap
  var dx= x2 > x1 ? x1 + w1 - x2 : x2 + w2 - x1;
  var dy= y2 > y1 ? y1 + h1 - y2 : y2 + h2 - y1;
  return dx * dy / aM;
}

var movingCards= { // singleton voor wat ik versleep
  /** positie linkerbovenhoek bovenste kaart */
  x: 0,
  y: 0,
  dx: 0,
  dy: 0,
  cardsOwner: null,
  aCard: new Array(),
  drop: function(){
    this.aCard= new Array();
    this.cardsOwner= null;
  },
  has: function(card){
    if (this.aCard.length < 1){
      return false;
    }
    return (this.aCard[0]==card);
  },
  draw: function(g, canvas, imgDeck){
    if (this.aCard.length==0){
      return;
    }
    this.x= this.aCard[0].x + this.dx;  
    this.y= this.aCard[0].y + this.dy;  
    for (var i= 0; i < this.aCard.length; i++){
      var card= this.aCard[i];
//      g.drawImage(imgDeck, 
//          card.iNum   * fSrcCardW, 
//          card.iColor * fSrcCardH, 
//          fSrcCardW, fSrcCardH, 
//          movingCards.x, movingCards.y + i * fNoOverlap * fDstCardH, 
//          fDstCardW, fDstCardH);
      _drawImage(g, imgDeck, 
          card.iNum   * fSrcCardW, 
          card.iColor * fSrcCardH, 
          fSrcCardW, fSrcCardH, 
          movingCards.x, movingCards.y + i * fNoOverlap * fDstCardH, 
          fDstCardW, fDstCardH);
    }
  } // draw(g, canvas, imgDeck)
};

var Deck= {
  nCardsLeft: 0,
  aCard: new Array(),
  init: function(){
    this.aCard= new Array();
    for(var i= 0; i < 13; i++){
      for(var j= 0; j < 4; j++){
        this.aCard[this.aCard.length]= new Card(j, i);
      }
    }
    this.nCardsLeft= this.aCard.length;
  },
  /**
   * @returns een willekeurige volgende kaart
   */
  next: function(){
    var i= Math.floor(Math.random() * this.nCardsLeft);
    cRet= this.aCard[i];
    this.aCard[i]= this.aCard[this.nCardsLeft - 1];
    this.nCardsLeft--;
    return cRet;
  }
};
Deck.init();

/**
 * Maak een vrije cel (linksboven)
 * @param pos positie: 0 voor de eerste, 3 voor de laatste
 * @returns {FreeCell}
 */
function FreeCell(pos){
  this.pos= pos;
}

FreeCell.prototype= {
  card: null,
  x: 0,
  y: 0,
  w: 0,
  h: 0,
  draw: function(g, canvas, imgDeck){
    // voor de marge tussen kaarten houden we de helft aan van wat
    // in de 'waterval' gebruikt wordt:
    var fMarg= (canvas.width - 8*fDstCardW) / 7 / 2;
    var fVMarg= 2*fMarg; // TODO: de juiste waarde hier
    var fIn= fDstCardW * .02;
    if (this.card==null || movingCards.cardsOwner==this){
      this.x= 0 + this.pos*fDstCardW + this.pos*fMarg + fIn;
      this.y= fVMarg + fIn;
      this.w= fDstCardW - 2 * fIn;     
      this.h= fDstCardH - 2 * fIn;
      g.fillStyle= EMPTY_GREEN;
      //roundRect(g, this.x, this.y, this.w, this.h);
      _roundRect(g, this.x, this.y, this.w, this.h);
      if (this == cFit){
        g.fillStyle= FILL_BLUE;
        //roundRect(g, this.x, this.y, this.w, this.h);
        _roundRect(g, this.x, this.y, this.w, this.h);
      }
    } else{
      var iSrcX= this.card.iNum * fSrcCardW;
      var iSrcY= this.card.iColor * fSrcCardH;
      this.card.x= 0 + this.pos*fDstCardW + this.pos*fMarg;
      this.card.y= fVMarg;
//      g.drawImage(imgDeck, 
//          iSrcX, iSrcY, 
//          fSrcCardW, fSrcCardH, 
//          this.card.x, this.card.y, 
//          fDstCardW, fDstCardH);
      _drawImage(g, imgDeck, 
          iSrcX, iSrcY, 
          fSrcCardW, fSrcCardH, 
          this.card.x, this.card.y, 
          fDstCardW, fDstCardH);
    }
  }, // draw(g, canvas, imgDeck)
  isOver: function(x, y){
    if (this.card==null){
      return false;
    }
    if (x < this.card.x || x > this.card.x + fDstCardW ||
        y < this.card.y || y > this.card.y + fDstCardH){
      return false; 
    }
    movingCards.cardsOwner= this;
    movingCards.aCard[0]= this.card;
    movingCards.x= this.card.x;
    movingCards.y= this.card.y;
    return true;
  }, // isOver(x, y)
  testReceiveMovingCards: function(){
    // onderzoek of de bovenste van de movingCards hier hangen
    // en erop zou kunnen (daar zijn nogal wat voorwaarden aan)
    // De globale variabele cFit krijgt de waarde van dit object
    // als de movingCards deze beter dan de voorgaande overlapt
    if (this.card != null){
      return false;
    }
    if (movingCards.aCard.length != 1){
      return false;
    }
    var fOver= getOverlap(this.x, this.y, this.w, this.h, 
        movingCards.x, movingCards.y, fDstCardW, fDstCardH);
    if (fOver > fFit){
      fFit= fOver;
      cFit= this;
    }
    return fOver > 0;
  }, // testReceiveMovingCards()
  receiveMovingCards: function(){
    if (!(this==cFit)){
      return false;
    }
    this.card= movingCards.aCard[0];
    return true;
  },
  releaseMovingCards: function(){
    if (this.card==movingCards.aCard[0]){
      this.card= null;
    }
  } // releaseMovingCards()
}; // FreeCell.prototype

/**
 * Maak een stapelcel (rechtsboven)
 * @param pos positie: 0 voor de eerste, 3 voor de laatste
 * @returns {StackCell}
 */
function StackCell(pos){
  this.pos= pos;
}

StackCell.prototype= {
  topcard: null,
  x: 0,
  y: 0,
  w: 0,
  h: 0,
  draw: function(g, canvas, imgDeck){
    // voor de marge tussen kaarten houden we de helft aan van wat
    // in de 'waterval' gebruikt wordt:
    var fMarg= (canvas.width - 8*fDstCardW) / 7 / 2;
    var fVMarg= 2*fMarg; // TODO: de juiste waarde hier
    var fIn= fDstCardW * .02;
    var fLeft= canvas.width - 4 * fDstCardW - 3 * fMarg;
    if (this.topcard==null){
      this.x= fLeft + this.pos*fDstCardW + this.pos*fMarg + fIn;
      this.y= fVMarg + fIn;
      this.w= fDstCardW - 2 * fIn;
      this.h= fDstCardH - 2 * fIn;
      g.fillStyle= EMPTY_GREEN;
      //roundRect(g, this.x, this.y, this.w, this.h);
      _roundRect(g, this.x, this.y, this.w, this.h);
      if (this == cFit){
        g.fillStyle= FILL_BLUE;
        //roundRect(g, this.x, this.y, this.w, this.h);
        _roundRect(g, this.x, this.y, this.w, this.h);
      }
    } else{
      this.x= fLeft + this.pos*fDstCardW + this.pos*fMarg;
      this.y= fVMarg;
      this.w= fDstCardW;
      this.h= fDstCardH;
      var iSrcX= this.topcard.iNum * fSrcCardW;
      var iSrcY= this.topcard.iColor * fSrcCardH;
      this.topcard.x= this.x;
      this.topcard.y= this.y;
//      g.drawImage(imgDeck, 
//          iSrcX, iSrcY, 
//          fSrcCardW, fSrcCardH, 
//          this.x, this.y, 
//          fDstCardW, fDstCardH);
      _drawImage(g, imgDeck, 
          iSrcX, iSrcY, 
          fSrcCardW, fSrcCardH, 
          this.x, this.y, 
          fDstCardW, fDstCardH);
      if (this == cFit){
        g.fillStyle= FILL_BLUE;
        //roundRect(g, this.x, this.y, this.w, this.h);
        _roundRect(g, this.x, this.y, this.w, this.h);
      }
    }
  }, // draw(g, canvas, imgDeck)
  testReceiveMovingCards: function(){
    // (voor beschrijving: zie boven onder de klasse FreeCell)
    if (movingCards.aCard.length != 1){
      return false; // je kunt maar √©√©n kaart tegelijk optassen
    }
    var mcard= movingCards.aCard[0];
    if (this.topcard==null){
      if (mcard.iNum > 0){
        return false;
      }
    } else{
      if (mcard.iColor != this.topcard.iColor){
        return false;
      }
      if (mcard.iNum != this.topcard.iNum + 1){
        return false;
      }
    }
    var fOver= getOverlap(this.x, this.y, this.w, this.h, 
        movingCards.x, movingCards.y, fDstCardW, fDstCardH);
    if (fOver > fFit){
      fFit= fOver;
      cFit= this;
    }
    return fOver > 0;
  }, // testReceiveMovingCards()
  receiveMovingCards: function(){
    if (!(this==cFit)){
      return false;
    }
    this.topcard= movingCards.aCard[0];
    return true;
  } 
}; // StackCell.prototype

/**
 * Maak een kolom (onderste helft)
 * @param pos positie: 0 voor de eerste, 7 voor de laatste
 * @returns {CascadeCell}
 */
function CascadeCell(pos){
  this.pos= pos;
  this.aCard= new Array();
  this.nCards= 0;
  this.x= 0;
  this.y= 0;
}

CascadeCell.prototype= {
  add: function(card){
    this.aCard[this.nCards]= card;
    this.nCards++;
  },
  getTopCard: function(){
    if (this.nCards==0){
      return null;
    }
    return this.aCard[this.nCards - 1];
  },
  draw: function(g, canvas, imgDeck){
    var fMarg= (canvas.width - 8*fDstCardW) / 7;
    var fVMarg= fDstCardH * 1.1;
    var fIn= fDstCardW * .02;
    var fLeft= this.pos*fDstCardW + this.pos*fMarg;
    if (rctDirty.w > 0){
      if (rctDirty.x > fLeft + fDstCardW
          || rctDirty.x + rctDirty.w < fLeft){
        return;
      }
    }
    this.x= fLeft;
    this.y= fVMarg;
    if (this.nCards==0 || movingCards.has(this.aCard[0])){
      g.fillStyle= EMPTY_GREEN;
//      roundRect(g, fLeft + fIn, fVMarg + fIn,
//          fDstCardW - 2 * fIn,
//          fDstCardH - 2 * fIn);
      _roundRect(g, fLeft + fIn, fVMarg + fIn,
          fDstCardW - 2 * fIn,
          fDstCardH - 2 * fIn);
      if (this == cFit){
        g.fillStyle= FILL_BLUE;
//        roundRect(g, fLeft + fIn, fVMarg + fIn,
//            fDstCardW - 2 * fIn,
//            fDstCardH - 2 * fIn);
        _roundRect(g, fLeft + fIn, fVMarg + fIn,
            fDstCardW - 2 * fIn,
            fDstCardH - 2 * fIn);
      }
    } else{
      var fOverlapH= (fCanvasH - fVMarg - fDstCardH) / 
          (this.nCards == 1 ? 1 : this.nCards - 1);
      if (fOverlapH > fDstCardH * fNoOverlap){
        fOverlapH= fDstCardH * fNoOverlap;
      }
      for (var i= 0 ; i < this.nCards; i++){
        var card= this.aCard[i];
        if (movingCards.has(card)){
          break;
        }
        var iSrcX= card.iNum * fSrcCardW;
        var iSrcY= card.iColor * fSrcCardH;
        card.x= fLeft;
        card.y= fVMarg + i * fOverlapH;
//        g.drawImage(imgDeck, 
//          iSrcX, iSrcY, 
//          fSrcCardW, fSrcCardH, 
//          card.x, card.y, 
//          fDstCardW, fDstCardH);
        _drawImage(g, imgDeck, 
          iSrcX, iSrcY, 
          fSrcCardW, fSrcCardH, 
          card.x, card.y, 
          fDstCardW, fDstCardH);
      }
      if (this == cFit){
        g.fillStyle= FILL_BLUE;
//        roundRect(g, 
//            fLeft, fVMarg + (this.nCards - 1) * fOverlapH,
//            fDstCardW, fDstCardH);
        _roundRect(g, 
            fLeft, fVMarg + (this.nCards - 1) * fOverlapH,
            fDstCardW, fDstCardH);
      }
    }
  }, // draw(g, canvas, imgDeck)
  /**
   * Kijk of er op een kaart geklikt is en zo ja,
   * zet dan die kaart (of het deel van de kolom)
   * in de singleton movingCards.  
   * @param {Number} x The top left x coordinate
   * @param {Number} y The top left y coordinate 
   */
  isOver: function(x, y){
    if (this.nCards < 1){
      return false;
    }
    // nu we weten hoe groot de kaarten zijn,
    // kunnen we bepalen of de (x, y) op de onderste ligt
    var cardN= this.aCard[this.nCards - 1];
    if (x > cardN.x && x < cardN.x + fDstCardW &&
        y > cardN.y && y < cardN.y + fDstCardH){
      movingCards.cardsOwner= this;
      movingCards.aCard[0]= cardN;
      movingCards.x= cardN.x;
      movingCards.y= cardN.y;
      return true;
    }
    var cBelow= cardN;
    // de onderste kaart is dan wel niet aangeklikt, maar een hogere?
    var nMax= table.getMoveCapacity();
    for(var i= this.nCards - 2; i >= 0; i--){
      var card= this.aCard[i];
      if (card.blackRedEquals(cBelow)
          || card.iNum != cBelow.iNum + 1){
        return false; // kaart past niet bij die daaronder
      }
      // voordat ik ga kijken of op deze kaart is geklikt:
      // zou ik dit aantal ineens kunnen verslepen?
      var nToPark= this.nCards - i; 
      if (nToPark > nMax){
        return false;
      }
      // nu controleren of de muis boven deze kaart zit
      if (x > card.x && x < card.x + fDstCardW &&
          y > card.y && y < card.y + fDstCardH){
        // idd, nu het hele stapeltje afgeven
        movingCards.cardsOwner= this;
        movingCards.x= cardN.x;
        movingCards.y= cardN.y;
        for(var j= i; j < this.nCards; j++){
          movingCards.aCard[j-i]= this.aCard[j];
        }
        return true;
      }
      cBelow= card;
    }
    return false;
  }, // isOver(x, y)
  /** of de gesleepte stapel kaarten op deze kolom mag */
  testReceiveMovingCards: function(){
    if (movingCards.cardsOwner==this){
      return false;
    }
    var iy= this.nCards==0 ? 0 : this.nCards - 1;
    var dy= iy * (fDstCardH * fNoOverlap);
    var fOver= getOverlap(this.x, this.y + dy, fDstCardW, fDstCardH, 
        movingCards.x, movingCards.y, fDstCardW, fDstCardH);
    if (fOver <= fFit){
      return false; // we hoeven niet eens te onderzoeken of het mag
    }
    mcard= movingCards.aCard[0];
    if (this.nCards > 0){
      // liggen er nog kaarten, dat moet het nieuwe setje passen
      var cardN= this.aCard[this.nCards - 1];
      if (mcard.blackRedEquals(cardN)){
        return false; // dezelfde kleur mag zeker niet
      }
      if (mcard.iNum != cardN.iNum - 1){
        return false; // nummer is niet aflopend
      }
    } else{
      // als het een vrije plek is, moeten we nog even
      // het totaal aantal vrije plekken tellen
      if (table.getMoveCapacityMin1() < movingCards.aCard.length){
        return false;
      }
    }
    fFit= fOver;
    cFit= this;
    return fOver > 0;
  }, // testReceiveMovingCards()
  receiveMovingCards: function(){
    if (!(this==cFit)){
      return false;
    }
    for (var i= 0; i < movingCards.aCard.length; i++){
      this.aCard[this.nCards++]= movingCards.aCard[i];
    }
    return true;
  }, // receiveMovingCards()
  releaseMovingCards: function(){
    for (var i= 0; i < this.nCards; i++){
      if (this.aCard[i]==movingCards.aCard[0]){
        this.nCards= i;
        break;
      }
    } 
  } // releaseMovingCards()
};   

/**
 * Draws a rounded rectangle using the current state of the canvas. 
 * @param {CanvasRenderingContext2D} ctx
 * @param {Number} x The top left x coordinate
 * @param {Number} y The top left y coordinate 
 * @param {Number} width The width of the rectangle 
 * @param {Number} height The height of the rectangle
 */
function roundRect(ctx, x, y, width, height) {
  var radius = width/15;
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

/**
 * Maak een kopie van de cellen, voor later herstel
 * @param aFreeCell
 * @param aStackCell
 * @param aCascadeCell
 * @returns {tableBackup}
 */
function TableBackup(aFreeCell, aStackCell, aCascadeCell){
  this.aFreeCard= new Array();
  this.aStackCard= new Array();
  this.aCascadeCards= new Array();
  for (var i= 0; i < 4; i++){
	  this.aFreeCard[i]= aFreeCell[i].card;
  }
  for (var i= 0; i < 4; i++){
    this.aStackCard[i]= aStackCell[i].topcard;
  }
  for (var i= 0; i < 8; i++){
    this.aCascadeCards[i]= new Array();
    for (var j= 0; j < aCascadeCell[i].nCards; j++){
      this.aCascadeCards[i][j]= aCascadeCell[i].aCard[j];
    }  
  }
};

/**
 * De virtuele tafel met 4+4+8 cellen met kaarten
 */
var table={
  aFreeCell: new Array(), // cellen linksboven
  aStackCell: new Array(), // cellen rechtsboven
  aCascadeCell: new Array(), // cellen met uitgespreide kaartkolommen
  aBackup: new Array(), // met instanties van TableBackup
  nUndo: 0, // aantal stappen dat we nog terug kunnen
  canvas: null, // gezet voor de autoPlay-timer
  imgDeck: null, // gezet voor de autoPlay-timer
  bWon: false, // na winst komt de felicitatie - ÈÈn keer
  /** plaats de cellen en verdeel de kaarten */
  init: function(){
    for (var i= 0; i < 4; i++){
      this.aFreeCell[i]= new FreeCell(i);
    }  
    for (var i= 0; i < 4; i++){
      this.aStackCell[i]= new StackCell(i);
    }  
    for (var i= 0; i < 8; i++){
      this.aCascadeCell[i]= new CascadeCell(i);
    }
    for(var i= 0; i < 4*13; i++){
      var y= Math.floor(i / 8);
      var x= i - y * 8;  
      this.aCascadeCell[x].add(Deck.next());
    }
  },
  /** laat de cellen hun kaarten tekenen */
  draw: function(g, canvas, imgDeck){
    for(var i= 0; i < 4; i++){
      this.aFreeCell[i].draw(g, canvas, imgDeck);
    }  
    for(var i= 0; i < 4; i++){
      this.aStackCell[i].draw(g, canvas, imgDeck);
    }  
    for(var i= 0; i < 8; i++){
      this.aCascadeCell[i].draw(g, canvas, imgDeck);
    }  
  },
  /** doe alles wat automatisch kan */
  autoPlay: function(canvas, imgDeck){
    this.canvas= canvas;
    this.imgDeck= imgDeck;
    setTimeout("table.autoPlayOnTimer()",100);
  },
  autoPlayOnTimer: function(){
    var canvas= this.canvas;
    var imgDeck= this.imgDeck;
    var g= canvas.getContext("2d");
    for (;;){
      var bAgain= false;
      // loop de stapels af en zoek kaarten die erop kunnen
      for(var i= 0; i < 4; i++){
        for(var j= 0; j < 8; j++){
          var card= this.aCascadeCell[j].getTopCard();
          if (card==null){
            continue;
          }
          if (card.iNum==0 && this.aStackCell[i].topcard==null){
            // aas wegleggen
            this.aStackCell[i].topcard= card;
            this.aCascadeCell[j].nCards--;
            bAgain= true;
            break;
          } else if (this.aStackCell[i].topcard != null
                && this.aStackCell[i].topcard.iColor == card.iColor
                && this.aStackCell[i].topcard.iNum + 1 == card.iNum
                && this.shouldStack(card)){
            // stapel verhogen
            this.aStackCell[i].topcard= card;
            this.aCascadeCell[j].nCards--;
            bAgain= true;
            break;
          }
        } // kaartkolommen (onderste helft) proberen af te stapelen
        if (bAgain){
          break;
        }
        for(var j= 0; j < 4; j++){
          var card= this.aFreeCell[j].card;
          if (card==null){
            continue;
          }
          if (card.iNum==0 && this.aStackCell[i].topcard == null){
            // een aas kan altijd
            this.aStackCell[i].topcard= card;
            this.aFreeCell[j].card= null;
            bAgain= true;
            break;
          }
          if (this.aStackCell[i].topcard != null
                && this.aStackCell[i].topcard.iColor == card.iColor
                && this.aStackCell[i].topcard.iNum + 1 == card.iNum
                && this.shouldStack(card)){
            this.aStackCell[i].topcard= card;
            this.aFreeCell[j].card= null;
            bAgain= true;
            break;
          }
        } // vrije cellen (linksboven) proberen leeg te maken
      } // alle 4 kaartstapels rechts  
      if (!bAgain){
        if (!this.bWon){
          if (this.isAllStacked()){
            this.bWon= true;
            if (confirm(congrats()
                + "\n\nKies  OK  voor een nieuw spel.")){
              window.location.href=window.location.href;
            }
          }
        }
        break;
      }
      canvas.width = canvas.width; // clear
      this.draw(g, canvas, imgDeck);
      setTimeout("table.autoPlayOnTimer()",100);
      break;
    }
  }, // autoPlay(g, canvas, imgDeck)
  /** kan de gegeven kaart zonder bezwaar worden opgestapeld? */
  shouldStack: function(card){
    // een kaart hoger dan een 2 kan alleen automatisch worden opgestapeld
    // als de stapels van de andere kleur (rood vs zwart) de lagere kaarten
    // bevatten, anders gezegd, een kaart bevatten die minstens zo hoog is.
    if (card.iNum < 2){
      return true;
    }
    var score= 0; // score van 2 nodig
    for (var i= 0; i < 4; i++){
      var card2= this.aStackCell[i].topcard;
      if (card2==null){
        continue; // lege stapel
      }
      if (card2.blackRedEquals(card)){
        continue;
      }
      if (card2.iNum >= card.iNum - 1){
        score++;
      }
    }
    return score == 2;
  }, // shouldStack(card)
  /**
   * Bereken de maximale omvang van een "supermove",
   * rekening houdend met vrije cellen en lege kolommen. 
   * @returns het maximaal aantal ineens te verplaatsen kaarten
   */
  getMoveCapacity: function(){
    var nF= 0;
    for (var i= 0; i < 4; i++){
      if (this.aFreeCell[i].card==null){
        nF++;
      }
    }
    var nC= 0;
    for (var i= 0; i < 8; i++){
      if (this.aCascadeCell[i].nCards==0){
        nC++;
      }
    }
    return Math.max((nF+1)*nC+1, (nF+1+nC));
  }, // getMoveCapacity: function()
  /**
   * Bereken de maximale omvang van een "supermove",
   * rekening houdend met vrije cellen en lege kolommen,
   * als het doel een lege kolom is. 
   * @returns het maximaal aantal ineens te verplaatsen kaarten
   */
  getMoveCapacityMin1: function(){
    var nF= 0;
    for (var i= 0; i < 4; i++){
      if (this.aFreeCell[i].card==null){
        nF++;
      }
    }
    var nC= 0;
    for (var i= 0; i < 8; i++){
      if (this.aCascadeCell[i].nCards==0){
        nC++;
      }
    }
    return Math.max((nF+1)*(nC-1)+1, (nF+1+(nC-1)));
  }, // getMoveCapacity: function()
  isAllStacked: function(){
	  var n= 0;
    for (var i= 0; i < 4; i++){
      var card= this.aStackCell[i].topcard;
      if (card!=null){
        n+= (card.iNum + 1);
      }
    }
    return (n==4*13);
  }, // isAllStacked: function()
  reset: function(){
    bak0= this.aBackup[0];
    this.aBackup= new Array();
    this.aBackup[0]= bak0;
    this.nUndo= 1;
    this.undo();
  }, // reset: function()
  redo: function(){
    if (this.aBackup[this.nUndo]!=null){
      this.nUndo+=2;
      this.undo();
    }
  }, // redo: function()
  undo: function(){
    var iLast= this.nUndo - 2;
    if (iLast < 0){
      iLast= 0;
    }
    var bak= this.aBackup[iLast];
    for (var i= 0; i < 4; i++){
      this.aFreeCell[i].card= bak.aFreeCard[i];
    }
    for (var i= 0; i < 4; i++){
      this.aStackCell[i].topcard= bak.aStackCard[i];
    }
    for (var i= 0; i < 8; i++){
      this.aCascadeCell[i].aCard= new Array();
      for (var j= 0; j < bak.aCascadeCards[i].length; j++){
        this.aCascadeCell[i].aCard[j]= bak.aCascadeCards[i][j];
      }
      this.aCascadeCell[i].nCards= this.aCascadeCell[i].aCard.length; 
    }
    if (this.nUndo > 1){ // de eerste (beginsituatie) laten we staan
      this.nUndo--; 
    }
  }, // undo: function()
  backup: function(){
    this.aBackup[this.nUndo++]= new TableBackup(
        this.aFreeCell, this.aStackCell, this.aCascadeCell);
  } // backup: function()
}; // var table
table.init();
table.backup();
