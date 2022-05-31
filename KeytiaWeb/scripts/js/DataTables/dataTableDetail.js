var pagePath = window.location.pathname;
var dataSet;
var IDTabla;
var columnsTable;
var primerRenglon;
var lastRowTot;

function GetDatosTabla(idTabla, nameWebMthod) {
    IDTabla = idTabla;

    jQNewLook.ajax({
        url: pagePath + "/" + nameWebMthod,
        async: false,
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.d != null) {
                dataSet = JSON.parse(result.d);
            }
            else { dataSet = JSON.parse(errTxt); }
        },
        error: function (XMLHttpRequest, callStatus, errorThrown) {
            dataSet = JSON.parse(errTxt);
        }
    });

    // Titulos de columnas de la tabla
    columnsTable = "[";
    primerRenglon = JSON.stringify(dataSet.data[0]);
    jQNewLook.each(JSON.parse(primerRenglon), function (key, value) {
        columnsTable += "{ \"data\" : " + '"' + key + '", \"title\" : ' + '"' + key + '" },';
    });
    columnsTable = columnsTable.slice(0, -1) + "]";
    columnsTable = JSON.parse(columnsTable);

    // Totales de la tabla  
    lastRowTot = JSON.stringify(dataSet.data[dataSet.data.length - 1]);
    lastRowTot = JSON.parse(lastRowTot);
    dataSet.data.splice([dataSet.data.length - 1], 1);  // NZ: Eliminamos los totales del JSON pues estos se incluiran aparte en el foother de la tabla

    DibujarTabla();
}

//Colocar la tabla
function DibujarTabla() {

    //Se agrega el html del foother para los totales.
    var totales = "<tfoot><tr>";
    var i = 0;
    for (var key in lastRowTot) {
        totales += "<th></th>";
        i++;
    }
    totales += "</tr></tfoot>";
    jQNewLook('#' + IDTabla).append(totales);


    //Se insertan los datos en la tabla
    var table = jQNewLook('#' + IDTabla).DataTable({

        // Titulos de columna en la tabla
        columns: columnsTable,

        // Registros a colocar en la tabla
        data: dataSet.data,

        // Mostrar el scroll en el eje Y. A cierta medida
        scrollY: '40vh',

        //Mostrar el scroll en el eje X
        scrollX: true,

        // Lenguaje que queremos que muestre la tabla
        language: { url: "https://cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json" },

        // Mostrar la paginación en la tabla
        paging: true,

        footerCallback: function (tfoot, data, start, end, display) {

            // Para dar formato a los importes de los totales.
            var regex = /^[A-Za-z\s]+$/;
            function numberWithCommas(x) {
                if (regex.test(x)) { return x; }
                else {
                    if (x.indexOf(".") >= 0) {
                        x = parseFloat(x).toFixed(2);
                        return Number(x).toLocaleString('en');
                    }
                    else {
                        return x.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    }
                }
            }

            // Llenamos los totales	  
            var x = 0;
            for (var key in lastRowTot) {
                jQNewLook(tfoot).find("th").eq(x).html(" " + numberWithCommas(lastRowTot[key].toString()));
                x++;
            }
        }
    });

    jQNewLook(window).resize(function () {
        table.columns.adjust();
        table.fixedHeader.adjust();
    });

}