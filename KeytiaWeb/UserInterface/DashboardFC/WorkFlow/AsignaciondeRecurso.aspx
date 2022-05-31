<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AsignaciondeRecurso.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.AsignaciondeRecurso" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script  type="text/javascript">
        function selection(rbtnPlanTarif) {
            var rbtn = document.getElementById(rbtnPlanTarif);
            var rbtnList = document.getElementsByTagName("input");
            for (i = 0; i < rbtnList.length; i++) {
                if (rbtnList[i].text == "radio" && rbtnList[i].id == rbtn.id)
                {
                    rbtnList[i].checked = false;
                }
            }
        }
    </script>

<style>
    .puesto{
    color:#58697D;
    font-size:16px;
    }
    .btn-keytia-lg{
    height:30px;
    }
    .switch {
    position: relative;
    display: inline-block;
    width: 47px;
    height: 20px;/*34*/
    bottom:-4px;
    }
    .switch input { 
    opacity: 0;
    width: 0;
    height: 0;
    }
    .slider {
    position: absolute;
    cursor: pointer;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: #ccc;
    -webkit-transition: .4s;
    transition: .4s;
    margin: 0px 0px;
    }

    .slider:before {
    position: absolute;
    content: "";
    height: 14px;
    width: 14px;
    left: 4px;
    bottom: 3px;
    background-color: white;
    -webkit-transition: .4s;
    transition: .4s;
    }
    input:checked + .slider {
    background-color: #009639;
    right: -2px;
    }
    input:focus + .slider {
    box-shadow: 0 0 1px #009639;
    }
    input:checked + .slider:before {
    -webkit-transform: translateX(26px);
    -ms-transform: translateX(26px);
    transform: translateX(26px);
    }
    /* Rounded sliders */
    .slider.round {
    border-radius: 34px;
    }
    .slider.round:before {
    border-radius: 50%;
    }

/* The Modal (background) */
.modal1 {
  display: none; /* Hidden by default */
  position: fixed; /* Stay in place */
  z-index: 9000; /* Sit on top */
  padding-top: 100px; /* Location of the box */
  left: 0;
  top: 0;
  width: 100%; /* Full width */
  height: 100%; /* Full height */
  overflow: auto; /* Enable scroll if needed */
  background-color: rgb(0,0,0); /* Fallback color */
  background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
}

/* Modal Content */
.modal1-content1 {
  position: relative;
  background-color: #fefefe;
  margin: auto;
  padding: 0;
  border: 1px solid #888;
  width: 50%;
  box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2),0 6px 20px 0 rgba(0,0,0,0.19);
  -webkit-animation-name: animatetop;
  -webkit-animation-duration: 0.4s;
  animation-name: animatetop;
  animation-duration: 0.4s
}

/* Add Animation */
@-webkit-keyframes animatetop {
  from {top:-300px; opacity:0} 
  to {top:0; opacity:1}
}

@keyframes animatetop {
  from {top:-300px; opacity:0}
  to {top:0; opacity:1}
}

/* The Close Button */
.close1 {
  color: white;
  float: right;
  font-size: 28px;
  font-weight: bold;
}

.close1:hover,
.close1:focus {
  color: #000;
  text-decoration: none;
  cursor: pointer;
}

.modal1-header1 {
  padding: 2px 16px;
  background-color: #5cb85c;
  color: white;
}

.modal1-body1 {padding: 2px 16px;}

