<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="Volumetria.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.TIM.Volumetria"
    ValidateRequest="true" EnableEventValidation="false" %>

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
        <div class="form-row">
            <asp:Label ID="lblCarrierConsumo" runat="server"></asp:Label>
            <asp:UpdatePanel ID="upFiltros" runat="server">
                <ContentTemplate>
                    <div class="col-md-1 col-sm-1">
                        <asp:Label ID="Label1" runat="server">Filtros:</asp:Label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlEmpre" runat="server" CssClass="dropdown-keytia form-control" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlEmpre_SelectedIndexChanged" AppendDataBoundItems="true">
                            <asp:ListItem Value="-1">TODAS</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlCarrier" runat="server" CssClass="dropdown-keytia form-control" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="col-md-2 col-sm-2">
                <asp:LinkButton ID="btnAplicar" runat="server" Text="Aplicar" OnClick="btnAplicar_Click" CssClass="btn btn-keytia-md"></asp:LinkButton>
            </div>
        </div>
    </asp:Panel>
    <br />
    <br />
    <asp:Panel ID="pnlReporte" runat="server" CssClass="row">
        <asp:TabContainer ID="TabContainerPrincipal" runat="server" CssClass="MyTabStyle">
            <asp:TabPanel ID="TabPanelUno" runat="server" HeaderText="Consumo" Visible="false">
                <ContentTemplate>
                    <asp:Panel ID="pnlMainHolder" runat="server">
                        <asp:Panel ID="pnlLeft" runat="server" CssClass="row">
                            <asp:Panel ID="Tab1Rep1" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>

                            <asp:Panel ID="Tab1Rep2" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>
                        </asp:Panel>
                        <asp:Panel ID="pnlRight" runat="server" CssClass="row">
                            <asp:Panel ID="Tab1Rep3" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>

                            <asp:Panel ID="Tab1Rep4" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>
                        </asp:Panel>
                    </asp:Panel>
                </ContentTemplate>
            </asp:TabPanel>
            <asp:TabPanel ID="TabPanelDos" runat="server" HeaderText="Inventario" Visible="false">
                <ContentTemplate>
                    <asp:Panel ID="pnlMainHolder2" runat="server">
                        <asp:Panel ID="pnlLeft2" runat="server" CssClass="row">
                            <asp:Panel ID="Tab2Rep1" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>

                            <asp:Panel ID="Tab2Rep2" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>
                        </asp:Panel>
                        <asp:Panel ID="pnlRight2" runat="server" CssClass="row">
                            <asp:Panel ID="Tab2Rep3" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>

                            <asp:Panel ID="Tab2Rep4" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>
                        </asp:Panel>
                    </asp:Panel>
                </ContentTemplate>
            </asp:TabPanel>
        </asp:TabContainer>
    </asp:Panel>
</asp:Content>

