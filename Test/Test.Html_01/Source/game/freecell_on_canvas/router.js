/*
 * Het oplossen van een FreeCell-puzzel is op te vatten als het plannen
 * van een route door een landschap met plaatsen, met daartussen
 * wegen. In die vergelijking is een weg een geldige zet, een plaats
 * is een volgende toestand op de tafel. Het eindpunt is de toestand dat
 * alle kaarten rechtsboven zijn opgestapeld. De beste oplossing is een
 * zo kort mogelijke route, dat wil zeggen het minimale aantal kaart- 
 * verschuivingen. Nu zijn niet alle verschuivingen even betekenisvol,
 * het verschuiven van een kaart van de ene vrij cel naar een andere
 * levert bijvoorbeeld per saldo niets op. Twee verschillende toestanden
 * die niet wezenlijk van elkaar verschillen noem ik variaties; toestanden
 * die wel wezenlijk van elkaar verschillen noem ik verzamelingen. Een
 * toestand kan ik vertalen in een verzamelingcode door de kaarten in
 * elk van de drie celgroepen te sorteren op waarde. Bij de vrije cellen
 * is zo'n waarde eenvoudig het kaartnummer (0 voor schoppen aas enz.),
 * bij cascades is dat een string van kaartnummers. Sorteren gaat dan het
 * eenvoudigst alfanumeriek. Om nu te bepalen of een nieuwe toestand nog
 * niet eerder is bereikt, volstaat het een lijst met verzamelingcodes
 * bij te houden.
 */

/**
 * Stippel een route uit tot het eind van het spel
 * @param t de tafel (freecell.js:table)
 */
