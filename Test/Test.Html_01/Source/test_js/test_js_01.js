//window.onload = function () {
//  logHtml("toto<br>");
//};

//addWindowOnloadEvent(
//  function () {
//    logHtml("toto<br>");
//  });
//$(document).ready(
//  function () {
//    logHtml("test<br>");
//    //test_01();
//    //test_hex_01();
//    test_colour_01(0xf7, 0xf3, 0xf7);
//  });

$(document).ready(test);

function test() {
    logHtml("test<br>");
    //test_01();
    //test_hex_01();
    //test_array_01();
    //test_color_01(0xf7, 0xf3, 0xf7);
    //test_rgb_01(0xf7, 0xf3, 0xf7);
    //test_rgba_01(0xf7, 0xf3, 0xf7, 0xff);
    //test_color_distance_01(0xf7, 0xf3, 0xf7, 0xf7, 0xf3, 0xf7);
    //test_color_distance_01(0xf7, 0xf3, 0xf7, 0xef, 0xef, 0xef);
    //test_color_distance_01(0x00, 0x00, 0x00, 0xff, 0xff, 0xff);
    //test_file_01();
    //test_replace_01();
    //test_replace_02();
    //test_date_toString_01();
    //test_date_addDays_01();
    //test_date_addMonths_01();
    test_magazine3k_encrypt_01();
}

function test_magazine3k_encrypt_01() {
  logHtml("test_magazine3k_encrypt_01<br>");
  var s = "LYptX:JWooFL.3QidI8PEHs.R6Fcw??MOx6J9vkB7MBkNTV";
  logHtml("string ........ : " + s + "<br>");
  logHtml("encrypt() ..... : " + encrypt(s) + "<br>");
  s = "wjSWs:PQuuDN.1SgfK+NGFu.vE7iO?OOl4Q4/1NqfbuBhX";
  logHtml("string ........ : " + s + "<br>");
  logHtml("encrypt() ..... : " + encrypt(s) + "<br>");
  s = "wjSWs:VK00ZX.BmUr+K5yxa.hK1sA?UYYqB51IUqPKffxQ";
  logHtml("string ........ : " + s + "<br>");
  logHtml("encrypt() ..... : " + encrypt(s) + "<br>");
}

function test_date_toString_01() {
  logHtml("test_date_toString_01<br>");
  var date = new Date();
  logHtml("today ..................... : " + date + "<br>");
  logHtml("today .toDateString() ..... : " + date.toDateString() + "<br>");
  logHtml("today .toGMTString() ...... : " + date.toGMTString() + "<br>");
  logHtml("today .toISOString() ...... : " + date.toISOString() + "<br>");
  logHtml("today .toLocaleDateString() : " + date.toLocaleDateString() + "<br>");
  logHtml("today .toLocaleString() ... : " + date.toLocaleString() + "<br>");
  logHtml("today .toLocaleTimeString() : " + date.toLocaleTimeString() + "<br>");
  logHtml("today .toString() ......... : " + date.toString() + "<br>");
  logHtml("today .toTimeString() ..... : " + date.toTimeString() + "<br>");
  logHtml("today .toUTCString() ...... : " + date.toUTCString() + "<br>");
  logHtml("today .toString2() ........ : " + date.toString2() + "<br>"); 
}

function test_date_addDays_01() {
  logHtml("test_date_addDays_01<br>");
  var date = new Date();
  logHtml("today ..................... : " + date.toString2() + "<br>");
  var i = 0;
  date = date.addDays(1); i++;
  logHtml("today + " + i + " ................. : " + date.toString2() + "<br>");
  date = date.addDays(1); i++;
  logHtml("today + " + i + " ................. : " + date.toString2() + "<br>");
  date = date.addDays(1); i++;
  logHtml("today + " + i + " ................. : " + date.toString2() + "<br>");
}

function test_date_addMonths_01() {
  logHtml("test_date_addMonths_01<br>");
  var date = new Date();
  logHtml("today ..................... : " + date.toString2() + "<br>");
  var i = -15;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
  logHtml("today + " + i + " month ........... : " + date.addMonths(i).toString2() + "<br>"); i++;
}

