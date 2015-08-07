// id input search : gsc-i-id1
//                   gsc-i-id1

// init from google custom search engine
/*************************************************************************
(function() {
  var cx = '018355485725569728206:_qdsw68uh48';
  var gcse = document.createElement('script'); gcse.type = 'text/javascript';
  gcse.async = true;
  //var src = (document.location.protocol == 'https:' ? 'https:' : 'http:') + '//www.google.com/cse/cse.js?cx=' + cx;
  //gcse.src = src;
  // src : http://www.google.com/cse/cse.js?cx=018355485725569728206:_qdsw68uh48
  gcse.src = (document.location.protocol == 'https:' ? 'https:' : 'http:') + '//www.google.com/cse/cse.js?cx=' + cx;
  var s = document.getElementsByTagName('script')[0];
  s.parentNode.insertBefore(gcse, s);
  //var s = document.getElementsByTagName('body')[0];
  //var s = document.body;
  //s.insertBefore(gcse, s.firstChild);
})();
*************************************************************************/

var gcse = {};
var gDayNames = [ "dimanche", "lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi" ];
var gMonthNames = [ "janvier", "février", "mars", "avril", "mai", "juin", "juillet", "août", "septembre", "octobre", "novembre", "décembre" ];
$(document).ready(cse_init);

function cse_init() {
  createPrintList();
  createPrintButtons();
  //addPrintButtons();
  addDateButtons();
  setPrint();
  setDate(new Date());
}

function createPrintList() {
  var prints = [];
  prints.push(createPrint("La croix", "quotidien", "lundi à samedi"));
  prints.push(createPrint("Le figaro", "quotidien", "lundi à ?"));
  prints.push(createPrint("Le monde", "quotidien", "mardi à dimanche"));
  prints.push(createPrint("Le parisien", "quotidien", "lundi à dimanche"));
  prints.push(createPrint("L'équipe", "quotidien", "lundi à samedi et dimanche exceptionnellement"));
  prints.push(createPrint("Les échos", "quotidien", "lundi à vendredi"));
  prints.push(createPrint("libération", "quotidien", "lundi à samedi"));
  
  prints.push(createPrint("France football", "hebdomadaire", "mardi et vendredi"));
  prints.push(createPrint("L'express", "hebdomadaire", "mercredi"));
  prints.push(createPrint("Les inrockuptibles", "hebdomadaire", "mercredi"));
  prints.push(createPrint("Challenges", "hebdomadaire", "jeudi"));
  prints.push(createPrint("Courrier international", "hebdomadaire", "jeudi"));
  prints.push(createPrint("Le nouvel observateur", "hebdomadaire", "jeudi"));
  prints.push(createPrint("Le point", "hebdomadaire", "jeudi"));
  prints.push(createPrint("Micro hebdo", "hebdomadaire", "jeudi"));
  prints.push(createPrint("Valeurs actuelles", "hebdomadaire", "jeudi"));
  prints.push(createPrint("Marianne", "hebdomadaire", "samedi"));
  prints.push(createPrint("Le journal du dimanche", "hebdomadaire", "dimanche"));

  prints.push(createPrint("Vocable anglais", "bi-mensuel"));
  prints.push(createPrint("60 millions de consommateurs", "mensuel"));
  prints.push(createPrint("Alternatives économiques", "mensuel"));
  prints.push(createPrint("Alternatives internationales", "trimestriel"));
  prints.push(createPrint("Android mobiles et tablettes", "bimestriel"));
  prints.push(createPrint("Art actuel", "bimestriel"));
  prints.push(createPrint("Beaux arts magazine", "mensuel"));
  prints.push(createPrint("Capital", "mensuel"));
  prints.push(createPrint("Cerveau et psycho", "bimestriel"));
  prints.push(createPrint("L'essentiel cerveau et psycho", "trimestriel"));
  prints.push(createPrint("Chronic'art", "trimestriel"));
  prints.push(createPrint("Connaissance des arts", "mensuel"));
  prints.push(createPrint("Course au large", "mensuel"));
  prints.push(createPrint("Download", "bimestriel"));
  prints.push(createPrint("Géo", "mensuel"));
  prints.push(createPrint("Géo histoire", "bimestriel"));
  prints.push(createPrint("Géo voyage", "bimestriel"));
  prints.push(createPrint("Historia", "mensuel"));
  prints.push(createPrint("Jeux vidéo magazine", "mensuel"));
  prints.push(createPrint("La recherche", "mensuel"));
  prints.push(createPrint("La revue du vin de France", "mensuel"));
  prints.push(createPrint("L'art de l'aquarelle", "trimestriel"));
  prints.push(createPrint("Le courrier de l'atlas", "mensuel"));
  prints.push(createPrint("Le monde diplomatique", "mensuel"));
  prints.push(createPrint("L'essentiel du mobile", "bimestriel"));
  prints.push(createPrint("L'ordinateur individuel", "mensuel"));
  prints.push(createPrint("National geographic", "mensuel"));
  prints.push(createPrint("Open source", "mensuel"));
  prints.push(createPrint("Photoshop", "mensuel"));
  prints.push(createPrint("Pirate informatique", "bimestriel"));
  prints.push(createPrint("Point de vue histoire", "mensuel"));
  prints.push(createPrint("Pour la science", "mensuel"));
  prints.push(createPrint("Première", "mensuel"));
  prints.push(createPrint("Programmez", "mensuel"));
  prints.push(createPrint("Retouche photo", "bimestriel"));
  prints.push(createPrint("Science et vie", "mensuel"));
  prints.push(createPrint("Science et vie guerres et histoire", "bimestriel"));
  prints.push(createPrint("Les cahiers de science et vie", "mensuel"));
  prints.push(createPrint("Sciences et avenir", "mensuel"));
  prints.push(createPrint("Vocable anglais", "bi-mensuel"));
  prints.push(createPrint("Top 500 sites", "trimestriel"));
  //prints.push(createPrint("", "", ""));
  gcse.prints = prints;
}

