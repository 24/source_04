function init_cse(opts_) {
  window.__gcse = window.__gcse || {};
  window.__gcse.ct = (new Date).getTime();
  window.__gcse.sacb = function() {};
  window.__gcse.scb = function() {
    var a = window.__gcse;
    a.plainStyle && delete opts_.rawCss;
    google.search.cse.element.init(opts_) &&
      ("explicit" != a.parsetags ?
        "complete" == document.readyState || "interactive" == document.readyState ?
          (google.search.cse.element.go(), a.callback&&a.callback()) :
          google.setOnLoadCallback(function() { google.search.cse.element.go(); a.callback && a.callback() }, !0) : a.callback&&a.callback())
  };
/*********************************************************************************************
  var b = document.createElement("script"),
      c = opts_.protocol + "://" + opts_.uds + "/jsapi?autoload=",
      d = encodeURIComponent,
      e = '{"name":"search","version":"1.0","callback":"__gcse.scb"',
      f = window.__gcse;
  if (!f || !f.plainStyle) {
    var g = opts_.protocol + "://www.google.com/cse/style/look/",
        h;
    h = opts_.theme.toLowerCase().replace("v2_","v2/");
    e += ',"style":"' + (g + h + ".css") + '"'
  }
  opts_.language && (e += ',"language":"' + opts_.language + '"');
  e += "}";
  b.src = c + d('{"modules":[' + e + ',{"name":"ads","version":"3","packages":["search"],"callback":"__gcse.sacb"}]}') + "";
  b.type = "text/javascript";
  document.getElementsByTagName("head")[0].appendChild(b);
*********************************************************************************************/
}

// b.src - jsapi.js
// http://www.google.com/jsapi?autoload=%7B%22modules%22%3A%5B%7B%22name%22%3A%22search%22%2C%22version%22%3A%221.0%22%2C%22callback%22%3A%22__gcse.scb%22%2C%22style%22%3A%22http%3A%2F%2Fwww.google.com%2Fcse%2Fstyle%2Flook%2Fv2%2Fdefault.css%22%2C%22language%22%3A%22fr%22%7D%2C%7B%22name%22%3A%22ads%22%2C%22version%22%3A%223%22%2C%22packages%22%3A%5B%22search%22%5D%2C%22callback%22%3A%22__gcse.sacb%22%7D%5D%7D

init_cse(
  {
    "cx" : "018355485725569728206:_qdsw68uh48",
    "language" : "fr",
    "theme" : "V2_DEFAULT",
    "uiOptions" : {
      "resultsUrl" : "",
      "enableAutoComplete" : true,
      "enableImageSearch" : false,
      "imageSearchLayout" : "popup",
      "resultSetSize" : "filtered_cse",
      "enableOrderBy" : true,
      "orderByOptions":
        [
          {
            "label" : "Relevance",
            "key" : ""
          },
          {
            "label" : "Date",
            "key" : "date"
          }
        ],
      "overlayResults" :false
    },
    "protocol" : "http",
    "uds" : "www.google.com",
    "rawCss" : "\n"
  });


/**********************************************************************************************************
(
function(opts_) {
  window.__gcse = window.__gcse || {};
  window.__gcse.ct = (new Date).getTime();
  window.__gcse.sacb = function() {};
  window.__gcse.scb = function() {
    var a = window.__gcse;
    a.plainStyle && delete opts_.rawCss;
    google.search.cse.element.init(opts_) &&
      ("explicit" != a.parsetags ?
        "complete" == document.readyState || "interactive" == document.readyState ?
          (google.search.cse.element.go(), a.callback&&a.callback()) :
          google.setOnLoadCallback(function() { google.search.cse.element.go(); a.callback && a.callback() }, !0) : a.callback&&a.callback())
  };
  var b = document.createElement("script"),
      c = opts_.protocol + "://" + opts_.uds + "/jsapi?autoload=",
      d = encodeURIComponent,
      e = '{"name":"search","version":"1.0","callback":"__gcse.scb"',
      f = window.__gcse;
  if (!f || !f.plainStyle) {
    var g = opts_.protocol + "://www.google.com/cse/style/look/",
        h;
    h = opts_.theme.toLowerCase().replace("v2_","v2/");
    e += ',"style":"' + (g + h + ".css") + '"'
  }
  opts_.language && (e += ',"language":"' + opts_.language + '"');
  e += "}";
  b.src = c + d('{"modules":[' + e + ',{"name":"ads","version":"3","packages":["search"],"callback":"__gcse.sacb"}]}') + "";
  b.type = "text/javascript";
  document.getElementsByTagName("head")[0].appendChild(b);
})(
  {
    "cx" : "018355485725569728206:_qdsw68uh48",
    "language" : "fr",
    "theme" : "V2_DEFAULT",
    "uiOptions" : {
      "resultsUrl" : "",
      "enableAutoComplete" : true,
      "enableImageSearch" : false,
      "imageSearchLayout" : "popup",
      "resultSetSize" : "filtered_cse",
      "enableOrderBy" : true,
      "orderByOptions":
        [
          {
            "label" : "Relevance",
            "key" : ""
          },
          {
            "label" : "Date",
            "key" : "date"
          }
        ],
      "overlayResults" :false
    },
    "protocol" : "http",
    "uds" : "www.google.com",
    "rawCss" : "\n"
  });
**********************************************************************************************************/