.modal1-footer1 {
  display: flex;
  padding: 2px 16px;
  background-color: #5cb85c;
  color: white;
  justify-content: flex-end;
}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
EnableScriptGlobalization="true">
</asp:ToolkitScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
    <div ID="pnlMainHolder" runat="server">
        <div ID="pnlRow_0" runat="server" CssClass="row">
            <div ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Asignación de Recursos</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                       <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <asp:Panel ID="rowSolicitud" runat="server" CssClass="form-group">
                                <asp:label ID="lbSolicitud" runat="server" CssClass="col-sm-2 control-label">Folio Solicitud: </asp:label>
                                <div class="col-sm-6">
                                    <asp:DropDownList runat="server" ID="cboSolicitud" AppendDataBoundItems="true" DataValueField="Idsolicitud" DataTextField="NomSolicitante" OnSelectedIndexChanged="cboSolicitud_SelectedIndexChanged" AutoPostBack="true"  CssClass="col-sm-2 form-control">
                                        <asp:ListItem Value="0"> Selecciona Una Solicitud </asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2" id="rowRechazo" runat="server" visible="false">
                                    <div class="form-horizontal" >
                                        <asp:Panel ID="rowBtnRechazar" runat="server" CssClass="form-group">
                                                <div class="col-sm-2"">
                                                    <asp:Button ID="btnRechazar" runat="server" Text="Rechazar Solicitud"  CssClass="btn btn-keytia-lg" OnClick="btnRechazar_Click"/>
                                                </div>
                                        </asp:Panel>
                                    </div>
                                </div> 
                                <asp:HiddenField runat="server" ID="hfEstatus"/>
                                <asp:HiddenField runat="server" ID="hfEvento" />
                            </asp:Panel>
                            <div runat="server" class="row" id="rowDatosEmple" visible="false">
                                <div class="col-sm-6">
                                <div class="form-horizontal">
                                        <asp:Panel ID="rowNomina" runat="server" CssClass="form-group">
                                            <asp:label ID="lblNomina" runat="server" CssClass="col-sm-4 control-label">No. Nomina: </asp:label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtNomina" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                <asp:HiddenField ID="hfNomina" runat="server"/>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="rowDepartamento" runat="server" CssClass="form-group">
                                            <asp:label ID="lblDepartamento" runat="server" CssClass="col-sm-4 control-label">Departamento: </asp:label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtDepartamento" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="rowEmail" runat="server" CssClass="form-group">
                                            <asp:label ID="lblEmail" runat="server" CssClass="col-sm-4 control-label">Email: </asp:label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="rowEmpresa" runat="server" CssClass="form-group">
                                            <asp:label ID="lblEmpresa" runat="server" CssClass="col-sm-4 control-label">Empresa: </asp:label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtEmpresa" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                <asp:HiddenField ID="hfEmpresa" runat="server"/>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="panel2" runat="server" CssClass="form-group">
                                            <asp:Label ID="Label2" runat="server" CssClass="col-sm-6 control-label" Text="Mostrar Líneas Disponibles."></asp:Label>
                                           <div class="col-sm-4">                                                
                                              <label class="switch">
                                                    <asp:CheckBox runat="server" ID="chkMostrar" OnCheckedChanged="chkMostrar_CheckedChanged" AutoPostBack="true"/>
                                                   <span class="slider"></span>
                                               </label>                                                    
                                          </div>                                            
                                        </asp:Panel>                                            
                                </div>
                            </div>                    
                                <div class="col-sm-6">
                                    <div class="form-horizontal">
                                        <asp:Panel ID="rowNombre" runat="server" CssClass="form-group">
                                            <asp:label ID="lblNombre" runat="server" CssClass="col-sm-4 control-label">Nombre: </asp:label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="rowPuesto" runat="server" CssClass="form-group">
                                            <asp:label ID="lblPuesto" runat="server" CssClass="col-sm-4 control-label">Puesto: </asp:label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtPuesto" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="rowPerfil" runat="server" CssClass="form-group">
                                            <asp:label ID="lblPerfil" runat="server" CssClass="col-sm-4 control-label">Perfil: </asp:label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtPerfil" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                <asp:HiddenField ID="hfPerilId" runat="server"/>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="rowPlan" CssClass="form-group">
                                            <asp:Label ID="lblPlan" runat="server" CssClass="col-sm-4 control-label">Plan:</asp:Label>
                                            <div class="col-sm-8">
                                                <asp:TextBox ID="txtPlan" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="Panel1" runat="server" CssClass="form-group">
                                            <asp:Label ID="Label1" runat="server" CssClass="col-sm-2 control-label"></asp:Label>                                        
                                           <div class="col-sm-10">
                                                <div class="input-group">
                                                    <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control puesto" Text="" placeholder="Buscar.."/>
                                                    <span class="input-group-btn">
                                                        <asp:Button runat="server" ID="btn1" CssClass="btn btn-keytia-lg" Text="Buscar" OnClick="btn1_Click"/>
                                                    </span>                                                    
                                                </div>
                                           </div>            
                                        </asp:Panel>
                                    </div>
                                </div>                           
                            </div>
                           <div runat="server" class="row" id="rowLineas" visible="false">
                               <div class="form-horizontal">
                                   <div class="col-sm-12">  
                                       <div  runat="server" id="divLineas">
                                        <asp:GridView runat="server" ID="gridLineas" HeaderStyle-CssClass="tableHeaderStyle" 
                                            CssClass="table table-bordered tableDashboard" AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <div class="col-sm-1">                             
                                                            <div class="mt-radio-inline">
                                                             <label class="mt-radio">
                                                                <input name="rbtnPlanTarif" type="radio" value='<%# Eval("ClaveLinea") %>' onclick="javascript.selection(this.id)" />
                                                                 <span></span>
                                                             </label>
                                                             </div>                                                       
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Linea" HeaderText="Línea" />                                             
                                                <asp:BoundField DataField="Plan" HeaderText="Plan" />
                                                <asp:BoundField DataField="Marca" HeaderText="Marca Dispositivo" />
                                                <asp:BoundField DataField="Modelo" HeaderText="Modelo Dispositivo" />
                                                <asp:BoundField DataField="IMEI" HeaderText="IMEI" />
                                                <asp:BoundField DataField="SimCard" HeaderText="SIM Card" />
                                                <asp:BoundField DataField="Empresa" HeaderText="Empresa" />                                                
                                            </Columns>
                                        </asp:GridView>
                                        </div>             
                                    </div>
                                   <div class="col-sm-10">
                                    <asp:Panel ID="rowAceptar" runat="server" CssClass="form-group">
                                        <div class="col-sm-offset-6 col-sm-8">
                                            <asp:Button ID="btnGuardar" runat="server" Text="Asignar" OnClick="btnGuardar_Click" CssClass="btn btn-keytia-lg"  />
                                        </div>
                                    </asp:Panel>
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
                            <div runat="server" class="row" id="rowLinDisp" visible="false">
                                   <div class="form-horizontal">
                                       <div class="col-sm-offset-2  col-sm-8">
                                           <div runat="server" id="divCantLineas">
                                           <asp:GridView runat="server" HeaderStyle-CssClass="tableHeaderStyle" ID="grdLineasDisp"
                                                CssClass="table table-bordered tableDashboard" AutoGenerateColumns="false" >
                                               <Columns>
                                                   <asp:BoundField DataField="Sociedad" HeaderText="Sociedad" HeaderStyle-Width="90" />
                                                   <asp:BoundField DataField="PlanTarifDesc" HeaderText="Plan" HeaderStyle-Width="90" />
                                                   <asp:BoundField DataField="Total" HeaderText="Líneas Disponibles" HeaderStyle-Width="90" />
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

                               <%--NZ: Modal para Editar el Hallazgo.--%>
    <asp:Panel ID="pnlPopupAsignaLinea" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="rule"></div>
        <div class="modal-dialog modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label runat="server" ID="lblTituloAsgnaLinea2" Font-Size="16px"></asp:Label>
                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrar"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <div id="RepCapturaCollapse" class="form-horizontal" role="form">
                        <div class="row">
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblLinea" runat="server" CssClass="control-label">Línea:</asp:Label>
                                <asp:TextBox ID="txtLineaAsignar" runat="server"  Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblLineaPlan" runat="server" CssClass="control-label">Plan:</asp:Label>
                                <asp:TextBox ID="txtLineaPlan" runat="server"  Enabled="false"  CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblLineaEmpresa" runat="server" CssClass="control-label">Empresa:</asp:Label>
                                <asp:TextBox ID="txtLineaEmpre" runat="server"  Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                            <asp:HiddenField id="claveLineaAsignar" runat="server"/>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCancelarModal" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm"/>
                    <asp:Button ID="btnGuardarModal" runat="server" Text="Aceptar" CssClass="btn btn-keytia-sm" OnClick="btnGuardarModal_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnEditHallazo" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEditHallazo" runat="server" PopupControlID="pnlPopupAsignaLinea"
        TargetControlID="lnkBtnEditHallazo" CancelControlID="btnCerrar" BackgroundCssClass="modalPopupBackground" DropShadow="false">
    </asp:ModalPopupExtender>
    <%----%>

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
</asp:UpdatePanel>

</asp:Content>