function createPrint(print, frequency, days) {
  return { print: print, frequency: frequency, frequencyCategory: getFrequencyCategory(frequency), days: days };
}

function getFrequencyCategory(frequency) {
  switch (frequency) {
    case "quotidien":
      return "quotidien";
    case "hebdomadaire":
    case "bi-mensuel":
      return "hebdomadaire";
    case "mensuel":
    case "bimestriel":
    case "trimestriel":
      return "mensuel";
  }
  return "???";
}

function createPrintButtons() {
  var prints = gcse.prints;
  var lastFrequencyCategory = null;
  for (var i in prints) {
    var print = prints[i];
    if (print.frequencyCategory !== lastFrequencyCategory) {
      if (lastFrequencyCategory)
        addPrintNewline();
      addPrintLabel(print.frequency + " : ");
    }
    addPrintButton(print);
    lastFrequencyCategory = print.frequencyCategory;
  }
}

function addDateButtons() {
  //var div = document.getElementById('date_select');
  var div = $("#date_select")[0];
  
  var button = document.createElement('button');
  button.innerText = "today";
  button.onclick =  function() { setDate(new Date()); setSearch(); };
  div.appendChild(button);
  
  var button = document.createElement('button');
  button.innerText = "previous day";
  button.onclick =  function() { setDate(gcse.date.addDays(-1)); setSearch(); };
  div.appendChild(button);
  
  var button = document.createElement('button');
  button.innerText = "next day";
  button.onclick =  function() { setDate(gcse.date.addDays(1)); setSearch(); };
  div.appendChild(button);
  
  var button = document.createElement('button');
  button.innerText = "previous week";
  button.onclick =  function() { setDate(gcse.date.addDays(-7)); setSearch(); };
  div.appendChild(button);
  
  var button = document.createElement('button');
  button.innerText = "next week";
  button.onclick =  function() { setDate(gcse.date.addDays(7)); setSearch(); };
  div.appendChild(button);
  
  var button = document.createElement('button');
  button.innerText = "previous month";
  button.onclick =  function() { setDate(gcse.date.addMonths(-1)); setSearch(); };
  div.appendChild(button);
  
  var button = document.createElement('button');
  button.innerText = "next month";
  button.onclick =  function() { setDate(gcse.date.addMonths(1)); setSearch(); };
  div.appendChild(button);
}

