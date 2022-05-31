function BlockUI(elementID) {
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_beginRequest(function() {
        $("#" + elementID).block({ message: '<table align = "center"><tr><td>' +
     '<img src="~/images/loadingAnim.gif"/></td></tr></table>',
            css: {},
            overlayCSS: { backgroundColor: '#000000', opacity: 0.6
            }
        });
    });
    prm.add_endRequest(function() {
        $("#" + elementID).unblock();
    });
}

//20150330 NZ Se comentaron estos estilos por que causan conflictos con el modal de buscar numeros de serie para el inventario.
//$(document).ready(function() {

//BlockUI("<%=pnlAddEditInventario.ClientID %>");
//    $.blockUI.defaults.css = {};
//});
function Hidepopup() {
    $find("popup").hide();
    return false;
}
