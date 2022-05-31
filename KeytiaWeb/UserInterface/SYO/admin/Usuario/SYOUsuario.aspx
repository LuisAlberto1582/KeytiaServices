<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="SYOUsuario.aspx.cs" Inherits="KeytiaWeb.UserInterface.SYO.admin.Usuario.SYOUsuario" %>

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
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Usuarios</asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                &nbsp;&nbsp;
                <asp:LinkButton ID="LinkButton1" Text="Agregar" runat="server" PostBackUrl="~/UserInterface/SYO/admin/Usuario/SYOUsuarioIns.aspx"
                    CssClass="buttonAdd"></asp:LinkButton>
            </asp:Panel>
        </div>
    </div>
    <asp:GridView ID="grdEmpleados" runat="server" AutoGenerateColumns="false" OnRowCommand="grdEmpleados_RowCommand"
        CssClass="DSOGrid" HeaderStyle-CssClass="titulosReportes" AllowPaging="true"
        OnPageIndexChanging="grdEmpleados_PageIndexChanging">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="btnModificar" runat="server" ImageUrl="~/Images/Update.png"
                        Height="20px" Width="20px" CommandName="Modificar" CommandArgument='<%# Eval("iCodCatalogo") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="btnEliminar" runat="server" ImageUrl="~/Images/Delete.png" Height="20px"
                        Width="20px" CommandName="Eliminar" CommandArgument='<%# Eval("iCodCatalogo") %>' />
                </ItemTemplate>
            </asp:TemplateField>
           <asp:BoundField HeaderText="Usuario" DataField="vchCodigo" />
           <asp:BoundField HeaderText="Descripcion" DataField="vchDescripcion" />
           <asp:BoundField HeaderText="Nombre" DataField="NomCompleto" />
           <asp:BoundField HeaderText="Uri" DataField="Email" />
        </Columns>
        <PagerSettings Position="Bottom" Mode="NumericFirstLast" PageButtonCount="4" FirstPageText="First"
            LastPageText="Last" />
        <RowStyle CssClass="grvitemStyle" />
        <AlternatingRowStyle CssClass="grvalternateItemStyle" />
    </asp:GridView>
</asp:Content>
