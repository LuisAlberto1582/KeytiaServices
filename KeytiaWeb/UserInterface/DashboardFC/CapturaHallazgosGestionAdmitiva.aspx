<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="CapturaHallazgosGestionAdmitiva.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.CapturaHallazgosGestionAdmitiva" %>

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
    <div>
        <asp:Label ID="lblInicio" runat="server" CssClass="page-title-keytia">Captura Gestión Administrativa</asp:Label>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="row">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">
                                <asp:Label ID="lblTitulo" runat="server">Datos</asp:Label></span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <div class="row">
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblFolio" runat="server" CssClass="control-label">Folio:</asp:Label>
                                    <asp:TextBox ID="txtFolio" runat="server" MaxLength="10" Enabled="false" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblCategoria" runat="server" CssClass="control-label">Categoría:</asp:Label>
                                    <asp:DropDownList ID="cboCategoria" runat="server" DataValueField="iCodCatalogo"
                                        DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblHallazgo" runat="server" CssClass="control-label">Hallazgo:</asp:Label>
                                    <asp:TextBox ID="txtHallazgo" runat="server" TextMode="MultiLine" Height="50px" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblCarrier" runat="server" CssClass="control-label">Carrier:</asp:Label>
                                    <asp:DropDownList ID="cboCarrier" runat="server" DataValueField="iCodCatalogo" DataTextField="Descripcion"
                                        OnSelectedIndexChanged="cboCarrierIndex_Changed" AutoPostBack="true" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblCtaMaestra" runat="server" CssClass="control-label">Cuenta:</asp:Label>
                                    <asp:DropDownList ID="cboCtaMaestra" runat="server" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblAnio" runat="server" CssClass="control-label">Año:</asp:Label>
                                    <asp:DropDownList ID="cboAnio" runat="server" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblMes" runat="server" CssClass="control-label">Mes:</asp:Label>
                                    <asp:DropDownList ID="cboMes" runat="server" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblTipoDestino" runat="server" CssClass="control-label">Servicio:</asp:Label>
                                     <asp:DropDownList ID="cboTipoDestino" runat="server" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblVariacion" runat="server" CssClass="control-label">Variación:</asp:Label>
                                    <asp:DropDownList ID="cboVariacion" runat="server" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>
                             <div class="row">
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblImporte" runat="server" CssClass="control-label">Importe:</asp:Label>
                                    <asp:TextBox ID="txtImporte" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblMoneda" runat="server" CssClass="control-label">Moneda:</asp:Label>
                                     <asp:DropDownList ID="cboMoneda" runat="server" DataValueField="iCodCatalogo" DataTextField="Descripcion"  CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblDescripcion" runat="server" CssClass="control-label">Descripción:</asp:Label>
                                    <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Height="50px" Enabled="false" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <br />
                                    <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-lg" OnClick="btnAceptar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <%--NZ: Modal para mensajes--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label ID="lblTituloModalMsn" runat="server" Text=""></asp:Label>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblBodyModalMsn" runat="server" Text=""></asp:Label>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
        TargetControlID="lnkBtnMsn" OkControlID="btnYes" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
    </asp:ModalPopupExtender>
</asp:Content>
