var pagePath = window.location.pathname;
$(function () {
    GetCustomers();
});
$("[id*=txtBuscar]").live("keyup", function () {
    GetCustomers();
});

function SearchTerm() {
    return jQuery.trim($("[id*=txtBuscar]").val());
}
function GetCustomers() {
    $.ajax({
        type: "POST",
        url: pagePath + "/GetCustomers",
        data: '{searchTerm: "' + SearchTerm() + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccess,
        failure: function (response) {
            alert(response.d);
        },
        error: function (response) {
            alert(response.d);
        }
    });
}
var row;
function OnSuccess(response) {
    var xmlDoc = $.parseXML(response.d);
    var xml = $(xmlDoc);
    var customers = xml.find("Table1");
    if (row == null) {
        row = $("[id*=grvListadoCargas] tr:last-child").clone(true);
    }
    $("[id*=grvListadoCargas] tr").not($("[id*=grvListadoCargas] tr:first-child")).remove();
    if (customers.length > 0) {
        $.each(customers, function () {
            var customer = $(this);
            var r = $(this).find("vchDescripcion").text();
            var icodCat = $(this).find("iCodCatalogo").text();
            $("td", row).eq(0).html("<button type='button'class='btn btn-link' onclick=myFunction(" + icodCat + ",'" + r + "');><span class='glyphicon glyphicon-edit'></span></button>");
            $("td", row).eq(1).html("<button type='button'class='btn btn-link' data-toggle='modal' data-target='#exampleModal' onclick=deleteCarga(" + icodCat + ",'" + r + "');><span class='glyphicon glyphicon-remove'></span></button>");
            $("td", row).eq(2).html($(this).find("vchDescripcion").text());
            $("td", row).eq(3).html($(this).find("AnioDesc").text());
            $("td", row).eq(4).html($(this).find("MesDesc").text());
            $("td", row).eq(5).html($(this).find("EstCargaDesc").text());
            $("td", row).eq(6).html($(this).find("Archivo01").text());
            $("td", row).eq(7).html($(this).find("Archivo02").text());
            $("td", row).eq(8).html($(this).find("Archivo03").text());
            $("td", row).eq(9).html($(this).find("Archivo04").text());
            $("[id*=grvListadoCargas]").append(row);
            row = $("[id*=grvListadoCargas] tr:last-child").clone(true);
        });
    } else {
        var empty_row = row.clone(true);
        $("td:first-child", empty_row).attr("colspan", $("td", row).length);
        $("td:first-child", empty_row).attr("align", "center");
        $("td:first-child", empty_row).html("No se encontraron cargas");
        $("td", empty_row).not($("td:first-child", empty_row)).remove();
        $("[id*=grvListadoCargas]").append(empty_row);
    }
}