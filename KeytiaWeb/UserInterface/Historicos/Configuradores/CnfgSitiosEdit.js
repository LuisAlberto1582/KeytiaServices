function Confirmar(doPostBack, recursos, msg, msgtitle) {
    if (recursos > 0) {
        jConfirm(msg, msgtitle, function(r) {
            if (r) {
                doPostBack();
            }
        });
    }
    else {
        doPostBack();
    }
}