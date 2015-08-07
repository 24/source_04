rem from http://docs.sencha.com/ext-js/4-1/#!/guide/getting_started

rem sencha create jsb -a index.html -p app.jsb3
rem sencha create jsb -a http://localhost/helloext/index.html -p app.jsb3
rem sencha build -p app.jsb3 -d .

rem err ReferenceError: Can't find variable: Ext - impossible d'accéder à /dev/sencha/extjs-4.1.1a/ext.js
rem ok avec ../extjs-4.1.1a/ext.js
rem sencha create jsb -a test_sencha_01.html -p test_sencha_01.jsb3

rem ok generate test_sencha_01.jsb3
rem sencha create jsb -a http://localhost/dev/html/test_sencha/test_sencha_01.html -p test_sencha_01.jsb3

rem generate all-classes.js (application's classes) and app-all.js (application plus all of the Ext JS classes required to run)
rem [ERROR] File 'C:\pib\_db\pbeuz\Dropbox\dev\project\php\html\test_sencha\..\..\sencha\extjs-4.1.1a\src\util\Observable.js' is either not existent or unreadble
sencha build -p test_sencha_01.jsb3 -d .

