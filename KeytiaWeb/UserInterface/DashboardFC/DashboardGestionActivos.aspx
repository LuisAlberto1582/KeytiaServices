<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DashboardGestionActivos.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.DashboardGestionActivos" %>

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
    <asp:Panel ID="pnlMainHolder" runat="server" CssClass="row">
        <asp:Panel ID="pnlLeft" runat="server" CssClass="col-md-4 col-sm-4">
            <asp:Panel ID="Rep1" runat="server">
            </asp:Panel>

            <asp:Panel ID="Rep3" runat="server">
            </asp:Panel>

            <asp:Panel ID="Rep5" runat="server">
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRight" runat="server" CssClass="col-md-4 col-sm-4">
            <asp:Panel ID="Rep2" runat="server">
            </asp:Panel>

            <asp:Panel ID="Rep4" runat="server">
            </asp:Panel>

            <asp:Panel ID="Rep6" runat="server">
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
