<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="SolPaquetes.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.TerniumSolPaquetes.SolPaquetes" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .list-group-item.active{
            z-index:0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <div ID="pnlMainHolder" runat="server">
    <div ID="pnlRow_0" runat="server" CssClass="row">
       <div ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
            <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia"></span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                          <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                              <div class="row" runat="server" id="row1">
                                 <div class="col-sm-12">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h4 class="labelsize">Solicitud de Paquetes</h4>
                                        </div>
                                        <div class="panel-body" style="font-size:15px;">
                                          <div class="row">
                                            <div class="col-sm-6">
                                                <asp:Panel ID="rowNombre" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblNombre" runat="server" CssClass="col-sm-4 control-label">Nombre: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:HiddenField runat="server" ID="hfIcodEmple"/>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowCencos" runat="server" CssClass="form-group">
                                                    <asp:Label runat="server" ID="lblCencos" CssClass="col-sm-4 control-label"> Centro de Costos:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtCencos" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                            <div class="col-sm-6">
                                                <asp:Panel ID="rowEmail" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblEmail" runat="server" CssClass="col-sm-4 control-label">Email: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="RowLinea" runat="server" CssClass="form-group">
                                                    <asp:Label runat="server" ID="Label2" CssClass="col-sm-4 control-label">Línea:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:DropDownList ID="cboLineas" runat="server" CssClass="col-sm-2 form-control" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="Linea">
                                                            <asp:ListItem Value="0"> Seleciona una Línea </asp:ListItem>
                                                        </asp:DropDownList>                                                               
                                                    </div>
                                                </asp:Panel>
                                             </div>
                                          </div>
                                          <div class="row" runat="server" id="rowATT1" visible="false">
                                            <div class="col-sm-12">
                                                <div class="alert alert-info" role="alert">
                                                    <p style="font-size:15.5px;">
                                                    <strong><em>IMPORTANTE:</em></strong> Si re cuerda su contraseña del Portal <strong><em>miAT&T</em></strong>, 
                                                    Favor ingresarla en los siguientes campos <strong><em>(Número ID, Password)</em></strong>, 
                                                    de lo contrario ingresar a la siguiente liga: <a href="javascript:window.open('https://www.att.com.mx/MiATTWeb/','','toolbar=yes');void 0"><strong><em>https://www.att.com.mx/MiATTWeb/</em></strong></a>
                                                    ingresar su número móvil y seleccionar la opción <strong><em>¿No Recuerdas tu Contraseña?</em></strong>
                                                    La cual le llegara una contraseña temporal al móvil y deberá colocarla en el cuadro de <strong><em>password</em></strong>, 
                                                    si aún no se ha registrado deberá de registrarse en el portal de <strong><em>miAT&T</em></strong> al finalizar el registro, 
                                                    ingresar los datos en los siguientes campos.
                                                    </p>
                                                </div>
                                            </div>
                                          </div>
                                          <div class="row" runat="server" id="rowDatosATT2" visible="false">
                                            <div class="col-sm-6">
                                                <asp:Panel ID="rowNumID" runat="server" CssClass="form-group">
                                                    <asp:Label runat="server" ID="lblNumId" CssClass="col-sm-4 control-label">Número ID:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtNumId" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                             </div>
                                            <div class="col-sm-6">
                                                <asp:Panel ID="rowPass" runat="server" CssClass="form-group">
                                                    <asp:Label runat="server" ID="lblPassword" CssClass="col-sm-4 control-label">Password:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                          </div>
                                          <div class="row" runat="server" id="rowPaquete1" visible="false">
                                            <div class="col-sm-6">
                                                <asp:Panel runat="server" ID="rowDestino" CssClass="form-group">
                                                    <asp:Label runat="server" ID="lblDestino" CssClass="col-sm-4 control-label">Destino:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox runat="server" ID="txtDestino" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                          </div>
                                          <div class="row" runat="server" id="rowPaquetes2" visible="false">
                                            <div class="col-sm-6">
                                                <asp:Panel runat="server" ID="rowInicio" CssClass="form-group">
                                                    <asp:Label ID="lblFecInicio" runat="server" CssClass="col-sm-4 control-label">A partir del día: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <cc1:DSODateTimeBox ID="pdtInicio" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                                            ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                                        </cc1:DSODateTimeBox>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel runat="server" ID="rowTotalDias" CssClass="form-group">
                                                    <asp:Label ID="lblTotalDias" runat="server" CssClass="col-sm-4 control-label">Total de días: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtTotalDis" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                            <div class="col-sm-6">
                                                <asp:Panel runat="server" ID="rowFecFin" CssClass="form-group">
                                                    <asp:Label ID="lblFecFin" runat="server" CssClass="col-sm-4 control-label">Al día: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <cc1:DSODateTimeBox ID="pdtFin" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                                            ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                                        </cc1:DSODateTimeBox>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel runat="server" ID="rowPaquete" CssClass="form-group">
                                                    <asp:label runat="server" ID="lblPaquete" CssClass="col-sm-4 control-label">Paquetes: </asp:label>
                                                    <div class="col-sm-8">
                                                        <asp:DropDownList runat="server" ID="cboPaquetes" CssClass="col-sm-2 form-control"></asp:DropDownList>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                          </div>
                                          <div class="row" runat="server" id="rowCobertura" visible="false">
                                            <div class="col-sm-12">
                                                <div class="list-group"> 
                                                    <a class="list-group-item active"> 
                                                        <h4 class="list-group-item-heading" runat="server" id="cobertura">List group item heading</h4>                                                                 
                                                    </a> 
                                                    <a class="list-group-item">                                                                 
                                                        <p class="list-group-item-tet" runat="server" id="descCobertura">Donec id elit non mi porta gravida at eget metus. Maecenas sed diam eget risus varius blandit.</p> 
                                                    </a> 
                                                </div>
                                            </div>             
                                          </div>
                                          <div class="row" id="rowPaquetes" runat="server" visible="false">
                                             <div class="col-sm-12">
                                               <asp:GridView runat="server" ID="grdPaquetes"  HeaderStyle-CssClass="tableHeaderStyle" 
                                               CssClass="table table-bordered tableDashboard" AutoGenerateColumns="false">
                                                    <Columns>
                                                       <asp:TemplateField HeaderText="">
                                                         <ItemTemplate>
                                                            <div class="col-sm-1">                             
                                                                <div class="mt-radio-inline">
                                                                     <label class="mt-radio">
                                                                       <input name="rbtnPaquete" type="radio" value='<%# Eval("ClavePaquete") %>'  />
                                                                       <span></span>
                                                                     </label>
                                                                </div>                                                       
                                                            </div>
                                                          </ItemTemplate>
                                                       </asp:TemplateField>
                                                       <asp:BoundField HeaderText="Paquete"/>
                                                       <asp:BoundField HeaderText="Tarifa"/>
                                                       <asp:BoundField HeaderText="MB"/>
                                                       <asp:BoundField HeaderText="Vigencia"/>
                                                    </Columns>
                                               </asp:GridView>
                                             </div>
                                          </div>
                                          <div class="row" runat="server">
                                            <div class="col-sm-10">                                                       
                                               <asp:Panel ID="rowAceptar" runat="server" CssClass="form-group" Visible="false">
                                                 <div class="col-sm-offset-6 col-sm-8">
                                                     <asp:Button ID="btnGuardar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-lg" />
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
</div>
</asp:Content>
