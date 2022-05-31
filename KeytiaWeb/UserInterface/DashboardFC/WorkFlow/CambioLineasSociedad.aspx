<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="CambioLineasSociedad.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.CambioLineasSociedad" %>
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
                            <span class="caption-subject titlePortletKeytia">Cambio Sociedad de Líneas </span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                     <div class="portlet-body">
                       <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                          <div class="row" runat="server" id="row1">
                            <div class="col-sm-10">
                                   <div class="form-inline">
                                       <asp:Panel ID="rowSociedad" runat="server" CssClass="form-group">
                                          <asp:label ID="lblSociedad" runat="server" CssClass="col-sm-4 control-label">Sociedad: </asp:label>
                                           <div class="col-sm-4">
                                                <asp:DropDownList runat="server" ID="cboSociedad" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="col-sm-2 form-control">
                                                    <asp:ListItem Value="0"> Seleccione una Sociedad</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                       </asp:Panel>
                                       <asp:Panel ID="rowPlan" runat="server" CssClass="form-group">
                                       <asp:label ID="lblPlan" runat="server" CssClass="col-sm-4 control-label">Plan: </asp:label>
                                           <div class="col-sm-4">
                                                <asp:DropDownList runat="server" ID="cboPlan" DataValueField="Id" DataTextField="Descripcion" AppendDataBoundItems="true"   CssClass="col-sm-2 form-control">
                                                    <asp:ListItem Value="0"> Seleccione un Plan </asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                      </asp:Panel>
                                        <asp:Panel ID="rowAceptar" runat="server" CssClass="form-group">
                                                <div class="col-sm-offset-4 col-sm-8">
                                                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-keytia-lg" OnClick="btnBuscar_Click"/>
                                                </div>
                                        </asp:Panel>
                                    </div>
                                </div>
                           </div>
                            <br />
                           <div class="row" runat="server" id="row2">
                               <div class="col-sm-12">
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
                                    <div runat="server" id="divGrid">
                                       <asp:GridView runat="server" ID="gvLineas" HeaderStyle-CssClass="tableHeaderStyle" 
                                            CssClass="table table-bordered tableDashboard" AutoGenerateColumns="false" ShowHeader="true">
                                            <Columns>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkRow" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Línea" ItemStyle-Width="90">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTelefono" runat="server" Text='<%# Eval("Tel") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField  DataField="Sociedad" HeaderText="Sociedad"/>
                                                <asp:BoundField DataField="PlanTarifDesc" HeaderText="Plan" />
                                                <asp:BoundField DataField="Marca" HeaderText="Marca" />
                                                <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
                                                <asp:BoundField DataField="IMEI" HeaderText="IMEI" />
                                                <asp:BoundField DataField="SIMCard" HeaderText="SIM" />
                                                
                                            </Columns>
                                        </asp:GridView>
                                    </div>

                               </div>
                           </div>
                           <div class="row" runat="server" id="row3" visible="false">
                                <div class="col-sm-10">
                                   <div class="form-inline">
                                       <asp:Panel ID="rowSociedad2" runat="server" CssClass="form-group">
                                          <asp:label ID="lblSociedad2" runat="server" CssClass="col-sm-4 control-label">Sociedad: </asp:label>
                                           <div class="col-sm-4">
                                                <asp:DropDownList runat="server" ID="cboSociedad2" DataValueField="iCodCatalogo" DataTextField="Descripcion" AppendDataBoundItems="true"   CssClass="col-sm-2 form-control">
                                                    <asp:ListItem Value="0"> Seleccione una Sociedad</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                       </asp:Panel>
                                       <asp:Panel ID="rowPlan2" runat="server" CssClass="form-group">
                                       <asp:label ID="lblPlan2" runat="server" CssClass="col-sm-4 control-label">Plan: </asp:label>
                                           <div class="col-sm-4">
                                                <asp:DropDownList runat="server" ID="cboPlan2" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="Descripcion"  CssClass="col-sm-2 form-control">
                                                    <asp:ListItem Value="0"> Seleccione un Plan </asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                      </asp:Panel>
                                       <asp:Panel ID="rowBtnGuardar" runat="server" CssClass="form-group">
                                            <div class="col-sm-offset-4 col-sm-8">
                                                    <asp:Button ID="btnGuardar" runat="server" Text="Cambiar" OnClick="btnGuardar_Click" CssClass="btn btn-keytia-lg" />
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
   <%-- <Triggers>
    <asp:PostBackTrigger ControlID="btnGuardar" />
</Triggers>--%>
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
