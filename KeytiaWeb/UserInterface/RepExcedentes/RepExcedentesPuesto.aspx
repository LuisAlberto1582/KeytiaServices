<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepExcedentesPuesto.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepExcedentes.RepExedentesPuesto" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
<link href="../InventarioTelecom/css/w2ui.css" rel="stylesheet" />
<script type="text/javascript" src="../InventarioTelecom/js/jquery.js"></script>
<script type="text/javascript" src="../InventarioTelecom/js/w2ui.js"></script>
<script type="text/javascript">var $jQuery2_3 = $.noConflict(true);</script>
<script type="text/javascript" src="OrdenaDatos.js"></script>
  <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
 <style type="text/css">
.modalUpload
{
           position: fixed;
    z-index: 999;
    height: 100%;
    width: 100%;
    top: 0;
    /*background-color: Black;
    filter: alpha(opacity=60);
    opacity: 0.6;
    -moz-opacity: 0.8;*/
}
.centerUpload
{
  z-index: 1000;
    margin: 300px auto;
    padding: 10px;
    width: 130px;
    padding-right:500px;
    /*background-color: White;
    border-radius: 10px;
    filter: alpha(opacity=100);
    opacity: 1;
    -moz-opacity: 1;*/

}
.center img
{
    height: 120px;
    width: 120px;
}
.txtDate1{
   float: left;
    margin-top: -10px;
    height: 45px;
}

.applyDate1 {
    background-color: #F58426;
    border: 1px solid #F58426;
    font-family: "Poppins",sans-serif;
    font-weight: 100;
    width: 60px;
    height: 45px;
    margin-top: 10px;
    color: white;
    font-size: 14px;
}
    .control-label1 {
        text-align: right;
        margin-bottom: 0;
        padding-top: 4px;
        padding-left: 9px;
    }
    .classButton{
    background-color:transparent;
    width:0px;
    height:0px;
    border:none;
}
    </style>
