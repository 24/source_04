// temps de chargement avec localhost apache http://localhost/dev/html/test_sencha/test_sencha_01.html :
//   ext.js               : entre 1.66 et 1.93
//   ext-debug.js         : entre 1.67 et 2.10
//   ext-all-debug.js     : entre 0.66 et 0.73
//   ext-all.js           : entre 0.51 et 0.61
// temps de chargement en direct file:///C:/pib/dev/OpenProject/sencha/test/test_sencha_01.html :
//   ext.js               : entre 0.59 et 0.63
//   ext.js + app-all.js  : entre 0.21 et 0.27

Ext.require('Ext.container.Viewport');
Ext.application({
    name: 'HelloExt',
    launch: function() {
        Ext.create('Ext.container.Viewport', {
            layout: 'fit',
            items: [
                {
                    title: 'Hello Ext',
                    html : 'Hello! Welcome to Ext JS.'
                }
            ]
        });
    }
});

