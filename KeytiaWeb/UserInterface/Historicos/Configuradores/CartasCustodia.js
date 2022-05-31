Historico.prototype.fnRenderRecurso = function(obj, imgClass, imgSrc, sdoPostBack) {
    var iCodRegistro = obj.aData[0];
    var btnID = (sdoPostBack.split(",'")[1]).split(":")[0] + iCodRegistro;
    sdoPostBack = sdoPostBack.replace("{0}", iCodRegistro);
    var ret = "<a id=\"" + btnID + "\" href=\"javascript:OcultarBotones(" + sdoPostBack + "," + iCodRegistro + ");\">";
    ret += "<img class='" + imgClass + "' src='" + imgSrc + "'></a>";
    return ret;
}
OcultarBotones = function(doPostBack, iCodRegistro) {
    var btnAceptarID = "btnAceptarRecurso" + iCodRegistro;
    var btnRechazarID = "btnRechazarRecurso" + iCodRegistro;
    var btnAceptar = document.getElementById(btnAceptarID);
    btnAceptar.style.display = "none";
    var btnRechazar = document.getElementById(btnRechazarID);
    btnRechazar.style.display = "none";
}