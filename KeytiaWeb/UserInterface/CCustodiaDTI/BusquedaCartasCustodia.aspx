<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="BusquedaCartasCustodia.aspx.cs" Inherits="KeytiaWeb.UserInterface.CCustodiaDTI.BuquedaCartasCustodia" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        function BuscaCarta() {
            var txt = document.getElementById("<%=txtFiltroBusquedaCCustodia.ClientID%>")
            $(txt).keypress(function (e) {
                var key = (e.keyCode ? e.keyCode : e.which);
                if (key == 13) { // 13 es keycode de 'enter'
                    var btn = document.getElementById("<%=btnBuscarSimpleCCustodia.ClientID%>");
                    btn.click();
                }
            })
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <!--Seccion del Title en página hija-->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Seccion del Body en página hija-->
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <div>
        <asp:Label ID="lblTitle" runat="server" Text="Empleados y Recursos" CssClass="page-title-keytia"></asp:Label>
    </div>

    <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
        <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
            <div class="portlet solid bordered">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-bar-chart font-dark hide"></i>
                        <span class="caption-subject titlePortletKeytia">Búsqueda</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                        <asp:TabContainer ID="tbcBuscaCCustodia" runat="server" CssClass="MyTabStyle">
                            <asp:TabPanel ID="tbpBuscaSimpleCCustodia" runat="server" HeaderText="Búsqueda Simple">
                                <ContentTemplate>
                                    <div class="row divCenter">
                                        <asp:Panel ID="pnlBuscaSimpleCCustodia" runat="server" CssClass="col-md-10 col-sm-10">
                                            <br />
                                            <div class="form-horizontal" role="form">
                                                <asp:Panel ID="tblBuscaSimpleCCustodia" runat="server" CssClass="form-group">
                                                    <div class="col-sm-4">
                                                        <asp:DropDownList ID="ddlFiltroCCustodia" runat="server" CssClass="form-control">
                                                            <asp:ListItem Text="Nombre del Empleado" Value="0" Selected="True"></asp:ListItem>
                                                            <asp:ListItem Text="Folio de la carta" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="No. de nómina" Value="2"></asp:ListItem>
                                                            <asp:ListItem Text="Extensión" Value="3"></asp:ListItem>
                                                            <asp:ListItem Text="Línea" Value="4"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-sm-6">
                                                        <asp:TextBox ID="txtFiltroBusquedaCCustodia" runat="server" CssClass="form-control" onkeyup="BuscaCarta();" autocomplete="off"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <asp:Button ID="btnBuscarSimpleCCustodia" Text="Buscar" runat="server"
                                                            CssClass="btn btn-keytia-md" OnClick="btnBuscarSimpleCCustodia_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel ID="tbpBusquedaDetalladaCCustodia" runat="server" HeaderText="Búsqueda Detallada" CssClass="row divCenter">
                                <ContentTemplate>
                                    <div class="row divCenter">
                                        <asp:Panel ID="pnlBusquedaDetalladaCCustodia" runat="server" CssClass="col-md-12 col-sm-12">
                                            <div class="form-horizontal" role="form">
                                                <div class="row">
                                                    <br />
                                                    <ul>
                                                        <li>Para realizar una búsqueda avanzada puede elegir una o varias de las siguientes opciones. </li>
                                                        <li>La carta custodia deberá cumplir con cada una de las condiciones que se especifique en la búsqueda. </li>
                                                    </ul>
                                                </div>
                                                <br />

                                                <div class="row">
                                                    <div class="col-md-6 col-sm-6">
                                                        <div class="form-group">
                                                            <asp:Label ID="lblFolio" runat="server" CssClass="col-sm-4 control-label">Folio de la carta:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtFolioCCustodia" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label ID="lblNombreEmple" runat="server" CssClass="col-sm-4 control-label">Nombre empleado:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtNombreEmpleado" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label ID="lblExten" runat="server" CssClass="col-sm-4 control-label">Extensión:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtExtension" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label ID="lblLinea" runat="server" CssClass="col-sm-4 control-label">Línea:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtLinea" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label ID="lblNoSerie" runat="server" CssClass="col-sm-4 control-label">No. de serie:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtNoSerie" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-6 col-sm-6">
                                                        <div class="form-group">
                                                            <asp:Label ID="Label1" runat="server" CssClass="col-sm-4 control-label">Estatus de la carta:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList ID="ddlEstatusCCustodia" runat="server" CssClass="form-control">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label ID="lblNomina" runat="server" CssClass="col-sm-4 control-label">No. de nómina:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtNoNomina" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label ID="lblCodAuto" runat="server" CssClass="col-sm-4 control-label">Código:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtCodAuto" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label ID="lblMac" runat="server" CssClass="col-sm-4 control-label">Mac address:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtMacAddress" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label ID="lblSitioEmple" runat="server" CssClass="col-sm-4 control-label">Sitio del empleado:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList ID="ddlSitioEmpleado" runat="server" CssClass="form-control">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="modal-footer">
                                                        <asp:Button ID="btnBuscarDetalladaCCustodia" runat="server" Text="Buscar"
                                                            CssClass="btn btn-keytia-md" OnClick="btnBuscarDetalladaCCustodia_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <br />
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                        </asp:TabContainer>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>

    <asp:Panel ID="pnlRow_1" runat="server" CssClass="row">
        <asp:Panel ID="Rep1" runat="server" CssClass="col-md-12 col-sm-12" Visible="false">
            <div class="portlet solid bordered">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-bar-chart font-dark hide"></i>
                        <span class="caption-subject titlePortletKeytia">Resultados</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepResultadoCollapse" aria-expanded="true" aria-controls="RepResultadoCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="collapse in" id="RepResultadoCollapse">
                        <div class="row form-horizontal">
                            <div class="col-sm-12">
                                <div class="input-group">
                                    <asp:Label ID="lblCartasEncontradas" runat="server" Text="" CssClass="control-label"></asp:Label>
                                    <span class="input-group-btn">
                                        <asp:Button ID="lbtnLimpiarBusquedaCCustodia" runat="server" Text="Limpiar búsqueda"
                                            OnClick="lbtnLimpiarBusquedaCCustodia_Click" CssClass="btn btn-keytia-sm"></asp:Button>
                                    </span>
                                </div>
                            </div>
                        </div>
                        <asp:Panel ID="pnlResultadoBusquedaGrid" runat="server" CssClass="table-responsive">
                            <asp:Literal ID="lMensajeBajaEmple" runat="server"></asp:Literal>
                            <asp:GridView ID="grvResultadoBusqueda" runat="server" CellPadding="4" CssClass="table table-bordered tableDashboard"
                                HeaderStyle-CssClass="tableHeaderStyle" AutoGenerateColumns="false" EmptyDataText="No existen resultados para esta búsqueda">
                                <Columns>
                                    <asp:HyperLinkField DataNavigateUrlFields="iCodCatEmple" DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc="
                                        DataTextField="Folio" HeaderText="Folio" />
                                    <asp:BoundField DataField="Estatus" HeaderText="Estatus" HtmlEncode="true" />
                                    <asp:HyperLinkField DataNavigateUrlFields="iCodCatEmple" DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc="
                                        DataTextField="Nomina" HeaderText="No. Nómina" />
                                    <asp:HyperLinkField DataNavigateUrlFields="iCodCatEmple" DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc="
                                        DataTextField="Linea" HeaderText="Línea"/>
                                    <asp:BoundField DataField="NombreEmple" HeaderText="Empleado" HtmlEncode="true" />
                                    <asp:BoundField DataField="Sitio" HeaderText="Sitio" HtmlEncode="true" />
                                    <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:d}" HtmlEncode="true" />
                                    <asp:BoundField DataField="InventarioAsignado" HeaderText="Eq." HtmlEncode="true" />
                                    <asp:BoundField DataField="Exten" HeaderText="Ext." HtmlEncode="true" />
                                    <asp:BoundField DataField="Cod" HeaderText="Cód." HtmlEncode="true" />
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>

                        <div class="row">
                            <div class="col-sm-2">
                                <asp:Button ID="btnRegresar" Text="Regresar" CssClass="btn btn-keytia-md" OnClick="btnRegresar_Click" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
