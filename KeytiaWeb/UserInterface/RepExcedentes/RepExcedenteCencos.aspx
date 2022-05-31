<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepExcedenteCencos.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.RepExcedentes.RepExcedenteCencos" %>
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
        sessionStorage.clear();
        var pagePath = window.location.pathname;
        var dataJSON;
        var t;
        function GetInfo(method) {
            $.ajax({
                url: pagePath + "/" + method,
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

        function Cabeceras(method) {
            GetInfo(method);
            var obj = dataJSON;
            var dataDisp = sortJSON(obj.records, 'Excedente', 'desc') ;
            t = dataDisp;
            var cols = [
                { field: "recid", caption: '#', size: '80px', sortable: true, frozen: true },
                { field: "CenCos", caption: 'Departamento', size: '200px', sortable: true, frozen: true },
                { field: "CantEmples", caption: 'Cantidad de Empleados', size: '180px', sortable: true, frozen: true },
                { field: "Renta", caption: 'Renta', size: '140px', render: 'money',sortable: true, frozen: true },
                { field: "Excedente", caption: 'Excedente', size: '140px', render: 'money',sortable: true },
                { field: "Total", caption: 'Total', size: '140px',render:'money', sortable: true }             
            ];
            $jQuery2_3('#excedenteCencos').w2grid({
                name: 'excedenteCencos',
                header: 'Excedente por Departamento',
                show: {
                    header: true,
                    toolbar: true,
                    lineNumbers: false,
                    footer: true
                },
                reorderColumns: true,
                searches: [
                    { type: 'text', field: 'CenCos', caption: 'Departamento', operator: 'contains' },
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
                records: dataDisp
            });
            w2ui['excedenteCencos'].hideColumn('recid');
            w2ui.excedenteCencos.on('click', function (event) {
                var grid = this;
                event.onComplete = function () {
                    var record = this.get(event.recid);
                    var e = record.recid;
                    var p = record.CenCos;
                    if (e != null) {
                        sessionStorage.clear();
                        sessionStorage.setItem('ClaveCencos', e);
                        sessionStorage.setItem('Cencos', p);
                        var ed = sessionStorage.getItem("ClaveCencos");
                        if (ed != null) {
                            window.location.href = 'RepExcedentesPuesto.aspx';
                        }
                    }
                }
                
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
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Excedentes por Departamento</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body" id="divContenedor">
                    <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form" >
                       <div class="row">
                            <div class="col-sm-12">
                                <div class="col-sm-4">
                                    <asp:Label ID="lblFechaInicio" runat="server" CssClass="control-label">Periodo:&nbsp&nbsp&nbsp&nbsp</asp:Label>
                                    <cc1:DSODateTimeBox ID="pdtInicio" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                        ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                    </cc1:DSODateTimeBox>
                                </div>
                                <div class="col-sm-4">
                                   <asp:Panel ID="pnlRangeFechas" runat="server" CssClass="form-group">
                                        <div class="col-sm-4">                          
                                            <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-lg" Text="Aceptar" OnClick="btnAplicarFecha_Click"/>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="row">                           
                            <div class="col-sm-12" id="detalle">                                               
                                <div id="excedenteCencos" style="height:430px;" ></div>
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
    <asp:PostBackTrigger ControlID="btnAplicarFecha" />
   
</Triggers>
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
