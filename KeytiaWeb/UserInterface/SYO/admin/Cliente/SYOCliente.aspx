<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="SYOCliente.aspx.cs" Inherits="KeytiaWeb.UserInterface.SYO.admin.Cliente.SYOCliente" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <%--Barra con boton y fechas--%>
    <div>
        <div>
            <div align="left" class="AutoHeight">
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Clientes</asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                &nbsp;&nbsp;
                <asp:LinkButton ID="LinkButton1" Text="Agregar" runat="server" PostBackUrl="~/UserInterface/SYO/admin/Cliente/SYOClienteIns.aspx"
                    CssClass="buttonAdd"></asp:LinkButton>
            </asp:Panel>
        </div>
    </div>
    <asp:GridView ID="grdClientes" runat="server" AutoGenerateColumns="false" onrowcommand="grdClientes_RowCommand" CssClass="DSOGrid"
        HeaderStyle-CssClass="titulosReportes" AllowPaging="true" OnPageIndexChanging="grdClientes_PageIndexChanging">
        <Columns>
           <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="btnModificar" runat="server" ImageUrl="~/Images/update.png" Height="20px" Width="20px" CommandName="Modificar" CommandArgument='<%# Eval("iCodCatalogo") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
          
            <asp:BoundField HeaderText="Clave" DataField="vchCodigo" />
            <asp:BoundField HeaderText="Descripcion" DataField="vchDescripcion" />
            <asp:BoundField HeaderText="Logo" DataField="Logo" />
            <asp:BoundField HeaderText="Logo de Exportación" DataField="LogoExportacion" />
        </Columns>
        <PagerSettings Position="Bottom" Mode="NumericFirstLast" PageButtonCount="4" FirstPageText="First"
            LastPageText="Last" />
        <RowStyle CssClass="grvitemStyle" />
        <AlternatingRowStyle CssClass="grvalternateItemStyle" />
    </asp:GridView>
</asp:Content>