function Router(t){
  //t.aFreeCell    : cellen linksboven
  //t.aStackCell   : cellen rechtsboven
  //t.aCascadeCell : cellen met uitgespreide kaartkolommen
  var aF= new Array();
  for (var i= 0; i < 4; i++){
    aF[i]= t.aFreeCell[i].card;
  }
  var aS= new Array();
  for (var i= 0; i < 4; i++){
    aS[i]= t.aStackCell[i].topcard;
  }
  var aC= new Array();
  for (var i= 0; i < 8; i++){
    var ac= t.aCascadeCell[i].aCard;
    aC[i]= new Array();
    for (var j= 0; j < t.aCascadeCell[i].nCards; j++){
      aC[i][j]= ac[j];
    }
  }
  var state0= new TState(aF, aS, aC);
  var code0= new CCode(state0);
  /*
   * Stap 1: Maak een todo-lijst met toestanden waarvan we uit kunnen gaan
   *         om andere toestanden te bereiken. Zet daarin de uitgangstoestand
   *         en zet de verzamelingcode daarvan in de lijst bereikte codes.
   * Stap 2: Pak de oudste van de todo-lijst en genereer daaruit een lijst
   *         nieuwe toestanden (volgens de spelregels uiteraard). Alleen
   *         toestanden waarvan de verzamelingcode niet in de lijst bereikte
   *         codes staat, worden toegevoegd aan de todo-lijst. Daarbij komt
   *         hun verzamelingcode natuurlijk in de lijst bereikte codes.
   * Stap 3: Uiteindelijk zijn er geen nieuwe stappen meer mogelijk. Is de
   *         eindtoestand bereikt, dan willen we natuurlijk weten *hoe* we
   *         die hebben bereikt. Daarom leggen we bij elke bereikte code vast
   *         welke eerdere toestand daartoe leidde. Het is dan mogelijk
   *         vanuit de eindsituatie terug te gaan naar de uitgangssituatie. 
   *         Wil je de speler een hint geven, dan wordt dat het noemen van 
   *         de eerste zet vanaf de uitgangspositie. 
   */
  var aToDo= new Array();
  var iToDo= 0;
  aToDo[0]= state0;
  /** van elke bereikte verzamelingcode vastleggen welke toestand daarbij
   * hoorde; bij elke toestand ligt vast hoe we daar kwamen (behalve bij de
   * eerste) */
  var vRStates= new Array(); // bereikte toestanden
  var vRCodes= new Array(); // verzamelingcodes van de bereikte toestanden
  var vRPreds= new Array(); // toestanden die aan de bereikte voorafgingen
  vRStates.push(state0);
  vRCodes.push(code0.s);
  vRPreds.push(null);
  var stateEnd= null;
  var stateMax= null; // toestand met de hoogste score
  var maxScore= 0;    // de hoogste score tot nu toe behaald
  var bLimited= false;
  var bImproved= false;
  while(iToDo < aToDo.length){
    var stateSrc= aToDo[iToDo];
    var aNextState= stateSrc.doMoves();
    for (var i= 0; i < aNextState.length; i++){
      var state2= aNextState[i];
      var code2= new CCode(state2);
      if (vRCodes.indexOf(code2.s) >= 0){
        // soortgelijke situatie al via een andere route bereikt
        continue;
      }
      if (state2==stateSrc){ // TODO: uitzoeken hoe dit kan
        continue;
      }
      // registreer de nieuw bereikte toestand en hoe we daar kwamen
      vRStates.push(state2);
      vRCodes.push(code2.s);
      vRPreds.push(stateSrc);
      var score= state2.getScore();
      var nFree= state2.getFreeCellCount();
      score += nFree > 0 ? 1 : -1; // dit bevordert leeglaten van vrije cellen
      if (score > maxScore){
        if (maxScore > 0){
          bImproved= true;
        }
        stateMax= state2;
        maxScore= score;
      }
      if (score == 4*13){
        stateEnd= state2;
        break;
      }
      // kijk of we vandaar verder kunnen
      aToDo[aToDo.length]= state2;
    }
    if (stateEnd!=null){
      break;
    }
    iToDo++;
    if (aToDo.length >= 1000 && bImproved){
      bLimited= true;
      break; // als we doorgaan duurt het te lang, en we weten al wat
    }
    if (aToDo.length >= 5000){
      //bLimited= true;
      //break; // als we doorgaan duurt het echt te lang
      alert("Sorry, ik kan zo gauw geen advies geven.");
      return;
    }
  } // hierna zijn er geen (nuttige) zetten meer te bedenken
  if (stateEnd!=null){
    alert("Het einde is in zicht!");
  } else if (aToDo.length==1){
    alert("Geen zetten meer mogelijk.");
  } else if (bLimited){
    if (stateMax==null){
      alert("Sorry, ik kan geen advies geven.");
    } else{
      //alert("Score " + maxScore + " is zeker haalbaar");
      // reken terug naar de beginsituatie
      var state2= stateMax;
      var state1= stateMax;
      var nMoves= 0;
      for(;;){
        var i= vRStates.indexOf(state1);
        var stateX= vRPreds[i];
        if (stateX==null){
          break; // uitgangssituatie gevonden, da's nu state1
        }
        state2= state1;
        state1= stateX;
        nMoves++; // TODO: hier wat mee doen
      }
      state1.alertChanges(state2);
    }  
  } else{
    alert("Dit komt niet meer goed.");
  }
} // function Router(t)

/**
 * leg de toestand van de tafel vast
 */
