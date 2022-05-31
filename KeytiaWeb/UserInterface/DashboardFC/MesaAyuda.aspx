<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="MesaAyuda.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.MesaAyuda" %>

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
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Inicio</asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
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
                            <form name="FR_Busqueda" method="post">
                            <table class="DSOGrid" cellspacing="0" rules="all" border="1" style="height: 100%;
                                width: 100%; border-collapse: collapse;">
                                <tbody>
                                    <tr class="titulosReportes">
                                        <th colspan="2">
                                            <asp:Label ID="lblTitulo" runat="server">Búsqueda</asp:Label>
                                        </th>
                                    </tr>
                                    <tr class="grvitemStyle">
                                        <td align="left" class="titulosFiltroDetallado">
                                            &nbsp;<asp:Label ID="lblNominaEmple" runat="server">Nómina del Empleado:</asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNomina" runat="server" Width="240" MaxLength="30"></asp:TextBox>
                                            &nbsp;&nbsp;&nbsp;
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
                                            &nbsp;<asp:Label ID="lblNoCoincidencias" runat="server">No se encontró ningun empleado con la nómina especificada.</asp:Label>
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
        <asp:Panel ID="pnlRep2" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep2" runat="server" CssClass="TopCenter Center" Width="95%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Rep2" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <form name="frmDatosEmple" method="post">
                            <table class="DSOGrid" cellspacing="0" rules="all" border="1" style="height: 100%;
                                width: 100%; border-collapse: collapse;">
                                <tbody>
                                    <tr class="titulosReportes">
                                        <th colspan="2">
                                            <asp:Label ID="lblDatosEmpleado" runat="server">Datos del Empleado</asp:Label>
                                        </th>
                                    </tr>
                                    <tr class="grvitemStyle">
                                        <td align="left" class="titulosFiltroDetallado">
                                            <table>
                                                <tr>
                                                    <td>
                                                        &nbsp;<asp:Label runat="server">Nómina:</asp:Label>
                                                        &nbsp;<asp:Label ID="lblNomina" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        &nbsp;<asp:Label runat="server">Nombre:</asp:Label>
                                                        &nbsp;<asp:Label ID="lblNombre" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:Label runat="server">Departamento:</asp:Label>
                                                        &nbsp;<asp:Label ID="lblDepartamento" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:Label runat="server">RFC:</asp:Label>
                                                        &nbsp;<asp:Label ID="lblRFC" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:Label runat="server">Puesto:</asp:Label>
                                                        &nbsp;<asp:Label ID="lblPuesto" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        &nbsp;<asp:Label runat="server">Email:</asp:Label>
                                                        &nbsp;<asp:Label ID="lblEmail" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:Label runat="server">Ubicación:</asp:Label>
                                                        &nbsp;<asp:Label ID="lblUbicacion" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:Label runat="server">Tipo:</asp:Label>
                                                        &nbsp;<asp:Label ID="lblTipoEmple" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:Label runat="server">Jefe:</asp:Label>
                                                        &nbsp;<asp:Label ID="lblJefe" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
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
        <asp:Panel ID="pnlRep3" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep3" runat="server" CssClass="TopCenter Center" Width="95%">
                <asp:TableRow>
                    <asp:TableCell Width="100%">
                        <asp:Panel ID="Rep3" runat="server" CssClass="TopCenter divToCenter">
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
