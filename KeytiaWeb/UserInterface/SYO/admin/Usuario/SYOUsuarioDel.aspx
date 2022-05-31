<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="SYOUsuarioDel.aspx.cs" Inherits="KeytiaWeb.UserInterface.SYO.admin.Usuario.SYOUsuarioDel" %>

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
    <div>
        <div>
            <div align="left" class="AutoHeight">
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Usuarios</asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                &nbsp;&nbsp;<asp:LinkButton ID="LinkButton1" Text="Regresar" runat="server" PostBackUrl="~/UserInterface/SYO/admin/Usuario/SYOUsuario.aspx"
                    CssClass="buttonBack"></asp:LinkButton>
            </asp:Panel>
        </div>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep0" runat="server" CssClass="TopCenter Center" Width="45%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Rep0" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <form name="FR_Busquedas" method="post">
                            <table class="DSOGrid" cellspacing="0" rules="all" border="1" id="RepPorSucursalMatGrid"
                                style="height: 100%; width: 100%; border-collapse: collapse;">
                                <tr class="titulosReportes">
                                    <th colspan="2">
                                        <asp:Label ID="lblTitulo" runat="server">Eliminar Usuario</asp:Label>
                                    </th>
                                </tr>
                                <tr class="grvitemStyle">
                                    <td>
                                        Uri:
                                    </td>
                                    <td>
                                        <asp:Label ID="lblUri" runat="server" Text="Label"></asp:Label>
                                    </td>
                                </tr>
                                <tr class="grvalternateItemStyle">
                                    <td>
                                        Descripcion:
                                    </td>
                                    <td>
                                        <asp:Label ID="lblDescripcion" runat="server" Text="Label"></asp:Label>
                                    </td>
                                </tr>
                                <tr class="grvitemStyle">
                                    <td>
                                        Nombre:
                                    </td>
                                    <td>
                                        <asp:Label ID="lblNombre" runat="server" Text="Label"></asp:Label>
                                    </td>
                                </tr>
                                <tr class="grvalternateItemStyle">
                                    <td>
                                        Paterno:
                                    </td>
                                    <td>
                                        <asp:Label ID="lblPaterno" runat="server" Text="Label"></asp:Label>
                                    </td>
                                </tr>
                                <tr class="grvitemStyle">
                                    <td>
                                        Materno:
                                    </td>
                                    <td>
                                        <asp:Label ID="lblMaterno" runat="server" Text="Label"></asp:Label>
                                    </td>
                                </tr>
                                 <tr class="grvitemStyle">
                                   <td>Tipo de URI:</td>
                                   <td>
                                     <div style="text-align: left; width: 50%; margin: auto;">
                                       <asp:CheckBoxList ID="CheckBoxList1" runat="server" Enabled="false">
                                           <asp:ListItem Value="1" onclick="UncheckOthers(this);">Es Movil</asp:ListItem>
                                           <asp:ListItem Value="2" onclick="UncheckOthers(this);">Es VR</asp:ListItem>
                                           <asp:ListItem Value="4" onclick="UncheckOthers(this);">Es Fijo</asp:ListItem>
                                       </asp:CheckBoxList>
                                         </div>
                                        <asp:CustomValidator ID="CustomValidator1" runat="server" ClientValidationFunction="ValidateCheckBoxList" ErrorMessage="Elija una casilla" ValidationGroup="vlg1" ></asp:CustomValidator>
                                       
                                     
                                   </td>
                                 </tr>
                                 <tr class="grvitemStyle">
                                    <td>
                                        Perfil de Usuario:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlPerfil" runat="server" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr class="grvalternateItemStyle">
                                    <td>
                                        Empresa:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlEmpresa" runat="server" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr class="grvitemStyle">
                                    <td>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" OnClick="btnEliminar_Click" />
                                    </td>
                                </tr>
                            </table>
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            </form>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <asp:Panel ID="pnlRep9" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep9" runat="server" CssClass="TopCenter divToCenter" Width="100%">
                <asp:TableRow>
                    <asp:TableCell Width="100%">
                        <asp:Panel ID="Rep9" runat="server" CssClass="TopCenter divToCenter">
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
