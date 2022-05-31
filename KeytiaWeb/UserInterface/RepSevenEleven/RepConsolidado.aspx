<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepConsolidado.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepSevenEleven.RepConsolidado" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="../InventarioTelecom/css/w2ui.css" rel="stylesheet" />
    <link href="CSS/StyleBody.css" rel="stylesheet" />
    <script type="text/javascript" src="../InventarioTelecom/js/jquery.js"></script>
    <script type="text/javascript" src="../InventarioTelecom/js/w2ui.js"></script>
    <script type="text/javascript">var $jQuery2_3 = $.noConflict(true);</script>
    <script type="text/javascript" src="../RepExcedentes/OrdenaDatos.js"></script>
    <script type="text/javascript" src="JS/RepConsolidado.js"></script>
    <script type="text/javascript" src="JS/RepConsolidadoCtaMaestra.js"></script>
    <script type="text/javascript" src="JS/RepTipoDestino.js"></script>
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
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="col-sm-12">
                                        <div class="col-sm-8">
                                            <asp:Panel runat="server" ID="row" CssClass="form-group">
                                                <asp:Label runat="server" CssClass="col-sm-5 control-label">Periodo Facturación: </asp:Label>
                                                <div class="col-sm-4">
                                                    <asp:DropDownList runat="server" ID="cboMes" DataValueField="vchCodigo" DataTextField="Descripcion" CssClass="form-control"></asp:DropDownList>
                                                </div>
                                                <div class="col-sm-3">
                                                    <asp:DropDownList runat="server" ID="cboAnio" CssClass="form-control" DataValueField="vchCodigo" DataTextField="Descripcion"></asp:DropDownList>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="col-sm-4">
                                            </div>
                                            <div class="col-sm-6">
                                                <asp:Panel ID="pnlRangeFechas" runat="server" CssClass="form-group">
                                                    <div class="col-sm-6">
                                                        <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-lg" Text="Aceptar" OnClick="btnAplicarFecha_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <div class="row">
                                                        <div class="col-sm-8">
                                                            <asp:Label runat="server" CssClass="col-sm-4 control-label">CONSUMO SIANA: </asp:Label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtLineas" GroupName="reportes" Checked="true" AutoPostBack="true" />Lineas
                                                            </label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnCtaMaestra" GroupName="reportes" AutoPostBack="true" />
                                                                Cuenta Maestra
                                                            </label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnTipoServicio" GroupName="reportes" AutoPostBack="true" />
                                                                Tipo Servicio
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <asp:Panel runat="server" ID="line" Height="400px">
                                                        <div id="ConsumoLinea" style="height: 400px;"></div>
                                                    </asp:Panel>
                                                    <asp:Panel runat="server" ID="ctaMaestra" Height="400px">
                                                        <div id="ConsumoCtaMaestra" style="height: 300px;"></div>
                                                    </asp:Panel>
                                                    <asp:Panel runat="server" ID="tipoDest" Height="400px">
                                                        <div id="tipoDestino" style="height: 400px;"></div>
                                                    </asp:Panel>
                                                    <asp:Button ID="btnExportar" runat="server" CssClass="classButton" OnClick="btnExportar_Click"/>
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
            <asp:PostBackTrigger ControlID="btnAplicarFecha" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportar" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnCtaMaestra" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtLineas" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnTipoServicio" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
