var pagePath = window.location.pathname;
var dataJSON2;
var dataDisp11 = [];
var totalM = 0;

function GetInfo(method2) {
    $.ajax({
        url: pagePath + "/" + method2,
        async: false,
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.d !== null) {
                dataJSON2 = JSON.parse(result.d);
            }
            else { dataJSON2 = JSON.parse(errTxt); }
        },
        error: function (XMLHttpRequest, callStatus, errorThrown) {
            dataJSON2 = JSON.parse(errTxt);
        }
    });
}

function ConsumoCtaMaestra(method2) {
    GetInfo(method2);
    var obj1 = dataJSON2;
    var dataDisp = sortJSON(obj1.records, 'Total', 'desc');
    $jQuery2_3.each(dataDisp, function (i, item) {

        totalM += dataDisp[i].Total;
        var rf = { "recid": dataDisp[i].recid, "FecFactura": dataDisp[i].FecFactura, "CuentaMaestra": dataDisp[i].CuentaMaestra,"Total": dataDisp[i].Total };
        dataDisp11.push(rf);

    });
    var sb = { w2ui: { summary: true }, recid: "", FecFactura: '<span style="float:right;font-weight:bold;font-size:14px;">Total:</span>', Total: totalM };
    dataDisp11.push(sb);

    var cols = [
        { field: "recid", caption: '#', size: '0px', sortable: true },
        { field: "FecFactura", caption: 'Fecha Factura', size: '140px', sortable: true, frozen: true },
        { field: "CuentaMaestra", caption: 'Cuenta Maestra', size: '140px', sortable: true },       
        { field: "Total", caption: 'Importe', size: '100px', render: 'money', sortable: true }
    ];

    $jQuery2_3('#ConsumoCtaMaestra').w2grid({
        name: 'ConsumoCtaMaestra',
        header: '',
        show: {
            header: true,
            toolbar: true,
            lineNumbers: false,
            footer: false
        },
        reorderColumns: true,
        searches: [
            { type: 'text', field: 'CuentaMaestra', caption: 'Cuenta Maestra', operator: 'contains' }
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
        columns: cols,
        records: dataDisp11
    });

    w2ui['ConsumoCtaMaestra'].hideColumn('recid');
}