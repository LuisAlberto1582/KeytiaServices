

    
var $jQuery2_2 = $.noConflict(true);


var config = {
    host: 'bikeytia.dti.com.mx',
    prefix: '/',
    port: 443,
    isSecure: true
};
require.config({
    baseUrl: (config.isSecure ? "https://" : "http://") + config.host + (config.port ? ":" + config.port : "") + config.prefix + "resources"
});
var qlikApps = [];

require(["js/qlik"], function (qlik) {

    var control = false;
    qlik.setOnError(function (error) {
        //alert(error.message);
        $('#popupText').append(error.message + "<br>");
        if (!control) {
            control = true;
            $('#popup').delay(1000).fadeIn(1000).delay(11000).fadeOut(1000);
        }
    });

    $("#closePopup").click(function () {
        $('#popup').hide();
    });

    function attach(elem) {
        var appid = elem.dataset.qlikAppid;
        var objid = elem.dataset.qlikObjid;
        var app = qlikApps[appid];
        if (!app) {
            app = qlik.openApp(appid, config);
            qlikApps[appid] = app;
        }
        app.getObject(elem, objid);
    }
    var elems = document.getElementsByClassName('qlik-embed');
    var ix = 0;
    for (; ix < elems.length; ++ix) {
        attach(elems[ix]);
    }
});
