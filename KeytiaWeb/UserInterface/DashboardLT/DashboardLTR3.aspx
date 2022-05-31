<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="DashboardLTR3.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardLT.DashboardLTR3" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

<%--    <script src="Scripts/jquery-1.7.1.js"></script>

    <script language="javascript">

        function MyFunction() {

            var field = 'nav';
            var url = window.location.href;
            if (url.indexOf('?' + field + '=') != 'Cons3Meses') {
                $(document).ready(function() {
                    var gridHeader = $('#<%=cons3Meses.ClientID%>').clone(true); // Here Clone Copy of Gridview with style
                    $(gridHeader).find("tr:gt(0)").remove(); // Here remove all rows except first row (header row)
                    $('#<%=cons3Meses.ClientID%> tr th').each(function(i) {
                        // Here Set Width of each th from gridview to new table(clone table) th 
                        $("th:nth-child(" + (i + 1) + ")", gridHeader).css('width', ($(this).width()).toString() + "px");
                    });
                    $("#cons3Mesespanel").append(gridHeader);
                    $('#cons3Mesespanel').css('position', 'absolute');
                    $('#cons3Mesespanel').css('top', $('#<%=cons3Meses.ClientID%>').offset().top);

                });
            }
        }
    </script>--%>

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
                        <td class="tdLblFechas">
                            <asp:Label ID="lblFechaFin" runat="server">Fin:</asp:Label>
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
    <asp:Panel ID="pnlMainContainer" runat="server" HorizontalAlign="Center" Width="100%" CssClass="maxWidth100Perc">
    </asp:Panel>
</asp:Content>