<script type="text/javascript">
    var ed;
    var ed = sessionStorage.getItem("ClaveCencos");
    var ce = sessionStorage.getItem("Cencos");
    if (ed === null) {
        ed = 0;
    }
            var pagePath = window.location.pathname;
            var dataJSON;
            function GetInfo(method) {
                $.ajax({
                    url: pagePath + "/" + method,
                    data: "{ 'cencosClave':'" + ed + "' }",
                    async: false,
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {
                        if (result.d != null) {
                            dataJSON = JSON.parse(result.d);
                        }
                        else { dataJSON = JSON.parse(errTxt); }
                    },
                    error: function (XMLHttpRequest, callStatus, errorThrown) {
                        dataJSON = JSON.parse(errTxt);
                    }
                });
            }

    function Excedentes(method) {
                GetInfo(method);
                var obj = dataJSON;
                var dataDisp = sortJSON(obj.records, 'Excedente', 'desc');
                t = dataDisp;
        var cols = [
                    { field: "recid", caption: 'ClavePuesto', size: '1', sortable: true, frozen: true },
                    { field: "CenCos", caption: 'Departamento', size: '180px', sortable: true, frozen: true },                 
                    { field: "Puesto", caption: 'Puesto', size: '200px', sortable: true },
                    { field: "CantEmples", caption: 'Cantidad de Empleados', size: '180px', sortable: true },
                    { field: "Renta", caption: 'Renta', render: 'money',size: '140px', sortable: true },
                    { field: "Excedente", caption: 'Excedente', render: 'money',size: '140px', sortable: true },
                    { field: "Total", caption: 'Total', render: 'money', size: '140px', sortable: true }
                ];

            $jQuery2_3('#excedentePuestos').w2grid({
                name: 'excedentePuestos',
                    header: 'Excedentes por Puestos',
                    show: {
                        header: true,
                        toolbar: true,
                        lineNumbers: false,
                        footer: true
                    },
                    reorderColumns: true,
                    searches: [
                        { type: 'text', field: 'Cencos', caption: 'Departamento', operator: 'contains' },
                        { type: 'text', field: 'Puesto', caption: 'Puesto', operator: 'contains' },
                        { type: 'text', field: 'CantEmples', caption: 'Cantidad de Empleados', operator: 'contains' },
                        { type: 'text', field: 'Renta', caption: 'Renta', operator: 'contains' },
                        { type: 'text', field: 'Excedente', caption: 'Excedente', operator: 'contains' },
                        { type: 'text', field: 'Total', caption: 'Total', operator: 'contains' }
                    ],
                    toolbar: {
                        items: [
                            { type: 'spacer' },
                            { type: 'break' },
                            {
                                type: 'button', id: 'item7', text: '<< Regresar ', img: '', checked: true,
                                onClick: function (event) {
                                    window.location.href = 'RepExcedenteCencos.aspx';
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
        w2ui['excedentePuestos'].hideColumn('recid');
        w2ui.excedentePuestos.on('click', function (event) {
            var grid = this;
            event.onComplete = function () {
                var record = this.get(event.recid);
                var e = record.recid;
                if (e != null && ed != null) {
                    //sessionStorage.clear();
                    sessionStorage.setItem('ClavePuesto', e);
                    var e = sessionStorage.getItem("ClavePuesto");
                    if (e != null && ed != null) {
                        if (e > 0 ) {
                            window.location.href = 'RepExedentesEmples.aspx';
                        }
                       
                    }
                }
            }
        }); 
    };
</script>
<script>
            var colGraf = [];
            var datosDisplay = [];
            var datosGrafica;
    </script>
<script type="text/javascript">
    var dataJSON1;
    function GetInfo1(method1) {
        $.ajax({
            url: pagePath + "/" + method1,
            data: "{ 'cencosClave1':'" + ed + "' }",
            async: false,
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result.d != null) {
                    dataJSON1 = JSON.parse(result.d);
                }
                else { dataJSON1 = JSON.parse(errTxt); }
            },
            error: function (XMLHttpRequest, callStatus, errorThrown) {
                dataJSON1 = JSON.parse(errTxt);
            }
        });
    }
    function Historico(method1) {
        GetInfo1(method1);
        var obj1 = dataJSON1;
        var dataDisp1 = obj1.records1;
        var columnas = dataDisp1[1];
        var columns1 = []; 
        datosGrafica = dataDisp1;

        for (var key in columnas) {
            var arr = key;
            colGraf.push(arr);

            var caption;
            var frosen;
            var size;
            var render;
            if (key === 'recid') {
                caption = "CONCEPTO";
                frosen = true;
                size = "85px";
            }
            else {
                caption = key;
                frosen = false;
                size = "145px";
                render= "money";
            }
            var auxCol = { "field": key, "caption": caption, "size": size, "render":render ,"sortable": false, "frozen": frosen };
            columns1.push(auxCol);  
            
        }
        $jQuery2_3('#excedenteHistorico').w2grid({
            name: 'excedenteHistorico',
            header: ce,
            show: {
                header: true,
                toolbar: false,
                lineNumbers: false,
                footer: true
            },
            reorderColumns: false,
            columns: columns1,
            records: dataDisp1
        });
    };
</script>
 <script type="text/javascript">
        google.charts.load('current', { packages: ['corechart', 'line'] });
     google.charts.setOnLoadCallback(drawBasic);
     function drawBasic() {
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'X');
            data.addColumn('number', 'Excedente');
            var columnsIn = datosGrafica[1];
            for (var key in columnsIn) {
                if (key != 'recid' && key != 'TOTAL') {
                    data.addRows([['' + key + '', parseInt(datosGrafica[1][key])]]);
                }
            }
         var options = {
             'height': 450,
             title: ce,
             legend: { position: 'middle' },
             hAxis: {
                 title: 'Mes',
                 titleTextStyle: {                 
                     bold: true,
                     fontSize: 16
                 }
             },
             vAxis: {
                 format: 'currency',
                 title: 'Excedente',
                 titleTextStyle: {
                     bold: true,
                     fontSize: 16
                 }
             }
         };
            
            var chart = new google.visualization.LineChart(document.getElementById('chart_div1'));
            chart.draw(data, options);
        }

</script>
         <script type="text/javascript">
             function downloadFile() {
                 console.log('entro');
                 var btn = '<%=btnExportar.ClientID%>';
                 var obj = document.getElementById(btn);
                 obj.click();
             }
 </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
EnableScriptGlobalization="true">
</asp:ToolkitScriptManager>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
<ProgressTemplate>
    <div class="modalUpload">
        <div class="centerUpload">
             <asp:Image class="center" runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
        </div>
    </div>
</ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
<div ID="pnlMainHolder" runat="server">
    <div ID="pnlRow_0" runat="server" CssClass="row">
        <div ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
            <div class="portlet solid bordered">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Excedentes por Puestos</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body" id="divContenedor">
                    <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form" >
                        <div class="row">                           
                            <div class="col-sm-12" id="detalle">
                                <div class="col-sm-12">
                                     <div id="excedentePuestos" style="height:280px;"></div>
                                </div>                     
                            </div>
                            <asp:Button ID="btnExportar" runat="server" CssClass="classButton" OnClick="btnExportar_Click"/> 
                        </div>
                        <div class="row">
                             <div class="col-sm-12">
                                <div class="col-sm-12">
                                    <br />
                                    <div id="excedenteHistorico" style="height:180px;"></div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class=" col-sm-12">
                                <div style="height:100%;width:100%; overflow:auto">
                                    <div id="chart_div1" style="height:450px"></div></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</ContentTemplate>
<Triggers>
  <asp:PostBackTrigger ControlID="btnExportar" />
</Triggers>
</asp:UpdatePanel>
<%--<script type="text/javascript">
        window.onsubmit = function () {          
                var updateProgress = $find("<%=UpdateProgress1.ClientID%>");
                window.setTimeout(function () {
                    updateProgress.set_visible(true);
                }, 100);
            
        }
</script>--%>
</asp:Content>
