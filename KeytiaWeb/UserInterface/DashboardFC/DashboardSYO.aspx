<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DashboardSYO.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.DashboardSYO" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
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
    <%--Barra con boton y fechas--%>
    <div>
        <div>
            <div align="left" class="AutoHeight">
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Inicio</asp:Label>               
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                <asp:Button ID="btnRegresar" runat="server" CssClass="buttonBack" OnClick="btnRegresar_Click"
                    Text="< Regresar" />
                &nbsp;
                <asp:Button ID="btnAplicar" runat="server" CssClass="buttonPlay" OnClick="btnAplicar_Click"
                    Text="Aplicar" />
                &nbsp;
                <asp:Button ID="btnExportarXLS" runat="server" Text="Exportar XLS" CssClass="buttonSave"
                    OnClick="btnExportarXLS_Click" />
            </asp:Panel>
            <div>
                <asp:Panel ID="pnlFechas" runat="server" CssClass="divToLeft">
                    <table>
                        <tr>
                            <td class="tdLblFechas">
                                <asp:Label ID="lblFechaInicio" runat="server">Inicio:</asp:Label>
                            </td>
                            <td>
                                <cc1:DSODateTimeBox ID="pdtInicio" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                    ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                </cc1:DSODateTimeBox>
                            </td>
                            <td class="tdLblFechas">
                                <asp:Label ID="lblFechaFin" runat="server">Fin:</asp:Label>
                            </td>
                            <td>
                                <cc1:DSODateTimeBox ID="pdtFin" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                    ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                </cc1:DSODateTimeBox>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>                               
            </div>
        </div>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep0" runat="server" CssClass="TopCenter divToCenter" Width="100%">
                <asp:TableRow>
                    <asp:TableCell Width="100%">
                        <asp:Panel ID="Rep0" runat="server" CssClass="TopCenter divToCenter">
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <asp:Panel ID="pnlLeft" runat="server" CssClass="divToLeft">
            <asp:Panel ID="Rep1" runat="server" CssClass="TopCenter divToCenter">
            </asp:Panel>
            <br />
            <asp:Panel ID="Rep3" runat="server" CssClass="TopCenter divToCenter">
            </asp:Panel>
            <br />
            <asp:Panel ID="Rep5" runat="server" CssClass="TopCenter divToCenter">
            </asp:Panel>
            <br />
            <asp:Panel ID="Rep7" runat="server" CssClass="TopCenter divToCenter">
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRight" runat="server" CssClass="divToRight">
            <asp:Panel ID="Rep2" runat="server" CssClass="TopCenter divToCenter">
            </asp:Panel>
            <br />
            <asp:Panel ID="Rep4" runat="server" CssClass="TopCenter divToCenter">
            </asp:Panel>
            <br />
            <asp:Panel ID="Rep6" runat="server" CssClass="TopCenter divToCenter">
            </asp:Panel>
            <br />
            <asp:Panel ID="Rep8" runat="server" CssClass="TopCenter divToCenter">
            </asp:Panel>
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
