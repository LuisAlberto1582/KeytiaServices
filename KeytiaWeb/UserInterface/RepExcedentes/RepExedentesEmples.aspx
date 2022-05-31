<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepExedentesEmples.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepExcedentes.RepExedentesEmples" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
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

<link href="../InventarioTelecom/css/w2ui.css" rel="stylesheet" />
<script type="text/javascript" src="../InventarioTelecom/js/jquery.js"></script>
<script type="text/javascript" src="../InventarioTelecom/js/w2ui.js"></script>
<script type="text/javascript">var $jQuery2_3 = $.noConflict(true);</script>
<script type="text/javascript" src="OrdenaDatos.js"></script>

<script type="text/javascript">
        var ed = sessionStorage.getItem("ClaveCencos");
        var e = sessionStorage.getItem("ClavePuesto");
            if (ed === null || e === null) {
                ed = 0;
                e = 0;
    }
            var pagePath = window.location.pathname;
            var dataJSON;
            var t;
            function GetInfo(method) {
                $.ajax({
                    url: pagePath + "/" + method,
                    data: "{ 'cencosClave':'" + ed + "', 'clavePuesto':'" + e + "' }",
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

            $jQuery2_3(document).ready(function () {
                $jQuery2_3('#excedenteEmples').w2grid.autoLoad = true;
                $jQuery2_3('#excedenteEmples').w2grid.skip = true;
            });

    function ExcedentesEmples(method) {
                GetInfo(method);
                var obj = dataJSON;
                var dataDisp = sortJSON(obj.records, 'Excedente', 'desc');
                t = dataDisp;
        var cols = [
                    { field: "recid", caption: 'Departamento', size: '200px', sortable: true, frozen: true },                 
                    { field: "Puesto", caption: 'Puesto', size: '200px', sortable: true },
                    { field: "Linea", caption: 'Línea', size: '130px', sortable: true },
                    { field: "NomCompleto", caption: 'Responsable',size: '200px', sortable: true },
                    { field: "Renta", caption: 'Renta', render: 'money',size: '135px', sortable: true },
                    { field: "Excedente", caption: 'Excedente', render: 'money', size: '135px', sortable: true },
                    { field: "Total", caption: 'Total', render: 'money', size: '135px', sortable: true }
                ];

            $jQuery2_3('#excedenteEmples').w2grid({
                    name: 'excedenteEmples',
                    header: 'Excedentes por Empleados',
                    show: {
                        header: true,
                        toolbar: true,
                        lineNumbers: false,
                        footer: true
                    },
                    reorderColumns: true,
                    searches: [
                        { type: 'text', field: 'recid', caption: 'Departamento', operator: 'contains' },
                        { type: 'text', field: 'Puesto', caption: 'Puesto', operator: 'contains' },
                        { type: 'text', field: 'Linea', caption: 'Línea', operator: 'contains' },
                        { type: 'text', field: 'NomCompleto', caption: 'Responsable', operator: 'contains' },
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
                                    window.location.href = 'RepExcedentesPuesto.aspx';
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
    };
</script>
     <script type="text/javascript">
         function downloadFile() {
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
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Excedentes por Empleado</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body" id="divContenedor">
                    <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form" >
                        <div class="row">
                            <div class="col-sm-12">
                               <div class="col-sm-12" id="detalle">                                               
                                    <div id="excedenteEmples" style="height:450px;width:100%;" ></div>
                                </div>
                            </div>
                             <asp:Button ID="btnExportar" runat="server" CssClass="classButton" OnClick="btnExportar_Click"/> 
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