function TState(aF, aS, aC){
  this.aF= aF;
  this.aS= aS;
  this.aC= aC;
} // function TState(it)
TState.prototype= {
  /**
   * Maak een kopie van dit object, met eigen arrays maar met
   * referenties naar dezelfde kaarten
   */
  clone: function(){
    var aF2= new Array();
    var aS2= new Array();
    var aC2= new Array();
    for (var i= 0; i < 4; i++){
      aF2[i]= this.aF[i];
    }
    for (var i= 0; i < 4; i++){
      aS2[i]= this.aS[i];
    }
    for (var i= 0; i < 8; i++){
      aC2[i]= new Array();
      for (var j= 0; j < this.aC[i].length; j++){
        aC2[i][j]= this.aC[i][j];
      }
    }
    return new TState(aF2, aS2, aC2);
  }, // clone: function()
  /** genereer alle toestanden die via geldige kaartverschuivingen
   * uit de huidige toestand zouden kunnen voortkomen */
  doMoves: function(){
    // Doe eerst de zetten die zonder meer in de goede richting gaan,
    // namelijk opstapelen van kaarten waarvan tenminste twee lager
    // van de tegenovergestelde kleur al opgestapeld ligt. Vinden we
    // zoiets, dan niet ook nog wat anders proberen, want dat andere
    // leidt zeker niet sneller tot succes.
    for (var iS= 0; iS < 4; iS++){
      var cS= this.aS[iS];
      if (cS==null){
        // hier kan een aas op. ligt die ergens voor het oprapen?
        for (var iC= 0; iC < 8; iC++){
          var aCCell= this.aC[iC];
          if (aCCell.length==0){
            continue;
          }
          var cC= aCCell[aCCell.length - 1];
          if (cC.iNum==0){
            // hebbes, die gaat naar de stapel
            var state2= this.clone();
            state2.stackOneFromCascade(iC, iS);
            return [state2];
          }
        } // cascadecellen afspeuren op zoek naar een aas
      } else{ // cS is de kaart bovenop de stapel
        // kijk of we hier nog iets op mogen, en dan kunnen, stapelen
        if (this.getMinOppositeStacked(cS) < cS.iNum - 2){
          continue; // mag wel, maar is niet met zekerheid verstandig
        }
        for (var iC= 0; iC < 8; iC++){
          var aCCell= this.aC[iC];
          if (aCCell.length==0){
            continue;
          }
          var cC= aCCell[aCCell.length - 1];
          if (cC.iNum==cS.iNum + 1 && cC.iColor == cS.iColor){
            // hebbes, die gaat naar de stapel
            var state2= this.clone();
            state2.stackOneFromCascade(iC, iS);
            return [state2];
          }
        } // cascadecellen afspeuren op zoek naar 1 hogere kaart
      }
    }
    // overige verplaatsingen, die misschien (even) nuttig zijn
    var aRet= new Array();
    // zet type 1: kaart van cascade (onder) op stapel (rechtsboven)
    for (var i= 0; i < 8; i++){
      var aCCell= this.aC[i];
      if (aCCell.length==0){
        continue;
      }
      var iStack= this.canStackFromCascade(i); 
      if (iStack >= 0){
        var state2= this.clone();
        state2.stackOneFromCascade(i, iStack);
        aRet.push(state2);
      }
    } // einde kaart van cascade op stapel leggen
    // zet type 1a: kaart van vrije cel op stapel
    for (var i= 0; i < 4; i++){
      if (this.aF[i]==null){
        continue;
      }
      for (var j= 0; j < 4; j++){
        if (this.aS[j]==null 
            || this.aS[j].iColor != this.aF[i].iColor
            || this.aS[j].iNum != this.aF[i].iNum - 1){
          continue;
        }
        var state2= this.clone();
        state2.stackOneFromFree(i, j);
        aRet.push(state2);
      }
    } // einde kaart van vrije cel op stapel
    // zet type 2b: kaart van een bezette vrije cel naar een plek in de casc.
    for (var iFree= 0; iFree < 4; iFree++){
      var cardF= this.aF[iFree];
      if (cardF==null){
        continue;
      }
      for (var iC= 0; iC < 8; iC++){
        if (this.canAddToCascade(iC, cardF)){
          var state2= this.clone();
          state2.aF[iFree]= null;
          state2.aC[iC].push(cardF);
          aRet.push(state2);
        }
      }
    } // einde zet type 2b
    // zet type 3: van cascadecel naar een andere
    for (var iCFrom= 0; iCFrom < 8; iCFrom++){
      var aCFrom= this.aC[iCFrom];
      if (aCFrom.length==0){
        continue;
      }
      var cardF= aCFrom[aCFrom.length - 1];
      for (var iC= 0; iC < 8; iC++){
        if (this.canAddToCascade(iC, cardF)){
          var state2= this.clone();
          state2.aC[iCFrom].pop();
          state2.aC[iC].push(cardF);
          aRet.push(state2);
        }
      }
    } // einde zet type 3
    var iFree= this.getAnyFreeCell();
    if (iFree >= 0){
      // zet type 2a: kaart van cascade (onder) naar een vrije cel
      for (var i= 0; i < 8; i++){
        var aCCell= this.aC[i];
        if (aCCell.length==0){
          continue; // hier is niks te halen
        }
        var state2= this.clone();
        state2.freeOneFromCascade(i, iFree);
        aRet.push(state2);
      } // einde kaart van cascade op vrije cel leggen
    }
    return aRet;
  }, // doMoves: function()
  /**
   * Zoek uit welke kaarten er van de tegenovergestelde kleuren liggen.
   * @param referentie
   * @return laagste van de zwarte (bij ref rood) of rode (bij ref zwart)
   */
  getMinOppositeStacked: function(cS){
    var aMax= [-1, -1, -1, -1]; // klaveren, ruiten, harten, schoppen
    // registreer wat we méér hebben
    for (var i= 0; i < 4; i++){
      if (this.aS[i]==null){
        continue;
      }
      aMax[this.aS[i].iColor]= this.aS[i].iNum;
    }
    if (cS.iColor==0 || cS.iColor==3){
      return aMax[1] < aMax[2] ? aMax[1] : aMax[2];
    } else{
      return aMax[0] < aMax[3] ? aMax[0] : aMax[3];
    }
  }, // getMinOppositeStacked: function(cS)
  /**
   * @returns het nummer van een vrije cel, anders -1
   */
  getAnyFreeCell: function(){
    for (var i= 0; i < 4; i++){
      if (this.aF[i]==null){
        return i;
      }
    }
    return -1; // alle vrije cellen zijn bezet
  }, // getAnyFreeCell: function()
  /**
   * @returns het aantal vrije cellen
   */
  getFreeCellCount: function(){
    var nFree= 0;
    for (var i= 0; i < 4; i++){
      if (this.aF[i]==null){
        nFree++;
      }
    }
    return nFree;
  }, // getFreeCellCount: function()
  /**
   * Tel het aantal kaarten op de stokken, maar wel zo dat een groot verschil
   * tussen de aantallen kaarten geen extra punten oplevert. In de praktijk:
   * de score is gelijk aan 4x de laagste plus 1 voor elke die 1 hoger is
   * of 2 voor elke die 2 of meer hoger is.
   * @returns iets tussen 0 en 4*13
   */
  getScore: function(){
    var score= 0;
    var iMin= 14;
    // bepaal eerst de laagste...
    for (var i= 0; i < 4; i++){
      if (this.aS[i]==null){
        iMin= 0;
      } else{
        iMin= Math.min(iMin, this.aS[i].iNum + 1);
      }
    }
    // en tel dan de punten volgens bovenstaand voorschrift
    score= 4 * iMin;
    for (var i= 0; i < 4; i++){
      if (this.aS[i]==null){
        continue;
      } else{
        var iOn= this.aS[i].iNum + 1;
        if (iOn - iMin > 2){
          score += 2;
        } else{
          score += (iOn - iMin);
        }
      }
    }
    return score;
  }, // getScore: function()
  /**
   * @param iC nummer van de cascade (0...7)
   * @param cardF kaart (van op een vrije cel)
   * @returns <code>true</code> als de gegeven kaart erop mag
   */
  canAddToCascade: function(iC, cardF){
    if (this.aC[iC].length==0){
      return true; // alles mag op een lege cascade-cel
    }
    var cardC= this.aC[iC][this.aC[iC].length - 1];
    if (cardC.iNum != cardF.iNum + 1
        || cardC.iColor==cardF.iColor
        || cardC.iColor + cardF.iColor == 3){
      return false;
    }
    return true;
  },
  /**
   * Kijk of het mogelijk is de onderste kaart van de gegeven cascade-cel 
   * naar een stapelcel (kies zelf de (of een) goede) te verplaatsen
   * @param iCas getal tussen 0 en 7
   * @returns index van een geschikte stapelcel of -1 indien er geen is
   */
  canStackFromCascade: function(iCas){
    var card= this.aC[iCas][this.aC[iCas].length - 1];
    for (var i= 0; i < 4; i++){
      var cardS= this.aS[i]; 
      if (cardS==null && card.iNum==0){ // aas
        return i;
      } else if (cardS != null 
          && card.iColor == cardS.iColor 
          && card.iNum == cardS.iNum + 1){
        return i;
      }
    }
    return -1;
  }, // canStackFromCascade: function(iCas)
  /**
   * Verplaats de onderste kaart van de gegeven cascade-cel naar
   * de gegeven stapelcel
   * @param iCas getal tussen 0 en 7
   * @param iStack getal tussen 0 en 3
   * @returns <code>true</code> bij succes
   */
  stackOneFromCascade: function(iCas, iStack){
    var card= this.aC[iCas].pop();
    this.aS[iStack]= card;
  }, // stackOneFromCascade: function(iCas)
  /**
   * Verplaats een kaart van de gegeven vrije cel naar
   * de gegeven stapelcel
   * @param iFree getal tussen 0 en 3
   * @param iStack getal tussen 0 en 3
   */
  stackOneFromFree: function(iFree, iStack){
    this.aS[iStack]= this.aF[iFree];
    this.aF[iFree]= null;
  }, // stackOneFromCascade: function(iCas)
  /**
   * Verplaats de onderste kaart van de gegeven cascade-cel naar
   * de gegeven vrije cel
   * @param iCas getal tussen 0 en 7
   * @param iFree getal tussen 0 en 3
   */
  freeOneFromCascade: function(iCas, iFree){
    var card= this.aC[iCas].pop();
    this.aF[iFree]= card;
  }, // freeOneFromCascade: function(iCas)
  /**
   * @param s2 de volgende toestand
   */
  alertChanges: function(s2){
    for (var i= 0; i < 4; i++){
      if (this.aF[i]==null && s2.aF[i]!=null){
        alert("Leg de " + s2.aF[i].getTitle() + " op een vrije cel");
        return;
      }
    } // iets op een vrije cel leggen
    for (var i= 0; i < 4; i++){
      if (this.aS[i]==null){
        continue;
      }
      if (this.aS[i].iNum < s2.aS[i].iNum){
        alert("Leg de " + s2.aS[i].getTitle() + " op de stapel");
        return;
      }
    } // kaart opstapelen
    for (var i= 0; i < 8; i++){
      if (this.aC[i].length < s2.aC[i].length){
        if (this.aC[i].length==0){
          alert("Leg de " + s2.aC[i][s2.aC[i].length - 1].getTitle()
              + " op de " + (i+1) + "e (lege) kolom");
        } else{
          alert("Leg de " + s2.aC[i][s2.aC[i].length - 1].getTitle()
              + " op de " + (i+1) + "e kolom");
        }      
        return;
      }
    } // kaart erbij op een kolom
  } // alertChanges: function(s2)
}; // TState.prototype

