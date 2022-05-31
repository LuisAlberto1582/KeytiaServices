<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepFacturacion.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepFacturacion.RepFacturacion" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
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
                                                <asp:Label runat="server" CssClass="col-sm-5 control-label">Periodo de Busqueda: </asp:Label>
                                                <div class="col-sm-4">
                                                    <asp:DropDownList runat="server" ID="cboMes" DataValueField="vchCodigo" DataTextField="Descripcion" CssClass="form-control"></asp:DropDownList>
                                                </div>
                                                <div class="col-sm-3">
                                                    <asp:DropDownList runat="server" ID="cboAnio" CssClass="form-control" DataValueField="vchCodigo" DataTextField="Descripcion"></asp:DropDownList>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel runat="server" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblCarrier" CssClass="col-sm-5 control-label">Carrier:</asp:Label>
                                                <div class="col-sm-4">
                                                    <asp:DropDownList runat="server" ID="cboCarrier" CssClass="form-control"></asp:DropDownList>
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
                                                        <div class="col-sm-10">
                                                            <asp:Label runat="server" CssClass="col-sm-4 control-label">Reporte Facturación: </asp:Label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnResumen" GroupName="reportes" Checked="true" AutoPostBack="true" />Resumen General
                                                            </label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnInfGral" GroupName="reportes" AutoPostBack="true" />
                                                                Información General
                                                            </label>
                                                            &nbsp;
                                                            <asp:Button ID="btnExportar" runat="server" CssClass="btn btn-keytia-sm" Text="Exportar" OnClick="btnExportar_Click" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <asp:Panel ID="Tab1Rep1" runat="server" CssClass="col-md-12 col-sm-12" Visible="false">
                                                            </asp:Panel>
                                                            <asp:Panel ID="Tab1Rep2" runat="server" CssClass="col-md-12 col-sm-12" Visible="false">
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
            <asp:PostBackTrigger ControlID="btnAplicarFecha" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportar" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnInfGral" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnResumen" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
