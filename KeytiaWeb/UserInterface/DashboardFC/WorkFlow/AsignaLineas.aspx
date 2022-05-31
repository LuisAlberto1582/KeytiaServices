<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AsignaLineas.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.AsignaLineas" %>

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
    <div ID="pnlMainHolder" runat="server">
        <div ID="pnlRow_0" runat="server" CssClass="row">
            <div ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Asignación de Líneas</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                       <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                        <div runat="server" class="row" id="rowLineas" visible="false">
                               <div class="form-horizontal">
                                   <div class="col-sm-12">  
                                       <div style="height:250px;overflow-y:auto;overflow-x:auto;">
                                        <asp:GridView runat="server" ID="gridLineas" HeaderStyle-CssClass="tableHeaderStyle" 
                                            CssClass="table table-bordered tableDashboard" AutoGenerateColumns="false" OnRowCommand="gridLineas_RowCommand" DataKeyNames="Solicitud,RECURSO">
                                            <Columns>
                                                <asp:ButtonField ButtonType="button" CommandName="Solcitud" 
                                                HeaderText="" Text="Asignar Línea"/>
                                                <asp:BoundField DataField="Solicitud" HeaderText="Solicitud" />                                             
                                                <asp:BoundField DataField="NOMBRE" HeaderText="Empleado" />
                                                <asp:BoundField DataField="MONTO" HeaderText="Monto" />
                                                <asp:BoundField DataField="CARRIER" HeaderText="Carrier" />
                                                <asp:BoundField DataField="RECURSO" HeaderText="Recurso" />
                                                <asp:ButtonField ButtonType="button" CommandName="Rechazar" 
                                                HeaderText="" Text="Rechazar Solicitud"/>
                                            </Columns>
                                        </asp:GridView>
                                        </div>             
                                    </div>
                               </div>
                           </div>
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
            </div>
        </div>
    </div>
        <%--NZ: Modal para mensajes--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label ID="lblTituloModalMsn" runat="server" Text=""></asp:Label>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <h5>
                        <asp:Label ID="lblBodyModalMsn" runat="server" Text=""></asp:Label>
                    </h5>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje" TargetControlID="lnkBtnMsn"
        BackgroundCssClass="modalPopupBackground" DropShadow="false" CancelControlID="btnCerrarMensajes">
    </asp:ModalPopupExtender>
    <%--NZ: Modal para Editar el Hallazgo.--%>
    <asp:Panel ID="pnlPopupAsignaLinea" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="rule"></div>
        <div class="modal-dialog modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label runat="server" ID="lblTituloAsgnaLinea"></asp:Label>
                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrar"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <div id="RepCapturaCollapse" class="form-horizontal" role="form">
                        <div class="row">
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblSolicitudModal" runat="server" CssClass="control-label">Folio Solicitud:</asp:Label>
                                <asp:TextBox ID="txtSolicitudModal" runat="server"  Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblLineaModal" runat="server" CssClass="control-label">Linea:</asp:Label>
                                <asp:TextBox ID="txtLineaModal" runat="server"   CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblPlanModal" runat="server" CssClass="control-label">Plan:</asp:Label>
                                <asp:TextBox ID="txtPlanModal" runat="server"  Enabled="false" CssClass="form-control"></asp:TextBox>
                               <asp:DropDownList ID="cboPlanModal" runat="server" DataValueField="iCodCatalogo" DataTextField="vchDescripcion"
                                     CssClass="form-control" Visible ="false">
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblMontoModal" runat="server" CssClass="control-label">Monto:</asp:Label>
                                <asp:TextBox ID="txtMontoModal" runat="server" Enabled="false"  CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCancelarModal" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" OnClick="btnCancelarModal_Click"/>
                    <asp:Button ID="btnGuardarModal" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnGuardarModal_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnEditHallazo" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEditHallazo" runat="server" PopupControlID="pnlPopupAsignaLinea"
        TargetControlID="lnkBtnEditHallazo" CancelControlID="btnCerrar" BackgroundCssClass="modalPopupBackground" DropShadow="false">
    </asp:ModalPopupExtender>
    <%----%>
</asp:Content>
