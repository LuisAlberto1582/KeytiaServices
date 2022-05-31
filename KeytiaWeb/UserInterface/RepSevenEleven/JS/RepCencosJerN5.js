var pagePath = window.location.pathname;
var urlFile = sessionStorage.getItem("url");
var pageback2 = sessionStorage.getItem("N2");
var pageback3 = sessionStorage.getItem("N3");
var pageback4 = sessionStorage.getItem("N4");
var dataJSON;
var ed = sessionStorage.getItem("recid4");
var em = sessionStorage.getItem("emple");
var ce = urlFile + " > " + "<a href='" + pageback2 + "'>" + sessionStorage.getItem("Plazas") + "</a>" + " > " + "<a href='" + pageback3 + "'>" + sessionStorage.getItem("Mercados") + "</a>" + " > " + "<a href= '" + pageback4+"'>"+sessionStorage.getItem("Tiendas")+"</a>" + " > " + sessionStorage.getItem("Conceptos");
var tipodest = sessionStorage.getItem("Conceptos").toUpperCase();
if (ed === null) {
    ed = 0;
}

function GetInfo(method) {
    $.ajax({
        url: pagePath + "/" + method,
        data: "{ 'iCodEmple':'" + em + "','Tdest':'" + ed + "' }",
        async: false,
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.d !== null) {
                dataJSON = JSON.parse(result.d);
            }
            else { dataJSON = JSON.parse(errTxt); }
        },
        error: function (XMLHttpRequest, callStatus, errorThrown) {
            dataJSON = JSON.parse(errTxt);
        }
    });
}
function ConsumoCencosN5(method) {
    GetInfo(method);
    var obj = dataJSON;
    var dataDisp = sortJSON(obj.records, 'TotalReal', 'desc');
    var cols;
    if (tipodest === 'CELULAR LOCAL') {
         cols = [
            { field: "recid", caption: '#', size: '0px', sortable: true, frozen: true },
            { field: "Centro de costos", caption: 'Centro de costos', size: '325px', sortable: true, frozen: true },
            { field: "Colaborador", caption: 'Colaborador', size: '305px', sortable: true },
            { field: "Extensión", caption: 'Linea', size: '115px', sortable: true },
            { field: "Numero Marcado", caption: 'Numero Marcado', size: '170px', sortable: true },
            { field: "Fecha", caption: 'Fecha', size: '120px', sortable: true },
            { field: "Hora", caption: 'Hora', size: '120px', sortable: true },
            { field: "Fecha Fin", caption: 'Periodo Facturación', size: '140px', sortable: true },
            { field: "Duracion", caption: 'Duracion', size: '90px', render: 'float', sortable: true },
            { field: "Llamadas", caption: 'Llamadas', size: '90px', render: 'float', sortable: true },
            { field: "TotalReal", caption: 'Importe', size: '90px', render: 'money', sortable: true },
            { field: "Sitio", caption: 'Sitio', size: '250px', sortable: true },
            { field: "Carrier", caption: 'Carrier', size: '120px', sortable: true },
            { field: "Tipo de destino", caption: 'Tipo de destino', size: '200px', sortable: true }
        ];
    }
    else {
         cols = [
            { field: "recid", caption: '#', size: '0px', sortable: true, frozen: true },
            { field: "Centro de costos", caption: 'Centro de costos', size: '325px', sortable: true, frozen: true },
            { field: "Colaborador", caption: 'Colaborador', size: '305px', sortable: true },
            { field: "Extensión", caption: 'Linea', size: '112px', sortable: true },
            { field: "Fecha Fin", caption: 'Mes de Factura', size: '120px', sortable: true },
            { field: "TotalReal", caption: 'Importe', size: '110px', render: 'money', sortable: true },
            { field: "Sitio", caption: 'Sitio', size: '250px', sortable: true },
            { field: "Carrier", caption: 'Carrier', size: '110px', sortable: true },
            { field: "Tipo de destino", caption: 'Tipo de destino', size: '200px', sortable: true }
        ];
    }


    $jQuery2_3('#ConsumoCencosN5').w2grid({
        name: 'ConsumoCencosN5',
        header: ce,
        show: {
            header: true,
            toolbar: true,
            lineNumbers: false,
            footer: true
        },
        reorderColumns: true,
        toolbar: {
            items: [
                { type: 'spacer' },
                { type: 'break' },
                {
                    type: 'button', id: 'item7', text: '<< Regresar ', img: '', checked: true,
                    onClick: function (event) {
                        window.location.href = 'RepCencosJerN4.aspx';
                    }
                },
                { type: 'spacer' },
                { type: 'break' },
                {
                    type: 'button', id: 'item8', text: 'Exportar', img: 'icon-page', checked: true,
                    onClick: function (event) {
                        downloadFile();
                    }
                },
                { type: 'spacer' },
                { type: 'break' }
            ]
        },
        columns: cols,
        records: dataDisp
    });

    w2ui['ConsumoCencosN5'].hideColumn('recid');
};