<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConsumoGlobalFiltro.aspx.cs" 
Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoGlobalFiltro" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .divToLefRepConsumoGlobalFiltro
        {
            width: 30%;
            float: left;
        }
        .divToRightRepConsumoGlobalFiltro
        {
            width: 70%;
            float: right;
        }
        .alineacionFiltros
        {
            text-align: left;
        }
        .modalProgress
        {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 0;
            filter: alpha(opacity=60);
            opacity: 0.95;
            -moz-opacity: 0.8;
        }
        .centerProgress
        {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            border-radius: 10px;
            filter: alpha(opacity=60);
            opacity: 1;
            -moz-opacity: 1;
        }
        .centerProgress img
        {
            height: 90px;
            width: 90px;
        }
    </style>
    <script type="text/javascript">

        function ActualizarRep() {

            var UpdPrincipal = '<%=UpdPrincipal.ClientID%>';
            if (UpdPrincipal != null) {
                __doPostBack(UpdPrincipal);
            }
        }
      
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
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                <asp:Label ID="lblTitulo" runat="server" Font-Size="Medium">Reporte multifiltros</asp:Label>
            </asp:Panel>
        </div>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%">
        <br />
        <asp:Panel ID="pnlLeft" runat="server" CssClass="divToLefRepConsumoGlobalFiltro">
            <asp:Panel ID="Rep1" runat="server" CssClass="TopCenter divToCenter">
            </asp:Panel>
            <br />
        </asp:Panel>
        <asp:Panel ID="pnlRight" runat="server" CssClass="divToRightRepConsumoGlobalFiltro">
            <asp:UpdatePanel ID="UpdPrincipal" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="Rep2" runat="server" CssClass="TopCenter divToCenter">
                    </asp:Panel>
                    <br />
                    <asp:Panel ID="Rep4" runat="server" CssClass="TopCenter divToCenter">
                    </asp:Panel>
                    <br />
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdateProgress runat="server" ID="ActualizandoUpdPrincipal" AssociatedUpdatePanelID="UpdPrincipal">
                <ProgressTemplate>
                    <div class="modalProgress">
                        <div class="centerProgress">
                            <asp:Image runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" />
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

