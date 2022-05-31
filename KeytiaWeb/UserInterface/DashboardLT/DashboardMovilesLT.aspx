<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="DashboardMovilesLT.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardLT.DashboardMovilesLT" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script>
        //document.write('<a href="' + document.referrer + '">Go Back</a>');
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <%--Barra con boton y fechas--%>
    <div>
        <div>
            <div align="left" style="height: 24px">
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Inicio</asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                <asp:Button ID="btnRegresar" runat="server" CssClass="buttonBack" OnClick="btnRegresar_Click" />
                &nbsp;<asp:Button ID="btnAplicar" runat="server" CssClass="buttonSave" OnClick="btnAplicar_Click" />
                &nbsp;<asp:Button ID="btnDashPrincipal" runat="server" CssClass="buttonSave" OnClick="btnDashPrincipal_Click"
                    Visible="false" />
                &nbsp;<asp:Button ID="btnDashMoviles" runat="server" CssClass="buttonSave" OnClick="btnDashMoviles_Click"
                    Visible="false" />
                <%--NZ--%>
                <asp:Label ID="lblTipoMoneda" runat="server" CssClass="tipoMoneda"></asp:Label>
                <%--NZ--%>
            </asp:Panel>
            <div>
                <table style="width: 38%;">
                    <tr>
                        <td class="tdLblFechas">
                            <asp:Label ID="lblFechaInicio" runat="server" Text="Inicio: "></asp:Label>
                        </td>
                        <td class="tdDSODT">
                            <cc1:DSODateTimeBox ID="pdtInicio" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                            </cc1:DSODateTimeBox>
                        </td>
                        <td class="tdLblFechas">
                            <asp:Label ID="lblFechaFin" runat="server" Text="Fin:"></asp:Label>
                        </td>
                        <td class="tdDSODT">
                            <cc1:DSODateTimeBox ID="pdtFin" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                            </cc1:DSODateTimeBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <br />
    <div id="divGoBack" runat="server" style="float: right">
    </div>
    <%--Tabla de 2 celdas de ancho x 4 celdas de alto celdas--%>
    <asp:Table ID="tblReportes" runat="server" HorizontalAlign="Center" CellSpacing="5"
        Width="100%">
        <asp:TableRow ID="tr1" runat="server">
            <asp:TableCell ID="tc1" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
            <asp:TableCell ID="tc2" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="tr2" runat="server">
            <asp:TableCell ID="tc3" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
            <asp:TableCell ID="tc4" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="tr3" runat="server">
            <asp:TableCell ID="tc5" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
            <asp:TableCell ID="tc6" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="tr4" runat="server">
            <asp:TableCell ID="tc7" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
            <asp:TableCell ID="tc8" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <%--Tabla de 2 celdas de ancho x 1 celdas de alto celdas--%>
    <asp:Table ID="tblGridYGraf" runat="server" HorizontalAlign="Center" CellSpacing="5"
        Width="100%">
        <asp:TableRow ID="tr1tblGridYGraf" runat="server">
            <asp:TableCell ID="tc1tblGridYGraf" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
            <asp:TableCell ID="tc2tblGridYGraf" runat="server" Width="50%" HorizontalAlign="Center">
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>
