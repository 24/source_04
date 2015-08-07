var gtools = {};
$(document).ready(pb_tools_init);

function Tools() {
  this.log = null;
}

function pb_tools_init() {
  gtools.log = getLogElement();
}

// How createDelegate() works in JavaScript – Function.apply() and Function.call() http://ash4aque.wordpress.com/2011/09/22/how-createdelegate-works-in-javascript-function-apply-and-function-call/
function createDelegate(that, f) {
  return function () {
    f.apply(that, arguments);
  };
}

function toHex(i, n) {
  if (n === undefined) n = 4;
  var s = "0000000" + i.toString(16);
  return s.substr(s.length - n);
}

/***********************************************************
function addWindowOnloadEvent(f) { 
  var onload = window.onload; 
  if (typeof window.onload !== 'function') { 
    window.onload = f; 
  } else { 
    window.onload = function() { 
      if (onload) { 
        onload(); 
      } 
      f(); 
    }; 
  } 
} 
***********************************************************/

function getUrlParams() {
  var t = location.search.substring(1).split('&');
  var f = [];
  for (var i = 0; i < t.length; i++) {
    var x = t[i].split('=');
    f[x[0]] = x[1];
  }
  return f;
}

function showHide(id) {
  var element = document.getElementById(id);
  if (element.style.display === "none")
    element.style.display = "";
  else
    element.style.display = "none";
}

var gLogIndex = 1;
function log(msg) {
  var s = "        " + gLogIndex++;
  console.log(s.substr(s.length - 6) + " " + dateToString(new Date()) + "  " + msg);
}

//function dateToString(date) {
//  // 2013-01-05 13:49:38.905
//  var m = "0" + date.getMonth();
//  var d = "0" + date.getDate();
//  var h = "0" + date.getHours();
//  var n = "0" + date.getMinutes();
//  var s = "0" + date.getSeconds();
//  var i = "0" + date.getMilliseconds();
//  return date.getFullYear() + "-" + m.substr(m.length - 2) + "-" + d.substr(d.length - 2) + " " + h.substr(h.length - 2)
//          + ":" + n.substr(n.length - 2) + ":" + s.substr(s.length - 2) + "." + i.substr(i.length - 3);
//}

Date.prototype.toString2 = function() {
  // date.getDate()  : day of the month (1-31)
  // date.getMonth() : month (0-11)
  var m = "0" + (this.getMonth() + 1);
  var d = "0" + this.getDate();
  var h = "0" + this.getHours();
  var n = "0" + this.getMinutes();
  var s = "0" + this.getSeconds();
  var i = "0" + this.getMilliseconds();
  return this.getFullYear() + "-" + m.substr(m.length - 2) + "-" + d.substr(d.length - 2) + " " + h.substr(h.length - 2)
    + ":" + n.substr(n.length - 2) + ":" + s.substr(s.length - 2) + "." + i.substr(i.length - 3);
};

Date.prototype.addDays = function(days) {
  return new Date(this.getTime() + (86400000 * days));
};

