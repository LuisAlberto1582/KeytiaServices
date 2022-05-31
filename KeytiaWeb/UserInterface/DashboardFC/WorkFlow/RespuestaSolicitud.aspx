<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RespuestaSolicitud.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.RespuestaSolicitud" %>

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

    <!-- Datos Solicitud -->
    <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
        <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
            <div class="portlet solid bordered">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-bar-chart font-dark hide"></i>
                        <span class="caption-subject titlePortletKeytia">Respuesta Solicitud
                        </span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RespuestaSolicitudCollapse" aria-expanded="true" aria-controls="RespuestaSolicitudCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="collapse in form-horizontal" id="RespuestaSolicitudCollapse" role="form">
                        <asp:Panel ID="pnlDatosSolicitud" runat="server" CssClass="form-horizontal" Visible="false">
                            <br />
                            <asp:Panel runat="server" CssClass="form-group">
                                <asp:Label runat="server" CssClass="col-sm-2 control-label" Font-Bold="true">Folio:</asp:Label>
                                <div class="col-sm-2">
                                    <asp:Label runat="server" CssClass="form-control" ID="lblFolio"></asp:Label>
                                </div>
                            </asp:Panel>

                            <asp:Panel runat="server" CssClass="form-group">
                                <asp:Label runat="server" CssClass="col-sm-2 control-label" Font-Bold="true">Nómina Empleado:</asp:Label>
                                <div class="col-sm-9">
                                    <asp:Label runat="server" CssClass="form-control" ID="lblNominaEmple"></asp:Label>
                                </div>
                            </asp:Panel>

                            <asp:Panel runat="server" CssClass="form-group">
                                <asp:Label runat="server" CssClass="col-sm-2 control-label" Font-Bold="true">Nombre:</asp:Label>
                                <div class="col-sm-9">
                                    <asp:Label runat="server" CssClass="form-control" ID="lblNombre"></asp:Label>
                                </div>
                            </asp:Panel>

                            <asp:Panel runat="server" CssClass="form-group">
                                <asp:Label runat="server" CssClass="col-sm-2 control-label" Font-Bold="true">Tipo Recurso:</asp:Label>
                                <div class="col-sm-9">
                                    <asp:Label runat="server" CssClass="form-control" ID="lblTipoRecurso"></asp:Label>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pnlExten" runat="server" CssClass="form-group">
                                <asp:Label runat="server" CssClass="col-sm-2 control-label" Font-Bold="true">Motivo del Rechazo:</asp:Label>
                                <div class="col-sm-9">
                                    <asp:TextBox ID="txtMotivoRechazo" runat="server" TextMode="MultiLine" Height="50px" CssClass="form-control"></asp:TextBox>
                                </div>
                            </asp:Panel>

                            <br />
                            <div class="form-group">
                                <div class="col-sm-offset-2 col-sm-10">
                                    <asp:Button runat="server" CssClass="btn btn-danger" OnClick="btnRechazar_Click" Text="Rechazar" ID="btnRechazar" />
                                </div>
                            </div>
                        </asp:Panel>

                        <!-- Alerta Success -->
                        <div style="width: 800px; float: none; margin: 0 auto;">
                            <asp:Panel ID="InfoPanelSucces" CssClass="alert alert-success text-center" runat="server" role="alert" Visible="false">
                                <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                <strong>
                                    <asp:Label ID="lblMensajeSuccess" runat="server"></asp:Label>
                                </strong>
                            </asp:Panel>
                            <br />
                        </div>
                        <!-- Alerta Danger -->
                        <div style="width: 800px; float: none; margin: 0 auto;">
                            <asp:Panel ID="pnlError" CssClass="alert alert-danger text-center" runat="server" role="alert" Visible="false">
                                <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                <strong>
                                    <asp:Label ID="lblMensajeDanger" runat="server"></asp:Label>
                                </strong>
                            </asp:Panel>
                            <br />
                        </div>
                        <!-- Alerta Info -->
                        <div style="width: 800px; float: none; margin: 0 auto;">
                            <asp:Panel ID="pnlInfo" CssClass="alert alert-info text-center" runat="server" role="alert" Visible="false">
                                <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                <strong>
                                    <asp:Label ID="lblMensajeInfo" runat="server"></asp:Label>
                                </strong>
                            </asp:Panel>
                            <br />
                        </div>

                    </div>
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