function test_replace_01() {
  logHtml("test_replace_01<br>");
  
  var s = "toto1tata1tutu1";
  //var s2 = s.replace('\n', '<br>\n');
  //var s2 = s.replace('/$/g', '@');
  var s2 = s.replace(/1/g, '@');
  logHtml("s : \"" + s + "\"<br>");
  //logHtml("s.replace('\n', '&lt;br&gt;\n') : \"" + s2 + "\"<br>");
  logHtml("s.replace('$', '@') : \"" + s2 + "\"<br>");
  logHtml("<br>");
  
  s = "toto\ntata\ntutu\n";
  s2 = s.replace(/\n/g, '@');
  logHtml("s : \"" + s + "\"<br>");
  logHtml("s.replace(/\n/g, '@') : \"" + s2 + "\"<br>");
}

function test_replace_02() {
  var s = "Mr Blue has a blue house and a blue car";
  var s2 = s.replace(/blue/g, "red");
  logHtml("test_replace_01<br>");
  logHtml("s : \"" + s + "\"<br>");
  logHtml("s.replace(/blue/g, \"red\") : \"" + s2 + "\"<br>");
}

// EXPLORING THE FILESYSTEM APIS http://www.html5rocks.com/en/tutorials/file/filesystem/
// window.requestFileSystem(type, size, successCallback, opt_errorCallback)
// type : Whether the file storage should be persistent. Possible values are window.TEMPORARY or window.PERSISTENT.
//        Data stored using TEMPORARY can be removed at the browser's discretion (for example if more space is needed).
//        PERSISTENT storage cannot be cleared unless explicitly authorized by the user or the app and requires the user to grant quota to your app. See requesting quota.
// size : Size (in bytes) the app will require for storage.
// successCallback : Callback that is invoked on successful request of a file system. Its argument is a FileSystem object.
// opt_errorCallback : Optional callback for handling errors or when the request to obtain the file system is denied. Its argument is a FileError object.

window.requestFileSystem  = window.requestFileSystem || window.webkitRequestFileSystem;

function onInitFs(fs) {
  console.log('Opened file system : ' + fs.name);
  logHtml('Opened file system : ' + fs.name + '<br>');
  logHtml('Path : ' + fs.fullPath + '<br>');
}

function onInitFs2(fs) {
  // exclusive: true, create new file only, error if file exists
  fs.root.getFile('/log.txt', {create: true, exclusive: false}, function(fileEntry) {

    // fileEntry.isFile === true
    // fileEntry.name == 'log.txt'
    // fileEntry.fullPath == '/log.txt'
    logHtml('file : ' + fileEntry.name + '<br>');
    logHtml('path : ' + fileEntry.fullPath + '<br>');
  }, errorHandler);

}

function onInitFs3(fs) {

  fs.root.getFile('log.txt', {create: true}, function(fileEntry) {

    // Create a FileWriter object for our FileEntry (log.txt).
    fileEntry.createWriter(function(fileWriter) {

      fileWriter.onwriteend = function(e) {
        console.log('Write completed.');
        logHtml('Write completed.' + '<br>');
      };

      fileWriter.onerror = function(e) {
        console.log('Write failed: ' + e.toString());
        logHtml('Write failed: ' + e.toString() + '<br>');
      };

      // Create a new Blob and write it to log.txt.
      var blob = new Blob(['Lorem Ipsum'], {type: 'text/plain'});

      fileWriter.write(blob);

    }, errorHandler);

  }, errorHandler);

}

function test_file_01() {
  // --unlimited-quota-for-files
  // window.PERSISTENT, window.TEMPORARY
  // 1024*1024 /*5MB*/
  window.requestFileSystem(window.PERSISTENT, 1024, onInitFs3, errorHandler);
}

