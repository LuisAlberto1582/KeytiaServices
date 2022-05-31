var pagePath = window.location.pathname;
var dataJSON;
var dataDisp1 = [];
var total = 0;
var totLlam = 0;
var totMin = 0;
var totSitios = 0;
var totT = 0;

function GetInfo(method) {
    $.ajax({
        url: pagePath + "/" + method,
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

function ConsumoCencos(method) {
    GetInfo(method);
    var obj = dataJSON;
    var dataDisp = sortJSON(obj.records, 'TOTALREAL', 'desc');
    $jQuery2_3.each(dataDisp, function (i, item) {

        total += dataDisp[i].TOTALREAL;
        totLlam += dataDisp[i].LLAMADAS;
        totMin += dataDisp[i].MINUTOS;
        totSitios += dataDisp[i].TOTALSITIOS;
        totT += 1;
        var rf = { "recid": dataDisp[i].recid, "Plazas": dataDisp[i].Plazas, "TOTALREAL": dataDisp[i].TOTALREAL, "LLAMADAS": dataDisp[i].LLAMADAS, "MINUTOS": dataDisp[i].MINUTOS, "TOTALSITIOS": dataDisp[i].TOTALSITIOS };
        dataDisp1.push(rf);

    });
    var sb = { w2ui: { summary: true }, recid: "", Plazas: '<span style="float:right;font-weight:bold;font-size:14px;">Total:</span>', TOTALREAL: total, LLAMADAS: totLlam, MINUTOS: totMin, TOTALSITIOS: totSitios };
    dataDisp1.push(sb);
    var st = { w2ui: { summary: true }, recid: "S-2", Plazas: '<span style="float:right;font-weight:bold;font-size:13px;">Total: ' + totT + "</span>" };
    dataDisp1.push(st);
    var cols = [
        { field: "recid", caption: '#', size: '0px', sortable: true, frozen: true },
        { field: "Plazas", caption: 'Plazas', size: '250px', sortable: true, frozen: true },
        { field: "TOTALREAL", caption: 'Total', size: '140px', render: 'money', sortable: true, frozen: true },
        { field: "LLAMADAS", caption: 'Cantidad de Llamadas', size: '150px', render: 'float', sortable: true },
        { field: "MINUTOS", caption: 'Cantidad de Minutos', size: '140px', render: 'float', sortable: true },
        { field: "TOTALSITIOS", caption: 'Cantidad de Sitios', size: '140px', render: 'float', sortable: true }
    ];

    $jQuery2_3('#ConsumoCencos').w2grid({
        name: 'ConsumoCencos',
        header: 'CONSUMO POR PLAZAS',
        show: {
            header: true,
            toolbar: true,
            lineNumbers: false,
            footer: false
        },
        reorderColumns: true,
        searches: [
            { type: 'text', field: 'Plazas', caption: 'Plazas', operator: 'contains' }
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
        records: dataDisp1
    });

    w2ui['ConsumoCencos'].hideColumn('recid');
    w2ui.ConsumoCencos.on('click', function (event) {
        var grid = this;
        event.onComplete = function () {
            var record = this.get(event.recid);
            var e = record.recid;
            var p = record.Plazas;
            if (e !== null) {
                sessionStorage.clear();
                sessionStorage.setItem('recid', e);
                sessionStorage.setItem('Plazas', p);
                sessionStorage.setItem('N1', pagePath);
                var ed = sessionStorage.getItem("recid");
                if (ed !== null) {
                    window.location.href = 'RepCencosJerN2.aspx';
                }
            }
        }
    });
};