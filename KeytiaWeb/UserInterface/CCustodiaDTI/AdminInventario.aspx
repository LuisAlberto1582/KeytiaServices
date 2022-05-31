<%@ Page Title="Administracion de inventario" Language="C#" MasterPageFile="~/KeytiaOH.Master" 
CodeBehind="AdminInventario.aspx.cs" Inherits="KeytiaWeb.UserInterface.CCustodiaDTI.AdminInventario" 
EnableEventValidation="false" AutoEventWireup="true"%>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript" language="javascript">
        function alerta(mensaje) {
            alert('Aviso: ' + mensaje);
        }

        actualizaMacAddress();
        
        function actualizaMacAddress() {
            if (document.getElementById('<%=chbPendiente.ClientID %>').checked) {
                document.getElementById('<%=txtMacAddress.ClientID %>').value = "PENDIENTE";
            }
            else {
                document.getElementById('<%=txtMacAddress.ClientID %>').value = "";
            }
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true" EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    
    <asp:Table ID="tblHeaderAdminInventario" runat="server" Width="100%">
    <asp:TableRow ID="tblrHeaderAdminInventario1" runat="server">
    <asp:TableCell ID="tblcTitulo" runat="server" HorizontalAlign="Left">
    <asp:Label Font-Bold="true" ID="lblTitulo" runat="server" Text="Administración de inventario"></asp:Label>
    </asp:TableCell>
    <asp:TableCell ID="tblrHeaderAdminInventario2" runat="server" HorizontalAlign="Right">
    <asp:LinkButton ID="lbtnRegresarPaginaBusq" runat="server" Text="Volver a los resultados de la búsqueda" Font-Bold="true" OnClick="lbtnRegresarPaginaBusq_Click"></asp:LinkButton>
    </asp:TableCell>
    </asp:TableRow>
    </asp:Table>
    <br />
    
    <asp:Label ID="lblMensajeTipoEstado" runat="server" Text="" Enabled="false" Visible="false"></asp:Label>
    <br />
    
    <!--Inventario-->
    <asp:Panel ID="pnlAddEditInventario" runat="server">
        <table align="center">
            <tr>
                <td>
                    <asp:Label ID="lblMarca" runat="server" Text="Marca"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="drpMarca" runat="server">
                    </asp:DropDownList>
                    <asp:CascadingDropDown UseContextKey = "true" ContextKey='<%# Bind("iCodMarca") %>' ID="ccdMarca"
                        runat="server" Category="Marca" TargetControlID="drpMarca" 
                        ServiceMethod="ObtieneTodasMarcas" ServicePath="~/UserInterface/CCustodiaDTI/CCustodia.asmx"
                        PromptText="-- Seleccionar --">
                    </asp:CascadingDropDown>
                    <asp:TextBox ID="txtMarca" runat="server" Visible="false" Enabled="false"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblModelo" runat="server" Text="Modelo"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="drpModelo" runat="server">
                    </asp:DropDownList>
                    <asp:CascadingDropDown UseContextKey = "true" ContextKey='<%# Bind("iCodModelo") %>' ID="ccdModelo"
                        runat="server" Category="Modelo" TargetControlID="drpModelo" ParentControlID="drpMarca"
                        ServiceMethod="ObtieneTodosModeloPorMarca" ServicePath="~/UserInterface/CCustodiaDTI/CCustodia.asmx" 
                        PromptText="-- Seleccionar --"> 
                    </asp:CascadingDropDown>
                    <asp:TextBox ID="txtModelo" runat="server" Visible="false" Enabled="false"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblTipoAparato" runat="server" Text="Tipo de Aparato"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="drpTipoAparato" runat="server" AppendDataBoundItems="true">
                    <asp:ListItem Text="-- Seleccionar --" Value="" /> 
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblNoSerie" runat="server" Text="No. de Serie"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtNoSerie" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbltMacAddress" runat="server" Text="MAC Address"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtMacAddress" runat="server"></asp:TextBox>
                    <asp:CheckBox ID="chbPendiente" runat="server" Text="PENDIENTE" Checked="false"/>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblFechaCompra" runat="server" Text="Fecha de Compra"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtFechaCompra" runat="server" ReadOnly="false" Enabled="true" Width="200"></asp:TextBox>
                    <asp:CalendarExtender ID="ceSelectorFecha" runat="server" TargetControlID="txtFechaCompra">
                    </asp:CalendarExtender>
                </td>
            </tr>
        </table>
        <br />
        <table align="center">
            <tr>
                <td align="center">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    
</asp:Content>
