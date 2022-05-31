<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="SeguimientoMiSolicitud.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.SeguimientoMiSolicitud" %>
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
                            <span class="caption-subject titlePortletKeytia">Mis Solicitudes</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                       <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <asp:Panel ID="rowSolicitud" runat="server" CssClass="form-group">
                                <asp:label ID="lbSolicitud" runat="server" CssClass="col-sm-2 control-label">Folio Solicitud: </asp:label>
                                <div class="col-sm-8">
                                    <asp:DropDownList runat="server" ID="cboSolicitudes" AppendDataBoundItems="true" DataValueField="IdSol" DataTextField="NomSol" OnSelectedIndexChanged="cboSolicitudes_SelectedIndexChanged" AutoPostBack="true"  CssClass="col-sm-2 form-control">
                                        <asp:ListItem Value="0"> Selecciona Una Solicitud </asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </asp:Panel>

                            <div runat="server" class="row" id="row1" visible="false">
                                <div class="col-sm-6">
                                    <div class="form-horizontal">
                                            <asp:Panel ID="rowSol" runat="server" CssClass="form-group">
                                                <asp:label ID="lblSolicitud" runat="server" CssClass="col-sm-4 control-label">No. Solicitud: </asp:label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtSolicitud" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel ID="rowTipoRecurso" runat="server" CssClass="form-group">
                                                <asp:label ID="lblTipoRecurso" runat="server" CssClass="col-sm-4 control-label">Tipo de Recurso: </asp:label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtTipoRecurso" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                    </div>
                                </div>                    
                                <div class="col-sm-6">
                                    <div class="form-horizontal">
                                        <asp:Panel ID="rowPerfil" runat="server" CssClass="form-group">
                                            <asp:label ID="lblPerfil" runat="server" CssClass="col-sm-4 control-label">Perfil: </asp:label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtPerfil" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="rowPlan" runat="server" CssClass="form-group">
                                            <asp:label ID="lblPlan" runat="server" CssClass="col-sm-4 control-label">Plan: </asp:label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtPlan" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </div>                           
                            </div>
                           <div runat="server" class="row" id="rowIconos" visible="false">
                                   <div class="col-sm-offset-4 col-sm-8">
                                       <div class="form-inline">
                                           <asp:Panel runat="server" CssClass="form-group">
                                               <asp:Label runat="server" CssClass="col-sm-12 control-label" Font-Size="17px">Estatus Finalizado:&nbsp;&nbsp;<i class="fas fa-check-circle"></i></asp:Label>
                                           </asp:Panel>
                                           <asp:Panel runat="server" CssClass="form-group">
                                               <asp:Label runat="server" CssClass="col-sm-12 control-label" Font-Size="15px">&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label>
                                           </asp:Panel>
                                           <asp:Panel runat="server" CssClass="form-group">
                                               <asp:Label runat="server" CssClass="col-sm-12 control-label" Font-Size="17px">Estatus Actual:&nbsp;&nbsp;<i class="fas fa-arrow-alt-circle-right"></i></asp:Label>
                                           </asp:Panel>
                                       </div>
                                   </div>
                           </div>
                            <div runat="server" class="row" id="rowLineas" visible="false">
                               <div class="form-horizontal">
                                   <div class="col-sm-offset-2 col-sm-8">  
                                       <div runat="server" id="divLineas">
                                      
                                        </div>             
                                    </div>
                               </div>
                           </div>
                                                   <!-- Alerta Info -->
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

                        </div>
                      </div>
                 </div>
            </div>
        </div>
    </div>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
