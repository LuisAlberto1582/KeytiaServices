<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="BusquedaCartasCustodia.aspx.cs" Inherits="KeytiaWeb.UserInterface.CCustodia.BuquedaCartasCustodia" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
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
        <asp:Label ID="lblTitle" runat="server" Text="Búsqueda de Cartas Custodia" Font-Size="Large"
            Font-Bold="true" ForeColor="#2E6E9E"></asp:Label>
        <br />
        <br />
        <br />
        <asp:TabContainer ID="tbcBuscaCCustodia" runat="server">
            <asp:TabPanel ID="tbpBuscaSimpleCCustodia" runat="server">
                <HeaderTemplate>
                    Búsqueda Simple
                </HeaderTemplate>
                <ContentTemplate>
                    <asp:Panel ID="pnlBuscaSimpleCCustodia" runat="server">
                        <asp:Table ID="tblBuscaSimpleCCustodia" runat="server" Height="32px" Width="306px">
                            <asp:TableRow ID="tblBuscaSimpleCCustodiaF1" runat="server">
                                <asp:TableCell ID="tblBuscaSimpleCCustodiaC1" runat="server">
                                    <asp:DropDownList ID="ddlFiltroCCustodia" runat="server">
                                        <asp:ListItem Text="Nombre del Empleado" Value="0" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Folio de la carta" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="No. de nómina" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Extensión" Value="3"></asp:ListItem>
                                    </asp:DropDownList>
                                </asp:TableCell>
                                <asp:TableCell ID="tblBuscaSimpleCCustodiaC2" runat="server">
                                    <asp:TextBox ID="txtFiltroBusquedaCCustodia" runat="server"></asp:TextBox>
                                </asp:TableCell>
                                <asp:TableCell ID="tblBuscaSimpleCCustodiaC3" runat="server">
                                    <asp:Button ID="btnBuscarSimpleCCustodia" Text="Buscar Cartas Custodia" runat="server"  
                                     CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" 
                                     OnClick="btnBuscarSimpleCCustodia_Click" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:TabPanel>
            <asp:TabPanel ID="tbpBusquedaDetalladaCCustodia" runat="server">
                <HeaderTemplate>
                    Búsqueda Detallada
                </HeaderTemplate>
                <ContentTemplate>
                    <asp:Panel ID="pnlBusquedaDetalladaCCustodia" runat="server">
                        <ul>
                            <li>Para realizar una búsqueda avanzada puede elegir una o varias de las siguientes
                                opciones. </li>
                            <li>La carta custodia deberá cumplir con cada una de las condiciones que se especifique
                                en la búsqueda. </li>
                        </ul>
                        <asp:Table ID="tblBusquedaDetalladaCCustodia" runat="server" HorizontalAlign="Center">
                            <asp:TableRow ID="tblBusquedaDetalladaCCustodiaF1" runat="server">
                                <asp:TableCell ID="tblBusquedaDetalladaCCustodiaC1" runat="server">
                                    Folio de la carta:
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladaCCustodiaC2" runat="server">
                                    <asp:TextBox ID="txtFolioCCustodia" runat="server"></asp:TextBox>
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladaCCustodiaC11" runat="server">
                                    Estatus de la carta:
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladaCCustodiaC12" runat="server">
                                    <asp:DropDownList ID="ddlEstatusCCustodia" runat="server">
                                    </asp:DropDownList>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="tblBusquedaDetalladoCCustodiaF2" runat="server">
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC3" runat="server">
                                    Nombre del empleado:
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladaCCustodiaC4" runat="server">
                                    <asp:TextBox ID="txtNombreEmpleado" runat="server"></asp:TextBox>
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladaCCustodiaC13" runat="server">
                                    No. de nómina:
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladaCCustodiaC14" runat="server">
                                    <asp:TextBox ID="txtNoNomina" runat="server"></asp:TextBox>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="tblBusquedaDetalladoCCustodiaF3" runat="server">
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC5" runat="server">
                                    Extensión:
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC6" runat="server">
                                    <asp:TextBox ID="txtExtension" runat="server"></asp:TextBox>
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC15" runat="server">
                                    Código:
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC16" runat="server">
                                    <asp:TextBox ID="txtCodAuto" runat="server"></asp:TextBox>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="tblBusquedaDetalladoCCustodiaF4" runat="server">
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC7" runat="server">
                                    No. de serie:
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC8" runat="server">
                                    <asp:TextBox ID="txtNoSerie" runat="server"></asp:TextBox>
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC17" runat="server">
                                    Mac address:
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC18" runat="server">
                                    <asp:TextBox ID="txtMacAddress" runat="server"></asp:TextBox>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="tblBusquedaDetalladoCCustodiaF5" runat="server">
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC9" runat="server">
                                        Sitio del empleado:
                                </asp:TableCell>
                                <asp:TableCell ID="tblBusquedaDetalladoCCustodiaC10" runat="server">
                                    <asp:DropDownList ID="ddlSitioEmpleado" runat="server">
                                    </asp:DropDownList>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <br />
                        <asp:Panel ID="pnlBotonBusquedaDetallada" runat="server" HorizontalAlign="Center">
                            <asp:Button ID="btnBuscarDetalladaCCustodia" runat="server" Text="Buscar Cartas Custodia" 
                              CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" 
                              OnClick="btnBuscarDetalladaCCustodia_Click" />
                        </asp:Panel>
                    </asp:Panel>
                </ContentTemplate>
            </asp:TabPanel>
        </asp:TabContainer>
        <asp:Table ID="tblHeadResultBusq" runat="server" Width="100%" Visible="false">
            <asp:TableRow ID="tblHeadResultBusqR1" runat="server">
                <asp:TableCell ID="tblHeadResultBusqR1C1" runat="server" HorizontalAlign="Left">
                    <asp:Label ID="lblCartasEncontradas" runat="server" Text=""></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="tblHeadResultBusqR1C2" runat="server" HorizontalAlign="Right">
                    <asp:LinkButton ID="lbtnLimpiarBusquedaCCustodia" runat="server" Text="[ Limpiar búsqueda ]"
                        Font-Bold="true" OnClick="lbtnLimpiarBusquedaCCustodia_Click"></asp:LinkButton>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <br />
        <asp:Panel ID="pnlResultadoBusquedaGrid" runat="server" HorizontalAlign="Center"
            Visible="false">
            <asp:Literal ID="lMensajeBajaEmple" runat="server"></asp:Literal>
            <asp:GridView ID="grvResultadoBusqueda" runat="server" CellPadding="4" CssClass="GridView"
                GridLines="Vertical" AutoGenerateColumns="false" EmptyDataText="No existen resultados para esta búsqueda"
                Width="100%" Style="text-align: justify; margin-top: 0px;">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="iCodCatEmple" DataNavigateUrlFormatString="~/UserInterface/CCustodia/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc="
                        DataTextField="Folio" HeaderText="Folio" />
                    <asp:BoundField DataField="Estatus" HeaderText="Estatus" HtmlEncode="true" />
                    <asp:HyperLinkField DataNavigateUrlFields="iCodCatEmple" DataNavigateUrlFormatString="~/UserInterface/CCustodia/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc="
                        DataTextField="Nomina" HeaderText="No. Nómina" />
                    <asp:BoundField DataField="NombreEmple" HeaderText="Empleado" HtmlEncode="true" />
                    <asp:BoundField DataField="Sitio" HeaderText="Sitio" HtmlEncode="true" />
                    <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:d}" HtmlEncode="true" />
                    <asp:BoundField DataField="InventarioAsignado" HeaderText="Eq." HtmlEncode="true" />
                    <asp:BoundField DataField="Exten" HeaderText="Ext." HtmlEncode="true" />
                    <asp:BoundField DataField="Cod" HeaderText="Cód." HtmlEncode="true" />
                </Columns>
                <RowStyle CssClass="GridRowOdd" />
                <AlternatingRowStyle CssClass="GridRowEven" />
            </asp:GridView>
            <br />
            <table align="center">
                <tr>
                    <td>
                        <asp:Button ID="btnRegresar" Text="Regresar" 
                         CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary"
                         OnClick="btnRegresar_Click" runat="server"  />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>
