<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="ConsumoGeneral.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.TIM.ConsumoGeneral" %>

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

    <asp:Panel ID="pToolBar" runat="server" CssClass="page-title-keytia">
        <div class="mt-radio-inline">
            <asp:Label ID="lblCarrierConsumo" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:DropDownList ID="ddlCarrier" runat="server" CssClass="selectpicker dropdown-keytia" AppendDataBoundItems="true">
                    <asp:ListItem Value="-1">TODOS</asp:ListItem>
                </asp:DropDownList>
            <asp:LinkButton ID="btnAplicar" runat="server" Text="Aplicar" OnClick="btnAplicar_Click" CssClass="btn btn-keytia-md"></asp:LinkButton>
        </div>
    </asp:Panel>

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
        <asp:Panel ID="pnlCenter" runat="server" CssClass="col-md-4 col-sm-4">
            <asp:Panel ID="Rep8" runat="server">
            </asp:Panel>

            <asp:Panel ID="Rep9" runat="server">
            </asp:Panel>

            <asp:Panel ID="Rep10" runat="server">
            </asp:Panel>

            <asp:Panel ID="Rep11" runat="server">
            </asp:Panel>

            <asp:Panel ID="Rep12" runat="server">
            </asp:Panel>
        </asp:Panel>
        <%-- <asp:Panel ID="Rep7" runat="server" CssClass="TopCenter divToCenter">
        </asp:Panel>--%>
    </asp:Panel>
</asp:Content>
