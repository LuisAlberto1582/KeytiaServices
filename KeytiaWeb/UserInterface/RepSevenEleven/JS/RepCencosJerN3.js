var pagePath = window.location.pathname;
var pageback2 = sessionStorage.getItem("N2");
var urlFile = sessionStorage.getItem("url");
var dataJSON;
var dataDisp1 = [];
var total = 0;
var totLlam = 0;
var totMin = 0;
var totSitios = 0;
var totalsitio = 0;
var ed = sessionStorage.getItem("recid2");
var ce = urlFile + " > " + "<a href='" + pageback2 + "'>" + sessionStorage.getItem("Plazas") + "</a>" + " > " + sessionStorage.getItem("Mercados");
if (ed === null) {
    ed = 0;
}
function GetInfo(method) {
    $.ajax({
        url: pagePath + "/" + method,
        data: "{ 'cencosClave3':'" + ed + "' }",
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

function ConsumoCencosN3(method) {
    GetInfo(method);
    var obj = dataJSON;
    var dataDisp = sortJSON(obj.records, 'TOTALREAL', 'desc');

    $jQuery2_3.each(dataDisp, function (i, item) {

        total += dataDisp[i].TOTALREAL;
        totLlam += dataDisp[i].LLAMADAS;
        totMin += dataDisp[i].MINUTOS;
        totSitios += dataDisp[i].TOTALSITIOS;
        totalsitio += 1;
        var rf = { "recid": dataDisp[i].recid, "Tiendas": dataDisp[i].Tiendas, "TOTALREAL": dataDisp[i].TOTALREAL, "LLAMADAS": dataDisp[i].LLAMADAS, "MINUTOS": dataDisp[i].MINUTOS, "TOTALSITIOS": dataDisp[i].TOTALSITIOS };
        dataDisp1.push(rf);
    });
    var sb = { w2ui: { summary: true }, recid: "S-1", Tiendas: '<span style="float:right;font-weight:bold;font-size:14px;">Total: </span>', TOTALREAL: total, LLAMADAS: totLlam, MINUTOS: totMin };
    dataDisp1.push(sb);
    var st = { w2ui: { summary: true }, recid: "S-2", Tiendas: '<span style="float:right;font-weight:bold;font-size:13px;">Total : ' + totalsitio + "</span>" };
    dataDisp1.push(st);
    var cols = [
        { field: "recid", caption: '#', size: '0px', sortable: true, frozen: true },
        { field: "Tiendas", caption: 'Tiendas', size: '450px', sortable: true, frozen: true },
        { field: "TOTALREAL", caption: 'Total', size: '140px', render: 'money', sortable: true, frozen: true },
        { field: "LLAMADAS", caption: 'Cantidad de Llamadas', size: '150px', render: 'float', sortable: true },
        { field: "MINUTOS", caption: 'Cantidad de Minutos', size: '140px', render: 'float', sortable: true }
    ];

    $jQuery2_3('#ConsumoCencosN3').w2grid({
        name: 'ConsumoCencosN3',
        header: ce,
        show: {
            header: true,
            toolbar: true,
            lineNumbers: false,
            footer: false
        },
        reorderColumns: true,
        searches: [
            { type: 'text', field: 'Tiendas', caption: 'Tiendas', operator: 'contains' }
        ],
        toolbar: {
            items: [
                { type: 'spacer' },
                { type: 'break' },
                {
                    type: 'button', id: 'item7', text: '<< Regresar ', img: '', checked: true,
                    onClick: function (event) {
                        window.location.href = 'RepCencosJerN2.aspx';
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
        records: dataDisp1
    });

    w2ui['ConsumoCencosN3'].hideColumn('recid');
    w2ui.ConsumoCencosN3.on('click', function (event) {
        var grid = this;
        event.onComplete = function () {
            var record = this.get(event.recid);
            var e = record.recid;
            var p = record.Tiendas;
            if (e !== null) {
                sessionStorage.setItem('recid3', e);
                sessionStorage.setItem('Tiendas', p);
                sessionStorage.setItem('N3', pagePath);
                var ed = sessionStorage.getItem("recid3");
                if (ed !== null) {
                    window.location.href = 'RepCencosJerN4.aspx';
                }
            }
        }
    });
};