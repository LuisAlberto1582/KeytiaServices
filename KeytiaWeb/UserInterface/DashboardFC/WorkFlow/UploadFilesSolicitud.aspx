<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="UploadFilesSolicitud.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.UploadFilesSolicitud" %>
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
<style>
.tooltip1 {
    position: relative;
    display: inline-block;
}
.tooltip1 .tooltiptext1 {
    visibility: hidden;
    position: absolute;
    width: 120px;
    background-color: #555;
    color: #fff;
    text-align:left;
    padding: 6px 6px 6px 6px;
    border-radius: 6px;
    z-index: 1;
    opacity: 0;
    transition: opacity 0.3s;
    font-size:16px;
}
.tooltip1:hover .tooltiptext1 {
    visibility: visible;
    opacity: 1;
}
.tooltip1-bottom {
  top: 135%;
  left: 50%;  
  margin-left: -60px;
}
.tooltip1-bottom::after {
    content: "";
    position: absolute;
    bottom: 100%;
    left: 50%;
    margin-left: -5px;
    border-width: 5px;
    border-style: solid;
    border-color: transparent transparent #555 transparent;
}
</style>
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
        <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Carga de Archivos Solicitud</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in form-inline" id="RepDetallCollapse" role="form">
                            <div class="Row">
                                <div style="width:80%; float: none; margin: 0 auto;">
                                <asp:Panel ID="pnlInfo" CssClass="alert alert-info text-center" runat="server" role="alert" Visible="true">
                                    <strong>
                                        <asp:Label ID="lblMensajeInfo" runat="server" style="font-size:16px;">Enviar la Carta Responsiva a la siguiente dirección: Torrre Juárez Piso 9, Calle Benito Juárez #800, Monterrey Centro, C.P. 64000, N.L. Con atención a Eduardo Ismael Cordero Nuñez / Marcela Alejandra Gómez Ortega.
                                        </asp:Label>
                                    </strong>
                                </asp:Panel>
                                <br />
                                </div>
                            </div>
                            <div class="row" id="rowFilter">
                                <div class="col-sm-12">
                                <asp:Panel ID="rowSolicitud" runat="server" CssClass="form-group">
                                    
                                    <asp:label ID="lblSolicitud" runat="server" CssClass="col-sm-6 control-label" Font-Size="16px">Folio Solicitud:  <div class="tooltip1"><i class="fas fa-info-circle"></i><span class="tooltiptext1 tooltip1-bottom">Este folio se encuentra en el correo de asignación de equipo<br /></span></div></asp:label>
                                    
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtFolio"  runat="server" CssClass="form-control" ></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="rowBuscar" runat="server" CssClass="form-group">
                                    <div class="col-sm-offset-4">
                                        <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" OnClick="btnAceptar_Click" CssClass="btn btn-keytia-lg" />
                                    </div>
                            </asp:Panel>
                                </div>
                            </div>
                            <div class="row" id="rowInter">
                               <asp:Panel ID="Panel1" runat="server" CssClass="form-group">                          
                                    <div class="col-sm-8">                                        
                                    </div>
                                </asp:Panel>
                            </div>
                            <div class="row" id="rowUploadFiles" runat="server" visible="false">
                            <asp:Panel ID="rowIne" runat="server" CssClass="form-group" Visible="false">                             
                                    <div class="col-sm-8">   
                                        <label ID="lblIne" runat="server" CssClass="col-sm-4 control-label">INE:</label>                                      
                                        <asp:FileUpload runat="server" ID="file"/>
                                    </div>
                            </asp:Panel>
                            <asp:Panel ID="rowCarta" runat="server" CssClass="form-group" Visible="false">                                
                                    <div class="col-sm-8">
                                        <label ID="lblCarta" runat="server" CssClass="col-sm-4 control-label">Carta Responsiva:</label>
                                        <asp:FileUpload id="FileCarta"  runat="server"  />
                                    </div>
                            </asp:Panel>
                            <asp:Panel ID="rowGuardar" runat="server" CssClass="form-group">
                                        <div class="col-sm-offset-5 col-sm-10">
                                            <asp:Button ID="UploadButton" runat="server" Text="Guardar" OnClick="UploadButton_Click" CssClass="btn btn-keytia-lg" />
                                        </div>
                            </asp:Panel>
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
</ContentTemplate>
<Triggers>
    <asp:PostBackTrigger ControlID="UploadButton" />
</Triggers>

</asp:UpdatePanel>
<script type="text/javascript">
        window.onsubmit = function () {          
                var updateProgress = $find("<%=UpdateProgress1.ClientID%>");
                window.setTimeout(function () {
                    updateProgress.set_visible(true);
                }, 100);
            
        }
</script>
    
</asp:Content>
