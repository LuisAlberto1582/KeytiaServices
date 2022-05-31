<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="RepTraficoTK.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardLT.Reportes.RepTraficoTK" %>

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
            <div align="left" style="height: 24px">
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
                <table style="width: 38%;">
                    <tr>
                        <td class="tdLblFechas">
                            <asp:Label ID="lblFechaInicio" runat="server">Inicio:</asp:Label>
                        </td>
                        <td class="tdDSODT">
                            <cc1:DSODateTimeBox ID="pdtInicio" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                            </cc1:DSODateTimeBox>
                        </td>
                        <%--20140905 AM. Se agrega fecha fin --%>
                        <td class="tdLblFechas">
                            <asp:Label ID="lblFechaFin" runat="server">Fin: </asp:Label>
                        </td>
                        <td class="tdDSODT">
                            <cc1:DSODateTimeBox ID="pdtFin" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                            </cc1:DSODateTimeBox>
                        </td>
                        <td style="width: auto;" class="tdLblFechasNoWidth">
                            <asp:Label ID="lblTks" runat="server">Grupo Troncal:</asp:Label>
                        </td>
                        <td style="width: auto">
                            <asp:DropDownList ID="ddlGpoTroncal" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <asp:Panel ID="pnlMainContainer" runat="server" HorizontalAlign="Center" Width="100%"
        CssClass="maxWidth100Perc">
    </asp:Panel>
</asp:Content>
