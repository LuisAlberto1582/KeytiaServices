<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="SolicitudesDetalle.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.SolicitudesDetalle" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        table.Detalle {
          border: 1px solid #e7ecf1;
          text-align: left;
          border-collapse: collapse;
          margin: 0 auto;
        }
        table.Detalle td {
         padding: 7px;
         line-height: 1.5;
         color: black;
         vertical-align: middle;
         border: 1px solid #CCCCCC;
         font-weight: 100 !important;
         font-family: 'Poppins', sans-serif;
         font-size: 11px;
        }
    </style>
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
                            <span class="caption-subject titlePortletKeytia">Solicitudes de Consumo Detallado</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                     <div class="portlet-body">
                       <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form" style="text-align: center;">
                          <div class="row" runat="server" id="row1">
                            <div class="col-sm-12">
                                <div class="row">
                                   <asp:Panel ID="rowLabel" runat="server" CssClass="form-group">
                                       <div class="col-sm-offset-4 col-sm-8">
                                            <asp:Label runat="server" id="lblMensaje"  Font-Size="Large" /> 
                                       </div>
                                   </asp:Panel>
                                </div>
                                <div class="row">
                                   <div class="table-responsive" style="width: 95% ;margin: 0 auto;">                   
                                        <asp:GridView ID="gvDetails" HeaderStyle-CssClass="tableHeaderStyle" 
                                            CssClass="table table-bordered tableDashboard" runat="server" 
                                            AutoGenerateColumns="false" DataKeyNames="iCodCatalogo">        
                                            <Columns>
                                            <asp:BoundField DataField="NombreReporte" HeaderText="Nombre" />
                                            <asp:BoundField DataField="FechaReg" HeaderText="Fecha de Solicitud" />
                                            <asp:BoundField DataField="Estatus" HeaderText="Estatus" />
                                            <asp:BoundField DataField="Email" HeaderText="Correo electronico" />
                                            <asp:BoundField DataField="FechaEnvio" HeaderText="Fecha de envio" />
                                            <asp:TemplateField HeaderText="Filtros">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkVerDetalle" runat="server" Text="Ver Detalle" OnClick="lnkVerDetalle_Click"></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Archivo">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkDownload" runat="server" Text="Descargar" OnClick="lnkDownload_Click"></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div> 
                           </div>
                           <br/>
                           <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" CssClass="btn btn-keytia-lg" OnClick="btnActualizar_Click"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%--NZ: Modal para detalle--%>
    <asp:Panel ID="pnlPopupDetalle" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label ID="lblTituloModalDetalle" runat="server" Text="Filtros"></asp:Label>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarDetalle"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblDetalle" runat="server" Text=""></asp:Label>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnDetalle" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqDetalle" runat="server" PopupControlID="pnlPopupDetalle"
        TargetControlID="lnkBtnDetalle" OkControlID="btnYes" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarDetalle">
    </asp:ModalPopupExtender>
</asp:Content>