//function addPrintButtons() {
//  
//  addPrintLabel("quotidien : ");
//  addPrintButton0("La croix");        // lundi à samedi
//  addPrintButton0("Le figaro");       // lundi à 
//  addPrintButton0("Le monde");        // mardi à dimanche
//  addPrintButton0("Le parisien");     // lundi à dimanche
//  addPrintButton0("L'équipe");        // lundi à samedi et dimanche exceptionnellement
//  addPrintButton0("Les échos");       // lundi à vendredi
//  addPrintButton0("libération");      // lundi à samedi
//  addPrintNewline();
//  
//  addPrintLabel("hebdo : ");
//  addPrintButton0("France football");          // mardi et vendredi
//  addPrintButton0("L'express");                // mercredi
//  addPrintButton0("Courrier international");   // jeudi
//  addPrintButton0("Le nouvel observateur");    // jeudi
//  addPrintButton0("Le point");                 // jeudi
//  addPrintButton0("Marianne");                 // samedi
//  addPrintButton0("Le journal du dimanche");   // dimanche
//  addPrintNewline();
//  
//  addPrintLabel("mensuel : ");
//  addPrintButton0("60 millions de consommateurs");
//  addPrintButton0("Alternatives économiques");
//  addPrintButton0("Android mobiles et tablettes");
//  addPrintButton0("Capital");
//  addPrintButton0("Jeux vidéo magazine");
//  addPrintButton0("L'essentiel du mobile");
//  addPrintButton0("L'ordinateur individuel");
//  addPrintButton0("Open source");
//  addPrintButton0("Photoshop");
//  addPrintButton0("Pirate informatique");
//  addPrintButton0("Pour la science");
//  addPrintButton0("Programmez");
//  addPrintButton0("Science et vie");
//  addPrintButton0("Sciences et avenir");
//  addPrintButton0("Vocable anglais");
//}

function addPrintNewline() {
  addPrintElement(document.createElement('br'));
}

function addPrintLabel(label) {
  addPrintElement(document.createTextNode(label));
}

//function addPrintButton0(print) {
//  var button = document.createElement('button');
//  button.innerText = print;
//  button.onclick =  function() { setPrint0(print); setSearch(); };
//  addPrintElement(button);
//}

function addPrintButton(print) {
  var button = document.createElement('button');
  button.innerText = print.print;
  button.onclick =  function() { printButton(print); };
  addPrintElement(button);
}

function addPrintElement(element) {
  var div = $("#print_select")[0];
  div.appendChild(element);
}

function printButton(print) {
  setPrint(print);
  setSearch();
}

function setPrint(print) {
  if (print)
    $("#print")[0].value = print.print;
  gcse.print = print;
  updateDate();
}

//function setPrint0(print) {
//  $("#print")[0].value = print;
//  gcse.print = print;
//}

function setDate(date) {
  gcse.date = date;
  updateDate();
}

function updateDate() {
  $("#date")[0].value = formatPrintDate();
}

//function setDateNext() {
//  setDate(gcse.date.addDays(1));
//}

function formatDateQuotidien(date) {
  // date.getDay()   : day of the week (0-6)
  // date.getDate()  : day of the month (1-31)
  // date.getMonth() : month (0-11)
  var d = date.getDate();
  if (d === 1) d = "1er";
  return "du " + gDayNames[date.getDay()] + " " + d + " " + gMonthNames[date.getMonth()] + " " + date.getFullYear();
}

function formatDateMensuel(date) {
  return gMonthNames[date.getMonth()] + " " + date.getFullYear();
}

function formatDateBimestriel(date) {
  // date.getMonth() : month (0-11)
  var m = date.getMonth();
  var y = date.getFullYear()
  if (m === 11)
    return gMonthNames[m] + " " + y + " " + gMonthNames[0] + " " + (y + 1);
  else
    return gMonthNames[m] + " " + gMonthNames[m + 1] + " " + date.getFullYear();
}

function formatDateTrimestriel(date) {
  // date.getMonth() : month (0-11)
  var m = date.getMonth();
  var y = date.getFullYear()
  if (m >= 10)
    return gMonthNames[m] + " " + y + " " + gMonthNames[(m + 2) % 12] + " " + (y + 1);
  else
    return gMonthNames[m] + " " + gMonthNames[m + 2] + " " + date.getFullYear();
}

function formatPrintDate() {
  if (!gcse.date) return;
  if (!gcse.print || gcse.print.frequencyCategory !== "mensuel")
    return formatDateQuotidien(gcse.date);
  else if (gcse.print.frequency === "bimestriel")
    return formatDateBimestriel(gcse.date);
  else if (gcse.print.frequency === "trimestriel")
    return formatDateTrimestriel(gcse.date);
  else
    return formatDateMensuel(gcse.date);
}

function setSearch() {
  //$("#gsc-i-id1")[0].value = $("#print")[0].value + " du " + formatDate(gcse.date);
  $("#gsc-i-id1")[0].value = $("#print")[0].value + " " + formatPrintDate();
}

function search() {
  var element = google.search.cse.element.getElement('standard0');
  //element.execute('news');
  element.execute();
}

