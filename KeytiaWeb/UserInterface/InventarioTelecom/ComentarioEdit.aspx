<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="ComentarioEdit.aspx.cs" Inherits="KeytiaWeb.UserInterface.InventarioTelecom.ComentarioEdit" %>

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
    <%--Barra con boton y fechas--%>
    <div>
        <asp:Label ID="lblInicio" runat="server" CssClass="page-title-keytia">Editar Comentarios de Equipos</asp:Label>       
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="row divCenter">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-8 col-sm-8">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Búsqueda</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div id="RepDetallCollapse" class="form-horizontal" role="form">
                            <asp:Panel ID="rowNoSerie" runat="server" CssClass="form-group">
                                <asp:Label ID="lblNoSerie" runat="server" CssClass="col-sm-3 control-label">No. de Serie:</asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox ID="txtNoSerie" runat="server" MaxLength="30" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-sm-3">
                                    <asp:Button ID="btnBuscarIP" runat="server" Text="Buscar" CssClass="btn btn-keytia-md" OnClick="btnBuscarIP_Click" />
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRep1" runat="server" CssClass="row divCenter">
            <asp:Panel ID="Rep1" runat="server" Visible="false" CssClass="col-md-10 col-sm-10">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">No hay resultado</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepNoRegCollapse" aria-expanded="true" aria-controls="RepNoRegCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div id="RepNoRegCollapse" class="form-horizontal" role="form">
                            <asp:Label ID="lblNoCoincidencias" runat="server">No se encontraron equipos con el No. de Serie especificado.</asp:Label>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRep2" runat="server" CssClass="row divCenter">
            <asp:Panel ID="Rep2" runat="server" Visible="false" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia"><asp:Label ID="lblTitulo" runat="server" CssClass="control-label"></asp:Label></span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepCapturaCollapse" aria-expanded="true" aria-controls="RepCapturaCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div id="RepCapturaCollapse" class="form-horizontal" role="form">                            
                            <div class="row">
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblCr" runat="server" CssClass="control-label">CR:</asp:Label>
                                    <asp:TextBox ID="txtCR" runat="server" MaxLength="10" Enabled="false" CssClass="form-control"></asp:TextBox>                                    
                                </div>
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblNombreCR" runat="server" CssClass="control-label">Nombre CR:</asp:Label>
                                    <asp:TextBox ID="txtNombreCR" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblTipo" runat="server" CssClass="control-label">Tipo Recurso:</asp:Label>
                                    <asp:TextBox ID="txtTipo" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblHostname" runat="server" CssClass="control-label">Hostname:</asp:Label>
                                    <asp:TextBox ID="txtHostname" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblDireccionIPDatos" runat="server" CssClass="control-label">Dirección IP:</asp:Label>
                                    <asp:HiddenField ID="hfNoSerie" runat="server" Value="" />
                                    <asp:TextBox ID="txtDireccionIPDatos" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblAdministrador" runat="server" CssClass="control-label">Administrador:</asp:Label>
                                    <asp:TextBox ID="txtAdministrador" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblComentarios" runat="server" CssClass="control-label">Comentarios:</asp:Label>
                                    <asp:TextBox ID="txtComentarios" runat="server" TextMode="MultiLine" Height="50px" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <br />
                                     <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-md" OnClick="btnAceptar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>         
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <%--NZ: Modal para mensajes--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" style="display: none;">
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
