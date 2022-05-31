<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="LineasExcedenPlanBase.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoDatosMovil.LineasExcedenPlanBase" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">/*Este script se utiliza para que funcione el autocomplete si se hace un postback mediante un boton u otro control asp*/
        $(document).ready(function () {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_initializeRequest(InitializeRequest);
            prm.add_endRequest(EndRequest);
            // Place here the first init of the autocomplete
            InitAutoCompl();
        });

        function InitializeRequest(sender, args) {
        }

        function EndRequest(sender, args) {
            // after update occur on UpdatePanel re-init the Autocomplete
            InitAutoCompl();
        }

        function InitAutoCompl() {

            $(function () {
                var idText = document.getElementById('<%=txtBuscar.ClientID%>');
                    var grvLineas = '<%=grvLineasPlan.ClientID%>';
                    var grvConceptos = '<%=grvConceptos.ClientID%>';
                    var grid;
                    if (grvLineas != null) {
                        grid = grvLineas;
                    }
                    else if (grvConceptos != null) {
                        grid = grvConceptos;
                    }
            <%--var grid = '<%=grvLineasPlan.ClientID%>';--%>

                    $(idText).keyup(function () {
                        var val = $(this).val().toUpperCase();
                        $('#' + grid + ' > tbody > tr').each(function (index, element) {
                            if ($(this).text().toUpperCase().indexOf(val) < 0)
                                $(this).hide();
                            else
                                $(this).show();
                        });
                    });
                });

        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <%--<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="modalUpload">
                <div class="centerUpload">
                    <asp:Image class="center" runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="pnlMainHolder" runat="server">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet solid bordered">
                            <div class="portlet-title">
                                <div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server"></span>
                                </div>
                                <div class="actions">
                                    Exportar:&nbsp;<asp:LinkButton ID="btnExportarXLS" runat="server" CssClass="exportExcel" OnClick="btnExportarXLS_Click"><i class="fas fa-file-excel"></i>&nbsp;Excel</asp:LinkButton>
                                    &nbsp;&nbsp;&nbsp;
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <div class="row">
                                                        <div class="col-sm-11">
                                                            <asp:Label runat="server" CssClass="col-sm-4 control-label">Reporte General: </asp:Label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnConsolidado" GroupName="reportes" Checked="true" AutoPostBack="true" OnCheckedChanged="rbtnConsolidado_CheckedChanged" />Consolidado por línea
                                                            </label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnConceptos" GroupName="reportes" AutoPostBack="true" OnCheckedChanged="rbtnConceptos_CheckedChanged" />
                                                                Desglose de Conceptos
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-sm-8">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                                                                <div class="col-offset-2 col-sm-4">
                                                                    <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <asp:Panel ID="Tab1Rep1" runat="server" CssClass="col-md-12 col-sm-12" Visible="false">
                                                                <div class="table-fixed-nz">
                                                                    <asp:GridView runat="server" ID="grvLineasPlan"
                                                                        CssClass="fixed_header table table-bordered tableDashboard"
                                                                        AutoGenerateColumns="false">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="iCodCatalogo" HeaderText="Editar" />
                                                                            <asp:BoundField DataField="iCodCatalogo" HeaderText="Eliminar" />
                                                                            <asp:BoundField DataField="vchDescripcion" HeaderText="Carga" />
                                                                            <asp:BoundField DataField="AnioDesc" HeaderText="Año" />
                                                                            <asp:BoundField DataField="MesDesc" HeaderText="Mes" />
                                                                            <asp:BoundField DataField="EstCargaDesc" HeaderText="Estatus Carga" />
                                                                            <asp:BoundField DataField="Archivo01" HeaderText="Archivo01" />
                                                                            <asp:BoundField DataField="Archivo02" HeaderText="Archivo02" />
                                                                            <asp:BoundField DataField="Archivo03" HeaderText="Archivo03" />
                                                                            <asp:BoundField DataField="Archivo04" HeaderText="Archivo04" />
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </asp:Panel>
                                                            <asp:Panel ID="Tab1Rep2" runat="server" CssClass="col-md-12 col-sm-12" Visible="false">
                                                                <div class="table-fixed-nz">
                                                                    <asp:GridView runat="server" ID="grvConceptos"
                                                                        CssClass="fixed_header table table-bordered tableDashboard"
                                                                        AutoGenerateColumns="false">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="iCodCatalogo" HeaderText="Editar" />
                                                                            <asp:BoundField DataField="iCodCatalogo" HeaderText="Eliminar" />
                                                                            <asp:BoundField DataField="vchDescripcion" HeaderText="Carga" />
                                                                            <asp:BoundField DataField="AnioDesc" HeaderText="Año" />
                                                                            <asp:BoundField DataField="MesDesc" HeaderText="Mes" />
                                                                            <asp:BoundField DataField="EstCargaDesc" HeaderText="Estatus Carga" />
                                                                            <asp:BoundField DataField="Archivo01" HeaderText="Archivo01" />
                                                                            <asp:BoundField DataField="Archivo02" HeaderText="Archivo02" />
                                                                            <asp:BoundField DataField="Archivo03" HeaderText="Archivo03" />
                                                                            <asp:BoundField DataField="Archivo04" HeaderText="Archivo04" />
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </asp:Panel>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <asp:Panel ID="Rep1" runat="server" CssClass="col-md-6 col-sm-6">
                                                            </asp:Panel>
                                                            <asp:Panel ID="Rep2" runat="server" CssClass="col-md-6 col-sm-6">
                                                            </asp:Panel>
                                                        </div>
                                                    </div>
                                                </div>
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
            <asp:PostBackTrigger ControlID="rbtnConsolidado" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnConceptos" />
        </Triggers>
        <%--        <Triggers>
            <asp:PostBackTrigger ControlID="btnBuscar" />
        </Triggers>--%>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportarXLS" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
