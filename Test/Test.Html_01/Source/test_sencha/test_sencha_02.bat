rem from http://docs.sencha.com/ext-js/4-1/#!/guide/getting_started

rem sencha create jsb -a index.html -p app.jsb3
rem sencha create jsb -a http://localhost/helloext/index.html -p app.jsb3
rem sencha build -p app.jsb3 -d .

rem ok generate test_sencha_01.jsb3
rem sencha create jsb -a test_sencha_01.html -p test_sencha_01.jsb3

rem ok generate test_sencha_01.jsb3
rem sencha create jsb -a http://localhost/dev/html/test_sencha/test_sencha_01.html -p test_sencha_01.jsb3
sencha create jsb -a test_sencha_02.html -p test_sencha_02.jsb3

rem ok generate all-classes.js (application's classes) and app-all.js (application plus all of the Ext JS classes required to run)
rem sencha build -p test_sencha_01.jsb3 -d .

