<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="FinalizaSolicitudes.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.FinalizaSolicitudes" %>
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
}
.centerUpload
{
  z-index: 1000;
    margin: 220px;
    padding: 10px;
    width: 130px;

}
.center img
{
    height: 120px;
    width: 120px;
}
    </style>
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
</asp:UpdateProgress>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>--%> 

        <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Finaliza Solicitud</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <div class="col-sm-12">
                                        <asp:GridView runat="server" ID="gridSolicitudes" HeaderStyle-CssClass="tableHeaderStyle" 
                                            CssClass="table table-bordered tableDashboard" AutoGenerateColumns="false"
                                            DataKeyNames="INE,Carta Responsiva">
                                            <Columns>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkRow" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Folio Solicitud" ItemStyle-Width="90">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdSolitud" runat="server" Text='<%# Eval("SolicitudId") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="SolicitanteNomina" HeaderText="Nomina" />
                                                <asp:BoundField DataField="SolicitanteNomCompleto" HeaderText="Nombre" />
                                                <asp:BoundField DataField="PlanTarifarioDesc" HeaderText="Plan Tarifario" />
                                                <asp:BoundField DataField="NumeroTelefonico" HeaderText="Linea" />
                                                <asp:BoundField DataField="SolicitantePuesto" HeaderText="Puesto" />
                                                <asp:TemplateField HeaderText="INE">
                                                    <ItemTemplate>
                                                        <asp:LinkButton CssClass="btn btn-default" ID="lnkDownloadINE" runat="server" Text="Descargar" OnClick="lnkDownloadINE_Click"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Carta Responsiva">
                                                    <ItemTemplate>
                                                        <asp:LinkButton CssClass="btn btn-default"  ID="lnkDownloadCarta" runat="server" Text="Descargar" OnClick="lnkDownloadCarta_Click"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                             </div>
                            <asp:Panel ID="rowGuardar" runat="server" CssClass="form-group">
                                    <div class="col-sm-offset-4 col-sm-8">
                                        <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" OnClick="btnAceptar_Click" CssClass="btn btn-keytia-lg" />
                                    </div>
                            </asp:Panel>
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
            </asp:Panel>
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
    <%--</ContentTemplate>
<Triggers>
    <asp:PostBackTrigger ControlID="btnAceptar" />
</Triggers>
</asp:UpdatePanel>
    <script type="text/javascript">
        window.onsubmit = function () {          
                var updateProgress = $find("<%=UpdateProgress1.ClientID%>");
                window.setTimeout(function () {
                    updateProgress.set_visible(true);
                }, 100);
            
        }
</script>--%>
</asp:Content>