/**
 * Maak van een tafeltoestand een ongesorteerde verzameling en codeer die
 * als één string. Twee gelijkwaardige tafels (alleen verwisselde vrije 
 * cellen bijvoorbeeld) krijgen dezelfde code.
 * @param tstate 
 */
function CCode(tstate){
  // member var: s (de verzamelingcode als string)
  var aF1= new Array();
  for (var i= 0; i < 4; i++){
    var c= tstate.aF[i];
    aF1[i]= c==null ? " " : c.getCode(); 
  }
  var aS1= new Array();
  for (var i= 0; i < 4; i++){
    var c= tstate.aS[i];
    aS1[i]= c==null ? " " : c.getCode(); 
  }
  var aC1= new Array();
  for (var i= 0; i < 8; i++){
    var ac= tstate.aC[i];
    var sc= "";
    for (var j= 0; j < ac.length; j++){
      card= ac[j]; 
      sc+= card.getCode();
    }
    aC1[i]= sc;
  }
  aF1.sort();
  aS1.sort();
  aC1.sort();
  // We hebben nu 3 subverzamelingen, elk gesorteerd zodat de oorspronkelijke
  // volgorde geen verschil meer maakt. Nu de drie nog in één string:
  this.s= aF1.toString() + ";" + aS1.toString() + ";";
  // die arrays zijn komma-gescheiden... geeft niet, zijn er maar weinig
  for (var i= 0; i < 8; i++){
    this.s+= aC1[i] + ","; // komma als scheider tussen cascades
  }
} // CCode(tstate)
