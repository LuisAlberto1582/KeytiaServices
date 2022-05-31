var pagePath = window.location.pathname;
var urlFile = sessionStorage.getItem("url");
var pageback2 = sessionStorage.getItem("N2");
var pageback3 = sessionStorage.getItem("N3");
var dataJSON;
var dataDisp1 = [];
var total = 0;
var totLlam = 0;
var totMin = 0;
var totSitios = 0;
var totalsitio = 0;
var ed = sessionStorage.getItem("recid3");
var ce = urlFile + " > " + "<a href='" + pageback2 + "'>" + sessionStorage.getItem("Plazas") + "</a>" + " > " + "<a href='" + pageback3 + "'>" + sessionStorage.getItem("Mercados") + "</a>" + " > "+sessionStorage.getItem("Tiendas");
if (ed === null) {
    ed = 0;
}
function GetInfo(method) {
    $.ajax({
        url: pagePath + "/" + method,
        data: "{ 'cencosClave4':'" + ed + "' }",
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

function ConsumoCencosN4(method) {
    GetInfo(method);
    var obj = dataJSON;
    var dataDisp = sortJSON(obj.records, 'TOTALREAL', 'desc');

    $jQuery2_3.each(dataDisp, function (i, item) {

        total += dataDisp[i].TOTALREAL;
        totLlam += dataDisp[i].LLAMADAS;
        totMin += dataDisp[i].MINUTOS;
        totalsitio += 1;
        var rf = { "recid": dataDisp[i].recid, "Emple": dataDisp[i].Emple, "Conceptos": dataDisp[i].Conceptos, "TOTALREAL": dataDisp[i].TOTALREAL, "LLAMADAS": dataDisp[i].LLAMADAS, "MINUTOS": dataDisp[i].MINUTOS };
        dataDisp1.push(rf);
    });
    var sb = { w2ui: { summary: true }, recid: "S-1", Conceptos: '<span style="float:right;font-weight:bold;font-size:14px;">Total: </span>', TOTALREAL: total, LLAMADAS: totLlam, MINUTOS: totMin };
    dataDisp1.push(sb);
    var st = { w2ui: { summary: true }, recid: "S-2", Conceptos: '<span style="float:right;font-weight:bold;font-size:13px;">Total : ' + totalsitio + "</span>" };
    dataDisp1.push(st);
    var cols = [
        { field: "recid", caption: '#', size: '0px', sortable: true, frozen: true },
        { field: "Emple", caption: '#1', size: '0px', sortable: true, frozen: true },
        { field: "Conceptos", caption: 'Conceptos', size: '250px', sortable: true, frozen: true },
        { field: "TOTALREAL", caption: 'Total', size: '140px', render: 'money', sortable: true, frozen: true },
        { field: "LLAMADAS", caption: 'Cantidad llamadas', size: '140px', render: 'float', sortable: true },
        { field: "MINUTOS", caption: 'Cantidad minutos', size: '140px', render: 'float', sortable: true }
    ];

    $jQuery2_3('#ConsumoCencosN4').w2grid({
        name: 'ConsumoCencosN4',
        header: ce,
        show: {
            header: true,
            toolbar: true,
            lineNumbers: false,
            footer: false
        },
        reorderColumns: true,
        searches: [
            { type: 'text', field: 'DESCRIPCION', caption: 'Tipo de Destino', operator: 'contains' }
        ],
        toolbar: {
            items: [
                { type: 'spacer' },
                { type: 'break' },
                {
                    type: 'button', id: 'item7', text: '<< Regresar ', img: '', checked: true,
                    onClick: function (event) {
                        window.location.href = 'RepCencosJerN3.aspx';
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

    w2ui['ConsumoCencosN4'].hideColumn('recid');
    w2ui['ConsumoCencosN4'].hideColumn('Emple');
    w2ui.ConsumoCencosN4.on('click', function (event) {
        var grid = this;
        event.onComplete = function () {
            var record = this.get(event.recid);
            var e = record.recid;
            var p = record.Conceptos;
            var em = record.Emple;
            if (e !== null) {
                sessionStorage.setItem('recid4', e);
                sessionStorage.setItem('emple', em);
                sessionStorage.setItem('Conceptos', p);
                sessionStorage.setItem('N4', pagePath);
                var ed = sessionStorage.getItem("recid4");
                if (ed !== null) {
                    window.location.href = 'RepCencosJerN5.aspx';
                }
            }
        }
    });
};