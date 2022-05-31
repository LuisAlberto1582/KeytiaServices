var pagePath = window.location.pathname;
var dataJSON3;
var dataDisp13 = [];
var total = 0;

function GetInfo3(method3) {
    $.ajax({
        url: pagePath + "/" + method3,
        async: false,
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.d !== null) {
                dataJSON3 = JSON.parse(result.d);
            }
            else { dataJSON3 = JSON.parse(errTxt); }
        },
        error: function (XMLHttpRequest, callStatus, errorThrown) {
            dataJSON = JSON.parse(errTxt);
        }
    });
}

function ConsumoTipoDestino(method3) {
    GetInfo3(method3);
    var obj3 = dataJSON3;
    var dataDisp3 = obj3.records;
    var columnas3 = dataDisp3[1];
    var columns4 = [];
    var fileTotale = [];
    var totalReg = 1;
    for (var key in columnas3) {
        var caption;
        var frozen;
        var size;
        var render;

        if (key === 'Tienda-Linea') {
            caption = key;
            frozen = true;
            size = "305px";
        }
        else if (key === 'recid') {
            caption = '#';
            frozen = true;
            size = "70px";
        }
        else {
            caption = key;
            frozen = false;
            size = "250px";

            if (key !== 'Tienda-Linea' && key !== 'Division' && key !== 'Mercado' && key !== 'Tienda-Mercado' && key !== 'recid' && key !== 'Cuenta Maestra') {
                caption = key;
                frozen = false;
                size = "150px";
                render = "money";
                var tFila = 0;
                $jQuery2_3.each(dataDisp3, function (i, item) {
                    dataDisp3.map(function (dato) {
                        if (dato[key] === "") {
                            dato[key] = 0;
                        }
                        else {
                            dato.Total = parseFloat(dato.Total);
                        }
                        return dato;
                    });
                });
            }
        }
        var auxCol = { "field": key, "caption": caption, "size": size, "render": render, "sortable": true, "frozen": frozen };
        columns4.push(auxCol);
    }
    //var r = sortJSON(dataDisp3, 'Total', 'desc');

    $jQuery2_3.each(dataDisp3, function (i, item) {
        total += dataDisp3[i].Total;      
    });

    for (var key1 in columnas3) {
        $jQuery2_3.each(dataDisp3, function (i, item) {
            dataDisp3.map(function (dato1) {
                if (dato1[key1] === "recid") {
                    dato1[key1] = totalReg;
                }               
                return dato1;
            });
        });

        totalReg++;
    }

    var sb = { w2ui: { summary: true }, recid: "", "Tienda-Linea": '<span style="float:right;font-weight:bold;font-size:14px;">Total:</span>', Total: total };
    //r.push(sb);
    dataDisp3.push(sb);
    //for (var key1 in columnas3) {
    //    if (key1 !== 'Empleado' && key1 !== 'Cecos (1er Nivel)' && key1 !== 'Cecos (2do Nivel)' && key1 !== 'Cecos (3er Nivel)' && key1 !== 'recid') {
    //        var suma = 0;
    //        $jQuery2_3.each(dataDisp3, function (i, item) {
    //            //console.log(dataDisp3[i][key1]);
    //            //total += parseFloat(dataDisp3[i][key1]);               
    //        });
    //        //var t = key1 + ":" + suma;
    //        //var sb = { w2ui: { summary: true }, recid: "", Empleado: '<span style="float:right;font-weight:bold;font-size:14px;">Total:</span>', key1: suma };
    //        //r.push(sb);
    //        //console.log(t);
    //    }

    //}


    $jQuery2_3('#tipoDestino').w2grid({
        name: 'tipoDestino',
        header: '',
        show: {
            header: true,
            toolbar: true,
            lineNumbers: false,
            footer: true
        },
        reorderColumns: true,
        searches: [
            { type: 'text', field: 'Tienda-Linea', caption: 'Tienda-Linea', operator: 'contains' },
            { type: 'text', field: 'Division', caption: 'Division', operator: 'contains' },
            { type: 'text', field: 'Mercado', caption: 'Mercado', operator: 'contains' },
            { type: 'text', field: 'Tienda-Mercado', caption: 'Tienda-Mercado', operator: 'contains' },
            { type: 'text', field: 'Cuenta Maestra', caption: 'Cuenta Maestra', operator: 'contains' }

        ],
        toolbar: {
            items: [
                { type: 'spacer' },
                { type: 'break' },
                {
                    type: 'button', id: 'item7', text: 'Exportar', img: 'icon-page', checked: true,
                    onClick: function (event) {
                        downloadFile();
                    }
                },
                { type: 'spacer' },
                { type: 'break' }
            ]
        },
        columns: columns4,
        records: dataDisp3//r
    });

    //w2ui['tipoDestino'].hideColumn('recid');
}