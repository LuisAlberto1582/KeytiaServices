<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="BusquedaPorElemento.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.BusquedaPorElemento" %>

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
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Búsquedas</asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
            </asp:Panel>
        </div>
    </div>
    
    <%--Panel--%>
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep0" runat="server" CssClass="TopCenter Center" Width="72%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Rep0" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <form name="FR_Busqueda" method="post">
                            <table class="DSOGrid" cellspacing="0" rules="all" border="1" style="height: 100%;
                                width: 100%; border-collapse: collapse;">
                                <tbody>
                                    <tr class="titulosReportes">
                                        <th colspan="4">
                                            <asp:Label ID="lblTitulo" runat="server">Búsqueda</asp:Label>
                                        </th>
                                    </tr>
                                    <tr class="grvitemStyle">
                                        <td align="left" class="titulosFiltroDetallado">
                                            &nbsp;<asp:Label ID="lblTexto" runat="server">Texto de Búsqueda:</asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtTexto" runat="server" Width="400"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="cboTipoElemento" runat="server" Width="240">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="buttonPlay" OnClick="btnAceptar_Click" />
                                        </td>
                                    </tr>
                                </tbody>
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
        <%--Sin Resultados--%>
        <asp:Panel ID="pnlRep1" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep1" runat="server" CssClass="TopCenter Center" Width="95%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Rep1" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <form name="FR_Busqueda" method="post">
                            <table class="DSOGrid" cellspacing="0" rules="all" border="1" style="height: 100%;
                                width: 100%; border-collapse: collapse;">
                                <tbody>
                                    <tr class="titulosReportes">
                                        <th colspan="2">
                                            <asp:Label ID="Label1" runat="server">No hay resultados</asp:Label>
                                        </th>
                                    </tr>
                                    <tr class="grvitemStyle">
                                        <td align="center" class="titulosFiltroDetallado">
                                            &nbsp;<asp:Label ID="lblNoCoincidencias" runat="server">No se encontró ninguna coincidencia para su búsqueda.</asp:Label>
                                        </td>
                                    </tr>
                                </tbody>
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
        <%--Resultados de la busqueda--%>
        <asp:Panel ID="pnlRep2" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep2" runat="server" CssClass="TopCenter Center" Width="95%">
                <asp:TableRow>
                    <asp:TableCell Width="100%">
                        <asp:Panel ID="Rep2" runat="server" CssClass="TopCenter divToCenter">
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </asp:Panel>
    
    <%--NZ: Modal para mensajes de Error--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server"TabIndex="-1" role="dialog" CssClass="modal-Keytia" style="display: none;">
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
    <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje" TargetControlID="lnkBtnMsn" 
       OkControlID="btnYes" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
    </asp:ModalPopupExtender>
</asp:Content>
