<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConsultaSolicitudRepPolizaContable.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepProsa.ConsultaSolicitudRepPolizaContable" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="CSS/StyleFile.css" rel="stylesheet" />
    <script type="text/javascript" src="scripts/FilterTable.js"></script>
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <div id="pnlMainHolder" runat="server">
        <div id="pnlRow_0" runat="server" cssclass="row">
            <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Solicitudes de reportes poliza contable</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body" id="divContenedor">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <asp:Button runat="server" ID="btnConsultar" CssClass="btn btn-keytia-sm" Text="Consultar" OnClick="btnConsultar_Click"/>
                                    &nbsp;&nbsp;&nbsp;
                                    <asp:Button CssClass="btn btn-keytia-sm" runat="server" ID="btnAgregar" Text="Agregar" OnClick="btnAgregar_Click"></asp:Button>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-sm-8">
                                            <div class="form-group">
                                                <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                                                <div class="col-offset-2 col-sm-4">
                                                    <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" runat="server" id="rowGrv">
                                        <div class="col-sm-12">
                                            <div class="clearfix"></div>
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="grvListadoCargas"
                                                    HeaderStyle-CssClass="tableHeaderStyle"
                                                    CssClass="table table-bordered tableDashboard" HeaderStyle-Font-Bold="true"
                                                    AutoGenerateColumns="false" DataKeyNames="iCodCatalogo">
                                                    <Columns>
                                                        <asp:BoundField DataField="vchDescripcion" HeaderText="Carga" />
                                                        <asp:BoundField DataField="AnioDesc" HeaderText="Año" />
                                                        <asp:BoundField DataField="MesDesc" HeaderText="Mes" />
                                                        <asp:BoundField DataField="EstCargaDesc" HeaderText="Estatus Carga" />
                                                        <asp:BoundField DataField="DestinatarioEmail" HeaderText="Destinatarios" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <!-- Alerta Success -->
                                            <div style="width: 400px; float: none; margin: 0 auto;">
                                                <asp:Panel ID="InfoPanelSucces" CssClass="alert alert-info text-center" runat="server" role="alert" Visible="false">
                                                    <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                                    <strong>
                                                        <asp:Label ID="lblMensajeSuccess" runat="server"></asp:Label>
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
                </div>
            </div>
        </div>
    </div>
</asp:Content>