Date.prototype.addMonths = function(months) {
  // date.getMonth() : month (0-11)
  var m = this.getMonth() + (months % 12);
  var y = this.getFullYear();
  if (months > 0)
     y += Math.floor(months / 12);
  else
    y += Math.ceil(months / 12);
  if (m >= 12) {
    m -= 12;
    y++;
  } else if (m < 0) {
    m += 12;
    y--;
  }
  return new Date(y, m, 1, this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
};

function getLogElement() {
  var log = document.getElementById('log');
  if (log === null)
    log = document.body;
  return log;
}

function logHtml(html) {
  // element.insertAdjacentHTML https://developer.mozilla.org/en-US/docs/DOM/element.insertAdjacentHTML
  // DOM Parsing and Serialization http://domparsing.spec.whatwg.org/#insertadjacenthtml%28%29
  // pas de ref de insertAdjacentHTML sur http://www.w3schools.com
  //document.body.insertAdjacentHTML('beforeend', html);
  if (gtools.log !== null)
    gtools.log.insertAdjacentHTML('beforeend', html);
}

/***************************************************************************************************************************************************************
// __defineGetter__ javascript bible page 1051
// __defineGetter__ ... ne marche dans IE9
// __defineGetter__ https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/Object/defineGetter
// get https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Operators/get?redirectlocale=en-US&redirectslug=JavaScript%2FReference%2FOperators%2FSpecial%2Fget
// Defining getters and setters https://developer.mozilla.org/en-US/docs/JavaScript/Guide/Working_with_Objects#Defining_getters_and_setters
// Object.defineProperty https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/Object/defineProperty?redirectlocale=en-US&redirectslug=Core_JavaScript_1.5_Reference%2FGlobal_Objects%2FObject%2FdefineProperty
// More SpiderMonkey changes: ancient, esoteric, very rarely used syntax for creating getters and setters is being removed http://whereswalden.com/2010/04/16/more-spidermonkey-changes-ancient-esoteric-very-rarely-used-syntax-for-creating-getters-and-setters-is-being-removed/
//MouseEvent.prototype.__defineGetter__("pbx", function () { if ("offsetX" in this) return this.offsetX; else return this.pageX - $(this.target).offset().left; });
//MouseEvent.prototype.__defineGetter__("pby", function () { if ("offsetY" in this) return this.offsetY; else return this.pageY - $(this.target).offset().top; });
***************************************************************************************************************************************************************/

/***************************************************************************************************************************************************************
// How do I get the coordinates of a mouse click on a canvas element?  http://stackoverflow.com/questions/55677/how-do-i-get-the-coordinates-of-a-mouse-click-on-a-canvas-element
// on chrome and firefox clientX, clientY, screenX, screenY, layerX, layerY, pageX, pageY
// on chrome and IE9 offsetX, offsetY, x, y
// screenX, screenY : coordonnées de l'écran
// clientX, clientY : coordonnées de la fenêtre du navigateur (ne tient pas compte ni du scroll de la page ni du scroll des éléments)
// x, y             : idem clientX, clientY
// pageX, pageY     : coordonnées dans la page du navigateur (tient compte du scroll de la page mais pas du scroll des éléments)
// layerX, layerY   : coordonnées du calque courant, coordonnées du canvas
// offsetX, offsetY : coordonnées du canvas
// offsetX/Y in Firefox  http://roimediaworks.com/web-development/offsetxy-in-firefox/
//var addOffset = function (event, element) {
//    if (!event.offsetX) {
//        event.offsetX = (event.pageX - $(element).offset().left);
//        event.offsetY = (event.pageY - $(element).offset().top);
//    }
//    return event;
//};  
// element pointé par la souris : currentTarget, srcElement, target, toElement
***************************************************************************************************************************************************************/

MouseEvent.prototype.getOffsetX = function () { if ("offsetX" in this) return this.offsetX; else return this.pageX - $(this.target).offset().left; };
MouseEvent.prototype.getOffsetY = function () { if ("offsetY" in this) return this.offsetY; else return this.pageY - $(this.target).offset().top; };

/************************************************************************************************************************
function xinspect(o,i){
  // from : How to inspect Javascript Objects http://stackoverflow.com/questions/5357442/how-to-inspect-javascript-objects
  if(typeof i==='undefined')i='';
  if(i.length>50)return '[MAX ITERATIONS]';
  var r=[];
  for(var p in o){
    var t=typeof o[p];
    r.push(i+'"'+p+'" ('+t+') => '+(t==='object' ? 'object:'+xinspect(o[p],i+'  ') : o[p]+''));
  }
  return r.join(i+'\n');
}
************************************************************************************************************************/

function rgb(r, g, b) {
  return ((r << 8) + g << 8) + b;
}

function rgba(r, g, b, a) {
  return (((r << 8) + g << 8) + b << 8) + a;
}

function rgbDistance(co1, co2) {
/*****************************************************************************************
 * ColourDistance http://www.compuphase.com/cmetric.htm
typedef struct {
   unsigned char r, g, b;
} RGB;

double ColourDistance(RGB e1, RGB e2)
{
  long rmean = ( (long)e1.r + (long)e2.r ) / 2;
  long r = (long)e1.r - (long)e2.r;
  long g = (long)e1.g - (long)e2.g;
  long b = (long)e1.b - (long)e2.b;
  return sqrt((((512+rmean)*r*r)>>8) + 4*g*g + (((767-rmean)*b*b)>>8));
}
*****************************************************************************************/
  var rmean = Math.floor((co1.r + co2.r) / 2);
  var r = co1.r - co2.r;
  var g = co1.g - co2.g;
  var b = co1.b - co2.b;
  return Math.sqrt((((512 + rmean) * r * r) >> 8) + (4 * g * g) + (((767 - rmean) * b * b) >> 8));
}
