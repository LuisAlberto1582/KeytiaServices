<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="Inventariolineasmoviles.aspx.cs" Inherits="KeytiaWeb.UserInterface.Inventario.lineas.Inventariolineasmoviles" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
     <asp:Label ID="lblTitulo" runat="server" CssClass="page-title-keytia">Inventario de líneas móviles</asp:Label>
                    <asp:Panel ID="pnlBarra" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header">
            &nbsp;&nbsp;
            <asp:Button ID="btnExportarXLS" runat="server" Text="Exportar XLS" CssClass="buttonPlay" OnClick="btnExportarXLS_Click" />
        </asp:Panel>
            <div id="grid" style="width: 100%; height: 550px;">
            </div>
            <div id="grid_json" style="margin: auto;">
            </div>
    <script src="../../../scripts/js/w2grid/w2ui.js"></script>
    <link href="../../../scripts/js/w2grid/w2ui.css" rel="stylesheet" />
    <script src="../../../scripts/js/w2grid/jquery.js"></script>
    <script src="../../../scripts/js/w2grid/w2ui.js"></script>
    <script type="text/javascript">    
        var $jQuery2_2 = $.noConflict(true);
        var pagePath = window.location.pathname;
        var dataJSON;
        $(function () {
            $.ajax({
                url: pagePath + "/ListaMoviles",
                async: false,
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result.d != null) {
                        dataJSON = JSON.parse(result.d);
                    }
                    else {
                        alert("error al parsear");
                    }
                },
                error: function (XMLHttpRequest, callStatus, errorThrown) {
                    alert("path http: " + XMLHttpRequest.status + "  Detalle" + XMLHttpRequest.responseText);
                    alert("error: " + errorThrown);
                    alert("error: " + callStatus);
                }
            });
        });

        $(function () {
            var obj;
            try {
                obj = dataJSON;
            }
            catch (err) {
                alert("Error obj");
                obj = JSON.parse(errTxt);
            }
            var dataDisp = obj;

            var cols = [
                { field: "Extension", caption: 'Linea', size: '100px', sortable: true },
                { field: "NoNomina", caption: 'No.Nomina', size: '140px', sortable: true },
                { field: "NombreCompleto", caption: 'Empleado', size: '140px', sortable: true },
                { field: "CodigoEmpleado", caption: 'Codigo Empleado', size: '100px', sortable: true },
                { field: "NombreCentrodeCostos", caption: 'Centro de Costos', size: '80px', sortable: true },
                { field: "CodigoCentrodeCostos", caption: 'Codigo Centro de Costos', size: '90px', sortable: true },
                { field: "NombreRazonSocial", caption: 'Razon social', size: '90px', sortable: true },
                { field: "CodigoRazonSocial", caption: 'Codigo Razon Social', size: '90px', sortable: true },
                { field: "NombreTipoPlan", caption: 'Tipo de Plan', size: '90px', sortable: true },
                { field: "CodigoTipoPlan", caption: 'Codigo Tipo Plan', size: '90px', sortable: true },
                { field: "NombreTipoEquipo", caption: 'Tipo de Equipo', size: '90px', sortable: true },
                { field: "CodigoTipoEquipo", caption: 'Codigo Tipo Equipo', size: '100px', sortable: true },
                { field: "ModeloEquipo", caption: 'Modelo del Equipo', size: '100px', sortable: true },
                { field: "IMEI", caption: 'IMEI', size: '120px', sortable: true },
                { field: "Plan", caption: 'Plan', size: '120px', sortable: true },
                { field: "PlazoForzoso", caption: 'Plazo Forzoso', size: '60px', sortable: true },
                { field: "RID", caption: 'RID', size: '120px', sortable: true },
                { field: "TopRID", caption: 'TopRID', size: '100px', sortable: true },
            ];

            $jQuery2_2('#grid').w2grid({
                name: 'grid',
                header: 'Lista de Equipos',
                show: {
                    toolbar: true
                },
                disableCVS: true,
                searches: [
                    { type: 'text', field: 'Extension', caption: 'Linea' },
                    { type: 'text', field: 'NoNomina', caption: 'No.Nomina' },
                    { type: 'text', field: 'NombreCompleto', caption: 'Empleado' },
                    { type: 'text', field: 'NombreCentrodeCostos', caption: 'NombreCentrodeCostos' },
                    { type: 'text', field: 'NombreRazonSocial', caption: 'NombreRazonSocial' },
                    { type: 'text', field: 'NombreTipoPlan', caption: 'NombreTipoPlan' },
                    { type: 'text', field: 'NombreTipoEquipo', caption: 'NombreTipoEquipo' },
                    { type: 'text', field: 'ModeloEquipo', caption: 'ModeloEquipo' },
                    { type: 'text', field: 'IMEI', caption: 'IMEI' },
                    { type: 'text', field: 'Plan', caption: 'Plan' },
                    { type: 'text', field: 'RID', caption: 'RID' }
                ],
                columns: cols,
                records: dataDisp
            });
        });
    </script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>
</asp:Content>
