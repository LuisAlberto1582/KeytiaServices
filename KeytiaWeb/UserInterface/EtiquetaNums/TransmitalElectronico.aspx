<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="TransmitalElectronico.aspx.cs" Inherits="KeytiaWeb.UserInterface.EtiquetaNums.TransmitalElectronico" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .puesto {
            color: #58697D;
            font-size: 16px;
        }

        .tooltip1 {
            position: relative;
            display: inline-block;
        }

            .tooltip1 .tooltiptext1 {
                visibility: hidden;
                position: absolute;
                width: 250px;
                background-color: #555;
                color: #fff;
                text-align: left;
                padding: 6px 6px 6px 6px;
                border-radius: 6px;
                z-index: 1;
                opacity: 0;
                transition: opacity 0.3s;
            }

            .tooltip1:hover .tooltiptext1 {
                visibility: visible;
                opacity: 1;
            }

        .tooltip1-right {
            top: -10px;
            left: 126%;
        }

            .tooltip1-right::after {
                content: "";
                position: absolute;
                top: 50%;
                right: 100%;
                margin-top: -5px;
                border-width: 5px;
                border-style: solid;
                border-color: transparent #555 transparent transparent;
            }
.modalUpload
{
           position: fixed;
    z-index: 999;
    height: 100%;
    width: 100%;
    top: 0;
    /*background-color: Black;
    filter: alpha(opacity=60);
    opacity: 0.6;
    -moz-opacity: 0.8;*/
}
.centerUpload
{
  z-index: 1000;
    margin: 300px auto;
    padding: 10px;
    width: 130px;
    padding-right:500px;
    /*background-color: White;
    border-radius: 10px;
    filter: alpha(opacity=100);
    opacity: 1;
    -moz-opacity: 1;*/

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
    <div ID="pnlMainHolder" runat="server">
        <div ID="pnlRow_0" runat="server" CssClass="row">
            <div ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Generación de Transmital Electrónico</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body" id="divContenedor">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form" >
                            <div class="row" runat="server" id="row1">
                                <div class="col-sm-12">
                                    <div class="panel panel-default">
                                        <div class="panel-heading"><h4>Transmital Electrónico</h4></div>
                                        <div class="panel-body">

                                            <div class="row" runat="server" id="DatosEmple">
                                                <div class="col-sm-8">
                                                    <asp:Panel runat="server" ID="row" CssClass="form-group">
                                                        <asp:Label runat="server" CssClass="col-sm-6 control-label puesto">Transmital de: </asp:Label>
                                                            <div class="col-sm-4">
                                                                <asp:DropDownList runat="server" ID="cboMes" DataValueField="vchCodigo" DataTextField="Descripcion" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <asp:DropDownList runat="server" ID="cboAnio" CssClass="form-control" DataValueField="vchCodigo" DataTextField="Descripcion"></asp:DropDownList> 
                                                            </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-sm-8">
                                                    <asp:Panel CssClass="form-group" runat="server">
                                                        <asp:Label runat="server" CssClass="col-sm-6 control-label puesto">Localidad</asp:Label>
                                                        <div class="col-sm-6">
                                                           <asp:DropDownList runat="server" DataValueField="iCodCatalogo" DataTextField="vchDescripcion" ID="cboLocalidad" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <div class="row">
                                            <br />
                                            <asp:Panel ID="rowBuscar" runat="server" CssClass="form-group">
                                                <div class="col-sm-offset-4 col-sm-8">
                                                    <asp:Button runat="server" id="btnBuscar" CssClass="btn btn-keytia-lg" text="Generar" OnClick="btnBuscar_Click"/>
                                                </div>               
                                            </asp:Panel>
                                       </div>
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <asp:Label runat="server" CssClass="col-sm-10 control-label puesto">*El mes que se seleccione corresponde al periodo en que se generaron las llamadas. </asp:Label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div style="width: 800px; float: none; margin: 0 auto;">
                                                    <asp:Panel ID="pnlInfo" CssClass="alert alert-info text-center" runat="server" role="alert" Visible="false">
                                                        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                                        <strong>
                                                            <asp:Label ID="lblMensajeInfo" runat="server"></asp:Label>
                                                            <br />
                                                            <asp:Label ID="lblMnesajeInfo2" runat="server"></asp:Label>
                                                        </strong>
                                                    </asp:Panel>
                                                </div>
                                                <asp:Panel ID="panelDescarga" runat="server" CssClass="form-group" Visible="false">
                                                    <div class="col-sm-offset-4 col-sm-8">
                                                         <asp:LinkButton CssClass="btn btn-primary btn-sm"  ID="lnkVerDetall" runat="server" Text="Descargar" OnClick="lnkVerDetall_Click"></asp:LinkButton>
                                                          <asp:HiddenField runat="server" id="hfpathArchivo"/>                                                        
                                                    </div>               
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
          <%--NZ: Modal para mensajes--%>
            <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display:none;">
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
    <asp:PostBackTrigger ControlID="btnBuscar" />
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
