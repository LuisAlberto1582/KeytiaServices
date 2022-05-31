var pagePath = window.location.pathname;
var dataJSON1;
var dataDisp0 = [];
var totalC = 0;

function GetInfo1(method1) {
    $.ajax({
        url: pagePath + "/" + method1,
        async: false,
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.d !== null) {
                dataJSON1 = JSON.parse(result.d);
            }
            else { dataJSON1 = JSON.parse(errTxt); }
        },
        error: function (XMLHttpRequest, callStatus, errorThrown) {
            dataJSON1 = JSON.parse(errTxt);
        }
    });
}

function ConsumoSianaConsolidado(method1) {
    GetInfo1(method1);
    var obj1 = dataJSON1;
    var dataDisp = sortJSON(obj1.records, 'IMPORTE', 'desc');
    var contadorreg=1;
    $jQuery2_3.each(dataDisp, function (i, item) {

        totalC += dataDisp[i].IMPORTE;
        var rf = { "recid": contadorreg, "CuentaMaestra": dataDisp[i].CuentaMaestra, "TELEFONOFACTURA": dataDisp[i].TELEFONOFACTURA.replace("'", ""), "LINEAKEYTIA": dataDisp[i].LINEAKEYTIA.replace("'", ""), "Tienda-Linea": dataDisp[i].TiendaLinea, "Division": dataDisp[i].Division, "Mercado": dataDisp[i].Mercado, "Tienda-Mercado": dataDisp[i].TiendaMercado, "CARRIER": dataDisp[i].CARRIER, "IMPORTE": dataDisp[i].IMPORTE };
        dataDisp0.push(rf);
        contadorreg++;
    });

    var sb = { w2ui: { summary: true }, recid: "", CuentaMaestra: '<span style="float:right;font-weight:bold;font-size:14px;">Total:</span>', IMPORTE: totalC };
    dataDisp0.push(sb);

    var cols = [
        { field: "recid", caption: '#', size: '50px', sortable: true, frozen: true },
        { field: "CuentaMaestra", caption: 'Cuenta Maestra', size: '110px', sortable: true, frozen: true },
        { field: "TELEFONOFACTURA", caption: 'Telefono Factura', size: '130px', sortable: true, frozen: true },
        { field: "LINEAKEYTIA", caption: 'Linea Keytia', size: '130px', sortable: true },
        { field: "Tienda-Linea", caption: 'Tienda-Linea', size: '280px', sortable: true },
        { field: "Division", caption: 'Division', size: '200px', sortable: true },
        { field: "Mercado", caption: 'Mercado', size: '200px', sortable: true },
        { field: "Tienda-Mercado", caption: 'Tienda-Mercado', size: '250px', sortable: true },
        { field: "CARRIER", caption: 'Carrier', size: '80px', sortable: true },
        { field: "IMPORTE", caption: 'Importe', size: '100px', render: 'money', sortable: true }
    ];

    $jQuery2_3('#ConsumoLinea').w2grid({
        name: 'ConsumoLinea',
        header: '',
        show: {
            header: true,
            toolbar: true,
            lineNumbers: false,
            footer: true
        },
        reorderColumns: true,
        searches: [
            { type: 'text', field: 'CuentaMaestra', caption: 'Cuenta Maestra', operator: 'contains' },
            { type: 'text', field: 'TELEFONOFACTURA', caption: 'Telefono Factura', operator: 'contains' },
            { type: 'text', field: 'LINEAKEYTIA', caption: 'Telefono Factura', operator: 'contains' },
            { type: 'text', field: 'Tienda-Linea', caption: 'Tienda-Linea', operator: 'contains' },
            { type: 'text', field: 'Division', caption: 'Division', operator: 'contains' },
            { type: 'text', field: 'Mercado', caption: 'Mercado', operator: 'contains' },
            { type: 'text', field: 'Tienda-Mercado', caption: 'Tienda-Mercado', operator: 'contains' }
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
        records: dataDisp0
    });

    //w2ui['ConsumoLinea'].hideColumn('recid');
}