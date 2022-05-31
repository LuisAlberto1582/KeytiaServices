<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="DashboardModeloNK.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardNK.DashboardModeloNK" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        /*Buttons*/.btnAplicar
        {
            background-image: url(   '../../images/playsmall.png' ) !important;
            background-repeat: no-repeat;
            background-position: left;
        }
        .tdLblFechas
        {
            width: 40px;
        }
        .tdDSODT
        {
            width: 140px;
        }
        /*GridViews*/.grvheaderStyle
        {
            background-color: #2E6E9E;
            color: #FFFFFF;
            font-weight: bold;
        }
        .grvitemStyle
        {
            background-color: #F1F4F8;
            color: #000000;
            border-style: solid;
            border-color: Gray;
            border-width: thin;
        }
        .grvalternateItemStyle
        {
            background-color: #FFFFFF;
            color: #000000;
            border-style: solid;
            border-color: Gray;
            border-width: thin;
        }
                 /*Label*/
        .titulos
        {
        font-family: verdana;
        color: Black;
        font-weight: bold;
        font-size: large;
}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <div>
        <div align="left" style="height: 24px">
            <asp:Label ID="lblInicio" runat="server" Text="Inicio" ForeColor="#365F91" Font-Bold="True"
                Font-Names="Tahoma" Font-Size="Medium"></asp:Label>
        </div>
    </div>
    <div>
        <asp:Panel ID="pToolBar" runat="server">
            <asp:Button ID="btnAplicar" runat="server" Text="Aplicar" 
                onclick="btnAplicar_Click" />
        </asp:Panel>
        <div>
            <table style="width: 38%;">
                <tr>
                    <td bgcolor="#4297D7" class="tdLblFechas">
                        <asp:Label ID="lblFechaInicio" runat="server" Text="Inicio: "></asp:Label>
                    </td>
                    <td class="tdDSODT">
                        <cc1:DSODateTimeBox ID="pdtInicio" runat="server" AppendText="" MaxDateTime="" 
                            NextText="">
                        </cc1:DSODateTimeBox>
                    </td>
                    <td bgcolor="#4297D7" class="tdLblFechas">
                        <asp:Label ID="lblFechaFin" runat="server" Text="Fin:"></asp:Label>
                    </td>
                    <td class="tdDSODT">
                        <cc1:DSODateTimeBox ID="pdtFin" runat="server">
                        </cc1:DSODateTimeBox>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div>
<%--    Grafica Historica--%>
        <div id="Graficahistorica">
            <table align="center" width="1100">
                <tr>
                    <td align="center" id="tdGrafCol">
                        <div align="center">
                            <asp:Label CssClass="titulos" ID="lblGrafCol" runat="server"></asp:Label></div>
                        <div align="center">
                            <asp:Chart ID="grafRepCol" runat="server" Width="550" Height="350" EnableTheming="True"
                                Palette="None">
                                <Series>
                                    <asp:Series Name="Actual" IsXValueIndexed="True">
                                    </asp:Series>
                                    <asp:Series Name="Anterior" IsXValueIndexed="True">
                                    </asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1" ShadowColor="White">
                                        <AxisX>
                                            <MajorGrid Enabled="False" />
                                        </AxisX>
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        
        <%--Reportes en GridViews--%>
        <div id="reportes" align="left">
            <table align="center" width="1100" style="border-top: solid 1px #b0b0b0; border-bottom: solid 1px #b0b0b0; vertical-align:top;">
                <tr>
                    <td height="15px">
                    </td>
                </tr>
                <tr valign="top">
                    <td align="center" width = "350">
                        <div align="center">
                            <asp:Label CssClass="titulos" ID="lblReporteTop10EmpMasCaros" runat="server"></asp:Label></div>
                        <br />
                        <br />
                        <div align="center">
                            <asp:GridView Width="350" ID="grvReporteTop10EmpMasCaros" runat="server" CellPadding="4" >
                                <RowStyle CssClass="grvitemStyle" />
                                <HeaderStyle CssClass="ui-widget-header"/>
                                <AlternatingRowStyle CssClass="grvalternateItemStyle" />
                            </asp:GridView>
                        </div>
                    </td>
                    <td width="20">
                    </td>
                    <td align="center" width = "350">
                        <div align="center">
                            <asp:Label CssClass="titulos" ID="lblReporteTop10SitMasCaros" runat="server"></asp:Label></div>
                        <br />
                        <br />
                        <div align="center">
                            <asp:GridView Width="350" ID="grvReporteTop10SitMasCaros" runat="server" CellPadding="4" >
                                <RowStyle CssClass="grvitemStyle" />
                                <HeaderStyle CssClass="ui-widget-header"/>
                                <AlternatingRowStyle CssClass="grvalternateItemStyle" />
                            </asp:GridView>
                        </div>
                    </td>
                    <td width="20">
                    </td>
                    <td align="center" width = "350">
                        <div align="center">
                            <asp:Label CssClass="titulos" ID="lblReporteTop10CenCosMasCaros" runat="server"></asp:Label></div>
                        <br />
                        <br />
                        <div align="center">
                            <asp:GridView Width="350" ID="grvReporteTop10CenCosMasCaros" runat="server" CellPadding="4" >
                                <RowStyle CssClass="grvitemStyle" />
                                <HeaderStyle CssClass="ui-widget-header"/>
                                <AlternatingRowStyle CssClass="grvalternateItemStyle" />
                            </asp:GridView>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td height="15px">
                    </td>
                </tr>
            </table>
            <br />
            <br />
        </div>
        <%--Graficas de pie--%>
        <div id="divGraficasPie" runat="server" align="center">
            
        </div>
    </div>
</asp:Content>
