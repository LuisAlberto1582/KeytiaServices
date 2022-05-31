<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="BusquedaInventarioDisp.aspx.cs" Inherits="KeytiaWeb.UserInterface.CCustodiaDTI.BusquedaInventarioDisp"
    ValidateRequest="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <!--Alta de inventario-->
    <asp:Panel ID="pnlFiltrosInventario" runat="server">
        <asp:Label Font-Bold="true" ID="lblTitulo" runat="server" Text="Administración de inventario"></asp:Label>
        <br />
        <br />
        <table align="center">
            <tr>
                <td>
                    <asp:Label ID="lblEstatus" runat="server" Text="Estatus:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblMarca" runat="server" Text="Marca:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblModelo" runat="server" Text="Modelo:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblTipoAparato" runat="server" Text="Tipo de Aparato:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblNoSerie" runat="server" Text="No. de Serie:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lbltMacAddress" runat="server" Text="MAC Address:"></asp:Label>
                </td>
                <td>
                    <asp:LinkButton ID="lbtnLimpiarBusqueda" runat="server" Text="[Limpiar búsqueda]"
                        OnClick="lbtnLimpiarBusqueda_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="drpEstatus" runat="server" AppendDataBoundItems="true">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="drpMarca" runat="server">
                    </asp:DropDownList>
                    <asp:CascadingDropDown UseContextKey="true" ContextKey='<%# Bind("iCodMarca") %>'
                        ID="ccdMarca" runat="server" Category="Marca" TargetControlID="drpMarca" ServiceMethod="ObtieneTodasMarcas"
                        ServicePath="~/UserInterface/CCustodiaDTI/CCustodia.asmx" PromptText="-- Seleccionar --">
                    </asp:CascadingDropDown>
                </td>
                <td>
                    <asp:DropDownList ID="drpModelo" runat="server">
                    </asp:DropDownList>
                    <asp:CascadingDropDown UseContextKey="true" ContextKey='<%# Bind("iCodModelo") %>'
                        ID="ccdModelo" runat="server" Category="Modelo" TargetControlID="drpModelo" ParentControlID="drpMarca"
                        ServiceMethod="ObtieneTodosModeloPorMarca" ServicePath="~/UserInterface/CCustodiaDTI/CCustodia.asmx"
                        PromptText="-- Seleccionar --">
                    </asp:CascadingDropDown>
                </td>
                <td>
                    <asp:DropDownList ID="drpTipoAparato" runat="server" AppendDataBoundItems="true">
                        <asp:ListItem Text="-- Seleccionar --" Value="0" />
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:TextBox ID="txtNoSerie" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:TextBox ID="txtMacAddress" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:LinkButton ID="lbtnBusqueda" runat="server" Text="[Buscar inventario]" OnClick="lbtnBusqueda_Click" />
                </td>
                <td>
                    <asp:LinkButton ID="lbtnAgregarInventario" runat="server" Text="[Agregar inventario]"
                        OnClick="lbtnAgregarInventario_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Label ID="lblEquiposEncontrados" runat="server" Text=""></asp:Label>
    <br />
    <br />
    <asp:Panel ID="pnlResultBusqInventarioGrid" runat="server" HorizontalAlign="Center"
        Visible="false">
        <asp:GridView ID="grvResultBusqInventario" runat="server" CellPadding="4" CssClass="GridView"
            GridLines="Vertical" AutoGenerateColumns="false" EmptyDataText="No existen resultados para esta búsqueda"
            Width="100%" Style="text-align: justify; margin-top: 0px;" DataKeyNames="Estatus">
            <Columns>
                <asp:BoundField DataField="Estatus" HeaderText="Estatus" HtmlEncode="true" />
                <asp:BoundField DataField="Marca" HeaderText="Marca" HtmlEncode="true" />
                <asp:BoundField DataField="Modelo" HeaderText="Modelo" HtmlEncode="true" />
                <asp:BoundField DataField="TipoDispositivo" HeaderText="Tipo de aparato" HtmlEncode="true" />
                <asp:BoundField DataField="NSerie" HeaderText="No. de serie" HtmlEncode="true" />
                <asp:BoundField DataField="MacAddress" HeaderText="Mac Address" HtmlEncode="true" />
                <asp:HyperLinkField DataNavigateUrlFields="iCodCatDisp" DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AdminInventario.aspx?iCodCatDisp={0}&st=tLaJQx5zLrc="
                    Text="[Modificar]" />
                <asp:HyperLinkField DataNavigateUrlFields="iCodCatDisp" DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AdminInventario.aspx?iCodCatDisp={0}&st=dwcjh%2B7jVOVukQvtst5CXQ=="
                    Text="[Borrar]" />
            </Columns>
            <RowStyle CssClass="GridRowOdd" />
            <AlternatingRowStyle CssClass="GridRowEven" />
        </asp:GridView>
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlResultBusqInventarioGrid2" runat="server" HorizontalAlign="Center"
        Visible="false">
        <asp:GridView ID="grvResultBusqInventario2" runat="server" CellPadding="4" CssClass="GridView"
            GridLines="Vertical" AutoGenerateColumns="false" EmptyDataText="No existen resultados para esta búsqueda"
            Width="100%" Style="text-align: justify; margin-top: 0px;">
            <Columns>
                <asp:BoundField DataField="Estatus" HeaderText="Estatus" HtmlEncode="true" />
                <asp:BoundField DataField="Marca" HeaderText="Marca" HtmlEncode="true" />
                <asp:BoundField DataField="Modelo" HeaderText="Modelo" HtmlEncode="true" />
                <asp:BoundField DataField="TipoDispositivo" HeaderText="Tipo de aparato" HtmlEncode="true" />
                <asp:BoundField DataField="NSerie" HeaderText="No. de serie" HtmlEncode="true" />
                <asp:BoundField DataField="MacAddress" HeaderText="Mac Address" HtmlEncode="true" />
            </Columns>
            <RowStyle CssClass="GridRowOdd" />
            <AlternatingRowStyle CssClass="GridRowEven" />
        </asp:GridView>
        <br />
    </asp:Panel>
</asp:Content>