function errorHandler(e) {
  var msg = '';

  switch (e.code) {
    case FileError.QUOTA_EXCEEDED_ERR:
      msg = 'QUOTA_EXCEEDED_ERR';
      break;
    case FileError.NOT_FOUND_ERR:
      msg = 'NOT_FOUND_ERR';
      break;
    case FileError.SECURITY_ERR:
      msg = 'SECURITY_ERR';
      break;
    case FileError.INVALID_MODIFICATION_ERR:
      msg = 'INVALID_MODIFICATION_ERR';
      break;
    case FileError.INVALID_STATE_ERR:
      msg = 'INVALID_STATE_ERR';
      break;
    default:
      msg = 'Unknown Error';
      break;
  };

  console.log('Error: ' + msg);
  logHtml('Error: ' + msg);
}

function test_01() {
  logHtml("<br>");
  logHtml("test_01<br>");
  t = new TestClass01();
  logHtml("getS():" + t.getS() + "<br>");
  logHtml("getI():" + t.getI() + "<br>");
  logHtml("state:" + t.state + "<br>");
}

function test_hex_01() {
  logHtml("<br>");
  logHtml("test_hex_01<br>");
  var i = 10;
  var n = 4;
  var s = "0000000" + i.toString(16);
  var s2 = s.substr(s.length - n);
  logHtml("i:" + i + "<br>");
  logHtml("i.toString(16):" + s2 + "<br>");
  logHtml("toHex(i):" + toHex(i) + "<br>");
  logHtml("toHex(i, 8):" + toHex(i, 8) + "<br>");
}

function test_array_01() {
  logHtml("<br>");
  logHtml("test_array_01<br>");
  var a = [];
  a[10] = 10;
  a[20] = 20;
  for (var i in a) {
    logHtml("a[" + i + "] = " + a[i] + "<br>");
  }
}

function test_rgb_01(r, g, b) {
  logHtml("test_rgb_01<br>");
  logHtml("color : r " + toHex(r, 2) + " g " + toHex(g, 2) + " b " + toHex(b, 2) + "<br>");
  logHtml("rgb() : " + toHex(rgb(r, g, b), 8) + "<br>");
}

function test_rgba_01(r, g, b, a) {
  logHtml("test_rgba_01<br>");
  logHtml("color : r " + toHex(r, 2) + " g " + toHex(g, 2) + " b " + toHex(b, 2)  + " a " + toHex(a, 2) + "<br>");
  logHtml("rgba() : " + toHex(rgba(r, g, b, a), 8) + "<br>");
}

function test_color_01(r, g, b) {
  logHtml("test_color_01<br>");
  var color = new RGBColour(r, g, b);
  var hsv = color.getHSV();
  var hsl = color.getHSL();
  logHtml("color : r " + r + " g " + g + " b " + b + "<br>");
  logHtml("hsv : h " + hsv.h + " s " + hsv.s + " v " + hsv.v + "<br>");
  logHtml("hsl : h " + hsl.h + " s " + hsl.s + " l " + hsl.l + "<br>");
  logHtml("CSSHSL : " + color.getCSSHSL() + "<br>");
  logHtml("CSSHSLA : " + color.getCSSHSLA() + "<br>");
}

function test_color_distance_01(r1, g1, b1, r2, g2, b2) {
  logHtml("test_color_distance_01<br>");
  var co1 = { 'r': r1, 'g': g1, 'b': b1 };
  var co2 = { 'r': r2, 'g': g2, 'b': b2 };
  var distance = rgbDistance(co1, co2);
  logHtml("color1 : r " + r1 + " g " + g1 + " b " + b1 + "<br>");
  logHtml("color2 : r " + r2 + " g " + g2 + " b " + b2 + "<br>");
  logHtml("distance : " + distance + "<br>");
}

function TestClass01() {
  this.s = "string toto";
  this.i = 1;
}

TestClass01.prototype = {
  constructor: TestClass01,
  getS: function () { return this.s; },
  getI: function () { return this.i; },
  state: { get function () { if (this.i < 10) return "small"; else return "big"; }}
};


